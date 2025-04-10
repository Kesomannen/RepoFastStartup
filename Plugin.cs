using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections;

namespace FastStartup;

[BepInPlugin("com.kesomannen.FastStartup", "FastStartup", "0.1.0")]
[BepInDependency("REPOLib", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin {
    static ConfigFile _config;
    static ManualLogSource _log;
    
    void Awake() {
        _config = Config;
        _log = Logger;
        
        On.RunManager.Awake += (orig, self) =>
        {
            orig(self);
            self.StartCoroutine(WaitOneFrame(SkipMainMenu));
        };

        On.LoadingUI.LevelAnimationStart += (_, self) =>
        {
            self.levelAnimationStarted = true;
            self.levelAnimationCompleted = true;
        };
    }

    void SkipMainMenu() {
        Logger.LogInfo("Skipping main menu.");
        RunManager.instance.ResetProgress();

        var files = SemiFunc.SaveFileGetAll();
        Logger.LogDebug($"Found {files.Count} save files.");
            
        var saveMode = _config.Bind(
            "General", 
            "SaveMode", 
            SaveMode.ResetOnStartup,
            "ResetOnStartup: Create a new save and resets it on each startup." +
            "CreateOnStartup: Create a brand new save on each startup." +
            "Reuse: Use an existing save on each startup, without resetting."
        ).Value;
        
        var saveFileName = _config.Bind(
            "General", 
            "SaveFileName",
            "REPO_FASTSTARTUP_SAVE",
            $"Which save file to use. Not applicable for the Reuse save mode."
        ).Value;
        
        Logger.LogDebug($"saveMode = {saveMode}");
        Logger.LogDebug($"saveFileName = {saveFileName}");

        if (saveMode == SaveMode.CreateOnStartup) {
            Logger.LogInfo("Creating new save file.");
            SemiFunc.SaveFileCreate();
        } else {
            var exists = files.Contains(saveFileName);
            Logger.LogDebug($"Save file exists: {exists}.");
            
            if (saveMode == SaveMode.ResetOnStartup) {
                if (exists) {
                    Logger.LogInfo("Deleting existing save file.");
                    SemiFunc.SaveFileDelete(saveFileName);
                } else {
                    Logger.LogInfo($"Creating new save file.");
                }
            } else if (!exists) {
                _log.LogWarning($"Could not find save file {saveFileName}!");
            }
            
            StatsManager.instance.saveFileCurrent = saveFileName;
            StatsManager.instance.SaveGame(saveFileName);
            SemiFunc.SaveFileLoad(saveFileName);
        }
        
        GameManager.instance.localTest = false;
        
        Logger.LogDebug("Instantiating networkConnectPrefab from SteamManager.");
        Instantiate(SteamManager.instance.networkConnectPrefab);
        
        Logger.LogDebug("Changing level.");
        RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
    }

    static IEnumerator WaitOneFrame(Action action) {
        yield return null;
        action();
    }

    enum SaveMode {
        ResetOnStartup,
        CreateOnStartup,
        Reuse,
    }
}
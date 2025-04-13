using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using System;
using System.Collections;

namespace FastStartup;

[BepInPlugin("com.kesomannen.FastStartup", "FastStartup", "0.1.0")]
[BepInDependency("REPOLib", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin {
    static bool _overrideLevelTrigger;
    
    internal static ManualLogSource Log;

    static bool _multiplayer;
    static SaveMode _saveMode;
    static string _saveFileName;
    static bool _overrideLevel;
    static string _levelName;
    
    void Awake() {
        Log = Logger;

        BindConfig();

        if (Chainloader.PluginInfos.ContainsKey("REPOLib")) {
            RepoLibCompat.Init(this);
        } else {
            On.RunManager.Awake += (orig, self) => {
                orig(self);
                self.StartCoroutine(YieldThen(SkipMainMenu));
            };
        }
        
        On.LoadingUI.LevelAnimationStart += (_, self) => {
            self.levelAnimationStarted = true;
            self.levelAnimationCompleted = true;
        };

        On.RunManager.SetRunLevel += (orig, self) => {
            if (_overrideLevelTrigger) {
                var level = FindLevel(_levelName);

                if (level == null) {
                    Log.LogWarning($"Level {_levelName} not found! Defaulting to random...");
                } else {
                    Log.LogDebug($"Overriding level to {level.name}.");
                    self.levelCurrent = level;
                    return;
                }
            }

            orig(self);
        };
    }

    void BindConfig() {
        _multiplayer = Config.Bind(
            "General", 
            "Multiplayer", 
            true,
            "Whether to start a multiplayer or singleplayer lobby"
        ).Value;
        
        _saveMode = Config.Bind(
            "General", 
            "SaveMode", 
            SaveMode.ResetOnStartup,
            "ResetOnStartup: Create a new save and reset it on each startup." +
            "CreateOnStartup: Create a brand new save on each startup." +
            "Reuse: Use an existing save, without resetting."
        ).Value;
        
        _saveFileName = Config.Bind(
            "General", 
            "SaveFileName",
            "REPO_FASTSTARTUP_SAVE",
            $"The name of the save file to use."
        ).Value;
        
        _overrideLevel = Config.Bind(
            "General", 
            "OverrideLevel",
            false,
            $"Whether to always start in the same level. Requires the LevelName option to be set."
        ).Value;

        _levelName = Config.Bind(
            "General",
            "LevelName",
            "",
            $"The name of the level to start at. The 'Level - ' prefix is optional. Only applicable when OverrideLevel is true."
        ).Value;
    }

    internal void SkipMainMenu() {
        Logger.LogInfo("Skipping main menu.");
        RunManager.instance.ResetProgress();

        var files = SemiFunc.SaveFileGetAll();

        if (_saveMode == SaveMode.CreateOnStartup) {
            Logger.LogDebug("Creating new save file.");
            SemiFunc.SaveFileCreate();
        } else {
            var exists = files.Contains(_saveFileName);
            
            if (_saveMode == SaveMode.ResetOnStartup) {
                if (exists) {
                    Logger.LogDebug("Deleting existing save file.");
                    SemiFunc.SaveFileDelete(_saveFileName);
                } else {
                    Logger.LogDebug("Creating new save file.");
                }
            } else if (!exists) {
                Log.LogWarning($"Could not find save file {_saveFileName}! Creating new file...");
            }
            
            StatsManager.instance.saveFileCurrent = _saveFileName;
            StatsManager.instance.SaveGame(_saveFileName);
            SemiFunc.SaveFileLoad(_saveFileName);
        }

        if (_overrideLevel) {
            _overrideLevelTrigger = true;
        }

        if (_multiplayer) {
            Logger.LogDebug("Starting multiplayer game.");
            Instantiate(SteamManager.instance.networkConnectPrefab);
        } else {
            Logger.LogDebug("Starting singleplayer game.");
        }
        
        RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
    }

    static IEnumerator YieldThen(Action action) {
        yield return null;
        action();
    }

    static Level FindLevel(string name) {
        return RunManager.instance.levels.Find(level => {
            if (level.name == name) return true;
            
            if (level.name.StartsWith("Level - ")) {
                return level.name["Level - ".Length..] == name;
            }
            
            return false;
        });
    }

    enum SaveMode {
        ResetOnStartup,
        CreateOnStartup,
        Reuse,
    }
}
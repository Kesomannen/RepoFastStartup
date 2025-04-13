using BepInEx;
using BepInEx.Logging;

namespace FastStartup;

[BepInPlugin("com.kesomannen.FastStartup", "FastStartup", "0.2.0")]
[BepInDependency("REPOLib", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin {
    static bool _overrideLevelTrigger;
    static Config _config;
    
    internal static ManualLogSource Log;
    
    void Awake() {
        Log = Logger;

        _config = new Config(Config);
        Patches.Init();
    }

    internal static void SkipMainMenu() {
        Log.LogInfo("Skipping main menu.");
        RunManager.instance.ResetProgress();

        var files = SemiFunc.SaveFileGetAll();

        if (_config.SaveMode == SaveMode.CreateOnStartup) {
            Log.LogDebug("Creating new save file.");
            SemiFunc.SaveFileCreate();
        } else {
            var exists = files.Contains(_config.SaveFileName);
            
            if (_config.SaveMode == SaveMode.ResetOnStartup) {
                if (exists) {
                    Log.LogDebug("Deleting existing save file.");
                    SemiFunc.SaveFileDelete(_config.SaveFileName);
                } else {
                    Log.LogDebug("Creating new save file.");
                }
            } else if (!exists) {
                Log.LogWarning($"Could not find save file {_config.SaveFileName}! Creating new file...");
            }
            
            StatsManager.instance.saveFileCurrent = _config.SaveFileName;
            StatsManager.instance.SaveGame(_config.SaveFileName);
            SemiFunc.SaveFileLoad(_config.SaveFileName);
        }

        if (_config.OverrideLevel) {
            _overrideLevelTrigger = true;
        }

        if (_config.Multiplayer) {
            Log.LogDebug("Starting multiplayer game.");
            Instantiate(SteamManager.instance.networkConnectPrefab);
        } else {
            Log.LogDebug("Starting singleplayer game.");
        }
        
        RunManager.instance.ChangeLevel(true, false, RunManager.ChangeLevelType.RunLevel);
    }

    internal static bool OverrideLevel() {
        if (!_overrideLevelTrigger) return false;
        _overrideLevelTrigger = false;
            
        var level = Utils.FindLevel(_config.LevelName);

        if (level == null) {
            Log.LogWarning($"Level {_config.LevelName} not found! Defaulting to random...");
            return false;
        }
        
        Log.LogDebug($"Overriding level to {level.name}.");
        RunManager.instance.levelCurrent = level;
        return true;
    }
}
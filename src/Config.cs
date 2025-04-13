using BepInEx.Configuration;

namespace FastStartup;

internal class Config(ConfigFile file) {
    public readonly bool Multiplayer = file.Bind(
        "General", 
        "Multiplayer", 
        true,
        "Whether to start a multiplayer or singleplayer lobby"
    ).Value;
    
    public readonly SaveMode SaveMode = file.Bind(
        "General", 
        "SaveMode", 
        SaveMode.ResetOnStartup,
        "ResetOnStartup: Create a new save and reset it on each startup." +
        "CreateOnStartup: Create a brand new save on each startup." +
        "Reuse: Use an existing save, without resetting."
    ).Value;
    
    public readonly string SaveFileName = file.Bind(
        "General", 
        "SaveFileName",
        "REPO_FASTSTARTUP_SAVE",
        "The name of the save file to use."
    ).Value;
    
    public readonly bool OverrideLevel = file.Bind(
        "General", 
        "OverrideLevel",
        false,
        "Whether to always start in the same level. Requires the LevelName option to be set."
    ).Value;
    
    public readonly string LevelName = file.Bind(
        "General",
        "LevelName",
        string.Empty,
        "The name of the level to start at. The 'Level - ' prefix is optional. Only applicable when OverrideLevel is true."
    ).Value;
}
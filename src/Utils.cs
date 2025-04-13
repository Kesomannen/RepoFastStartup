namespace FastStartup;

internal static class Utils {
    public static Level FindLevel(string name) {
        return RunManager.instance.levels.Find(level => {
            if (level.name == name) return true;
            
            if (level.name.StartsWith("Level - ")) {
                return level.name["Level - ".Length..] == name;
            }
            
            return false;
        });
    }
}
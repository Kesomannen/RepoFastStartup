namespace FastStartup;

internal static class RepoLibCompat {
    public static void Init(Plugin plugin) {
        Plugin.Log.LogDebug($"REPOLib detected.");
        REPOLib.BundleLoader.OnAllBundlesLoaded += plugin.SkipMainMenu;
    }
}
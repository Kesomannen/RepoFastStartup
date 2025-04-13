namespace FastStartup;

internal static class RepoLibCompat {
    public static void Init() {
        Plugin.Log.LogDebug("REPOLib detected.");
        REPOLib.BundleLoader.OnAllBundlesLoaded += Plugin.SkipMainMenu;
    }
}
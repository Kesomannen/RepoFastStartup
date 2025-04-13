using BepInEx.Bootstrap;
using SingularityGroup.HotReload;
using System;
using System.Collections;

namespace FastStartup;

internal static class Patches {
    internal static void Init() {
        if (Chainloader.PluginInfos.ContainsKey("REPOLib")) {
            RepoLibCompat.Init();
        } else {
            On.RunManager.Awake += (orig, self) => {
                orig(self);
                self.StartCoroutine(YieldThen(Plugin.SkipMainMenu));
            };
        }
        
        On.LoadingUI.LevelAnimationStart += (_, self) => {
            self.levelAnimationStarted = true;
            self.levelAnimationCompleted = true;
        };

        On.RunManager.SetRunLevel += (orig, self) => {
            if (!Plugin.OverrideLevel()) {
                orig(self);
            }
        };

        //On.NetworkConnect.CreateLobby += CreateLobbyPatch;
    }
    
    static IEnumerator YieldThen(Action action) {
        yield return null;
        action();
    }

    /*
    static IEnumerator CreateLobbyPatch(On.NetworkConnect.orig_CreateLobby orig, NetworkConnect self) {
        var enumerator = orig(self);
        while (enumerator.MoveNext()) {
            yield return enumerator.Current;
        }
        
        Plugin.Log.LogInfo("Lobby creation finished!");
    }
    */
}
# FastStartup

Developer tool to skip the main menu, lobby screen and level load animation.

![Startup](https://github.com/Kesomannen/RepoFastStartup/blob/master/assets/start.gif?raw=true)

If REPOLib is installed, this mod waits for all asset bundles to load before skipping.

## Config

| Name              | Type   | Default                | Description                                                                                                                                                                                   |
|-------------------|--------|------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Multiplayer**   | Bool   | `true`                 | Whether to start a multiplayer or singleplayer lobby                                                                                                                                          |
| **SaveMode**      | Enum   | `ResetOnStartup`       | **ResetOnStartup:** Create a new save and reset it on each startup.<br/>**CreateOnStartup:** Create a brand new save on each startup.<br/>**Reuse:** Use an existing save, without resetting. |
| **SaveFileName**  | String | `REPO_FASTSTARTUP_SAVE` | The name of the save file to use.                                                                                                                                                             |
| **OverrideLevel** | Bool   | `false`                | Whether to always start in the same level. Requires the `LevelName` option to be set.                                                                                                           |
| **LevelName**     | String |                        | The name of the level to start at. The 'Level - ' prefix is optional. Only applicable when OverrideLevel is true.                                                                                                           |

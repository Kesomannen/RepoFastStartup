# FastStartup

Developer tool to skip the main menu, lobby screen and level load animation.

![Startup](https://github.com/Kesomannen/RepoFastStartup/blob/master/assets/start.gif?raw=true)

## Config

| Name             | Type   | Default               | Description                                                                                                                                                                                                    |
| ---------------- | -------| --------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **SaveMode**     | Enum   | ResetOnStartup        | **ResetOnStartup:** Create a new save and resets it on each startup.<br/>**CreateOnStartup:** Create a brand new save on each startup.<br/>**Reuse:** Use an existing save on each startup, without resetting. |
| **SaveFileName** | String | REPO_FASTSTARTUP_SAVE | Which save file to use. Not applicable for the **Reuse** save mode.                                                                                                                                            |

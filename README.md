# QoL

Adds various quality of life additions to Ittle Dew 2.

## Features
**Note:** Not all features are enabled by default. You'll need [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/latest) to use all the features.

### Skip Splash
Skips the Ludosity splash screen on startup, saving ~5 seconds with each launch of the game.

### Run in Background
Allows the game to continue running without the window having focus.

### HUD Customization
You can toggle specific parts of the HUD on/off.

### Vanilla Bug Fixes
- **Fluffy Lake Warp:** Fixes the bug where returning to spawn from anywhere in Fluffy Fields would take you to lake.
- **Adds Missing Resolution Options:** Fixes the bug where some resouliton options are unavailable in the resolution selection in game settings, such as for 1440p displays.

## How to Install
1. [BepInEx 5.4.22 or later 5.x versions](https://github.com/bepinex/bepinex/releases/latest)
    - Do **not** use BepInEx 6.x, as it may not be compatible.
    - Unzip the BepInEx folder into the root of your game's installation directory (i.e. there should be a `BepInEx` folder in the same folder as `ID2.exe`).
1. Run the game once to generate the BepInEx plugins folder, then quit before performing the next step.
1. Download `QoL.zip` included in the [latest release](https://github.com/Chris-Is-Awesome/ID2-QoL/releases/latest). Extract the downloaded zip to your BepInEx plugins folder (`BepInEx\plugins`).

## Dependencies

- [ModCore](https://github.com/Chris-Is-Awesome/ID2-ModCore)
- [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager/releases/latest) (soft-dependency; required if you want to configure the mod's default settings)

## Configuration

This mod uses [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) configuration menu.
To open it, press <kbd>F1</kbd> (default) to open the menu.
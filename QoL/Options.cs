using BepInEx.Configuration;
using ID2.ModCore;

namespace ID2.QoL;

internal class Options
{
	private readonly ConfigFile configFile;

	public Options()
	{
		configFile = Plugin.Instance.Config;

		QoLMod.Options.skipSplashToggleEntry = Helpers.CreateOption
		(
			configFile: configFile,
			key: "Skip Splash Screen",
			description: "When enabled, the Ludosity splash screen is skipped on startup, and it'll go straight into the main menu.",
			defaultValue: QoLMod.Options.Defaults.SkipSplash
		);

		QoLMod.Options.runInBackgroundToggleEntry = Helpers.CreateOption
		(
			configFile: configFile,
			key: "Run in Background",
			description: "When enabled, the game will not pause when another window has focus.",
			defaultValue: QoLMod.Options.Defaults.RunInBackground,
			onChanged: value => QoLMod.Options.RunInBackgroundToggled(value)
		);

		QoLMod.Options.hudToShowEntry = Helpers.CreateOption
		(
			configFile: configFile,
			key: "HUD to Show",
			description: "Toggle HUD elements on/off.",
			defaultValue: QoLMod.Options.Defaults.HUDToShow,
			onChanged: QoLMod.HUDShownChanged
		);

		QoLMod.Options.fixFluffyLakeWarpBugEntry = Helpers.CreateOption
		(
			configFile: configFile,
			key: "Fix Fluffy Lake Warp Bug",
			description: "When enabled, the vanilla bug where returning to spawn from anywhere in Fluffy Fields would take you to lake is patched. Reload Fluffy Fields for change to apply.",
			defaultValue: QoLMod.Options.Defaults.FixFluffyLakeWarpBug
		);

		QoLMod.Options.fixResolutionOptionsBugEntry = Helpers.CreateOption
		(
			configFile: configFile,
			key: "Fix Missing Resolution Options Bug",
			description: "When enabled, the vanilla bug where some resolution options are unavailable in the resolution selection in game settings is patched. Reload open settings menu for change to apply.",
			defaultValue: QoLMod.Options.Defaults.FixResolutionOptionsBug
		);

		QoLMod.Options.fixMapManExceptionBugEntry = Helpers.CreateOption
		(
			configFile: configFile,
			key: "Ignore MapMan Exception",
			description: "Ignores the MapMan event exception that gets thrown by vanilla code. This can make it more confusing for mod developers to track errors in the log. It's preferred that you leave this enabled.",
			defaultValue: QoLMod.Options.Defaults.FixMapManExceptionBug,
			attributes: new() { IsAdvanced = true }
		);
	}
}
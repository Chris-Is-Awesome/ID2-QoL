using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ID2.ModCore;
using System.Collections;
using UnityEngine;

namespace ID2.QoL;

[BepInPlugin(PluginInfo.guid, PluginInfo.name, PluginInfo.version)]
public class Plugin : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;
	private static Plugin instance;

	public static Plugin Instance => instance;

	private void Awake()
	{
		instance = this;
		Logger = base.Logger;

		try
		{
			// Mod initialization code here
			new Options();

			Harmony harmony = new(PluginInfo.name);
			harmony.PatchAll();
		}
		catch (System.Exception ex)
		{
			QoL.Logger.LogError($"Unhandled exception during initialization: {ex.Message}");
			return;
		}

		Logger.LogInfo($"Initialized [{PluginInfo.name} {PluginInfo.version}]");
	}

	private void Start()
	{
		if (QoLMod.Options.SkipSplash)
			QoLMod.SkipSplash();

		QoLMod.RunInBackground = QoLMod.Options.runInBackgroundToggleEntry.Value;
		QoLMod.MuteAudioOnStartup = true;
	}

	private void OnEnable()
	{
		Events.OnSceneLoaded += QoLMod.OnSceneLoaded;
	}

	private void OnDisable()
	{
		Events.OnSceneLoaded -= QoLMod.OnSceneLoaded;
	}

	/// <summary>
	/// Starts a Coroutine on the Plugin MonoBehaviour.<br/>
	/// This is useful for if you need to start a Coroutine<br/>from a non-MonoBehaviour class.
	/// </summary>
	/// <param name="routine">The routine to start.</param>
	/// <returns>The started Coroutine.</returns>
	public static Coroutine StartRoutine(IEnumerator routine)
	{
		return Instance.StartCoroutine(routine);
	}
}
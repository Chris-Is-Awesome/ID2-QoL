using BepInEx.Configuration;
using HarmonyLib;
using ID2.ModCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ID2.QoL;

internal class QoLMod
{
	[System.Flags]
	public enum HUDType
	{
		None = 0,
		Health = 1 << 0,
		Keys = 1 << 1,
		Items = 1 << 3,
		Hint = 1 << 4,
		Map = 1 << 5,
		GetItemMessage = 1 << 6,
		BossHealth = 1 << 7,
		AreaTitles = 1 << 8,
		Signs = 1 << 9,
		All = Health | Keys | Items | Hint | Map | GetItemMessage | BossHealth | AreaTitles | Signs
	}

	private static readonly Dictionary<HUDType, GameObject> hudAnchors = new();
	private static Transform overlayParent;

	public static bool RunInBackground
	{
		set
		{
			Application.runInBackground = value;
		}
	}
	public static bool MuteAudioOnStartup
	{
		set
		{
			if (value)
			{
				SoundPlayer.Instance.SoundVolume = 0;
				MusicPlayer.Instance.MusicVolume = 0;
			}
		}
	}
	private static List<HUDType> AllHUDElements
	{
		get
		{
			return System.Enum.GetValues(typeof(HUDType))
				.Cast<HUDType>()
				.Where(h => h != HUDType.None && h != HUDType.All)
				.ToList();
		}
	}
	private static List<HUDType> ActiveHUDElements
	{
		get
		{
			HUDType value = Options.hudToShowEntry.Value;

			if (value == HUDType.None)
				return [];

			if (value == HUDType.All)
				return AllHUDElements;

			return AllHUDElements
				.Where(h => (value & h) != 0)
				.ToList();
		}
	}

	public static void SkipSplash()
	{
		Utility.LoadLevel(1);
	}

	public static void HUDShownChanged(HUDType _)
	{
		if (Globals.Player == null)
			return;

		UpdateHUDShown();
	}

	public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (Globals.Player == null)
			return;

		UpdateHUDShown();
	}

	private static void HUDObjectSetup(OverlayWindow overlay)
	{
		HUDType hudType = overlay.name switch
		{
			"HealthMeter" => HUDType.Health,
			"KeyHUD" or "LocalKeyHUD" => HUDType.Keys,
			"MeleeIcon" or "ForceWandIcon" or "DynamiteIcon" or "IceRingIcon" => HUDType.Items,
			"InfoButtonIcon" => HUDType.Hint,
			"MapButtonIcon" => HUDType.Map,
			"ItemGetOverlay" => HUDType.GetItemMessage,
			"BossHealthMeter" => HUDType.BossHealth,
			"TitleOverlay" => HUDType.AreaTitles,
			"SpeechBubble" => HUDType.Signs,
			_ => HUDType.None
		};

		if (hudType == HUDType.None)
			return;

		Transform anchor;

		if (overlay._anchorGroup != null)
		{
			OverlayWindow.CamState camState = OverlayWindow.GetCameraState(overlay.overlayCam);
			anchor = camState.GetAnchorGroup(overlay._anchorGroup);
		}
		else if (overlay._doAnchor)
			anchor = overlay.anchorTrans;
		else
			anchor = overlay.transform;

		if (hudAnchors.ContainsKey(hudType))
			hudAnchors[hudType] = anchor.gameObject;
		else
			hudAnchors.Add(hudType, anchor.gameObject);
	}

	private static void UpdateHUDShown()
	{
		foreach (var kvp in hudAnchors)
		{
			// Skip objects that don't exist yet (such as signs)
			if (kvp.Value == null)
				continue;

			bool active = ActiveHUDElements.Contains(kvp.Key);

			if (kvp.Key == HUDType.Items)
			{
				if (overlayParent == null)
					overlayParent = Camera.main.GetComponentInParent<EntityHUD>().transform.Find("OverlayCamera");

				List<OverlayWindow> overlays = overlayParent.GetComponentsInChildren<OverlayWindow>(true).ToList();
				GameObject meleeIcon = overlays.Find(o => o.name == "MeleeIcon").transform.parent.gameObject;
				GameObject forceWandIcon = overlays.Find(o => o.name == "ForceWandIcon").transform.parent.gameObject;
				GameObject dynamiteIcon = overlays.Find(o => o.name == "DynamiteIcon").transform.parent.gameObject;
				GameObject iceRingIcon = overlays.Find(o => o.name == "IceRingIcon").transform.parent.gameObject;

				meleeIcon?.SetActive(active);
				forceWandIcon?.SetActive(active);
				dynamiteIcon?.SetActive(active);
				iceRingIcon?.SetActive(active);

				continue;
			}

			kvp.Value.SetActive(active);
		}
	}

	public struct Options
	{
		public static ConfigEntry<bool> skipSplashToggleEntry;
		public static ConfigEntry<bool> runInBackgroundToggleEntry;
		public static ConfigEntry<HUDType> hudToShowEntry;
		public static ConfigEntry<bool> fixFluffyLakeWarpBugEntry;
		public static ConfigEntry<bool> fixResolutionOptionsBugEntry;
		public static ConfigEntry<bool> fixMapManExceptionBugEntry;

		public static bool SkipSplash => skipSplashToggleEntry.Value;
		public static bool FixFluffyLakeWarpBug => fixFluffyLakeWarpBugEntry.Value;
		public static bool FixResolutionOptionsBug => fixResolutionOptionsBugEntry.Value;
		public static bool FixMapManExceptionBug => fixMapManExceptionBugEntry.Value;

		public struct Defaults
		{
			public static bool SkipSplash => false;
			public static bool RunInBackground => false;
			public static HUDType HUDToShow => HUDType.All;
			public static bool FixFluffyLakeWarpBug => false;
			public static bool FixResolutionOptionsBug => true;
			public static bool FixMapManExceptionBug => true;
		}

		public static void RunInBackgroundToggled(bool value)
		{
			RunInBackground = value;
		}
	}

	[HarmonyPatch]
	private static class Patches
	{
		[HarmonyPrefix, HarmonyPatch(typeof(EntityHUD), nameof(EntityHUD.ConnectHealthMeter))]
		private static bool ShowBossHealthbar()
		{
			return ActiveHUDElements.Contains(HUDType.BossHealth);
		}

		[HarmonyPrefix, HarmonyPatch(typeof(ShowTitleEventObserver), nameof(ShowTitleEventObserver.DoShow))]
		private static bool ShowTitleHUD()
		{
			return ActiveHUDElements.Contains(HUDType.AreaTitles);
		}

		[HarmonyPrefix, HarmonyPatch(typeof(Sign), nameof(Sign.Show))]
		private static bool SignShowHUD()
		{
			return ActiveHUDElements.Contains(HUDType.Signs);
		}

		[HarmonyPrefix, HarmonyPatch(typeof(EntityHUD), nameof(EntityHUD.GotItem))]
		private static bool GotItemHUD(EntityHUD __instance, Item item)
		{
			if (!__instance.ShouldShow(item.ItemId))
				return false;

			return ActiveHUDElements.Contains(HUDType.GetItemMessage);
		}

		[HarmonyPrefix, HarmonyPatch(typeof(EntityHUD), nameof(EntityHUD.VariableUpdated))]
		private static bool WeaponIconUpdate(string var)
		{
			return var switch
			{
				"melee" or "forcewand" or "dynamite" or "icering" => ActiveHUDElements.Contains(HUDType.Items),
				_ => true,
			};
		}

		[HarmonyPostfix, HarmonyPatch(typeof(OverlayWindow), nameof(OverlayWindow.SetupOverlay))]
		private static void HUDOverlaySetup(OverlayWindow __instance)
		{
			HUDObjectSetup(__instance);
		}

		// Fixes vanilla "Fluffy lake warp" bug where the respawn override for intro dialogue would
		// continue to override the spawn position anytime you loaded into Fluffy room A, causing a
		// pause warp to return you to lake instead of last entrance you came from.
		[HarmonyPrefix, HarmonyPatch(typeof(ChangeRespawnerEventObserver), nameof(ChangeRespawnerEventObserver.DoChange))]
		private static bool FixFluffyLakeWarpBug(ChangeRespawnerEventObserver __instance)
		{
			if (Options.FixFluffyLakeWarpBug && SceneManager.GetActiveScene().name == "FluffyFields" && __instance._isDefault)
				return ModCore.Globals.MainSaver.LevelStorage.GetLocalSaver("A").LoadInt("IntroDialog-39--26") == 0;

			return true;
		}

		// Fixes vanilla bug where list of resolution options in slider can get messed up
		// the original bug was caused by implicitly casting the index equation to int instead of flooring it
		// so depending on how many resolution options player had, it could skip some or repeat some indices
		// in slider list options.
		[HarmonyPrefix, HarmonyPatch(typeof(GuiSliderComboListApplier), nameof(GuiSliderComboListApplier.SliderChanged))]
		private static bool GuiSliderComboListApplier_SliderChanged_Patch(GuiSliderComboListApplier __instance, float value)
		{
			if (!Options.FixResolutionOptionsBug)
				return true;

			int index = Mathf.FloorToInt(value * (__instance.currChildren.Count - 1));

			if (index < 0 && index >= __instance.currChildren.Count)
				return false;

			__instance._valueNode?.ApplyContent(__instance.GetActiveInData()[index], true);

			GameObject child = __instance.currChildren[index];
			GuiClickable clickable = child.GetComponentInChildren<GuiClickable>();
			clickable.SendClick();

			return false;
		}

		// Fixes vanilla "mapmandone" event causing exception to be logged for no reason, which always
		// confuses me at first like "oh no, what did I break? Oh, it's just Mapman error. So this just
		// removes that exception log.
		[HarmonyPrefix, HarmonyPatch(typeof(GenericGameEvents), nameof(GenericGameEvents.DoSend))]
		private static bool FixMapManException(List<GenericGameEvents.EventFunc> funcs, object data)
		{
			if (!Options.FixMapManExceptionBug)
				return true;

			for (int i = 0; i < funcs.Count; i++)
			{
				try
				{
					if (funcs[i] != null)
						funcs[i](data);
				}
				catch { }
			}

			return false;
		}
	}
}
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using System.Reflection;

namespace SkipFacts;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;
    public static ManualLogSource Log;

    private void Awake()
    {
        Instance = this;
        Log = Logger;
        new Harmony(PluginInfo.PLUGIN_GUID).PatchAll();
    }
}

[HarmonyPatch(typeof(LevelSelectController), nameof(LevelSelectController.clickPlay))]
public class LevelSelectControllerPlayPatch : MonoBehaviour
{
    static bool Prefix(LevelSelectController __instance)
    {
        if (__instance.back_clicked) return false;
        if (IsConnectedToMultiplayer()) return true;
        __instance.back_clicked = true;
        __instance.bgmus.Stop();
        __instance.clipPlayer.cancelCrossfades();
        __instance.doSfx(__instance.sfx_musend);
        if (__instance.alltrackslist[__instance.songindex].json_format)
        {
            GlobalVariables.playing_custom_track = true;
            GlobalVariables.customtrack_paths = null;
        }
        else
            GlobalVariables.playing_custom_track = false;
        GlobalVariables.levelselect_index = __instance.songindex;
        GlobalVariables.chosen_track = __instance.alltrackslist[__instance.songindex].trackref;
        GlobalVariables.chosen_track_data = __instance.alltrackslist[__instance.songindex];
        __instance.fadeOut("gameplay", 0.4f);
        return false;
    }

    private static bool IsConnectedToMultiplayer()
    {
        Type multi = Type.GetType("TootTallyMultiplayer.MultiplayerManager, TootTallyMultiplayer");
        if (multi == null) return false;
        var prop = multi.GetProperty("IsConnectedToMultiplayer");
        return prop != null && (bool)prop.GetValue(multi, null);
    }
}

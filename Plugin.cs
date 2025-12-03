using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

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
        if (__instance.back_clicked)
            return false;
        __instance.back_clicked = true;
        __instance.bgmus.Stop();
        __instance.clipPlayer.cancelCrossfades();
        __instance.doSfx(__instance.sfx_musend);
        LeanTween.moveX(__instance.playbtnobj, 640f, 0.6f).setEaseInQuart();
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
        __instance.fadeOut("gameplay", 0.65f);
        return false;
    }
}

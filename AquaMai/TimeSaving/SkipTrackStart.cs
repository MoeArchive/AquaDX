﻿using HarmonyLib;
using Monitor;

namespace AquaMai.TimeSaving;

public class SkipTrackStart
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof (TrackStartMonitor), "IsEnd")]
    public static bool IsEnd(ref bool __result)
    {
        __result = true;
        return false;
    }
}

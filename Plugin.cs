using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.VR;

[BepInPlugin("com.ctm.patchdebugcameracontrol", "patch.DebugCameraControl", "1.0.0")]
public class VRDebugModPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo("DebugCameraControl patch - https://github.com/CoachsTimeMachine/patch.debugcameracontrol");
        var harmony = new Harmony("com.ctm.patchdebugcameracontrol");
        harmony.PatchAll();
        Logger.LogInfo("Patched all methods!");
    }
}

//Always return true on VRDevice.isPresent to make the game think a VR headset is always connected.
[HarmonyPatch(typeof(VRDevice), nameof(VRDevice.isPresent), MethodType.Getter)]
public class Patch_VRDevice_isPresent
{
    static bool Prefix(ref bool __result)
    {
        __result = true;
        return false;
    }
}

//Always return debug input from ControllerIO.Update by calling LMAGOHCBOGK() directly and skipping the original Update logic.
[HarmonyPatch(typeof(ControllerIO), "Update")]
public class Patch_ControllerIO_Update
{
    static bool Prefix(ControllerIO __instance)
    {
        AccessTools.Method(typeof(ControllerIO), "LMAGOHCBOGK")
                    .Invoke(__instance, null);
        return false;
    }
}

// Force set DebugCameraControl.Awake flag to be true to prevent it from destroying itself (enabling DebugCameraControl).
[HarmonyPatch(typeof(DebugCameraControl), "Awake")]
public class Patch_DebugCameraControl_Awake
{
    static bool Prefix(DebugCameraControl __instance)
    {
        // Skip the original Awake (which would Destroy the component).
        // Set IBFMMGKIJME = true so the free-cam starts enabled immediately.
        AccessTools.Property(typeof(DebugCameraControl), "IBFMMGKIJME")
                   .SetValue(__instance, true, null);
        return false;
    }
}
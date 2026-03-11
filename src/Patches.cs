using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace RegentWithASteelPipe;

[HarmonyPatch(typeof(SfxCmd), nameof(SfxCmd.Play), new[] { typeof(string), typeof(float) })]
internal static class SfxCmdPlayPatch
{
    private const string ForgeSfx = "event:/sfx/characters/regent/regent_forge";

    private const string RefineSfx = "event:/sfx/characters/regent/regent_refine";

    private const string SovereignBladeSfx = "event:/sfx/characters/regent/regent_sovereign_blade";

    private static bool Prefix(string sfx, float volume)
    {
        if (!string.Equals(sfx, ForgeSfx, StringComparison.Ordinal) &&
            !string.Equals(sfx, RefineSfx, StringComparison.Ordinal) &&
            !string.Equals(sfx, SovereignBladeSfx, StringComparison.Ordinal))
        {
            return true;
        }

        return !PipeSfxService.TryPlay(volume);
    }
}

[HarmonyPatch(typeof(CardModel), "get_Portrait")]
internal static class SovereignBladePortraitPatch
{
    private static void Postfix(CardModel __instance, ref Texture2D __result)
    {
        if (__instance is not SovereignBlade)
        {
            return;
        }

        __result = CustomAssetService.CardPortrait ?? __result;
    }
}

[HarmonyPatch(typeof(NSovereignBladeVfx), nameof(NSovereignBladeVfx._Ready))]
internal static class SovereignBladeVfxReadyPatch
{
    private static void Postfix(NSovereignBladeVfx __instance)
    {
        PipeBladeVisualService.Initialize(__instance);
    }
}

[HarmonyPatch(typeof(NSovereignBladeVfx), nameof(NSovereignBladeVfx._Process))]
internal static class SovereignBladeVfxProcessPatch
{
    private static void Postfix(NSovereignBladeVfx __instance)
    {
        PipeBladeVisualService.Update(__instance);
    }
}

using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace RegentWithASteelPipe;

[ModInitializer(nameof(Initialize))]
internal static class ModEntry
{
    internal const string ModId = "codex.regent_with_a_steel_pipe";

    public static void Initialize()
    {
        new Harmony(ModId).PatchAll(Assembly.GetExecutingAssembly());
        Console.WriteLine("[RegentWithASteelPipe] Initialized.");
    }
}

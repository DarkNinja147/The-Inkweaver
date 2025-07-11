#pragma warning disable IDE0130 // Namespace does not match folder structure
using MonoMod.Utils;

namespace Inkweaver;

public static class Hooks
{
    private static bool IsInit { get; set; }

    public static void ApplyInitHooks()
    {
        On.RainWorld.OnModsInit += RainWorld_OnModsInit;
    }
    private static void ApplyHooks()
    {
        SlideShow_HooksIL.ApplyHooks();
        Player_Hooks.ApplyHooks();
    }
    private static void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        try
        {
            if (IsInit)
            {
                return;
            }

            IsInit = true;
            Enums.InitEnums();
            ApplyHooks();
        }
        catch (Exception e)
        {
            e.LogDetailed();
        }
        finally
        {
            orig(self);
        }
    }
}
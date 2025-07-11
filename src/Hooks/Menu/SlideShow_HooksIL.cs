#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CA2201 // Do not raise reserved exception types
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Inkweaver;

internal class SlideShow_HooksIL
{
    public static void ApplyHooks()
    {
        try
        {
            IL.MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding.Update += OE_GourmandEnding_Update;
            IL.MoreSlugcats.MSCRoomSpecificScript.OE_NPCControl.Update += IL_OE_NPCControl_Update;
            On.MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding.Update += On_OE_GourmandEnding_Update;
            IL.RainWorldGame.BeatGameMode += RainWorldGame_BeatGameMode;
            IL.RainWorldGame.GoToRedsGameOver += RainWorldGame_GoToRedsGameOver;
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError(ex);
        }
    }

    private static void RainWorldGame_BeatGameMode(ILContext il)
    {
        try
        {
            ILCursor cursor = new(il);

            if (cursor.TryGotoNext(
                MoveType.After,
                x => x.MatchStfld<SaveState>(nameof(SaveState.skipNextCycleFoodDrain))))
            {
                cursor.Emit(OpCodes.Ldarg_0); // Emits the instance of RainWorldGame onto the stack
                cursor.Emit(OpCodes.Ldloc_0); // Emits the text local variable onto the stack
                static string SaveDenLocation(RainWorldGame game, string text)
                {
                    Plugin.Logger.LogWarning("attempting to save!");
                    if (game.StoryCharacter == Enums.Inkweaver)
                    {
                        Plugin.Logger.LogWarning("Saving!");
                        game.GetStorySession.saveState.deathPersistentSaveData.altEnding = true;
                        game.GetStorySession.saveState.deathPersistentSaveData.ascended = false;
                        game.GetStorySession.saveState.deathPersistentSaveData.karma = game.GetStorySession.saveState.deathPersistentSaveData.karmaCap;
                        text = "OE_SEXTRA";
                    }
                    else
                    {
                        Plugin.Logger.LogWarning("Not Inkweaver!");
                    }
                    return text;
                }
                cursor.EmitDelegate(SaveDenLocation);

                Plugin.Logger.LogDebug(il);
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError(ex);
        }
    }

    private static void On_OE_GourmandEnding_Update(On.MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding.orig_Update orig, MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding self, bool eu)
    {
        if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.altEnding)
        {
            self.Destroy();
            return;
        }
        else
        {
            orig(self, eu);
        }
    }

    static bool IsNotGourmandOrInkweaver(bool isNotGourmand, MoreSlugcats.MSCRoomSpecificScript.OE_NPCControl self) => isNotGourmand && self.room.game.rainWorld.inGameSlugCat != Enums.Inkweaver;

    private static void IL_OE_NPCControl_Update(ILContext il)
    {
        ILCursor c = new(il);
        try
        {
            c.GotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Inequality")));

            c.MoveAfterLabels();
            c.Emit(OpCodes.Ldarg, 0);
            c.EmitDelegate(IsNotGourmandOrInkweaver);

            Plugin.Logger.LogDebug(il);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError(ex);
        }
    }

    static bool IsGourmandOrInkweaver(bool isGourmand, MoreSlugcats.MSCRoomSpecificScript.OE_GourmandEnding self) => isGourmand || self.room.game.rainWorld.inGameSlugCat == Enums.Inkweaver;
    private static void OE_GourmandEnding_Update(ILContext il)
    {
        ILCursor c = new(il);
        try
        {
            for (int i = 0; i < 2; i++)
            {
                c.GotoNext(MoveType.After,
                    x => x.MatchCallOrCallvirt(typeof(ExtEnum<SlugcatStats.Name>).GetMethod("op_Equality")));

                c.MoveAfterLabels();
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate(IsGourmandOrInkweaver);
            }
            Plugin.Logger.LogDebug(il);
        }
        catch (Exception ex)
        {
            Plugin.Logger.LogError(ex);
        }
    }
    private static void RainWorldGame_GoToRedsGameOver(ILContext il)
    {
        var c = new ILCursor(il);

        if (!c.TryGotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt<PlayerProgression>(nameof(PlayerProgression.SaveWorldStateAndProgression)),
                x => x.MatchPop()))
        {
            throw new Exception("Goto Failed");
        }

        c.Emit(OpCodes.Ldarg_0);
        c.EmitDelegate<Action<RainWorldGame>>((self) =>
        {
            if (self.GetStorySession?.saveStateNumber != Enums.Inkweaver)
            {
                return;
            }

            self.GetStorySession.saveState.deathPersistentSaveData.altEnding = true;
            self.manager.nextSlideshow = Enums.Scenes.Outro_Inkweaver_B;
            self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.SlideShow);
        });
    }
}

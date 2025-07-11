#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable CA1707 // Identifiers should not contain underscores

using BepInEx.Logging;
using MoreSlugcats;
using SlugBase.Features;
using static SlugBase.Features.FeatureTypes;

namespace Inkweaver;

internal class Player_Hooks
{
    public static readonly PlayerFeature<bool> NewRoom_save = PlayerBool("inkweaver/NewRoom_save");
    public static readonly PlayerFeature<bool> NewRoom_robo = PlayerBool("inkweaver/NewRoom_robo");
    public static readonly PlayerFeature<bool> ctor = PlayerBool("inkweaver/ctor");
    public static readonly PlayerFeature<bool> IsInkweaver = PlayerBool("inkweaver/is_inkweaver");
    public static void ApplyHooks()
    {
        On.Player.ctor += Inkweaver_ctor;
        On.Player.NewRoom += Inkweaver_NewRoom;
    }

    private static void Inkweaver_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
    {
        orig(self, newRoom);
        try
        {
            if (IsInkweaver.TryGet(self, out bool isInkweaver) && isInkweaver)
            {
                if (NewRoom_save.TryGet(self, out bool save) && save)
                {
                    //Logger.Log(BepInEx.Logging.LogLevel.Info, "SaveOnStart is "+ SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(newRoom.game.GetStorySession.saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool sav1) + sav1);
                    //Logger.Log(BepInEx.Logging.LogLevel.Info, "hasSavedOnWall is "+ SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(newRoom.game.GetStorySession.saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool sav2).ToString());
                    if (!SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(newRoom.game.GetStorySession.saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool saved) || saved == false)
                    {
                        //Logger.Log(BepInEx.Logging.LogLevel.Info, "Attempting to load the next statement.");
                        if (String.Equals(newRoom.roomSettings.name, "UW_A12", StringComparison.OrdinalIgnoreCase))
                        {
                            Plugin.Logger.Log(LogLevel.Info, "Reached save point.");
                            Plugin.Logger.Log(LogLevel.Info, newRoom.game.GetStorySession.saveState.deathPersistentSaveData.SaveToString(false, false));
                            //newRoom.game.GetStorySession.saveState.deathPersistentSaveData.AddDeathPosition(newRoom, new Vector2(1f, 1f));
                            //RainWorldGame.ForceSaveNewDenLocation(newRoom.game, "GATE_UW_LC", true);
                            SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(newRoom.game.GetStorySession.saveState.deathPersistentSaveData).Set<bool>("hasSavedOnWall", true);
                        }
                    }
                }
                if (NewRoom_robo.TryGet(self, out bool robo) && robo)
                {
                    if (newRoom.game.IsStorySession)
                    {
                        var inkweaverAncientBot = new AncientBot(self.mainBodyChunk.pos, new Color(0.2f, 0f, 1f), self, true);
                        self.myRobot = inkweaverAncientBot;
                        if (SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(newRoom.game.GetStorySession.saveState.deathPersistentSaveData).TryGet<string>("currentlyLoadedRoom", out string loaded) && loaded != newRoom.roomSettings.name)
                        {
                            self.room.AddObject(self.myRobot);
                        }
                        SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(newRoom.game.GetStorySession.saveState.deathPersistentSaveData).Set<string>("currentlyLoadedRoom", newRoom.roomSettings.name);
                        if (newRoom.game.GetStorySession.saveState.hasRobo != true)
                        {
                            newRoom.game.GetStorySession.saveState.hasRobo = true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Plugin.Logger.Log(LogLevel.Error, ex.Message);
        }
    }
    private static void Inkweaver_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
    {
        orig(self, abstractCreature, world);
        if (IsInkweaver.TryGet(self, out bool isInkweaver) && isInkweaver)
        {

            if (self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon != true)
            {
                self.room.game.GetStorySession.saveState.deathPersistentSaveData.ripMoon = true;
            }
        }
    }
}
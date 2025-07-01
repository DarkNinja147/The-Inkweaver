using BepInEx;
using MoreSlugcats;
using RWCustom;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace Inkweaver
{
    [BepInPlugin("darkninja.inkweaver", "Inkweaver Scug", "0.3.0")]
    class Plugin : BaseUnityPlugin
    {
        public static readonly PlayerFeature<bool> NewRoom_save = PlayerBool("inkweaver/NewRoom_save");
        public static readonly PlayerFeature<bool> NewRoom_robo = PlayerBool("inkweaver/NewRoom_robo");
        public static readonly PlayerFeature<bool> ctor = PlayerBool("inkweaver/ctor");
        public static readonly PlayerFeature<bool> IsInkweaver = PlayerBool("inkweaver/is_inkweaver");

        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            On.Player.ctor += Inkweaver_ctor;
            On.Player.NewRoom += Inkweaver_NewRoom;
        }

        private void LoadResources(RainWorld rainWorld) { }

        private void Inkweaver_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
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
                                Logger.Log(BepInEx.Logging.LogLevel.Info, "Reached save point.");
                                Logger.Log(BepInEx.Logging.LogLevel.Info, newRoom.game.GetStorySession.saveState.deathPersistentSaveData.SaveToString(false, false));
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
            catch (Exception exception)
            {
                Logger.Log(BepInEx.Logging.LogLevel.Error, exception.Message);
            }
        }
        private void Inkweaver_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
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
        private static void SpawnArti(RainWorldGame game, WorldCoordinate pos) {
            AbstractCreature abstractCreature = new(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Slugcat), null, pos, game.GetNewID());
            abstractCreature.state = new PlayerState(abstractCreature, 0, MoreSlugcatsEnums.SlugcatStatsName.Artificer, false);
            IntVector2 foodNeeded = SlugcatStats.SlugcatFoodMeter(MoreSlugcatsEnums.SlugcatStatsName.Artificer);
            Player pl2 = new(abstractCreature, game.world)
            {
                SlugCatClass = MoreSlugcatsEnums.SlugcatStatsName.Artificer
            };
            pl2.slugcatStats.name = MoreSlugcatsEnums.SlugcatStatsName.Artificer;
            pl2.playerState.slugcatCharacter = MoreSlugcatsEnums.SlugcatStatsName.Artificer;
            pl2.setPupStatus(false);
            pl2.playerState.forceFullGrown = true;
            pl2.slugcatStats.maxFood = foodNeeded.x;
            pl2.slugcatStats.foodToHibernate = foodNeeded.y;
            pl2.playerState.foodInStomach = pl2.slugcatStats.maxFood;
            //pl2.input = new Player.InputPackage[10];
        }
        public static SlugcatStats.Name SlugName = new("Inkweaver", false);
    }
}
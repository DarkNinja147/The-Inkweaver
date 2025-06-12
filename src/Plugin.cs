using BepInEx;
using MoreSlugcats;
using SlugBase.Features;
using System;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace Inkweaver
{
    [BepInPlugin(MOD_ID, "Inkweaver Scug", "0.1.1")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "darkninja.inkweaver";

        public static readonly PlayerFeature<bool> NewRoom = PlayerBool("inkweaver/NewRoom");
        public static readonly PlayerFeature<bool> ctor = PlayerBool("inkweaver/ctor");
        public static readonly PlayerFeature<bool> IsInkweaver = PlayerBool("inkweaver/is_inkweaver");

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            On.Player.ctor += Inkweaver_ctor;
            On.Player.NewRoom += Inkweaver_NewRoom;
        }


        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld) { }

        // Implement IsInkweaver
        private void Inkweaver_NewRoom(On.Player.orig_NewRoom orig, Player self, Room newRoom)
        {
            orig(self, newRoom);
            try
            {
                if (IsInkweaver.TryGet(self, out bool isInkweaver) && isInkweaver)
                {
                    if (NewRoom.TryGet(self, out bool save) && save)
                    {
                        //Logger.Log(BepInEx.Logging.LogLevel.Info, "SaveOnStart is "+ SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool sav1) + sav1);
                        //Logger.Log(BepInEx.Logging.LogLevel.Info, "hasSavedOnWall is "+ SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool sav2).ToString());
                        if (!SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool saved) || saved == false)
                        {
                            //Logger.Log(BepInEx.Logging.LogLevel.Info, "Attempting to load the next statement.");
                            if (newRoom.roomSettings.name.ToUpper() == "UW_A12")
                            {
                                Logger.Log(BepInEx.Logging.LogLevel.Info, "Attempting to save..");
                                Logger.Log(BepInEx.Logging.LogLevel.Info, (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.SaveToString(false, false).ToString());
                                SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).Set<bool>("hasSavedOnWall", true);
                                RainWorldGame.ForceSaveNewDenLocation(self.room.game, "UW_A12", true);
                            }
                        }
                    }
                    if (NewRoom.TryGet(self, out bool robo) && robo)
                    {
                        if (self.room.game.IsStorySession)
                        {
                            self.myRobot = new AncientBot(new Vector2(470f, 1790f), new Color(0.2f, 0f, 1f), self, true);
                            self.room.AddObject(self.myRobot);
                            if ((self.room.game.session as StoryGameSession).saveState.hasRobo != true)
                            {
                                (self.room.game.session as StoryGameSession).saveState.hasRobo = true;
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
            throw new NotImplementedException();
        }
    }
}
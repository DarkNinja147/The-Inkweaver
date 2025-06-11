using BepInEx;
using MoreSlugcats;
using SlugBase.Features;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace Inkweaver
{
    [BepInPlugin(MOD_ID, "Inkweaver Scug", "0.1.1")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "darkninja.inkweaver";

        public static readonly PlayerFeature<bool> StartWithRobo = PlayerBool("inkweaver/start_with_robo");
        public static readonly PlayerFeature<bool> SaveOnStart = PlayerBool("inkweaver/save_on_start");

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            On.Player.ctor += Set_Robo;
            On.Player.ctor += Save_On_Start;
        }


        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld) { }

        // Implement SaveOnStart
        private void Save_On_Start(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (SaveOnStart.TryGet(self, out bool save) && save)
            {
                if (SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool saved) && !saved)
                {
                    if (self.room.roomSettings.name.ToUpper() == "UW_A12")
                    {
                        (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.SaveToString(false, false);
                        SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).Set<bool>("hasSavedOnWall", true);
                        //SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(savedata).Set<DeathPersistentSaveData>("savedata", savedata);
                    }
                }
            }
        }

        // Implement StartWithRobo
        private void Set_Robo(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (StartWithRobo.TryGet(self, out bool robo) && robo)
            {
                if (self.room.game.IsStorySession)
                {
                    self.myRobot = new AncientBot(new Vector2(470f, 1790f), new Color(0.2f, 0f, 1f), self, true);
                    if ((self.room.game.session as StoryGameSession).saveState.hasRobo != true)
                    {
                        self.room.AddObject(self.myRobot);
                        (self.room.game.session as StoryGameSession).saveState.hasRobo = true;
                        //RainWorldGame.ForceSaveNewDenLocation(self.room.game, "UW_A12", true);
                    }
                }
            }
        }
    }
}
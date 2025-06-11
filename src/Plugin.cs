using BepInEx;
using JetBrains.Annotations;
using MoreSlugcats;
using SlugBase.Features;
using SlugBase.SaveData;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;

namespace SlugTemplate
{
    [BepInPlugin(MOD_ID, "Inkweaver Scug", "0.1.1")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "darkninja.inkweaver";

        public static readonly PlayerFeature<float> SuperJump = PlayerFloat("inkweaver/super_jump");
        public static readonly PlayerFeature<bool> ExplodeOnDeath = PlayerBool("inkweaver/explode_on_death");
        public static readonly GameFeature<float> MeanLizards = GameFloat("inkweaver/mean_lizards");
        public static readonly PlayerFeature<bool> StartWithRobo = PlayerBool("inkweaver/start_with_robo");
        public static readonly PlayerFeature<bool> SaveOnStart = PlayerBool("inkweaver/save_on_start");

        // Add hooks
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);

            // Put your custom hooks here!
            On.Player.Jump += Player_Jump;
            On.Player.Die += Player_Die;
            On.Lizard.ctor += Lizard_ctor;
            On.Player.ctor += Set_Robo;
            //On.Player.ctor += Save_On_Start;
        }



        // Load any resources, such as sprites or sounds
        private void LoadResources(RainWorld rainWorld) { }

        /*// Implement SaveOnStart
        private void Save_On_Start(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (SaveOnStart.TryGet(self, out bool save) && save)
            {
                if (SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).TryGet<bool>("hasSavedOnWall", out bool saved) && saved)
                {
                    if (self.room.roomSettings.name == "UW_A12")
                    {
                        (self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData.SaveToString(false, false);
                        SlugBase.SaveData.SaveDataExtension.GetSlugBaseData((self.room.game.session as StoryGameSession).saveState.deathPersistentSaveData).Set<bool>("hasSavedOnWall",true);
                        //SlugBase.SaveData.SaveDataExtension.GetSlugBaseData(savedata).Set<DeathPersistentSaveData>("savedata", savedata);
                    }
                }
            }
        }*/

        // Implement StartWithRobo
        private void Set_Robo(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            if (StartWithRobo.TryGet(self, out bool robo) && robo)
            {
                orig(self, abstractCreature, world);
                if (self.room.game.IsStorySession)
                {
                    if ((self.room.game.session as StoryGameSession).saveState.hasRobo != true)
                    {
                        self.myRobot = new AncientBot(new Vector2(500f, 0f), new Color(0.2f, 0f, 1f), self, true);
                        self.room.AddObject(self.myRobot);
                        (self.room.game.session as StoryGameSession).saveState.hasRobo = true;
                    }
                }
            }
        }

        // Implement MeanLizards
        private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            if (MeanLizards.TryGet(world.game, out float meanness))
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
        }


        // Implement SuperJump
        private void Player_Jump(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            if (SuperJump.TryGet(self, out var power))
            {
                self.jumpBoost *= 1f + power;
            }
        }

        // Implement ExlodeOnDeath
        private void Player_Die(On.Player.orig_Die orig, Player self)
        {
            bool wasDead = self.dead;

            orig(self);

            if (!wasDead && self.dead
                && ExplodeOnDeath.TryGet(self, out bool explode)
                && explode)
            {
                // Adapted from ScavengerBomb.Explode
                var room = self.room;
                var pos = self.mainBodyChunk.pos;
                var color = self.ShortCutColor();
                room.AddObject(new Explosion(room, self, pos, 7, 250f, 6.2f, 2f, 280f, 0.25f, self, 0.7f, 160f, 1f));
                room.AddObject(new Explosion.ExplosionLight(pos, 280f, 1f, 7, color));
                room.AddObject(new Explosion.ExplosionLight(pos, 230f, 1f, 3, new Color(1f, 1f, 1f)));
                room.AddObject(new ExplosionSpikes(room, pos, 14, 30f, 9f, 7f, 170f, color));
                room.AddObject(new ShockWave(pos, 330f, 0.045f, 5, false));

                room.ScreenMovement(pos, default, 1.3f);
                room.PlaySound(SoundID.Bomb_Explode, pos);
                room.InGameNoise(new Noise.InGameNoise(pos, 9000f, self, 1f));
            }
        }
    }
}
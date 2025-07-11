global using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using MoreSlugcats;
using RWCustom;

namespace Inkweaver;

[BepInDependency("slime-cubed.slugbase")] // SlugBase
[BepInPlugin(MOD_ID, "The Inkweaver", "0.3.0")]
internal class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "darkninja.inkweaver";
    public static new ManualLogSource Logger { get; private set; } = null!;

    public void OnEnable()
    {
        Logger = base.Logger;
        On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
        Hooks.ApplyInitHooks();
    }

    private void LoadResources(RainWorld rainWorld) { }
    private static void SpawnArti(RainWorldGame game, WorldCoordinate pos)
    {
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
}

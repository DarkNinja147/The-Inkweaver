using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

/*
 * This file contains fixes to some common problems when modding Rain World.
 * Unless you know what you're doing, you shouldn't modify anything here.
 */

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: IgnoresAccessChecksTo("Assembly-CSharp")]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[module: UnverifiableCode]
#pragma warning restore CS0618 // Type or member is obsolete


internal static class Extras
{
    private static bool _initialized;

    // Ensure resources are only loaded once and that failing to load them will not break other mods
    public static On.RainWorld.hook_OnModsInit WrapInit(Action<RainWorld> loadResources)
    {
        return (orig, self) =>
        {
            orig(self);
            try
            {
                if (!_initialized)
                {
                    _initialized = true;
                    loadResources(self);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        };
    }
}
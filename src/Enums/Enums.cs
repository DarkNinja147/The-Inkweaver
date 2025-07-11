namespace Inkweaver;

public static partial class Enums
{
    public static SlugcatStats.Name Inkweaver { get; } = new("Inkweaver");
    //public static SlugcatStats.Name AAAAAA { get; } = SlugBase.SlugBaseCharacter.Get(new SlugcatStats.Name("Inkweaver")).Name;
    public static void InitEnums()
    {
        _ = Inkweaver;
    }
}
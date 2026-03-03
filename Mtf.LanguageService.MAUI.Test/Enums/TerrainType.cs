using System.ComponentModel;

namespace Mtf.LanguageService.MAUI.Test.Enums;

[Flags]
public enum TerrainType : ulong
{
    Unknown = 0,

    Plains = 1 << 0,

    Snowfield = 1 << 1,

    Hills = 1 << 2,

    Mountains = 1 << 3,

    Swamp = 1 << 4,

    Desert = 1 << 5,

    Urban = 1 << 6,

    Water = 1 << 7,

    Crevice = 1 << 8,

    Tunnels = 1 << 9,

    Borderlands = 1 << 10,

    Riverbank = 1 << 11,

    Stronghold = 1 << 12,

    Cave = 1 << 13,

    Underground = 1 << 14,

    [Description("Arctic forest")]
    ArcticForest = 1 << 15,

    [Description("Temperate forest")]
    TemperateForest = 1 << 16,

    [Description("Austral cold region")]
    AustralColdRegion = 1 << 17,

    [Description("Tropical river")]
    TropicalRiver = 1 << 18,

    [Description("Tropical forest")]
    TropicalForest = 1 << 19,

    [Description("Ice field")]
    Icefield = 1 << 20,

    [Description("Abbit mines")]
    AbbitMines = 1 << 21,

    [Description("Iron ore mines")]
    IronOreMines = 1 << 22,

    [Description("Coal mines")]
    CoalMines = 1 << 23,

    [Description("Gold mines")]
    GoldMines = 1 << 24,

    [Description("Copper mines")]
    CopperMines = 1 << 25,

    [Description("Silver mines")]
    SilverMines = 1 << 26,

    [Description("Gemstone mines")]
    GemstoneMines = 1 << 27,

    [Description("Cursed Land")]
    CursedLand = 1 << 28,

    [Description("Inner Hall")]
    InnerHall = 1 << 29,

    [Description("Volcanic shaft")]
    VolcanicShaft = 1 << 30,

    [Description("Subterranean river")]
    SubterraneanRiver = 1UL << 31,

    [Description("Inner chamber")]
    InnerChamber = 1 << 32,

    [Description("Coastal barren")]
    CoastalBarren = 1 << 33,

    [Description("Coastal island")]
    CoastalIsland = 1 << 34,

    [Description("Old, dilapidated building")]
    OldDilapidatedBuilding = 1 << 35,

    [Description("Tree hollow")]
    TreeHollow = 1 << 36,

    [Description("Deep underground")]
    DeepUnderground = Underground | Cave | Crevice | InnerChamber | VolcanicShaft | SubterraneanRiver,

    Forest = ArcticForest | TemperateForest | TropicalForest,

    Mines = AbbitMines | IronOreMines | CoalMines | GoldMines | CopperMines | SilverMines | GemstoneMines,

    Anywhere = Plains | Forest | Hills | Mines | Mountains | Swamp | Desert | Urban | Water | OldDilapidatedBuilding | TreeHollow | AustralColdRegion | Snowfield | CursedLand | DeepUnderground
}
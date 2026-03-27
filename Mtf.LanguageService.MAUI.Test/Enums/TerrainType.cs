using System.ComponentModel;

namespace Mtf.LanguageService.MAUI.Test.Enums;

[Flags]
public enum TerrainType : ulong
{
    Unknown = 0,

    Plains = 1UL << 0,

    Snowfield = 1UL << 1,

    Hills = 1UL << 2,

    Mountains = 1UL << 3,

    Swamp = 1UL << 4,

    Desert = 1UL << 5,

    Urban = 1UL << 6,

    Water = 1UL << 7,

    Crevice = 1UL << 8,

    Tunnels = 1UL << 9,

    Borderlands = 1UL << 10,

    Riverbank = 1UL << 11,

    Stronghold = 1UL << 12,

    Cave = 1UL << 13,

    Underground = 1UL << 14,

    [Description("Arctic forest")]
    ArcticForest = 1UL << 15,

    [Description("Temperate forest")]
    TemperateForest = 1UL << 16,

    [Description("Austral cold region")]
    AustralColdRegion = 1UL << 17,

    [Description("Tropical river")]
    TropicalRiver = 1UL << 18,

    [Description("Tropical forest")]
    TropicalForest = 1UL << 19,

    [Description("Ice field")]
    Icefield = 1UL << 20,

    [Description("Abbit mines")]
    AbbitMines = 1UL << 21,

    [Description("Iron ore mines")]
    IronOreMines = 1UL << 22,

    [Description("Coal mines")]
    CoalMines = 1UL << 23,

    [Description("Gold mines")]
    GoldMines = 1UL << 24,

    [Description("Copper mines")]
    CopperMines = 1UL << 25,

    [Description("Silver mines")]
    SilverMines = 1UL << 26,

    [Description("Gemstone mines")]
    GemstoneMines = 1UL << 27,

    [Description("Cursed Land")]
    CursedLand = 1UL << 28,

    [Description("Inner Hall")]
    InnerHall = 1UL << 29,

    [Description("Volcanic shaft")]
    VolcanicShaft = 1UL << 30,

    [Description("Subterranean river")]
    SubterraneanRiver = 1UL << 31,

    [Description("Inner chamber")]
    InnerChamber = 1UL << 32,

    [Description("Coastal barren")]
    CoastalBarren = 1UL << 33,

    [Description("Coastal island")]
    CoastalIsland = 1UL << 34,

    [Description("Old, dilapidated building")]
    OldDilapidatedBuilding = 1UL << 35,

    [Description("Tree hollow")]
    TreeHollow = 1UL << 36,

    Order = 1UL << 37,

    [Description("Deep underground")]
    DeepUnderground = Underground | Cave | Crevice | InnerChamber | VolcanicShaft | SubterraneanRiver,

    [Description("Forest")]
    Forest = ArcticForest | TemperateForest | TropicalForest,

    [Description("Mines")]
    Mines = AbbitMines | IronOreMines | CoalMines | GoldMines | CopperMines | SilverMines | GemstoneMines,

    [Description("Anywhere")]
    Anywhere = Plains | Forest | Hills | Mines | Mountains | Swamp | Desert | Urban | Water | OldDilapidatedBuilding | TreeHollow | AustralColdRegion | Snowfield | CursedLand | DeepUnderground
}
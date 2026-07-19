using Mtf.Extensions;
using Mtf.LanguageService.Enums;
using Mtf.LanguageService.MAUI.Test.Enums;
using System.Collections.ObjectModel;

namespace Mtf.LanguageService.MAUI.Test;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Lng.DefaultLanguage = Language.Hungarian;
        Translator.Translate(this);
        LoadAvailablePlaces();
    }

    public ObservableCollection<TerrainType> AvailablePlaces { get; } = [];

    public TerrainType SelectedPlace { get; set; } = TerrainType.Order;

    private void LoadAvailablePlaces()
    {
        var values = Enum
            .GetValues<TerrainType>()
            .OrderBy(x => Lng.Elem(x.GetDescription()));

        AvailablePlaces.Clear();
        foreach (var value in values)
        {
            AvailablePlaces.Add(value);
        }
    }
}

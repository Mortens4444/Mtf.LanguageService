using CommunityToolkit.Mvvm.ComponentModel;
using M.A.G.U.S.Qualifications;
using M.A.G.U.S.Qualifications.Combat;
using Mtf.LanguageService.MAUI.Test.Enums;

namespace Mtf.LanguageService.MAUI.Test.ViewModels;

internal partial class QualificationDetailsViewModel : ObservableObject
{
    private QualificationLevel selectedLevel;

    public QualificationDetailsViewModel()
    {
        Qualification = new BlindFighting();

        AvailableLevels = [ QualificationLevel.Base, QualificationLevel.Master ];
        SelectedLevel = Qualification.QualificationLevel;

        UpdateDerived();
    }

    public Qualification Qualification { get; }
    public QualificationLevel[] AvailableLevels { get; }

    public QualificationLevel SelectedLevel
    {
        get => selectedLevel;
        set
        {
            if (SetProperty(ref selectedLevel, value))
            {
                UpdateDerived();
            }
        }
    }

    private bool canLearn;
    public bool CanLearn
    {
        get => canLearn;
        set => SetProperty(ref canLearn, value);
    }

    public int RequiredSp => SelectedLevel == QualificationLevel.Base ? Qualification.QpToBaseQualification : Qualification.QpToMasterQualification;

    private string requiredSpText;
    public string RequiredSpText
    {
        get => requiredSpText;
        set => SetProperty(ref requiredSpText, value);
    }

    private void UpdateDerived()
    {
        requiredSpText = RequiredSp >= 0 ? $"{RequiredSp} {Lng.Elem("SP")}" : Lng.Elem("Not learnable");
        OnPropertyChanged(nameof(RequiredSp));
        OnPropertyChanged(nameof(RequiredSpText));
    }
}

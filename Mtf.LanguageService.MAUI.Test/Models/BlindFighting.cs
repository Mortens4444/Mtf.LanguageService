using Mtf.LanguageService.MAUI.Test.Enums;

namespace M.A.G.U.S.Qualifications.Combat;

public class BlindFighting(QualificationLevel qualificationLevel = QualificationLevel.Base, int level = 1) : Qualification(qualificationLevel, level)
{
    public override string Name => "Blind fighting";

    public override int QpToBaseQualification => 10;

    public override int QpToMasterQualification => 30;
}

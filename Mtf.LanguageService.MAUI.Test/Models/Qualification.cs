using Mtf.LanguageService.MAUI.Test.Enums;

namespace M.A.G.U.S.Qualifications;

public abstract class Qualification
{
    public Qualification(QualificationLevel qualificationLevel = QualificationLevel.Base, int level = 1)
    {
        QualificationLevel = qualificationLevel;
        if (qualificationLevel == QualificationLevel.Base)
        {
            BaseQualificationLevel = level;
            MasterQualificationLevel = 0;
        }
        else
        {
            BaseQualificationLevel = 0;
            MasterQualificationLevel = level;
        }
    }

    public virtual string Category => GetType().Namespace?[(GetType().Namespace.LastIndexOf('.') + 1)..] ?? String.Empty;

    public virtual string Name => GetType().Name;

    public virtual string Description => String.Empty;

    public QualificationLevel QualificationLevel { get; set; }

    public int BaseQualificationLevel { get; private set; }

    public int MasterQualificationLevel { get; set; }

    public int ActualLevel => QualificationLevel == QualificationLevel.Base ? BaseQualificationLevel : MasterQualificationLevel;

    public virtual int QpToBaseQualification { get; }

    public virtual int? QpToMaxBaseQualification { get; }

    public virtual int QpToMasterQualification { get; }

    public override string ToString()
    {
        return Name;
    }
}

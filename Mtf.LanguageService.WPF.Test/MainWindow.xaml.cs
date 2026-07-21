using System.Windows;
using System.Windows.Documents;

namespace Mtf.LanguageService.WPF.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            lv.ItemsSource = new[] { Lng.Elem("Users"), Lng.Elem("Groups") };
            rtb.Document.Blocks.Add(new Paragraph(new Run(Lng.Elem("File name"))));

            Lng.DefaultLanguage = Enums.Language.Arabic;
            Translator.Translate(this);
            FlowDirection = Lng.IsRtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}

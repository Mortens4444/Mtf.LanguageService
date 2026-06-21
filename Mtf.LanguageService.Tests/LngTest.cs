using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mtf.LanguageService.Core;
using Mtf.LanguageService.Enums;

namespace Mtf.LanguageService.Tests
{
    [TestClass]
    public class LngTest
    {
        [TestMethod]
        public void LngElemTest()
        {
            // Test translation to Hungarian
            Assert.AreEqual("Igen", Lng.Elem(Language.Hungarian, "Yes"));
            Assert.AreEqual("óra", Lng.Elem(Language.Hungarian, "hour"));
            Assert.AreEqual("élőholtak", Lng.Elem(Language.Hungarian, "undead"));
            Assert.AreEqual("Élőholtak", Lng.Elem(Language.Hungarian, "Undead"));

            // Test translation to German
            Assert.AreEqual("Ja", Lng.Elem(Language.German, "Yes"));

            // Test some elements to ensure not deleting rows accidentally on sheets
            Assert.AreEqual("WMI Adattár", Lng.Elem(Language.Hungarian, "WMI Repository"));
            Assert.AreEqual("WMI 存储库", Lng.Elem(Language.Chinese, "WMI Repository"));
            Assert.AreEqual("WMI-Repository", Lng.Elem(Language.German, "WMI Repository"));

            // Test translation with non-existing translate
            Assert.AreEqual("Yes", Lng.Elem(Language.Albanian, "Yes"));

            // Test translation with non-existing language element
            Assert.AreEqual("Non-existing language element", Lng.Elem(Language.German, "Non-existing language element"));

            Lng.DefaultLanguage = Language.Hungarian;
            Assert.AreEqual("WMI Adattár", Lng.Elem("WMI Repository"));

            Lng.DefaultLanguage = Language.Chinese;
            Assert.AreEqual("WMI 存储库", Lng.Elem("WMI Repository"));

            // Test translation from Hungarian to English
            var translateFromHungarian = Lng.Translate(Language.Hungarian, "Ismétlődés", Language.English);
            Assert.AreEqual("Repetition", translateFromHungarian);

            // Test translation from Hungarian to Hungarian
            translateFromHungarian = Lng.Translate(Language.Hungarian, "Ismétlődés", Language.Hungarian);
            Assert.AreEqual("Ismétlődés", translateFromHungarian);
        }
    }
}

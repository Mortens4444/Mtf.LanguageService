using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mtf.LanguageService.Ods;
using Mtf.LanguageService;
using System;
using System.IO;

namespace Mtf.LanguageService.Tests
{
    [TestClass]
    public class TranslatorAndOdsTests
    {
        [TestMethod]
        public void OdsLanguageElementLoader_Loads_From_Stream()
        {
            // Languages.ods is copied to the test output by the test project file
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages.ods");
            Assert.IsTrue(File.Exists(path), "Languages.ods must be present in test output for this test.");

            var loader = new OdsLanguageElementLoader();
            using var fs = File.OpenRead(path);
            var dict = loader.LoadElements(fs);

            Assert.IsNotNull(dict);
            Assert.IsTrue(dict.Count > 0, "Expected at least one language element parsed from Languages.ods");
        }

        [TestMethod]
        public void Lng_AllLanguageElements_IsAccessible()
        {
            var all = Lng.AllLanguageElements;
            Assert.IsNotNull(all);
            Assert.IsTrue(all.Count > 0, "AllLanguageElements should contain entries after lazy load.");
        }
    }
}

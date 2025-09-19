using Mtf.LanguageService.Enums;
using Mtf.LanguageService.Interfaces;
using Mtf.LanguageService.Models;
using System.Data;

namespace Mtf.LanguageService.Ods
{
    public class OdsLanguageElementLoader : ILanguageElementLoader
    {
        /// <summary>
        /// Memory usage can be reduced if only the current language elements are loaded, not all languages.
        /// </summary>
        public Dictionary<Translation, List<string>> LoadElements(string filePath)
        {
            var odsReader = new OdsReader();
            var dataSet = odsReader.ReadFile(filePath);

            var allLanguageElements = new Dictionary<Translation, List<string>>();
            foreach (DataTable table in dataSet.Tables)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];
                    var item = row.ItemArray.First();

                    var language = (Language)Enum.Parse(typeof(Language), table.TableName);

                    var englishText = OdsLanguageElementLoader.GetRowValue(dataSet.Tables["English"].Rows[i]);
                    var key = new Translation(language, OdsLanguageElementLoader.Normalize(englishText));
                    var currentRowValue = OdsLanguageElementLoader.Normalize(OdsLanguageElementLoader.GetRowValue(row));
                    if (!String.IsNullOrEmpty(currentRowValue))
                    {
                        if (allLanguageElements.ContainsKey(key))
                        {
                            if (allLanguageElements[key].Contains(currentRowValue))
                            {
                                throw new Exception($"Element already present in dictionary: {currentRowValue}, Sheet: {table.TableName}, Row: {i + 1}");
                            }
                            allLanguageElements[key].Add(currentRowValue);
                        }
                        else
                        {
                            allLanguageElements.Add(key, new List<string> { currentRowValue });
                        }
                    }
                }
            }

            return allLanguageElements;
        }

        /// <summary>
        /// Load from a stream. First tries to call OdsReader.ReadFile(Stream) via reflection,
        /// otherwise writes a temp file and calls the file-based API.
        /// </summary>
        public Dictionary<Translation, List<string>> LoadElements(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            // Try to call ReadFile(Stream) if OdsReader implements it
            var odsReader = new OdsReader();
            var readerType = odsReader.GetType();
            var method = readerType.GetMethod("ReadFile", new[] { typeof(Stream) });

            if (method != null)
            {
                // Use the stream overload directly
                var ds = method.Invoke(odsReader, new object[] { stream }) as DataSet;
                if (ds == null) throw new InvalidOperationException("OdsReader.ReadFile(Stream) returned null.");
                return OdsLanguageElementLoader.ParseDataSet(ds);
            }

            // Fallback: write to temp file and call file-based API
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".ods");
            try
            {
                // Ensure stream position is at start if possible
                try { if (stream.CanSeek) stream.Position = 0; } catch { /* ignore */ }

                using (var fs = File.Create(tempPath))
                {
                    stream.CopyTo(fs);
                }

                return LoadElements(tempPath);
            }
            finally
            {
                try { if (File.Exists(tempPath)) File.Delete(tempPath); } catch { /* ignore */ }
            }
        }

        private static Dictionary<Translation, List<string>> ParseDataSet(DataSet dataSet)
        {
            var allLanguageElements = new Dictionary<Translation, List<string>>();

            foreach (DataTable table in dataSet.Tables)
            {
                for (var i = 0; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    var item = row.ItemArray.First();

                    var language = (Language)Enum.Parse(typeof(Language), table.TableName);

                    var englishText = OdsLanguageElementLoader.GetRowValue(dataSet.Tables["English"].Rows[i]);
                    var key = new Translation(language, OdsLanguageElementLoader.Normalize(englishText));
                    var currentRowValue = OdsLanguageElementLoader.Normalize(OdsLanguageElementLoader.GetRowValue(row));
                    if (!String.IsNullOrEmpty(currentRowValue))
                    {
                        if (allLanguageElements.ContainsKey(key))
                        {
                            if (allLanguageElements[key].Contains(currentRowValue))
                            {
                                throw new Exception($"Element already present in dictionary: {currentRowValue}, Sheet: {table.TableName}, Row: {i + 1}");
                            }
                            allLanguageElements[key].Add(currentRowValue);
                        }
                        else
                        {
                            allLanguageElements.Add(key, new List<string> { currentRowValue });
                        }
                    }
                }
            }

            return allLanguageElements;
        }

        private static string GetRowValue(DataRow dataRow)
        {
            return dataRow.ItemArray.First() as string;
        }

        private static string Normalize(string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return null;
            }
            return str.Replace("\\r", "\r")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t");
        }
    }
}

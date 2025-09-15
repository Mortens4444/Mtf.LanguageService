using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mtf.LanguageService.Enums;
using Mtf.LanguageService.Interfaces;
using Mtf.LanguageService.Models;

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

                    var englishText = GetRowValue(dataSet.Tables["English"].Rows[i]);
                    var key = new Translation(language, Normalize(englishText));
                    var currentRowValue = Normalize(GetRowValue(row));
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

        private string GetRowValue(DataRow dataRow)
        {
            return dataRow.ItemArray.First() as string;
        }

        private string Normalize(string str)
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

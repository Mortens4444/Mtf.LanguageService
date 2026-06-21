using Mtf.Extensions;
using Mtf.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace Mtf.LanguageService.Ods
{
    public class OdsReader
    {
        private static readonly List<XmlNamespace> Namespaces = new List<XmlNamespace>
        {
            new XmlNamespace("table", "urn:oasis:names:tc:opendocument:xmlns:table:1.0"),
            new XmlNamespace("office", "urn:oasis:names:tc:opendocument:xmlns:office:1.0"),
            new XmlNamespace("style", "urn:oasis:names:tc:opendocument:xmlns:style:1.0"),
            new XmlNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0"),
            new XmlNamespace("draw", "urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"),
            new XmlNamespace("fo", "urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"),
            new XmlNamespace("dc", "http://purl.org/dc/elements/1.1/"),
            new XmlNamespace("meta", "urn:oasis:names:tc:opendocument:xmlns:meta:1.0"),
            new XmlNamespace("number", "urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"),
            new XmlNamespace("presentation", "urn:oasis:names:tc:opendocument:xmlns:presentation:1.0"),
            new XmlNamespace("svg", "urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"),
            new XmlNamespace("chart", "urn:oasis:names:tc:opendocument:xmlns:chart:1.0"),
            new XmlNamespace("dr3d", "urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"),
            new XmlNamespace("math", "http://www.w3.org/1998/Math/MathML"),
            new XmlNamespace("form", "urn:oasis:names:tc:opendocument:xmlns:form:1.0"),
            new XmlNamespace("script", "urn:oasis:names:tc:opendocument:xmlns:script:1.0"),
            new XmlNamespace("ooo", "http://openoffice.org/2004/office"),
            new XmlNamespace("ooow", "http://openoffice.org/2004/writer"),
            new XmlNamespace("oooc", "http://openoffice.org/2004/calc"),
            new XmlNamespace("dom", "http://www.w3.org/2001/xml-events"),
            new XmlNamespace("xforms", "http://www.w3.org/2002/xforms"),
            new XmlNamespace("xsd", "http://www.w3.org/2001/XMLSchema"),
            new XmlNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            new XmlNamespace("rpt", "http://openoffice.org/2005/report"),
            new XmlNamespace("of", "urn:oasis:names:tc:opendocument:xmlns:of:1.2"),
            new XmlNamespace("rdfa", "http://docs.oasis-open.org/opendocument/meta/rdfa#"),
            new XmlNamespace("config", "urn:oasis:names:tc:opendocument:xmlns:config:1.0")
        };

        public DataSet ReadFile(string filePath)
        {
            using (var zipArchive = ZipFile.OpenRead(filePath))
            {
                var contentXml = zipArchive.GetXmlDocument("content.xml");

                var xmlNamespaceManager = contentXml.InitializeXmlNamespaceManager(Namespaces);
                var odsFile = new DataSet(Path.GetFileName(filePath));
                var xmlNodeList = contentXml.SelectNodes("/office:document-content/office:body/office:spreadsheet/table:table", xmlNamespaceManager);
                foreach (XmlNode tableNode in xmlNodeList)
                {
                    odsFile.Tables.Add(GetSheet(tableNode, xmlNamespaceManager));
                }

                return odsFile;
            }
        }

        private DataTable GetSheet(XmlNode tableNode, XmlNamespaceManager xmlNamespaceManager)
        {
            var sheet = new DataTable(tableNode.Attributes["table:name"].Value);
            XmlNodeList rowNodes = tableNode.SelectNodes("table:table-row", xmlNamespaceManager);

            int rowIndex = 0;
            foreach (XmlNode rowNode in rowNodes)
            {
                rowIndex = GetRow(rowNode, sheet, xmlNamespaceManager, rowIndex);
            }

            return sheet;
        }

        private int GetRow(XmlNode rowNode, DataTable sheet, XmlNamespaceManager xmlNamespaceManager, int rowIndex)
        {
            XmlAttribute rowsRepeated = rowNode.Attributes["table:number-rows-repeated"];
            if (rowsRepeated == null || Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture) == 1)
            {
                while (sheet.Rows.Count < rowIndex)
                {
                    sheet.Rows.Add(sheet.NewRow());
                }

                var row = sheet.NewRow();
                var cellNodes = rowNode.SelectNodes("table:table-cell", xmlNamespaceManager);

                int cellIndex = 0;
                foreach (XmlNode cellNode in cellNodes)
                {
                    cellIndex = GetCell(cellNode, row, cellIndex);
                }

                sheet.Rows.Add(row);

                return rowIndex + 1;
            }
            if (sheet.Rows.Count == 0)
            {
                sheet.Rows.Add(sheet.NewRow());
                sheet.Columns.Add();
            }
            return rowIndex + Convert.ToInt32(rowsRepeated.Value, CultureInfo.InvariantCulture);
        }

        private int GetCell(XmlNode cellNode, DataRow row, int cellIndex)
        {
            var cellRepeated = cellNode.Attributes["table:number-columns-repeated"];
            if (cellRepeated == null)
            {
                var sheet = row.Table;
                while (sheet.Columns.Count <= cellIndex)
                {
                    sheet.Columns.Add();
                }
                row[cellIndex] = ReadCellValue(cellNode);
                return cellIndex + 1;
            }
            return cellIndex + Convert.ToInt32(cellRepeated.Value, CultureInfo.InvariantCulture);
        }

        private string ReadCellValue(XmlNode cell)
        {
            var cellValue = cell.Attributes["office:value"];
            return cellValue?.Value ?? cell.InnerText;
        }
    }
}

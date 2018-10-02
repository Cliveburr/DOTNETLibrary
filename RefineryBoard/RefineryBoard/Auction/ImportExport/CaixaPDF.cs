using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RefineryBoard.Auction.ImportExport
{
    public class CaixaPDFReader
    {
        public static Regex LineWithValue = new Regex(@"R\$\s*(.*?\,\d+)");
        public static Regex LineWithWeight = new Regex(@"\s?(\d+\.?\d*\,?\d*)G");
        public static Regex LotNumber = new Regex(@"^\d+\.\d+\-\d+$");
        public static Regex ContractNumber = new Regex(@"^\d+\.\d+\.\d+\-\d+$");
        public static Regex NewPage = new Regex(@"P.gina\s+\d+\s+de\s+\d+");
        public static Regex[] PureGold = new Regex[] {
            new Regex(@"(pedras?)|(diamantes?)|(rel.gio)|(couro)|(n.o nobre)|(p.rola)", RegexOptions.IgnoreCase),
            new Regex(@"folheado", RegexOptions.IgnoreCase)
        };
        public static Regex HasPureGold = new Regex(@"ouro", RegexOptions.IgnoreCase);

        public List<CaixaLote> Read(string file)
        {
            var text = pdfText(file);

            var lines = text.Split(new[] { '\n' }, StringSplitOptions.None);

            var tr = new List<CaixaLote>();
            var actual = new List<string>();

            foreach (var line in lines)
            {
                if (NewPage.IsMatch(line))
                {
                    var newLote = CaixaLote.Create(actual);
                    if (newLote != null)
                    {
                        tr.Add(newLote);
                    }
                    else
                    {
                        var textInvalid = string.Join(Environment.NewLine, actual);   // analisar todos que não foram identificados como lote
                        Console.Write(textInvalid + Environment.NewLine);
                    }

                    actual = new List<string> { line };

                    continue;
                }

                if (LineWithValue.IsMatch(line))
                {
                    var newLote = CaixaLote.Create(actual);
                    if (newLote != null)
                    {
                        tr.Add(newLote);
                    }
                    else
                    {
                        var textInvalid = string.Join(Environment.NewLine, actual);   // analisar todos que não foram identificados como lote
                        Console.Write(textInvalid + Environment.NewLine);
                    }

                    actual = new List<string> { line };

                    continue;
                }

                actual.Add(line);
            }

            return tr;
        }

        private string pdfText(string path)
        {
            var reader = new PdfReader(path);
            string text = string.Empty;
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                text += PdfTextExtractor.GetTextFromPage(reader, page);
            }
            reader.Close();
            return text;
        }
    }

    public class CaixaLote
    {
        public IList<string> Lines { get; set; }
        public string LotNumber { get; set; }
        public string ContractNumber { get; set; }
        public string Description { get; set; }
        public double Weight { get; set; }
        public double Value { get; set; }

        public double ValuePerGran
        {
            get
            {
                return Value / Weight;
            }
        }

        public bool IsPureGold
        {
            get
            {
                foreach (var reg in CaixaPDFReader.PureGold)
                {
                    if (reg.IsMatch(Description))
                        return false;
                }

                if (!CaixaPDFReader.HasPureGold.IsMatch(Description))
                    return false;

                return true;
            }
        }

        public static CaixaLote Create(List<string> lines)
        {
            if (!lines.Any())
            {
                return null;
            }

            var lotNumber = string.Empty;
            var contractNumber = string.Empty;
            var description = string.Empty;
            var weight = string.Empty;
            var value = string.Empty;

            foreach (var line in lines)
            {
                if (CaixaPDFReader.LotNumber.IsMatch(line))
                {
                    lotNumber = line;
                    continue;
                }

                if (CaixaPDFReader.ContractNumber.IsMatch(line))
                {
                    contractNumber = line;
                    continue;
                }

                description += line + " ";
            }

            var valueMath = CaixaPDFReader.LineWithValue.Match(description);
            if (valueMath.Success)
            {
                value = valueMath.Groups[1].Value;
            }

            var weightMatch = CaixaPDFReader.LineWithWeight.Match(description);
            if (weightMatch.Success)
            {
                weight = weightMatch.Groups[1].Value;
            }

            if (string.IsNullOrEmpty(lotNumber) ||
                string.IsNullOrEmpty(contractNumber) ||
                string.IsNullOrEmpty(value) ||
                string.IsNullOrEmpty(weight))
            {
                return null;
            }

            return new CaixaLote
            {
                Lines = lines,
                LotNumber = lotNumber,
                ContractNumber = contractNumber,
                Description = description,
                Value = Double.Parse(value.Replace(".", "").Replace(',', '.')),
                Weight = Double.Parse(weight.Replace(".", "").Replace(',', '.'))
            };
        }

        public override string ToString()
        {
            return string.Concat(Lines);
        }
    }

    public class ExportToXML
    {
        public static void Write(string fileName, List<CaixaLote> lotes)
        {
            var objExcel = new Microsoft.Office.Interop.Excel.Application();

            var objWorkbook = objExcel.Workbooks.Add();
            var objPlan = objWorkbook.Worksheets.get_Item(1);

            int line = 1, col = 1;

            foreach (var lote in lotes)
            {
                objPlan.Cells[line, col++] = lote.LotNumber;

                objPlan.Cells[line, col++] = lote.ContractNumber;

                objPlan.Cells[line, col++] = lote.Weight;

                objPlan.Cells[line, col++] = lote.Value;

                objPlan.Cells[line, col++] = lote.Description;

                line++;
                col = 1;
            }

            objWorkbook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                        false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            objExcel.Workbooks.Close();
            objExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objPlan);
            objPlan = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objWorkbook);
            objWorkbook = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(objExcel);
            objExcel = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
}
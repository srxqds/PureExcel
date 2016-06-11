/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 03 2016
// 模块描述：
//----------------------------------------------------------------*/
using System.Collections.Generic;

namespace PureExcel
{
    public class Worksheet
    {
        //common escape char by xml
        public const string LtEscape = "&lt;";//<
        public const string Lt = "<";
        public const string GtEscape = "&gt;";//>
        public const string Gt = ">";
        public const string AmpEscape = "&amp;";//&
        public const string Amp = "&";
        public const string AposEscape = "&apos;";//'
        public const string Apos = "'";
        public const string QuotEscape = "&quot;";//"
        public const string Quot = "\"";
        /// <summary>
        /// Collection of rows in this worksheet
        /// </summary>
        public List<Row> Rows { get; set; }
        public int Index { get; internal set; }
        public string Name { get; set; }

        public int RowCount { get; internal set; }
        public int ColumnCount { get; internal set; }

        public Excel Excel { get; private set; }
        
        private int ColumnStart { get; set; }

        internal string FileName
        {
            get
            {
                return GetFileName(this.Index);
            }
        }

        public static string GetFileName(int index)
        {
            return string.Format("xl/worksheets/sheet{0}.xml", index);
        }
        
        public Worksheet(Excel excel)
        {
            Excel = excel;
        }

        public string GetCell(int rowIndex, int columnIndex, string defaultValue = "")
        {
            if (rowIndex < 0 || rowIndex > RowCount-1
                || columnIndex < 0 || columnIndex > ColumnCount)
                return defaultValue;
            Row row = this.Rows[rowIndex];
            foreach (Cell cell in row.Cells)
            {
                if (cell.ColumnIndex == columnIndex + ColumnStart)
                {
                    string cellValue = cell.Value;
                    cellValue = cellValue.Replace(LtEscape, Lt);
                    cellValue = cellValue.Replace(GtEscape, Gt);
                    cellValue = cellValue.Replace(AmpEscape, Amp);
                    cellValue = cellValue.Replace(AposEscape, Apos);
                    cellValue = cellValue.Replace(QuotEscape, Quot);
                    return cellValue;
                }
            }
            return defaultValue;
        }
        public void Read()
        {
			XMLNode document = XMLParser.Parse(Excel.Archive.GetXmlText(FileName));
            XMLNodeList rowList = document.GetNodeList("worksheet>0>sheetData>0>row");
            
            Rows = GetRows(rowList);
            this.RowCount = Rows.Count;
            
            
        }

		private List<Row> GetRows(XMLNodeList rowElements)
        {
            List<Row> rowList = new List<Row>();
		    int columnEnd = 0;
            foreach (XMLNode rowElement in rowElements)
            {
                Row row = new Row(rowElement, Excel.SharedStrings);
                if (row.Cells != null && row.Cells.Count != 0)
                {
                    rowList.Add(row);
                    if (row.ColumnStart < ColumnStart)
                        ColumnStart = row.ColumnStart;
                    if (row.ColumnEnd > columnEnd)
                        columnEnd = row.ColumnEnd;
                }
            }
		    this.ColumnCount = columnEnd - this.ColumnStart;
		    return rowList;
        }
    }
}

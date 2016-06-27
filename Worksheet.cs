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

        public Worksheet(Excel excel)
        {
            Excel = excel;
        }
        
        //rowIndex and columnIndex begin from 0.
        //because worksheet trim the empty row ,and iterator cann't now which row is empty.
        public string GetCell(int rowIndex, int columnIndex, string defaultValue = "")
        {
            if (!IsCeilValid(rowIndex, columnIndex))
                return defaultValue;

            Row row = this.Rows[rowIndex];
            Cell cell = row.GetCell(columnIndex);
            if (cell == null)
                return defaultValue;
            string cellValue = cell.Value;
            cellValue = cellValue.Replace(LtEscape, Lt);
            cellValue = cellValue.Replace(GtEscape, Gt);
            cellValue = cellValue.Replace(AmpEscape, Amp);
            cellValue = cellValue.Replace(AposEscape, Apos);
            cellValue = cellValue.Replace(QuotEscape, Quot);
            return cellValue;
        }

        public string GetComment(int rowIndex, int columnIndex)
        {
            string commentFile = "xl/comments" + Index + ".xml";
            string commentText = this.Excel.Archive.GetXmlText(commentFile);
            XMLNode commentRootNode = XMLParser.Parse(commentText);
            
            //default value 
            int columnName = columnIndex + 1;
            int rowName = rowIndex + 1;
            if (this.Rows != null && this.Rows.Count > rowIndex)
            {
                Row row = this.Rows[rowIndex];
                columnName = columnIndex + row.ColumnStart;
                rowName = row.RowIndex;
            }
            string commentCeilName = Cell.GetExcelColumnName(columnName) + rowName;
            XMLNodeList commentList = commentRootNode.GetNodeList("comments>0>commentList>0>comment");
            if (commentList != null && commentList.Count > 0)
            {
                foreach (XMLNode commentNode in commentList)
                {
                    if (commentNode.GetValue("@ref") == commentCeilName)
                    {
                        return commentNode.GetValue("text>0>r>1>t>0>_text");
                    }
                }
            }
            return null;
        }

        private bool IsCeilValid(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex > RowCount - 1
                   || columnIndex < 0 || columnIndex > ColumnCount - 1)
                return false;
            return true;
        }

        public void Read()
        {
            string fileNameInZip = string.Format("xl/worksheets/sheet{0}.xml", Index);
            XMLNode document = XMLParser.Parse(Excel.Archive.GetXmlText(fileNameInZip));
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

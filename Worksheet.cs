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
        /// <summary>
        /// Collection of rows in this worksheet
        /// </summary>
        public List<Row> Rows { get; set; }
        public int Index { get; internal set; }
        public string Name { get; set; }

        public int RowCount { get; internal set; }
        public int ColumnCount { get; internal set; }

        internal Excel m_Excel { get; private set; }
        private int m_ColumnStart { get; set; }

        internal Worksheet(Excel mExcel)
        {
            m_ColumnStart = 1;
            m_Excel = mExcel;
        }

        //rowIndex and columnIndex begin from 0.
        //because worksheet trim the empty row ,and iterator cann't now which row is empty.
        public string GetCell(int rowIndex, int columnIndex, string defaultValue = "")
        {
            if (!IsCeilValid(rowIndex, columnIndex))
                return defaultValue;
            Row row = this.Rows[rowIndex];
            Cell cell = row.GetCell(columnIndex + m_ColumnStart);
            if (cell == null)
                return defaultValue;
            return Cell.DecodeEscapeString(cell.Value);
        }

        public string GetComment(int rowIndex, int columnIndex)
        {
            string commentFile = "xl/comments" + Index + ".xml";
            XMLNode commentRootNode = this.m_Excel.m_Archive.GetXmlNode(commentFile);
            if (commentRootNode == null)
                return null;
            //default value
            int columnName = columnIndex + m_ColumnStart;
            int rowName = rowIndex + 1;
            if (this.Rows != null && this.Rows.Count > rowIndex)
            {
                Row row = this.Rows[rowIndex];
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
            XMLNode document = m_Excel.m_Archive.GetXmlNode(fileNameInZip);
            if (document != null)
            {
                XMLNodeList rowList = document.GetNodeList("worksheet>0>sheetData>0>row");
                Rows = GetRows(rowList);
                this.RowCount = Rows.Count;
            }
        }

		private List<Row> GetRows(XMLNodeList rowElements)
        {
            List<Row> rowList = new List<Row>();
		    int columnEnd = 0;
            foreach (XMLNode rowElement in rowElements)
            {
                Row row = new Row(rowElement, m_Excel.m_SharedStrings);
                if (row.Cells != null && row.Cells.Count != 0)
                {
                    rowList.Add(row);
                    if (row.m_ColumnStart < m_ColumnStart)
                        m_ColumnStart = row.m_ColumnStart;
                    if (row.m_ColumnEnd > columnEnd)
                        columnEnd = row.m_ColumnEnd;
                }
            }
		    this.ColumnCount = columnEnd - this.m_ColumnStart;
		    return rowList;
        }
    }
}

/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 03 2016
// 模块描述：
//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;
using System.IO;

namespace Excel
{
    public class Worksheet
    {
        /// <summary>
        /// Collection of rows in this worksheet
        /// </summary>
        public IEnumerable<Row> Rows { get; set; }

        public IEnumerable<string> Headings { get; set; }

        public int Index { get; internal set; }
        public string Name { get; set; }

        public FastExcel FastExcel { get; private set; }

        internal string FileName
        {
            get
            {
                return Worksheet.GetFileName(this.Index);
            }
        }

        public static string GetFileName(int index)
        {
            return string.Format("xl/worksheets/sheet{0}.xml", index);
        }

        public Worksheet() { }

        public Worksheet(FastExcel fastExcel)
        {
            FastExcel = fastExcel;
        }

        public bool Exists
        {
            get
            {
                return !string.IsNullOrEmpty(this.FileName);
            }
        }


        public void Read()
        {
            FastExcel.PrepareArchive();
            
            
            IEnumerable<Row> rows = null;

            //using (Stream stream = FastExcel.Archive.GetXmlStream(FileName))
            //{
			XMLNode document = XMLParser.Parse(FastExcel.Archive.GetXmlText(FileName));
            int skipRows = 0;

			XMLNodeList rowList = document.GetNodeList("worksheet>0>sheetData>0>row");

			rows = GetRows(rowList);
            //}

            Rows = rows;
        }

		private IEnumerable<Row> GetRows(XMLNodeList rowElements)
        {
            foreach (XMLNode rowElement in rowElements)
            {
                yield return new Row(rowElement, FastExcel.SharedStrings);
            }
        }

    }
}

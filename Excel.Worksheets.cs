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

namespace PureExcel
{
    public partial class Excel
    {
        private Worksheet[] _worksheets;
        /// <summary>
        /// List of worksheets, loaded on first access of property
        /// </summary>
        public Worksheet[] Worksheets
        {
            get
            {
                if (_worksheets == null)
                {
                    _worksheets = GetWorksheetProperties(); 
                }
                return _worksheets;
            }
        }

        private Worksheet[] GetWorksheetProperties()
        {
            PrepareArchive();

            var worksheets = new List<Worksheet>();

            string xmlText = this.Archive.GetXmlText("xl/workbook.xml");

			XMLNode document = XMLParser.Parse(xmlText);

            if (document == null)
            {
                throw new Exception("Unable to load workbook.xml");
            }
			XMLNodeList nodeList = document.GetNodeList ("workbook>0>sheets>0>sheet");

			foreach (XMLNode node in nodeList)
            {
                var worksheet = new Worksheet(this);
				worksheet.Index = int.Parse(node.GetValue ("@sheetId"));
				worksheet.Name = node.GetValue ("@name");
                worksheets.Add(worksheet);
            }
            return worksheets.ToArray();
        }
    }
}

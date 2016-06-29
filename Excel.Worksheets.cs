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
        private Worksheet[] m_Worksheets;
        /// <summary>
        /// List of worksheets, loaded on first access of property
        /// </summary>
        public Worksheet[] WorkSheets
        {
            get
            {
                if (m_Worksheets == null)
                {
                    m_Worksheets = GetWorksheetProperties(); 
                }
                return m_Worksheets;
            }
        }

        private Worksheet[] GetWorksheetProperties()
        {
            PrepareArchive();
            var worksheets = new List<Worksheet>();
			XMLNode document = this.m_Archive.GetXmlNode("xl/workbook.xml");
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

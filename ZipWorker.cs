/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 04 2016
// 模块描述：
//----------------------------------------------------------------*/
using System.IO;
using System.IO.Compression;
using System.Text;

namespace PureExcel
{
    public static  class ZipWorker
    {
        public static string GetXmlText(this ZipArchive archive, string fileNameInZip)
        {
            ZipArchive.ZipEntry entry = archive.GetEntry(fileNameInZip);
            string xml = string.Empty;
            if (entry != null)
            {
                MemoryStream ms = new MemoryStream();
                archive.ExtractFile(entry, ms);
                xml = Encoding.UTF8.GetString(ms.ToArray());
                ms.Dispose();
            }
            return xml;
        }

        public static XMLNode GetXmlNode(this ZipArchive archive, string fileNameInZip)
        {
            string xmlText = GetXmlText(archive, fileNameInZip);
            if (string.IsNullOrEmpty(xmlText))
                return null;
            return XMLParser.Parse(xmlText);
        }

/*#if UNITY_EDITOR
        [UnityEditor.MenuItem("Test/ParserExcel")]
		public static void DoTest()
		{
			string excelFile = EditorUtils.ProjectPath + "Assets/Editor/GameConfig/c场景配置表1.xlsx";

			m_Excel excel = new m_Excel(excelFile);
			var works = excel.WorkSheets;
			Worksheet work = excel.Read(0);
			int rowIndex = 0;

			foreach(Row row in work.Rows)
			{
				int cellIndex = 0;
				foreach(Cell cell in row.Cells)
				{
					UnityEngine.Debug.Log ("ceil(" + rowIndex + "," + cellIndex + "):" + cell.Value);
					cellIndex++;
				}
				rowIndex++;
			}
		}
#endif*/
    }
}

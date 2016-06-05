/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 04 2016
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;

namespace Excel
{
    public static  class ZipWorker
    {
        public static Stream GetXmlStream(this ZipArchive archive,string fileNameInZip)
        {
            ZipArchive.ZipEntry entry = archive.GetEntry(fileNameInZip);
            MemoryStream ms = new MemoryStream();
            if (entry != null)
            {
                archive.ExtractFile(entry, ms);
            }
            return ms;
        }

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

		[MenuItem("Test/ParserExcel")]
		public static void DoTest()
		{
			string excelFile = EditorUtils.ProjectPath + "Assets/Editor/GameConfig/c场景配置表1.xlsx";

			FastExcel fastExcel = new FastExcel(excelFile);
			var works = fastExcel.Worksheets;
			Worksheet work = fastExcel.Read(0);
			int rowIndex = 0;

			foreach(Row row in work.Rows)
			{
				int cellIndex = 0;
				foreach(Cell cell in row.Cells)
				{
					Debug.Log ("ceil(" + rowIndex + "," + cellIndex + "):" + cell.Value);
					cellIndex++;
				}
				rowIndex++;
			}

		}
    }
}

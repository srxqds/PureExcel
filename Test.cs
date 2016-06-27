/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 28 2016
// 模块描述：
//----------------------------------------------------------------*/

namespace PureExcel
{
    public class Test
    {
    #if UNITY_EDITOR
        [UnityEditor.MenuItem("Test/DoExcelParser")]
        public static void DoTest()
        {
            string excelFilePath = @"G:\Comment.xlsx";
            Excel excelParser = new Excel(excelFilePath);
            Worksheet workSheet = excelParser.Read(0);
            UnityEngine.Debug.LogError("Cell(0,0):" + workSheet.GetCell(0,0) + " comment:" + workSheet.GetComment(0,0));

            Worksheet workSheet1 = excelParser.Read(1);
            UnityEngine.Debug.LogError("Cell(2,1):" + workSheet1.GetCell(2, 1) + " comment:" + workSheet1.GetComment(2, 1));
            excelParser.Dispose();
        }
#endif
    }

}

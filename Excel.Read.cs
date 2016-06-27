/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 03 2016
// 模块描述：
//----------------------------------------------------------------*/

namespace PureExcel
{
    public partial class Excel
    {
        public Worksheet Read(int sheetIndex)
        {
			//excel index begin from 1
			foreach (Worksheet workSheet in Worksheets) 
			{
				if (workSheet.Index == sheetIndex + 1) 
				{
					workSheet.Read ();
					return workSheet;
				}
			}
			return null;
        }

        public Worksheet Read(string sheetName)
        {
			foreach (Worksheet workSheet in Worksheets) 
			{
				if (workSheet.Name == sheetName) 
				{
					workSheet.Read ();
					return workSheet;
				}
			}
			return null;
        }

    }
}

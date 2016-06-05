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
using System.Text;
using System.Text.RegularExpressions;

namespace Excel
{
    /// <summary>
    /// Contains the actual value
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Column Numnber (Starts at 1)
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// The value that is stored
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Create a new Cell
        /// </summary>
        /// <param name="cellElement">Cell</param>
        /// <param name="sharedStrings">The collection of shared strings used by this document</param>
		public Cell(XMLNode cellElement, SharedStrings sharedStrings)
        {
			bool isTextRow = cellElement.GetValue ("@t") == "s";
			string columnName = cellElement.GetValue ("@r");

            this.ColumnNumber = GetExcelColumnNumber(columnName);

            if (isTextRow)
            {
				string textValue = cellElement.GetValue ("v>0>_text");
				this.Value = sharedStrings.GetString(textValue);
            }
            else
            {
				this.Value = cellElement.GetValue("_text");
            }

        }

        
        //http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa
        /// <summary>
        /// Convert Column Number into Column Name - Character(s) eg 1-A, 2-B
        /// </summary>
        /// <param name="columnNumber">Column Number</param>
        /// <returns>Column Name - Character(s)</returns>
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = string.Concat(Convert.ToChar(65 + modulo), columnName);
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        //http://stackoverflow.com/questions/181596/how-to-convert-a-column-number-eg-127-into-an-excel-column-eg-aa
        /// <summary>
        /// Covert Column Name - Character(s) into a Column Number eg A-1, B-2, A1 - 1, B9 - 2
        /// </summary>
        /// <param name="columnName">Column Name - Character(s) optinally with the Row Number</param>
        /// <param name="includesRowNumber">Specify if the row number is included</param>
        /// <returns>Column Number</returns>
        public static int GetExcelColumnNumber(string columnName, bool includesRowNumber = true)
        {
            if (includesRowNumber)
            {
                columnName = Regex.Replace(columnName, @"\d", "");
            }

            int[] digits = new int[columnName.Length];
            for (int i = 0; i < columnName.Length; ++i)
            {
                digits[i] = Convert.ToInt32(columnName[i]) - 64;
            }
            int mul = 1; int res = 0;
            for (int pos = digits.Length - 1; pos >= 0; --pos)
            {
                res += digits[pos] * mul;
                mul *= 26;
            }
            return res;
        }

        
    }
}

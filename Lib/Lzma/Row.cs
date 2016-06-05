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

namespace Excel
{
    /// <summary>
    /// Row that contains the Cells
    /// </summary>
    public class Row
    {
        /// <summary>
        /// The Row Number (Row numbers start at 1)
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// The collection of cells for this row
        /// </summary>
        public IEnumerable<Cell> Cells { get; set; }

        /// <summary>
        /// Create a new Row
        /// </summary>
        /// <param name="rowNumber">Row number starting with 1</param>
        /// <param name="cells">Cells on this row</param>
        public Row(int rowNumber, IEnumerable<Cell> cells)
        {
            if (rowNumber <= 0)
            {
                throw new Exception("Row numbers starting at 1");
            }
            this.RowNumber = rowNumber;
            this.Cells = cells;
        }

		public Row(XMLNode rowElement, SharedStrings sharedStrings)
        {
            try
            {
				this.RowNumber = int.Parse(rowElement.GetValue("@r"));
            }
            catch (Exception ex)
            {
                throw new Exception("Row Number not found", ex);
            }

			XMLNodeList cellList = rowElement.GetNodeList ("c");
                        
			if (cellList != null && cellList.Count > 0)
            {
				this.Cells = GetCells(rowElement.GetNodeList("c"), sharedStrings);
            }
        }

		private IEnumerable<Cell> GetCells(XMLNodeList rowElement, SharedStrings sharedStrings)
        {
			foreach (XMLNode cellElement in rowElement)
            {
                Cell cell = new Cell(cellElement, sharedStrings);
                if (cell.Value != null)
                {
                    yield return cell;
                }
            }
        }

        

        
    }
}

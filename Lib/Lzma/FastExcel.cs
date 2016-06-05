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
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Excel
{
    public partial class FastExcel: IDisposable
    {
        public string FileName { get; private set; }
        
        internal SharedStrings SharedStrings { get; set; }
        internal ZipArchive Archive { get; set; }


        public FastExcel(string excelFile)
        {
            this.FileName = excelFile;
            
        }

        internal void PrepareArchive()
        {
            if (this.Archive == null)
            {
				Archive = ZipArchive.Open(this.FileName, FileAccess.Read);
                Archive.Entries = Archive.GetEntries();
            }

            // Get Strings file
            if (this.SharedStrings == null )
            {
                this.SharedStrings = new SharedStrings(this.Archive);
            }
        }
        
        public class WorksheetProperties
        {
            public int CurrentIndex { get; set; }
            public int SheetId { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Saves any pending changes to the Excel stream and adds/updates associated files if needed
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.Archive == null)
            {
                return;
            }

            this.Archive.Dispose();
        }
    }
}

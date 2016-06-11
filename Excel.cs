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
using System.IO;
using System.IO.Compression;

namespace PureExcel
{
    public partial class Excel: IDisposable
    {
        public string FileName { get; private set; }
        
        internal SharedStrings SharedStrings { get; set; }
        internal ZipArchive Archive { get; set; }
        
        public Excel(string excelFile)
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

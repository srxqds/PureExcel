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
        
        internal SharedStrings m_SharedStrings { get; set; }
        internal ZipArchive m_Archive { get; set; }
        
        public Excel(string excelFile)
        {
            this.FileName = excelFile;
        }

        internal void PrepareArchive()
        {
            if (this.m_Archive == null)
            {
				m_Archive = ZipArchive.Open(this.FileName, FileAccess.Read);
                m_Archive.Entries = m_Archive.GetEntries();
            }
            // Get Strings file
            if (this.m_SharedStrings == null )
            {
                this.m_SharedStrings = new SharedStrings(this.m_Archive);
            }
        }
        
        /// <summary>
        /// Saves any pending changes to the m_Excel stream and adds/updates associated files if needed
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.m_Archive == null)
            {
                return;
            }
            this.m_Archive.Dispose();
        }
    }
}

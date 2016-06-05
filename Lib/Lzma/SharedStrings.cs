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
    /// <summary>
    /// Read and update xl/sharedStrings.xml file
    /// </summary>
    public class SharedStrings
    {
        //A dictionary is a lot faster than a list
        private List<string> StringArray { get; set; }

        private bool SharedStringsExists { get; set; }
        private ZipArchive ZipArchive { get; set; }

        public bool PendingChanges { get; private set; }

        public bool ReadWriteMode { get; set; }

        internal SharedStrings(ZipArchive archive)
        {
            this.ZipArchive = archive;
            
            this.SharedStringsExists = true;

			this.StringArray = new List<string> ();
			string xmlText= this.ZipArchive.GetXmlText ("xl/sharedStrings.xml");
			if (string.IsNullOrEmpty (xmlText)) {
				
				this.SharedStringsExists = false;
				return;
			} else {
                
				XMLNode document = XMLParser.Parse (xmlText);
				if (document == null) {
					this.SharedStringsExists = false;
					return;
				}

				List<XMLNode> nodeList = new List<XMLNode> ();
				XMLNodeList siNodeList = document.GetNodeList ("sst>0>si");
				UnityEngine.Debug.LogError (siNodeList);
				foreach (XMLNode node in siNodeList) 
				{
					//UnityEngine.Debug.Log ("node:" + node);
					nodeList.AddRange (node.GetDeepNodeList ("t"));
				}
				int i = 0;
				foreach (XMLNode node in nodeList) 
				{
					string textValue = node.GetValue ("_text");
					this.StringArray.Add (textValue);
				} 
			}
        }

        internal string GetString(string position)
        {
            int pos = 0;
            if (int.TryParse(position, out pos))
            {
				if (pos >= 0 && pos < this.StringArray.Count)
					return StringArray[pos];
            }
            // TODO: should I throw an error? this is a corrupted excel document
            return string.Empty;
            
        }

    }
}

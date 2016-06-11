/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 03 2016
// 模块描述：
//----------------------------------------------------------------*/
using System.Collections.Generic;
using System.IO.Compression;

namespace PureExcel
{
    /// <summary>
    /// Read and update xl/sharedStrings.xml file
    /// </summary>
    public class SharedStrings
    {
        //A dictionary is a lot faster than a list
        private List<string> StringArray { get; set; }
        private bool SharedStringsExists { get; set; }

        internal SharedStrings(ZipArchive archive)
        {
            this.SharedStringsExists = true;
			this.StringArray = new List<string> ();
			string xmlText= archive.GetXmlText ("xl/sharedStrings.xml");
			if (string.IsNullOrEmpty (xmlText))
            {
				this.SharedStringsExists = false;
			}
            else
            {
				XMLNode document = XMLParser.Parse (xmlText);
				if (document == null)
                {
					this.SharedStringsExists = false;
					return;
				}
				//List<XMLNode> nodeList = new List<XMLNode> ();
                //only one share string in one si!!!
				XMLNodeList siNodeList = document.GetNodeList ("sst>0>si");
				foreach (XMLNode node in siNodeList)
				{
				    XMLNodeList tList = node.GetDeepNodeList("t");
                    //handle for <t xml:space="preserve"> </t>
				    string tValue = string.Empty;
				    foreach (var tNode in tList)
				    {
				        tValue += tNode.GetValue("_text");
                    }
                    this.StringArray.Add(tValue);
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

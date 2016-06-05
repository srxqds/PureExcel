/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：Xml 解析工具
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：February 29 2016
// 模块描述：
//----------------------------------------------------------------*/

#if WP_BUILD
using ArrayList = MarkerMetro.Unity.WinLegacy.Plugin.Collections.ArrayList;
#else
using System.Collections.Generic;
#endif


public class XMLNodeList : List<XMLNode>
{
    public XMLNode this[int index]
    {
        get
        {
            return base[index] as XMLNode;
        }
    }

	public XMLNode Pop()
	{
		XMLNode result = this[this.Count - 1] as XMLNode;
		this.RemoveAt(this.Count - 1);
		return result;
	}

	public void Push(XMLNode node)
	{
		this.Add(node);
	}

	public override string ToString()
	{
		string toText = string.Empty;
		foreach (XMLNode node in this) 
		{
			toText += node + "\n";
		}
		return toText;
	}
}

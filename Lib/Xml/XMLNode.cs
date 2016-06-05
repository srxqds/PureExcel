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
using Hashtable = MarkerMetro.Unity.WinLegacy.Plugin.Collections.Hashtable;//MakerMetro.Unity.WinLegacy.Plugin.Collections.Hashtable;
#else
using System.Collections;
#endif

public class XMLNode : Hashtable
{
	public XMLNodeList GetNodeList(string path)
	{
		return this.GetObject(path) as XMLNodeList;
	}

	public XMLNodeList GetDeepNodeList(string name)
	{
		XMLNodeList nodeList = new XMLNodeList ();
		IDictionaryEnumerator iter = this.GetEnumerator();
		while (iter.MoveNext())
		{
			if (iter.Value is XMLNodeList) 
			{
				XMLNodeList children = iter.Value as XMLNodeList;

				//UnityEngine.Debug.Log ("children Count:" + children.Count + " name: " + name + " key:" + iter.Key + " " + name );
				//it‘s can't use "iter.Key == name",because iter.Key is object.
				if (iter.Key.Equals(name)) 
				{
					
					nodeList.AddRange (children);
				}
				else 
				{
					foreach(XMLNode child in children)
						nodeList.AddRange (child.GetDeepNodeList (name));
				}
			}
			
		}

		return nodeList;
	}
	public XMLNode GetNode(string path)
	{
		return this.GetObject(path) as XMLNode;
	}
	public string GetValue(string path)
	{
		return this.GetObject(path) as string;
	}

	private object GetObject(string path)
	{
		XMLNode currentNode = this;
		XMLNodeList currentNodeList = null;
		bool listMode = false;
		string[] array = path.Split(new char[]
		{
			'>'
		});
		object result;
		for (int i = 0; i < array.Length; i++)
		{
			if (listMode)
			{
                currentNode = (currentNodeList[int.Parse(array[i])] as XMLNode);
                listMode = false;

			}
			else
			{
				object obj = currentNode[array[i]];

				if (!(obj is XMLNodeList))
				{
                    // reached a leaf node/attribute
                    if (i != array.Length - 1)
					{
						string actualPath = "";
						for (int j = 0; j <= i; j++)
						{
                            actualPath = actualPath + ">" + array[j];
						}
                        UnityEngine.Debug.Log("xml path search truncated. Wanted: " + path + " got: " + actualPath);
                    }
					result = obj;
					return result;
				}
                currentNodeList = (obj as XMLNodeList);
                listMode = true;
			}
		}
		if (listMode)
		{
			result = currentNodeList;
			return result;
		}
		result = currentNode;
		return result;
	}

	public override string ToString ()
	{
		IDictionaryEnumerator iter = this.GetEnumerator();
		string toText = string.Empty;
		while (iter.MoveNext())
		{
			if (iter.Value is XMLNodeList)
				toText += "\n";
			else 
				toText += iter.Key + ", ";
			toText += iter.Value + "\n";
		}
		return toText;
	}




}

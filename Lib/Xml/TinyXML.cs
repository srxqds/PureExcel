/*----------------------------------------------------------------
// Copyright (C) 2015 广州，Lucky Game
//
// 模块名：
// 创建者：D.S.Qiu
// 修改者列表：
// 创建日期：June 06 2016
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;

namespace TinyXML
{
	public class TinyXML 
	{
		public string text;
		public string name;
		public List<TinyXML> children = new List<TinyXML> ();

		private Dictionary<string,string> attributes;

		public string GetAttribute(string attName)
		{
			if (attributes != null && attributes.ContainsKey (attName))
				return this.attributes [attName];
			return null;
				
		}

		public TinyXML GetNode (string path)
		{
			
			return null;
		}

		public List<TinyXML> GetNodeList(string path)
		{
			return null;
		}

		public TinyXML GetNode(string nodeName,int index)
		{
			int count = 0;
			for (int i = 0; i < children.Count; i++) 
			{
				TinyXML node = children [i];
				if (node.name == nodeName) 
				{
					if (index == count)
						return node;
					else
						count++;
				}
			}
			return null;
		}

		private const char LT = '<';
		private const char GT = '>';
		private const char SQR = ']';
		private const char DASH = '-';
		private const char SPACE = ' ';
		private const char QUOTE = '"';
		private const char SLASH = '/';
		private const char QMARK = '?';
		private const char EQUALS = '=';
		private const char EXCLAMATION = '!';



		public static TinyXML Parse(string content)
		{
			if (string.IsNullOrEmpty(content))
				return null;
			var rootNode = new TinyXML();
			//trim the situation: <br/>
			content = content.Replace ("/>", " />");
			rootNode.text = string.Empty;

			string nodeContents = "";

			bool inElement = false;
			bool collectNodeName = false;
			bool collectAttributeName = false;
			bool collectAttributeValue = false;
			bool quoted = false;
			string attName = "";
			string attValue = "";
			string nodeName = "";
			string textValue = "";

			bool inMetaTag = false;
			bool inComment = false;
			bool inDoctype = false;
			bool inCDATA = false;

			Stack<TinyXML> parents = new Stack<TinyXML>();

			TinyXML currentNode = rootNode;
			for (var i = 0; i < content.Length; i++)
			{

				char c= content[i];
				char cn;
				char cnn;
				char cp;
				cn = cnn = cp = '\0';
				if ((i + 1) < content.Length) cn = content[i + 1];
				if ((i + 2) < content.Length) cnn = content[i + 2];
				if (i > 0) cp = content[i - 1];

				if (inMetaTag)
				{
					if (c == QMARK && cn == GT)
					{
						inMetaTag = false;
						i++;
					}
					continue;
				}
				else
				{
					if (!quoted && c == LT && cn == QMARK)
					{
						inMetaTag = true;
						continue;
					}
				}
				if (inDoctype)
				{
					if (cn == GT)
					{
						inDoctype = false;
						i++;
					}
					continue;
				}
				else if (inComment)
				{
					if (cp == DASH && c == DASH && cn == GT)
					{
						inComment = false;
						i++;
					}
					continue;
				}
				else
				{
					if (!quoted && c == LT && cn == EXCLAMATION)
					{
						if (content.Length > i + 9 && content.Substring(i, 9) == "<![CDATA[")
						{
							inCDATA = true;
							i += 8;
						}
						else if (content.Length > i + 9 && content.Substring(i, 9) == "<!DOCTYPE")
						{
							inDoctype = true;
							i += 8;
						}
						else if (content.Length > i + 2 && content.Substring(i, 4) == "<!--")
						{
							inComment = true;
							i += 3;
						}
						continue;
					}
				}

				if (inCDATA)
				{
					if (c == SQR && cn == SQR && cnn == GT)
					{
						inCDATA = false;
						i += 2;
						continue;
					}
					textValue += c;
					continue;
				}

				if (inElement)
				{
					if (collectNodeName)
					{
						if (c == SPACE) 
						{
							collectNodeName = false;
						} 
						else if (c == GT)
						{
							collectNodeName = false;
							inElement = false;
						}

						if (!collectNodeName && nodeName.Length > 0)
						{

							// close tag
							if (nodeName[0] == SLASH)
							{
								// close tag
								if (textValue.Length > 0)
								{
									currentNode.text += textValue;
								}

								textValue = "";
								nodeName = "";
								currentNode = parents.Pop();
							}
							else
							{

								if (textValue.Length > 0)
								{
									currentNode.text += textValue;
								}
								textValue = "";
								TinyXML newNode = new TinyXML();
								newNode.text = "";

								newNode.name = nodeName;

								if (currentNode.children == null)
								{
									currentNode.children = new List<TinyXML>();
								}
								currentNode.children.Add(newNode);
								parents.Push(currentNode);
								currentNode = newNode;
								nodeName = "";
							}

						}
						else
						{
							nodeName += c;
						}
					}
					else
					{
						if (!quoted && c == SLASH && cn == GT)
						{
							inElement = false;
							collectAttributeName = false;
							collectAttributeValue = false;
							currentNode.AddAttribute (attName, attValue);

							i++;
							currentNode = parents.Pop();
							attName = "";
							attValue = "";
						}
						else if (!quoted && c == GT)
						{
							inElement = false;
							collectAttributeName = false;
							collectAttributeValue = false;
							currentNode.AddAttribute (attName, attValue);

							attName = "";
							attValue = "";
						}
						else {
							if (collectAttributeName)
							{
								if (c == SPACE || c == EQUALS)
								{
									collectAttributeName = false;
									collectAttributeValue = true;
								}
								else
								{
									attName += c;
								}
							}
							else if (collectAttributeValue)
							{
								if (c == QUOTE)
								{
									if (quoted)
									{
										collectAttributeValue = false;
										currentNode.AddAttribute (attName, attValue);
										attValue = "";
										attName = "";
										quoted = false;
									}
									else
									{
										quoted = true;
									}
								}
								else {
									if (quoted)
									{
										attValue += c;
									}
									else
									{
										if (c == SPACE)
										{
											collectAttributeValue = false;
											currentNode.AddAttribute (attName, attValue);

											attValue = "";
											attName = "";
										}
									}
								}
							}
							else if (c == SPACE)
							{

							}
							else
							{
								collectAttributeName = true;
								attName = "" + c;
								attValue = "";
								quoted = false;
							}
						}
					}
				}
				else
				{
					if (c == LT)
					{
						inElement = true;
						collectNodeName = true;
					}
					else
					{
						textValue += c;
					}
				}
			}
			UnityEngine.Debug.Log (rootNode);
			return rootNode;
		}

		private void AddAttribute(string attName,string attValue)
		{

			if (!string.IsNullOrEmpty(attName))
			{
				if (!string.IsNullOrEmpty(attValue))
				{
					attValue = string.Empty;
				}
				if(this.attributes == null)
					this.attributes = new Dictionary<string, string>();
				this.attributes.Add(attValue,attValue);
			}
		}


	}
}

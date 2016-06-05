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
using Hashtable =MarkerMetro.Unity.WinLegacy.Plugin.Collections.Hashtable;
#else
using System.Collections;
using Utility;
using System.Text;
#endif

//todo:转义字符，容错处理
public static class XMLParser
{
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

    public static XMLNode ParseFile(string filePath)
    {
        if (FileUtility.Exists(filePath))
            return Parse(FileUtility.ReadAllBytes(filePath));
        return null;
    }

    public static XMLNode Parse(byte[] data)
    {
        string content = Encoding.UTF8.GetString(data);
        return Parse(content);
    }

    public static XMLNode Parse(string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;
        var rootNode = new XMLNode();
		//trim the situation: <br/>
		content = content.Replace ("/>", " />");
        rootNode["_text"] = "";

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

        XMLNodeList parents = new XMLNodeList();

        XMLNode currentNode = rootNode;
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
								currentNode["_text"] += textValue;
							}

							textValue = "";
							nodeName = "";
							currentNode = parents.Pop();
                        }
                        else
                        {

							if (textValue.Length > 0)
							{
								currentNode["_text"] += textValue;
							}
							textValue = "";
							XMLNode newNode = new XMLNode();
							newNode["_text"] = "";

							newNode["_name"] = nodeName;

							if (currentNode[nodeName] == null)
							{
								currentNode[nodeName] = new XMLNodeList();
							}
							XMLNodeList a = currentNode[nodeName] as XMLNodeList;
							a.Push(newNode);
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
                        if (!string.IsNullOrEmpty(attName))
                        {
                            if (!string.IsNullOrEmpty(attValue))
                            {
                                currentNode["@" + attName] = attValue;
                            }
                            else
                            {
                                currentNode["@" + attName] = true;
                            }
                        }

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
                        if (!string.IsNullOrEmpty(attName))
                        {
                            currentNode["@" + attName] = attValue;
                        }

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
                                    currentNode["@" + attName] = attValue;
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
                                        currentNode["@" + attName] = attValue;
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
        return rootNode;
    }
}

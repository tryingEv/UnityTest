/****************************************************************
*FileName:     YamlParasePrefab.cs 
*Author:       Tree
*UnityVersion：2018.3.0f2 
*Date:         2019-01-09 10:27 
*Description:    
*History:         
*****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using YamlDotNet.RepresentationModel;
using System.IO;
using System.Text;

public class ParasePrefabUtility
{

    #region parase yaml Logic

    /// <summary>
    /// 预制体yaml解析为文本
    /// </summary>
    /// <param name="objPrefab"></param>
    /// <returns></returns>
    private static string PrasePrefabToText(GameObject objPrefab)
    {
        var assetPath = AssetDatabase.GetAssetPath(objPrefab).Replace("Assets/", "");
        var prefabPath = string.Format("{0}/{1}", Application.dataPath, assetPath);
        if (!File.Exists(prefabPath)) return null;

        var type = Path.GetExtension(prefabPath);
        if (!type.Equals(".prefab"))
        {
            EditorUtility.DisplayDialog("tip", "please select ui prefab", "sure");
            return null;
        }
        string context = File.ReadAllText(prefabPath);
        Debug.Log(context);
        return context;
    }

    /// <summary>
    /// 将yaml文本解析为UINode
    /// </summary>
    /// <param name="objPrefab">预制体</param>
    /// <param name="bindingOnPrefabUiScript">预制体绑定的UI文件的GUID</param>
    /// <param name="scriptNodeList">解析的所有脚本</param>
    /// <returns></returns>
    public static List<UINode> PrasePrefabTextToUINode(GameObject objPrefab, string bindingOnPrefabUiScript, ref List<ScriptNode> scriptNodeList)
    {
        if (null == objPrefab) return null;

        string context = ParasePrefabUtility.PrasePrefabToText(objPrefab);
        if (string.IsNullOrEmpty(context)) return null;

        int startIndex = 0;
        int endIndex = 0;
        List<string> strList = new List<string>();
        while (true)
        {
            if (startIndex >= context.Length)
            {
                break;
            }

            endIndex = context.IndexOf("--- !", startIndex);
            if (startIndex < endIndex)
            {
                strList.Add(context.Substring(startIndex, endIndex - startIndex));
                startIndex = endIndex + 4;
            }
            else
            {
                if (startIndex < context.Length)
                {
                    strList.Add(context.Substring(startIndex, context.Length - startIndex));
                }
                break;
            }
        }

        List<ObjNode> objList = new List<ObjNode>();
        List<TransformNode> transList = new List<TransformNode>();
        for (int i = 1; i < strList.Count; i++)
        {
            var paraseStr = strList[i];
            PraseNote(paraseStr, bindingOnPrefabUiScript, ref objList, ref transList, ref scriptNodeList);
        }

        List<UINode> nodeList = GetUINode(objList, transList);
        return nodeList;
    }

    private static List<UINode> GetUINode(List<ObjNode> objList, List<TransformNode> transList)
    {
        if (transList.Count > 0 && objList.Count > 0 && transList.Count == objList.Count)
        {
            //将objNode和对应的transformNode 组合成uinode
            List<UINode> nodeList = new List<UINode>();
            for (int i = 0; i < objList.Count; i++)
            {
                ObjNode objNode = objList[i];
                string transformGuid = objNode.TransformGuid;
                TransformNode transNode = transList.Find(p => { return (p.Guid.Equals(transformGuid)); });
                if (null != transNode)
                {
                    nodeList.Add(new UINode() { ObjNode = objNode, TransNode = transNode, ParentNode = null });
                }
            }

            //设置每个uinode的父级节点
            for (int i = 0; i < nodeList.Count; i++)
            {
                UINode node = nodeList[i];
                string parentGuid = node.TransNode.ParentGuid;
                if (!string.IsNullOrEmpty(parentGuid))
                {
                    UINode parentNode = nodeList.Find(p => { return (p.ObjNode.TransformGuid.Equals(parentGuid)); });
                    if (null != parentNode) node.ParentNode = parentNode;
                }
            }

            return nodeList;
        }

        return null;
    }

    private static void PraseNote(string str, string bindingOnPrefabUiScript, ref List<ObjNode> objList, ref List<TransformNode> transList, ref List<ScriptNode> scriptNodeList)
    {
        string[] strs = str.Split('\n');

        if (strs.Length > 2)
        {
            string type = strs[1];
            if (type.Equals("GameObject:"))
            {
                ObjNode node = PraseStrToObjNode(strs);
                if (null != node) objList.Add(node);
            }
            else if (type.Equals("Transform:") || type.Equals("RectTransform:"))
            {
                TransformNode node = PraseStrToTransformNode(strs);
                if (null != node) transList.Add(node);
            }
            else if (type.Equals("MonoBehaviour:"))
            {
                ScriptNode node = PraseStrToScriptNode(strs, bindingOnPrefabUiScript);
                if (null != node) scriptNodeList.Add(node);
                
            }
        }
    }

    /// <summary>
    /// 解析GameObject yaml
    /// </summary>
    /// <param name="str"></param>
    private static ObjNode PraseStrToObjNode(string[] prefabStrs)
    {
        if (null == prefabStrs || prefabStrs.Length < 2) return null;

        ObjNode node = new ObjNode();

        node.Guid = ParaseGuid(prefabStrs[0]);
        List<string> componetList = new List<string>();
        string nodeName = string.Empty;
        for (int i = 2; i < prefabStrs.Length; i++)
        {
            string pstr = prefabStrs[i];
            if (pstr.IndexOf("- component") >= 0)
            {
                string[] comStrs = pstr.Split(':');
                if (comStrs.Length >= 3)
                {
                    string guid = comStrs[2].Replace("}", "").Trim();
                    componetList.Add(guid);
                }
            }
            else if (pstr.IndexOf("m_Name") >= 0)
            {
                string[] nameStrs = pstr.Split(':');
                nodeName = nameStrs[1].Trim();
            }
        }

        node.Name = nodeName;
        node.ComponetsGuid = componetList;
        if (componetList.Count > 0)
        {
            node.TransformGuid = componetList[0];
        }
        return node;
    }

    /// <summary>
    /// 解析Transform yaml
    /// </summary>
    /// <param name="str"></param>
    private static TransformNode PraseStrToTransformNode(string[] prefabStrs)
    {
        if (null == prefabStrs || prefabStrs.Length < 2) return null;

        TransformNode node = new TransformNode();


        List<string> childrenList = new List<string>();
        string parentGuid = string.Empty;
        bool isBeginParaseChildern = false;
        for (int i = 2; i < prefabStrs.Length; i++)
        {
            string pstr = prefabStrs[i];
            if (isBeginParaseChildern)
            {
                if (pstr.IndexOf("- {fileID: ") > 0)
                {
                    string[] fileStrs = pstr.Split(':');
                    if (fileStrs.Length >= 2)
                    {
                        var fileStr = fileStrs[1].Replace("}", "").Trim();
                        childrenList.Add(fileStr);
                    }
                }
                else
                {
                    isBeginParaseChildern = false;
                }
            }

            if (!isBeginParaseChildern)
            {
                if (pstr.IndexOf("m_Children") >= 0 && !pstr.Equals("m_Children: []"))
                {
                    isBeginParaseChildern = true;
                }
                else if (pstr.IndexOf("m_Father") >= 0)
                {
                    string[] nameStrs = pstr.Split(':');
                    if (nameStrs.Length >= 3)
                    {
                        parentGuid = nameStrs[2].Replace("}", "").Trim();
                    }
                }
            }
        }

        node.Guid = ParaseGuid(prefabStrs[0]);
        node.ChildrenGuid = childrenList;
        node.ParentGuid = parentGuid;
        return node;
    }

    public static ScriptNode PraseStrToScriptNode(string[] prefabStrs, string bindingOnPrefabUiScript)
    {
        if (null == prefabStrs || prefabStrs.Length <= 0) return null;
        string guid = ParaseGuid(prefabStrs[0]);
        string scriptGuid = ParaseScriptId(prefabStrs);
        bool isNeedToParaseMembers = scriptGuid.Equals(bindingOnPrefabUiScript);
        if (!isNeedToParaseMembers)
        {
            return new ScriptNode() { Guid = guid, ScriptGuid = scriptGuid, MembersValue = null };
        }

        var isParaseMember = false;
        string arrayValueName = string.Empty;
        List<ScriptMemberValue> memebersValueList = null;
        for (int i = 2; i < prefabStrs.Length; i++)
        {
            var str = prefabStrs[i];
            if (isParaseMember)
            {
                if (str.IndexOf("- {fileID:") >= 0) //数组类型的值
                {
                    if (!string.IsNullOrEmpty(arrayValueName))
                    {
                        ScriptMemberValue value = memebersValueList.Find(p => { return (p.Name.Equals(arrayValueName)); });
                        if (null != value)
                        {
                            string[] valueStrs = str.Split(':');
                            value.ArrayValue.Add(valueStrs[1].Replace("}", "").Trim());
                        }
                    }
                }
                else
                {
                    string[] strs = str.Split(':');
                    //判断是否为值类型数组(不解析)
                    if (!string.IsNullOrEmpty(arrayValueName))
                    {
                        if (str.Contains("-") && strs.Length < 2)
                        {
                            ScriptMemberValue value = memebersValueList.Find(p => { return (p.Name.Equals(arrayValueName)); });
                            if (null != value) memebersValueList.Remove(value);
                            continue;
                        }
                        else
                        {
                            arrayValueName = string.Empty;
                        }
                    }
                    string memeberValueName = strs[0].Trim();
                    var memberValue = new ScriptMemberValue() { Name = memeberValueName, IsArray = false, ArrayValue = null, Value = string.Empty, isFileType = true };
                    if (strs.Length > 2)    //非数组类型，且为控件类型(格式：TextA: {fileID: 114712569201221842})
                    {
                        memberValue.Value = strs[2].Replace("}", "").Trim();
                    }
                    else if (strs.Length == 2)   //数组类型
                    {
                        if (string.IsNullOrEmpty(strs[1]))
                        {
                            memberValue.IsArray = true;
                            memberValue.ArrayValue = new List<string>();
                            arrayValueName = memeberValueName;
                        }
                        else //值类型(格式：abc: 15820000001)
                        {
                            memberValue.Value = strs[1].Trim();
                            memberValue.isFileType = false;
                        }
                        
                    }
                   
                    if (null == memebersValueList)
                    {
                        memebersValueList = new List<ScriptMemberValue>();
                    }
                    memebersValueList.Add(memberValue);
                }

            }
            else
            {
                if (str.IndexOf("m_EditorClassIdentifier") >= 0)
                {
                    isParaseMember = true;
                }
            }
        }

        return new ScriptNode() { ScriptGuid = scriptGuid, Guid = guid, MembersValue = memebersValueList };
    }

    /// <summary>
    /// 解析guid(格式：!u!1 &1345169841588886)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string ParaseGuid(string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        string[] guiIdStr = str.Split('&');
        if (guiIdStr.Length >= 2)
        {
            return guiIdStr[1];
        }

        return string.Empty; ;
    }

    /// <summary>
    /// 解析guid(格式：!u!1 &1345169841588886)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string ParaseScriptId(string[] strs)
    {
        if (null == strs && strs.Length <= 0) return null;
        foreach (var item in strs)
        {
            if (item.Contains("m_Script: "))
            {
                string[] scriptStrs = item.Split(',');
                string[] valuse = scriptStrs[1].Split(':');
                return valuse[1].Trim();
            }
        }
        return string.Empty; ;
    }

    #endregion

    #region 解析通用方法
    /// <summary>
    /// 获取绑定在预制体上的UI脚本guid
    /// </summary>
    /// <param name="objPrefab"></param>
    /// <returns></returns>
    public static string GetBingdingScripGuid(GameObject objPrefab)
    {
        if (null == objPrefab) return null;

        string path = AssetDatabase.GetAssetPath(objPrefab);
        string[] dependenceFilesPath = AssetDatabase.GetDependencies(path);
        for (int i = 0; i < dependenceFilesPath.Length; i++)
        {
            var dependenceFilePath = dependenceFilesPath[i];
            string type = Path.GetExtension(dependenceFilesPath[i]);
            if (type.Equals(".cs") && dependenceFilePath.Contains(AuoCreateUIScript.SCRIPT_ASSET_PATH))
            {
                return AssetDatabase.AssetPathToGUID(dependenceFilePath);
            }
        }

        return null;
    }
    #endregion
}

#region node class
public class UINode
{
    public ObjNode ObjNode;
    public TransformNode TransNode;
    public UINode ParentNode;

}

public class ObjNode
{
    public string Name;
    public string Guid;
    public string TransformGuid;
    public List<string> ComponetsGuid;
}

public class ScriptNode
{
    /// <summary>
    /// 对应的脚本meta文件中的guid
    /// </summary>
    public string ScriptGuid;
    public string Guid;
    public List<ScriptMemberValue> MembersValue;
}

public class ScriptMemberValue
{
    public string Name;
    public string Value;
    public bool IsArray;
    public bool isFileType;
    public List<string> ArrayValue;
}

public class TransformNode
{
    public List<string> ChildrenGuid;
    public string ParentGuid;
    public string Guid;
}
#endregion





/****************************************************************
*FileName:     AuoCreateUIScript.cs 
*Author:       Tree
*UnityVersion：2017.3.1p4 
*Date:         2019-01-11 14:25 
*Description:    
*History:         
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

class AuoCreateUIScript
{
    public const string SCRIPT_ASSET_PATH = "Assets/Scripts";
    private const string UI_TEMPLATE_PATH = "Assets/Editor/yaml/Template.txt";
    //private static Dictionary<string, string> widgetTypeDict = null;

    //private static bool RegisterWidgetType(string guid, string type)
    //{
    //    if (null == widgetTypeDict) widgetTypeDict = new Dictionary<string, string>();
    //    if (widgetTypeDict.ContainsKey(guid)) return false;

    //    widgetTypeDict.Add(guid, type);
    //    return true;
    //}

    //private static void InitWidgetType()
    //{
    //    if (null != widgetTypeDict) return;

    //    RegisterWidgetType("f70555f144d8491a825f0804e09c671c", "Text");
    //    RegisterWidgetType("f70555f144d8491a825f0804e09c671c", "Text");
    //    RegisterWidgetType("f70555f144d8491a825f0804e09c671c", "Text");
    //    RegisterWidgetType("f70555f144d8491a825f0804e09c671c", "Text");
    //}

    [MenuItem("tools/PraseUIPrefab")]
    private static void AutoCreateScript()
    {
        GameObject objPrefab = Selection.activeGameObject;
        if (null == objPrefab) return;

        string bindingOnPrefabUiScriptGuid = ParasePrefabUtility.GetBingdingScripGuid(objPrefab);
        List<ScriptNode> scriptNodeList = new List<ScriptNode>();
        List<UINode> nodeList = ParasePrefabUtility.PrasePrefabTextToUINode(objPrefab, bindingOnPrefabUiScriptGuid, ref scriptNodeList);
        string declareStr = string.Empty;
        string initWidgetStr = string.Empty;
        GetUIStr(scriptNodeList, nodeList, bindingOnPrefabUiScriptGuid, ref declareStr, ref initWidgetStr);
        CreateUIScript(objPrefab.name, declareStr, initWidgetStr);
    }

    private static void CreateUIScript(string prefabName, string declareStr, string initWidgetStr)
    {
        string templatePath = string.Format("{0}{1}", Application.dataPath, UI_TEMPLATE_PATH.Replace("Assets", "")).Replace("//", "/");
        if (!File.Exists(templatePath))
        {
            EditorUtility.DisplayDialog("tip", "never find ui class template", "ok");
            return;
        }
        string scriptPath = string.Format("{0}{1}", Application.dataPath, string.Format("{0}/{1}.cs", SCRIPT_ASSET_PATH, prefabName).Replace("Assets", "")).Replace("//", "/");
        if (File.Exists(scriptPath))
        {
            File.Delete(scriptPath);
        }

        string content = File.ReadAllText(templatePath).Replace("@className", prefabName).
            Replace("@declare", declareStr).Replace("@initWidget", initWidgetStr);

        File.WriteAllText(scriptPath, content, System.Text.Encoding.UTF8);
    }

    private static void GetUIStr(List<ScriptNode> scripNodeList, List<UINode> nodeList, string bindingOnPrefabUiScript, ref string declareStr, ref string initWidgetStr)
    {
        if (null == scripNodeList || null == nodeList) return;
        ScriptNode bindingScript = scripNodeList.Find(p => { return (p.ScriptGuid.Equals(bindingOnPrefabUiScript)); });
        if (null == bindingScript) return;

        string declareTemplate = "private {0} {1};";
        string findNodeFileTemplate = "{0} =  root.Find(\"{1}\").GetComponent<{2}>();";
        string valeTypeTemplate = "{0} = {1};";
        List<ScriptMemberValue> memberParamList = bindingScript.MembersValue;
        StringBuilder declareSb = new StringBuilder();
        StringBuilder findNodeSb = new StringBuilder();
        foreach (var item in memberParamList)
        {
            string memberParaName = item.Name;
            if (string.IsNullOrEmpty(memberParaName)) continue;

            if (item.IsArray && item.ArrayValue.Count > 0)
            {
                string type = GetType(item.isFileType, item.ArrayValue[0], scripNodeList, nodeList);
                var str = string.Format(declareTemplate, string.Format("{0}[]", type), memberParaName);
                declareSb.Append("\t").Append(str).Append("\n");
                if (item.isFileType)
                {
                    for (int i = 0; i < item.ArrayValue.Count; i++)
                    {
                        string nodePath = GetNodePath(item.ArrayValue[i], nodeList);
                        var findNodeStr = string.Format(findNodeFileTemplate, string.Format("{0}[{1}]", memberParaName, i), nodePath, type);
                        findNodeSb.Append("\t").Append(findNodeStr).Append("\n");
                    }
                }
            }
            else
            {
                string type = GetType(item.isFileType, item.Value, scripNodeList, nodeList);
                var str = string.Format(declareTemplate, type, memberParaName);
                declareSb.Append("\t").Append(str).Append("\n");

                string nodePath = GetNodePath(item.Value, nodeList);
                var findNodeStr = string.Empty;
                if (item.isFileType)
                {
                    findNodeStr = string.Format(findNodeFileTemplate, memberParaName, nodePath, type);
                }
                else
                {
                    findNodeStr = string.Format(valeTypeTemplate, memberParaName, item.Value);
                }
                findNodeSb.Append("\t").Append(findNodeStr).Append("\n");
            }
        }
        declareStr = declareSb.ToString();
        initWidgetStr = findNodeSb.ToString();
    }


    private static string GetType(bool isFile, string value, List<ScriptNode> scripNodeList, List<UINode> nodeList)
    {
        var type = string.Empty;
        if (isFile)
        {
            type = GetTypeByGuid(value, scripNodeList, nodeList);
        }
        else
        {
            if (value.Contains("."))
            {
                type = "float";
            }
            else
            {
                type = "int";
            }
        }

        return type;
    }

    private static string GetTypeByGuid(string guid, List<ScriptNode> scripNodeList, List<UINode> nodeList)
    {
        //看是否为MonoBehaviour
        ScriptNode scriptNode = scripNodeList.Find(p => { return (p.Guid.Equals(guid)); });
        if (null != scriptNode)
        {
            string scriptGuid = scriptNode.ScriptGuid;
            string filePath = AssetDatabase.GUIDToAssetPath(scriptGuid);
            string fileName = Path.GetFileName(filePath);
            return fileName;
        }

        UINode node = nodeList.Find(p => { return p.ObjNode.Guid.Equals(guid); });
        if (null != node) return "GameObject";

        UINode transNode = nodeList.Find(p => { return p.TransNode.Guid.Equals(guid); });
        if (null != transNode) return "Transform";

        Debug.LogError("find type failed guid = " + guid);
        return string.Empty;
    }

    private static string GetNodePath(string componentGuid, List<UINode> uiNodeList)
    {
        if (string.IsNullOrEmpty(componentGuid) || (null == uiNodeList || uiNodeList.Count <= 0)) return null;
        UINode selfNode = uiNodeList.Find(p =>
        {
            List<string> componentsGuid = p.ObjNode.ComponetsGuid;
            if (null != componentsGuid && componentsGuid.Count > 0)
            {
                string guid = componentsGuid.Find(pp => { return (pp.Equals(componentGuid)); });
                return !string.IsNullOrEmpty(guid);
            }
            return false;
        });

        if (null == selfNode)
        {
            selfNode = uiNodeList.Find(p =>
            {
                return p.ObjNode.Guid.Equals(componentGuid);
            });
        }

        if (null == selfNode) return null;
        Debug.Log("selfNode is " + selfNode.ObjNode.Name);

        //查找父节点
        UINode parentNode = selfNode.ParentNode;
        if (null == parentNode) //没有父节点，说明是根节点
        {
            return selfNode.ObjNode.Name;
        }

        UINode searchNode = selfNode;
        var sb = new StringBuilder();
        while (true)
        {
            if (null == searchNode) break;

            UINode pNode = searchNode.ParentNode;
            if (null != pNode && null != pNode.ParentNode)  //过滤掉预制体节点
            {
                sb.Insert(0, string.Format("/{0}", pNode.ObjNode.Name));
            }
            searchNode = pNode;
        }

        sb.Append(string.Format("/{0}", selfNode.ObjNode.Name));
        sb.Remove(0, 1);
        return sb.ToString();
    }
}


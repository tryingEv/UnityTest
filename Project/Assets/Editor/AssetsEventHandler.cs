using UnityEditor;
using UnityEngine;

public class AssetsEventHandler : UnityEditor.AssetModificationProcessor
{
    private static string GetFileName(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        int index = path.LastIndexOf('/');
        return path.Substring(index + 1);
    }

    /// <summary>
    /// 选中文件后，如果返回true则在Inspector面板中会显示信息，否则不显示
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="msg"></param>
    /// <returns></returns>
    public static bool IsOpenForEdit(string assetPath, out string msg)
    {
        msg = null;
        string fileName = GetFileName(assetPath).Replace(".meta", string.Empty);
        Debug.Log("---- IsOpenForEdit asstePath = " + assetPath + ", file name = " + fileName);//---- IsOpenForEdit asstePath = Assets/scene/EditorExtensionTest.unity.meta
        int index = fileName.LastIndexOf('.');
        if (index >= 0)
        {
            string type = fileName.Substring(index + 1);
            switch (type)
            {
                case "unity":
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 创建文件时候会调用
    /// </summary>
    /// <param name="assetName"></param>
    public static void OnWillCreateAsset(string assetName)
    {
        AddFileHeadComment.AddFileHead(assetName);
        Debug.Log("----- create asset, name is " + assetName);
    }

    /// <summary>
    /// 删除文件时候会调用
    /// </summary>
    /// <param name="assetName"></param>
    public static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
    {
        Debug.Log("----- delete file, name is " + assetName);
        //return AssetDeleteResult.DidDelete;//不可删除

        return AssetDeleteResult.DidNotDelete; //可以删除
    }

    /// <summary>
    /// 移动文件时会调用
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="targetPath"></param>
    /// <returns></returns>
    public static AssetMoveResult OnWillMoveAsset(string assetPath, string targetPath)
    {
        Debug.Log("---- move asset, path = " + assetPath + ", targetPath = " + targetPath);
        return AssetMoveResult.DidNotMove;
    }

    /// <summary>
    /// 保存文件调用
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] OnWillSaveAssets(string[] path)
    {
        foreach (var p in path)
        {
            Debug.Log("save path is " + p);
        }
        return path;
    }
}

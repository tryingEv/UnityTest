/****************************************************************
*FileName:     BuildAssetBundle.cs 
*Author:       Tree
*UnityVersion：2017.3.1p4 
*Date:         2018-12-03 18:16 
*Description:    
*History:         
*****************************************************************/

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundle
{
    [MenuItem("Build AssetBundle/Prefab in ShaderArt directory")]
    public static void BuildShaderArtAssetBundle()
    {
        string rootDirectory = string.Format("{0}/ShaderArt/Prefab", Application.dataPath) ;
        if (!Directory.Exists(rootDirectory))
        {
            Debug.LogError("root directory is not exist");
            return;
        }

        Dictionary<string, List<string>> dependDict = new Dictionary<string, List<string>>();
        string[] prefabsPath = Directory.GetFiles(rootDirectory, "*.prefab");
        foreach (var file in prefabsPath)
        {
            var path = file.Replace("//", "/").Replace("\\", "/");
            if (!File.Exists(path))
                continue;

            var rootFile = path.Replace(Application.dataPath, "Assets");
            FindTargetFileDependFiles(rootFile, ref dependDict);
        }
        Debug.Log("dependDict count " + dependDict.Count);

        if (dependDict.Count <= 0)
        {
            Debug.LogError("not found gameObject need to build");
            return;
        }
        AssetBundleBuild[] bundles = new AssetBundleBuild[dependDict.Count];
        int index = 0;
        foreach (var dvalue in dependDict)
        {
            var key = dvalue.Key;
            var list = dvalue.Value;
            bundles[index] = new AssetBundleBuild() { assetBundleName = key, assetNames = list.ToArray()};
            index++;
        }
        string output = "AssetsBundle/ShaderTest";
        var fullPath = string.Format("{0}/{1}", Application.dataPath, output);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        BuildPipeline.BuildAssetBundles(string.Format("Assets/{0}", output), bundles, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }


    public static void FindTargetFileDependFiles(string targetFilePath, ref Dictionary<string, List<string>> dependDict)
    {
        string[] dependFiles = AssetDatabase.GetDependencies(targetFilePath);
        foreach (var fileName in dependFiles)
        {
            if (fileName.Contains(targetFilePath))
                continue;

            int index = fileName.LastIndexOf("/");
            if (index < 0)
                continue;

            var floderName = Path.GetDirectoryName(fileName).Replace("Assets/", "");
            if (dependDict.ContainsKey(floderName))
            {
                List<string> list = dependDict[floderName];
                if (null == list)
                    continue;

                if (list.Contains(fileName))
                    continue;

                list.Add(fileName);
            }
            else
            {
                dependDict.Add(floderName, new List<string>() { fileName });
            }

            //FindTargetFileDependFiles(fileName, ref dependDict);
        }
    }
}
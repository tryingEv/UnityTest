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
    public const string ASSETBUNDLE_FLODER = "StreamingAssets";

    private static readonly List<string> _dependMaterials = new List<string>();

    [MenuItem("Build AssetBundle/BuildModel")]
    public static void StartBuildModelAssetBundle()
    {
        var assetBundlePath = Path.Combine(Application.dataPath, ASSETBUNDLE_FLODER);
        if (!Directory.Exists(assetBundlePath)) Directory.CreateDirectory(assetBundlePath);

        var list = new AssetBundleBuild[1];
        var abuild = new AssetBundleBuild();
        abuild.assetNames = new[] {"Assets/Prefabs/Bamboo3.prefab", "Assets/Prefabs/B-Boy_Player.prefab"};
        abuild.assetBundleName = "Amodels";
        list[0] = abuild;
        BuildPipeline.BuildAssetBundles(assetBundlePath, list, BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.Android);
    }

    [MenuItem("Build AssetBundle/Cube and Sphere")]
    public static void StartBuildCubeAssetsBundle()
    {
        var path = Path.Combine(Application.dataPath, "Resource/Prefabs/TestAssetsBundle").Replace("\\", "/");
        var files = Directory.GetFiles(path);
        _dependMaterials.Clear();
        var assetBundlePath = Path.Combine(Application.dataPath, ASSETBUNDLE_FLODER).Replace("\\", "/");
        if (!Directory.Exists(assetBundlePath)) Directory.CreateDirectory(assetBundlePath);

        foreach (var fpath in files)
        {
            var filePath = fpath.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            var fileType = Path.GetExtension(filePath);
            if (fileType.Equals(".meta")) continue;
            Debug.Log("---- file path " + filePath);
            var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject)) as GameObject;
            if (null == obj) continue;
            var render = obj.GetComponent<MeshRenderer>();
            if (null == render) continue;
            var mats = render.sharedMaterials;
            if (null != mats)
                foreach (var mat in mats)
                {
                    var texture = mat.mainTexture;
                    if (null != texture)
                    {
                        var texturePath = string.Format("Assets/Art/Textures/{0}.psd", texture.name.ToUpper());
                        if (!_dependMaterials.Contains(texturePath))
                        {
                            _dependMaterials.Add(texturePath);
                            BuildAssetBundleInStreamingAssetsFloder(texturePath);
                        }
                    }

//                    var materialName = mat.name;
//                    if(string.IsNullOrEmpty(materialName)) continue;
//                    var materialPath = string.Format("Assets/Art/Material/{0}.mat", materialName);
//                    if (!_dependMaterials.Contains(materialPath))
//                    {
//                        _dependMaterials.Add(materialPath);
//                    }
//
//                    BuildAssetBundleInStreamingAssetsFloder(materialPath);
                }
        }

        foreach (var file in files)
        {
            var filePath = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
            BuildAssetBundleInStreamingAssetsFloder(filePath);
        }
    }

    public static void BuildAssetBundleInStreamingAssetsFloder(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;

        var bundlePath = "Assets/StreamingAssets";
        var filePath = fileName.Replace(Application.dataPath, "Assets").Replace("\\", "/");
        var assetBundleBuild = new AssetBundleBuild
            {assetBundleName = Path.GetFileName(filePath), assetNames = new[] {filePath}};
        BuildPipeline.BuildAssetBundles(bundlePath, new[] {assetBundleBuild},
            BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }
}
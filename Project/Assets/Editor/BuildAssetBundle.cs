/****************************************************************
*FileName:     BuildAssetBundle.cs 
*Author:       Tree
*UnityVersion：2017.3.1p4 
*Date:         2018-12-03 18:16 
*Description:    
*History:         
*****************************************************************/

using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class BuildAssetBundle 
{
    public const string ASSETBUNDLE_FLODER = "StreamingAssets";

    [MenuItem("Build AssetBundle/BuildModel")]
    public static void StartBuildModelAssetBundle()
    {
        string assetBundlePath = Path.Combine(Application.dataPath, ASSETBUNDLE_FLODER);
        if (!Directory.Exists(assetBundlePath))
        {
            Directory.CreateDirectory(assetBundlePath);
        }

        AssetBundleBuild[] list = new AssetBundleBuild[1];
        AssetBundleBuild abuild = new AssetBundleBuild();
        abuild.assetNames = new string[]{"Assets/Prefabs/Bamboo3.prefab", "Assets/Prefabs/B-Boy_Player.prefab" };
        abuild.assetBundleName = "Amodels";
        list[0] = abuild;
        BuildPipeline.BuildAssetBundles(assetBundlePath, list, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
    }

    private static List<string> _dependMaterials = new List<string>();
    [MenuItem("Build AssetBundle/Cube and Sphere")]
    public static void StartBuildCubeAssetsBundle()
    {
        string path = Path.Combine(Application.dataPath, "Resource/Prefabs/TestAssetsBundle").Replace("\\", "/");
        string[] files = Directory.GetFiles(path);
        _dependMaterials.Clear();
        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i];
            string fileType = Path.GetExtension(fileName);
            if (fileType.Equals(".meta")) continue;
            Debug.Log("---- file name " + fileName);
            MeshRenderer render = AssetDatabase.LoadAssetAtPath(fileName, typeof(MeshRenderer)) as MeshRenderer;
            if (null == render) continue;
            Material[] mats = render.materials;
            foreach(Material mat in mats)
            {
                string materialName = mat.name;
                if (!_dependMaterials.Contains(materialName))
                {
                    string materialPath = string.Format("Assets/Art/Material/{0}", materialName).Replace("\\", "/");
                    _dependMaterials.Add(materialPath);

                    //BuildPipeline.build
                }
            }
        }
    }
}

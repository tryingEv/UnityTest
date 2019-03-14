using System;
using System.IO;
using UnityEngine;

public class AddFileHeadComment
{
    public static void AddFileHead(string path)
    {
        var filePath = path.Replace(".meta", string.Empty);
        var fileType = Path.GetExtension(filePath);
        if (!fileType.Equals(".cs")) return;
        var headStr =
            @"/****************************************************************
*FileName:     #SCRIPTFULLNAME# 
*Author:       Tree
*UnityVersion：#UNITYVERSION# 
*Date:         #DATE# 
*Description:    
*History:         
*****************************************************************/";
        var fullPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), filePath);
        var headerContent = headStr.Replace("#SCRIPTFULLNAME#", Path.GetFileName(filePath))
            .Replace("#UNITYVERSION#", Application.unityVersion)
            .Replace("#DATE#", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        var content = string.Format("{0}\n{1}", headerContent, File.ReadAllText(fullPath));
        File.WriteAllText(fullPath, content);
    }
}
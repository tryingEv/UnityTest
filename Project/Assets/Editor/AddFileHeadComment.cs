using System.IO;
using UnityEngine;

public class AddFileHeadComment
{
    public static void AddFileHead(string path)
    {
        string filePath = path.Replace(".meta", string.Empty);
        string fileType = Path.GetExtension(filePath);
        if (!fileType.Equals(".cs")) return;
        string headStr = 
@"/****************************************************************
*FileName:     #SCRIPTFULLNAME# 
*Author:       Tree
*UnityVersion：#UNITYVERSION# 
*Date:         #DATE# 
*Description:    
*History:         
*****************************************************************/";
        string fullPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), filePath);
        string headerContent = headStr.Replace("#SCRIPTFULLNAME#", Path.GetFileName(filePath)).Replace("#UNITYVERSION#", Application.unityVersion).Replace("#DATE#", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        string content = string.Format("{0}\n{1}", headerContent, File.ReadAllText(fullPath));
        File.WriteAllText(fullPath, content);
    }

}
/****************************************************************
*FileName:     ReadExcel.cs 
*Author:       Tree
*UnityVersion：2017.3.1p4 
*Date:         2019-03-08 09:56 
*Description:    
*History:         
*****************************************************************/
using OfficeOpenXml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ReadExcel
{
    public struct ExcelData
    {
        public string Level;
        public string RoleName;
        public string Desc;
    }
    [MenuItem("tools/parase excel")]
    public static void ParaseExcel()
    {
        List<ExcelData> list = new List<ExcelData>();
        string path = string.Format("{0}/{1}", Application.dataPath, "Editor/ReadExcel/UserLevel.xlsx") ;
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        ExcelPackage excelPackage = new ExcelPackage(fs);
        if (null == excelPackage) return;

        ExcelWorksheets worksheets = excelPackage.Workbook.Worksheets;
        if (worksheets.Count <= 0) return;
        var sheet = worksheets[1];
        if (null == sheet) return;
        int rowCount = sheet.Dimension.End.Row;
        if (rowCount < 1) return;   //表头不读

        int colums = sheet.Dimension.End.Column;
        Debug.LogError("--- sheet colums " + colums);
        for (int i = 1; i <= rowCount; i++)
        {
            string level = sheet.Cells[i, 1].Text;
            string roleName = sheet.Cells[i, 2].Text;
            string desc = sheet.Cells[i, 3].Text;

            list.Add(new ExcelData()
            {
                Level = level,
                RoleName = roleName,
                Desc = desc
            });
        }

        Debug.LogError("list count " + list.Count);
    }

}

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class EditorTest 
{
    [MenuItem("GameObject/Test MenuItem in GameObject Menu", false, 22)]
    public static void TestMenuItem()
    {
        Debug.Log("----> Test MenuItem in GameObject Menu");
    }

    /// <summary>
    /// 在Inspector面板中Transform组件右键鼠标将会显示该菜单，如果需要在其他组件中显示，则只需要将Transform组件换成其他组件
    /// </summary>
    [MenuItem("CONTEXT/Transform/RightClickOnTransformComponet")]
    public static void TestMenuItemInInspectorComponetRightClick()
    {
        Debug.Log("----> RightClickOnTransformComponet");
    }

    /// <summary>
    /// MenuItem的第二个参数为true时，表示在该函数返回true时显示，同时需要有一个对应的函数在何时不显示，如TestMenuItemSecondPararmentHide
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/TestMenuItemSecondPararment", true, 11)]
    public static bool TestMenuItemSecondPararmentShow()
    {
        return (null != Selection.activeObject);
    }
    
    [MenuItem("Assets/TestMenuItemSecondPararment", false, 11)]
    public static bool TestMenuItemSecondPararmentHide()
    {
        return (null == Selection.activeObject);
    }
}

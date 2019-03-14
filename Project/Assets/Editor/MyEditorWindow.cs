using UnityEditor;
using UnityEngine;

public class MyEditorWindow : EditorWindow
{
    private int _popupIndex;
    private readonly string[] _poupuStr = {"popup1", "popup2", "popup3"};
    private Color _selectedColor = Color.white;
    private float _sliderValue;
    private string _textFieldStr = string.Empty;
    private bool _togSelected;
    private string _togState = string.Empty;

    [MenuItem("EditorExtension/OpenEditor")]
    public static void OpenEditorWindow()
    {
        var rect = new Rect(0f, 0f, 500f, 500f);
        var window = (MyEditorWindow) GetWindowWithRect(typeof(MyEditorWindow), rect, false, "window");
        window.Show();
        Debug.Log("OpenEditorWindow");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Time since start: ", EditorApplication.timeSinceStartup.ToString());
        Repaint();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        _togSelected = EditorGUILayout.Toggle("toggle:", _togSelected);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(_togSelected ? "slected toggle" : "not selected toggle");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        _togSelected = EditorGUILayout.ToggleLeft("ToggleLeft:", _togSelected);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(_togSelected ? "slected ToggleLeft" : "not selected ToggleLeft");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        _textFieldStr = EditorGUILayout.TextField("textField", _textFieldStr);

        EditorGUILayout.Space();
        EditorGUILayout.TextArea("锄禾日当午\n汗滴禾下土\n谁知盘中餐\n粒粒皆辛苦");

        EditorGUILayout.Space();
        _sliderValue = EditorGUILayout.Slider("血量：", _sliderValue, 0, 100f);

        EditorGUILayout.Space();
        _selectedColor = EditorGUILayout.ColorField(_selectedColor);

        EditorGUILayout.Space();
        _popupIndex = EditorGUILayout.Popup(_popupIndex, _poupuStr);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("popup index: ", _popupIndex.ToString());

        EditorGUILayout.EndVertical();
    }
}
using UnityEditor;
using UnityEngine;

public class MyEditorWindow : EditorWindow
{
    private bool _togSelected = false;
    private string _togState = string.Empty;
    private string _textFieldStr = string.Empty;
    private float _sliderValue = 0f;
    private Color _selectedColor = Color.white;
    private int _popupIndex = 0;
    private string[] _poupuStr = { "popup1", "popup2", "popup3" };
    [MenuItem("EditorExtension/OpenEditor")]
    public static void OpenEditorWindow()
    {
        Rect rect = new Rect(0f, 0f, 500f, 500f);
        MyEditorWindow window = (MyEditorWindow)GetWindowWithRect(typeof(MyEditorWindow), rect, false, "window");
        window.Show();
        Debug.Log("OpenEditorWindow");
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Time since start: ", EditorApplication.timeSinceStartup.ToString());
        this.Repaint();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        this._togSelected = EditorGUILayout.Toggle("toggle:", this._togSelected);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(this._togSelected ? "slected toggle" : "not selected toggle");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        this._togSelected = EditorGUILayout.ToggleLeft("ToggleLeft:", this._togSelected);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(this._togSelected ? "slected ToggleLeft" : "not selected ToggleLeft");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        this._textFieldStr = EditorGUILayout.TextField("textField", this._textFieldStr);

        EditorGUILayout.Space();
        EditorGUILayout.TextArea("锄禾日当午\n汗滴禾下土\n谁知盘中餐\n粒粒皆辛苦");

        EditorGUILayout.Space();
        this._sliderValue = EditorGUILayout.Slider("血量：", this._sliderValue, 0, 100f);

        EditorGUILayout.Space();
        this._selectedColor = EditorGUILayout.ColorField(this._selectedColor);
        
        EditorGUILayout.Space();
        this._popupIndex = EditorGUILayout.Popup(this._popupIndex, _poupuStr);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("popup index: ", this._popupIndex.ToString());

        EditorGUILayout.EndVertical();

       
    }
}

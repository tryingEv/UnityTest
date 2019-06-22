/****************************************************************
*FileName:     AccelerationCtrl.cs 
*Author:       Tree
*UnityVersion：2017.3.1p4 
*Date:         2019-06-19 14:42 
*Description:    
*History:         
*****************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationCtrl : MonoBehaviour {
    //public Button Btn;
    //public Text Text;
    //public Button BtnRest;
    //private StringBuilder sb = new StringBuilder();

    private Transform _trans;
    public float[] _MoveRange = new float[4];
    public float[] _moveSpeed = new float[2];
    //private float[] _MoveRange = new float[] { 400f, 400f, 220f, 220f };
    //private float[] _moveSpeed = new float[] { 1, 1 };
    private Vector3 _lastAcceleration = Vector3.zero;
    private float _huDu = 0.3f;
    private Vector3 _startPos = Vector3.zero;


    private void Awake()
    {
        _trans = this.transform;
        _startPos = _trans.localPosition;

        //Btn.onClick.AddListener(() =>
        //{
        //    sb.Remove(0, sb.Length);
        //    Text.text = sb.ToString();
        //});


    }

    void Start ()
    {

    }

    private void FixedUpdate()
    {
        UpdateTransPos();
    }

    private void UpdateTransPos()
    {
        Vector3 acces = GetAcceleration();
        if (_lastAcceleration.x != acces.x || _lastAcceleration.y != acces.y)
        {
            //范围
            float horRange = acces.x <= 0 ? _MoveRange[0] : _MoveRange[1];
            float verRange = acces.y <= 0 ? _MoveRange[2] : _MoveRange[3];
            float speedX = _moveSpeed[0];
            float x = 0f;
            if (acces.x == 0)
            {
                x = _startPos.x;
            }
            else
            {
                x = _startPos.x + speedX * horRange * (acces.x / Mathf.Abs(acces.x)) * (Mathf.Min(Mathf.Abs(acces.x), _huDu) / _huDu);
            }
            x = RoundFloat(x, 10);

            float y = 0;
            float speedY = _moveSpeed[1];
            if (acces.y == 0)
            {
                y = _startPos.y;
            }
            else
            {
                y = _startPos.y + speedY * verRange * (acces.y / Mathf.Abs(acces.y)) * (Mathf.Min(Mathf.Abs(acces.y), _huDu) / _huDu);
            }
            y = RoundFloat(y, 10);
            _trans.localPosition = Vector3.Lerp(_trans.localPosition, new Vector3(x, y, _startPos.z), 0.3f);

            _lastAcceleration = acces;
        }
    }

    private Vector3 GetAcceleration()
    {
        Vector3 acces = Input.acceleration;
        acces.x = RoundFloat(acces.x);
        acces.y = RoundFloat(acces.y);
        acces.z = RoundFloat(acces.z);
        return acces;
    }

    private float RoundFloat(float number, float factor = 100 )
    {
        int num = (int)(number * factor);
        return (num / factor);
    }
}

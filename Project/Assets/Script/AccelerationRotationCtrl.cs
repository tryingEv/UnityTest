/****************************************************************
*FileName:     AccelerationRotationCtrl.cs 
*Author:       Tree
*UnityVersion：2017.3.1p4 
*Date:         2019-06-22 13:55 
*Description:    
*History:         
*****************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccelerationRotationCtrl : MonoBehaviour
{
    public enum AccelerationType
    {
        EMove = 1,   //移动
        ERotation,  //旋转
    }

    public AccelerationType OperType;

    #region 旋转操作设置
    /// <summary>
    /// x, y, z三个轴的旋转范围
    /// </summary>
    public Vector3 RotationRange = Vector3.zero; //x, y, z轴

    /// <summary>
    ///  x, y, z三个轴的旋转速度
    /// </summary>
    public Vector3 RotationSpeed = Vector3.one; //x, y, z

    #endregion

    #region 移动操作设置
    /// <summary>
    /// 左右上下移动范围
    /// </summary>
    public Vector4 PosRange = Vector2.zero; //左右上下

    /// <summary>
    /// 水平方向和竖直方向移动速度
    /// </summary>
    public Vector2 PosSpeed = Vector3.one; //x, y轴
    #endregion


    private Transform _trans;
    /// <summary>
    /// 上一次加速度值
    /// </summary>
    private Vector3 _lastAcceleration = Vector3.zero;

    /// <summary>
    /// 起始位置
    /// </summary>
    private Vector3 _startPos = Vector3.zero;

    /// <summary>
    /// 起始旋转角度
    /// </summary>
    private Vector3 _startRotation = Vector3.zero;

    /// <summary>
    /// 最大加速度
    /// </summary>
    private const float maxAccelerationValue = 0.3f;    //加速度最大值


    #region 更新逻辑

    private void UpdateRotation()
    {
        if (null == _trans) return;

        Vector3 acceleration = GetAcceleration();

        if (_lastAcceleration.x == acceleration.x && _lastAcceleration.y == acceleration.y && _lastAcceleration.z == acceleration.z)
        {
            return;
        }

        switch (OperType)
        {
            case AccelerationType.EMove:
                {
                    float posx = GetTransValue(acceleration.x, _startPos.x, acceleration.x <= 0 ?  PosRange.x : PosRange.y, PosSpeed.x);
                    float posy = GetTransValue(acceleration.y, _startPos.y, acceleration.y <= 0 ? PosRange.w : PosRange.z, PosSpeed.y);
                    Vector3 pos = _trans.localPosition;
                    pos.x = posx;
                    pos.y = posy;
                    _trans.localPosition = Vector3.Lerp( _trans.localPosition, pos, 0.3f);
                }
                break;
            case AccelerationType.ERotation: //旋转
                {
                    float rx = GetTransValue(acceleration.x, _startRotation.x, RotationRange.x, RotationSpeed.x);
                    float ry = GetTransValue(acceleration.y, _startRotation.y, RotationRange.y, RotationSpeed.y);
                    //float rz = GetTransValue(acceleration.z, _startRotation.z, RotationRange.z, RotationSpeed.z);
                    Vector3 rotation = _trans.localRotation.eulerAngles;
                    rotation.x = ry;
                    rotation.y = -rx;
                    //rotation.z = rz;
                    _trans.localRotation = Quaternion.Lerp(_trans.localRotation, Quaternion.Euler(rotation), 0.3f) ;
                }
                break;
            default:
                break;
        }
        _lastAcceleration = acceleration;
    }

    private float GetTransValue(float accelerationValue, float baseValue, float range, float speed)
    {
        float value = baseValue;
        if (0 != accelerationValue)
        {
            value = baseValue + range * GetAccelerationFactor(accelerationValue) * speed;
        }
        if (accelerationValue > 0)
        {
            value = Mathf.Min(value, baseValue + range);
        }
        else if (accelerationValue < 0)
        {
            value = Mathf.Max(value, baseValue - range);
        }
        
        return value;
    }

    private float GetAccelerationFactor(float accelerationValue)
    {
        if (accelerationValue > 0)
        {
            accelerationValue = Mathf.Min(accelerationValue, maxAccelerationValue);
        }
        else if (accelerationValue < 0)
        {
            accelerationValue = Mathf.Max(accelerationValue, -maxAccelerationValue);
        }
        else
        {
            return 0f;
        }
        
        float value = accelerationValue / maxAccelerationValue;
        return RoundFloat(value);
    }

    private Vector3 GetAcceleration()
    {
        Vector3 acces = Input.acceleration;
        acces.x = RoundFloat(acces.x, 10);
        acces.y = RoundFloat(acces.y, 10);
        acces.z = -RoundFloat(acces.z, 10);
        return acces;
    }

    private float RoundFloat(float number, float factor = 100)
    {
        int num = (int)(number * factor);
        return (num / factor);
    }

    #endregion


    #region 通用接口

    private void FixedUpdate()
    {
        UpdateRotation();
    }

    private void Awake()
    {
        _trans = this.transform;
        _startPos = _trans.localPosition;
        _startRotation = _trans.localRotation.eulerAngles;
    }

    #endregion



}

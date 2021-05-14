using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FakeUniversalGravitationCenterMode
{
    FUGC_FOCUS_MOVE_POS,                                           // 强制移动位置
    FUGC_ADD_VELOCITY,                                             // 
}

public class FakeUniversalGravitationCenter : MonoBehaviour
{
    public float FakeEffectRadius = 10;                            // 周围的受影响范围
    
    public bool FakeMaxCloseOption = false;                        // 最大靠近范围开关
    public float FakeMaxCloseRadius = 1;                           // 启用最大靠近范围的情况下，最大靠近半径

    public float moveDelPerSecond = 5f;                            // 每秒移动多少距离
    public Color gizmosColor = new Color(0, 1, 0, 0.2f);
    
    public Transform centerTransform => transform;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(centerTransform.position, FakeEffectRadius);
    }
}

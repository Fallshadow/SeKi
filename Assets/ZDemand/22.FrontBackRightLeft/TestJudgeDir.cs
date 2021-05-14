using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJudgeDir : MonoBehaviour
{
    public Transform My = null;
    public Transform Other = null;
    void Update()
    {
        var position = My.position;
        var position1 = Other.position;
        Vector3 dir = new Vector3(position.x - position1.x, 0 ,position.z - position1.z);
        float angletmp = Vector3.SignedAngle(dir, My.forward, Vector3.up);
        Debug.Log(angletmp);
    }
}

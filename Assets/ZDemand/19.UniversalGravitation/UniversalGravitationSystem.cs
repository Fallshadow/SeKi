using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UniversalGravitationSystem : MonoBehaviour
{
    public bool isOpenUniversalGravitationSystem = false;
    [Header("一般游戏中的吸附")]
    public bool isOpenFakeGameUniversalGravitationSystem = false;
    public float minDistanceLimit = 0.3f;
    public List<FakeUniversalGravitationCenter> FakeGameCenters = new List<FakeUniversalGravitationCenter>();
    public List<Transform> FakeGameObjects = new List<Transform>();

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isOpenUniversalGravitationSystem)
        {
            universalGravitation();
        }
        if (isOpenFakeGameUniversalGravitationSystem)
        {
            fakeGameUniversalGravitationSystem();
        }
    }

    private void universalGravitation()
    {
        
    }
    
    private void fakeGameUniversalGravitationSystem()
    {
        foreach (var fakeGameCenter in FakeGameCenters)
        {
            foreach (var fakeGameObject in FakeGameObjects)
            {
                Vector3 resultGOPosition = fakeGameObject.transform.position;
                Vector3 goPosition = resultGOPosition;
                Vector3 centerFakePosition = fakeGameCenter.centerTransform.position;
                float distance = Vector3.Distance(goPosition, centerFakePosition);
                // 可以接受的不进行抖动的范围
                if (distance < minDistanceLimit)
                {
                    return;
                }
                if (distance < fakeGameCenter.FakeEffectRadius)
                {
//                    Debug.Log($"----------------------吸附开始------------------------");
//                    Debug.Log($"游戏物体{fakeGameObject.name}  游戏吸附中心{fakeGameCenter.name}");
                    
                    // 普通吸附到一点
                    float moveDelDistance = fakeGameCenter.moveDelPerSecond * Time.deltaTime;
                    Vector3 vector3MoveTowardNormalize = Vector3.Normalize(centerFakePosition - goPosition);
                    Vector3 vector3MovePos = moveDelDistance * vector3MoveTowardNormalize;
                    resultGOPosition += vector3MovePos;

//                    #region 普通吸附到一点LOG
//
//                    Debug.Log($"游戏物体{goPosition}  游戏吸附中心{centerFakePosition}");
//                    Debug.Log($"两者距离{distance}");
//                    Debug.Log($"单位方向向量{vector3MoveTowardNormalize}");
//                    Debug.Log($"游戏吸附中心每秒移动吸力{fakeGameCenter.moveDelPerSecond}");
//                    Debug.Log($"游戏吸附物体本次移动距离{moveDelDistance}");
//                    Debug.Log($"游戏吸附物体本次移动坐标{vector3MovePos}");
//
//                    #endregion

                    // 圆形范围修正
                    if (fakeGameCenter.FakeMaxCloseOption)
                    {
                        float afterCommonMoveDistance = Vector3.Distance(resultGOPosition, centerFakePosition);
                        // Debug.Log($"普通吸附后的距离 {afterCommonMoveDistance}");
                        if (afterCommonMoveDistance < fakeGameCenter.FakeMaxCloseRadius)
                        {
                            resultGOPosition = centerFakePosition - vector3MoveTowardNormalize * fakeGameCenter.FakeMaxCloseRadius;
                        }
                    }
                    
                    
                    fakeGameObject.transform.position = resultGOPosition;
                    // Debug.Log($"最终游戏物体坐标{resultGOPosition}");
                }
            }
        }
    }
}

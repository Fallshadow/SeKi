using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMarkTest : MonoBehaviour
{
    // ？ 操作符表示可空数值类型，比如bool? int? 这相当于多了一个判断条件，避免使用神奇数字0之类的作为判断条件而引起的不必要问题
    // ？？ 这个操作符可以用来给空设置默认值，用来简化判断再好不过
    public int? x = null;
    private void Start()
    {
        int y = x ?? 1000;
    }
}

using UnityEngine;
using UnityEditor;

public class UnityAPITestMenuItem
{
    // itemName itemName 是指表示方式类似于路径名的菜单项。 例如，菜单项可能为“GameObject/Do Something”。
    // isValidateFunction 如果 isValidateFunction 为 true，它将表示一个验证 函数，并在系统调用具有相同 itemName 的菜单函数之前进行调用。
    // priority 菜单项显示的顺序。
    [MenuItem("UnityStudy/Editor/MenuItem", false, 2)]
    public static void Param_MenuItem()
    {
        Debug.Log("我是真实的");
    }

    [MenuItem("UnityStudy/Editor/MenuItem", true)]
    public static bool Param_MenuItem_ValiFunc()
    {
        Debug.Log("我是验证函数");
        if(Selection.objects.Length != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

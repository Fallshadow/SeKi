using UnityEditor;

[InitializeOnLoad]
public class UnityAPITestInitializeOnLoad
{
    static UnityAPITestInitializeOnLoad()
    {
        ASeKi.debug.PrintSystem.Log("UnityAPITestInitializeOnLoad");
    }
}

using System.IO;
using UnityEditor;
using UnityEngine;
using BindingFlags = System.Reflection.BindingFlags;

public static class TestSomething
{
    [MenuItem("Tests/一些代码测试/C#Api/File.Copy")]
    public static void CSharpFileCopy()
    {
        //string pathSrcPathFolder = Application.dataPath + "/../" + "/TestFileCopy";
        //string pathTargetPathFolder = Application.dataPath + "/../" + "/TestFileCopy" + "/Test.txt";
        
        string pathSrcPathFolder = Application.dataPath + "/TestFileCopy";
        string pathTargetPathFolder = Application.dataPath + "/TestFileCopy";
        
        Debug.Log($"Application.dataPath { Application.dataPath }");
        Debug.Log($"pathSrcPathFolder { pathSrcPathFolder }");
        Debug.Log($"pathTargetPathFolder { pathTargetPathFolder }");
        
        foreach (var file in Directory.GetFiles (pathSrcPathFolder))
        {
            string pathTargetPath = pathTargetPathFolder + "/" + file.Substring(pathSrcPathFolder.Length + 1);
            
            Debug.Log($"file.Length : {file.Length} file.Substring : {file.Substring (pathSrcPathFolder.Length + 1)}");
            Debug.Log($"pathTargetPath { pathTargetPath }");
            
            if (File.Exists(pathTargetPath))
            {
                Debug.Log($"{File.Exists("F:\\SeKi\\SeKi\\Assets\\TestFileCopy\\Test.txt")}");
                File.Copy("F:/SeKi/SeKi/TestFileCopy/Test.txt",  "F:/SeKi/SeKi/Assets/TestFileCopy/Test.txt", true);
            }
            else
            {
                File.Copy(file,  pathTargetPath);
            }
        }
        
        Debug.Log($"总结：" +
                  $"File.Copy 相同路径文件会报错 " +
                  $"\\..\\这种可以" +
                  $"正反斜杠没区别");
    }
    
    [MenuItem("Tests/一些代码测试/C#Api/File.Exists")]
    public static void CSharpFileExists()
    {
        string path = $"{Application.dataPath}/../TestFileCopy/Test.txt";
        Debug.Log($"TestFileCopy {path} { File.Exists(path)}");
        
    }
}

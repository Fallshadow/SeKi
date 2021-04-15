using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FBXAnimationExporter : EditorWindow
{
    //const string RESOURCE_CHARACTER_PATH = "Assets/ResourcesRaw/Art/Character/Human/Player/";
    //const string EXPORT_CHARACTER_PATH = "Assets/ResourcesClient/Models/Player/Animation/"; 
    const string RESOURCE_CHARACTER_PATH = "Assets/Resources/Actor/";
    const string EXPORT_CHARACTER_PATH = "Assets/Resources/ActorAnim/";

    const string FEX_ANIM = ".anim";

    const string FEX_FBX = ".fbx";

    static string customResourcePath = RESOURCE_CHARACTER_PATH;
    static string customExportPath = EXPORT_CHARACTER_PATH;
    static bool useCustomGroupEnabled = false;

    static string resourcePath = "";
    static string exportPath = "";

    //     const string OPEN_FOLDER_PANEL_DEFAULT_PATH = "ResourcesRaw/Art/Character/Human/Player/Male/Player01_M";
    //     const string SAVE_FOLDER_PANEL_DEFAULT_PATH = "ResourcesClient/Models/Player/Animation";
    const string OPEN_FOLDER_PANEL_DEFAULT_PATH = "ResourceRex/Character/Humman/";
    const string SAVE_FOLDER_PANEL_DEFAULT_PATH = "ResourceRex/Character/Humman/";

    //dir, path
    static Dictionary<string, List<string>> filePathDict = new Dictionary<string, List<string>>();

    //animator curve
    static bool useGravityWeightCurve = false;
    static AnimationCurve curve = null;
    static AnimationCurve gravityWeightCurve
    {
        get
        {
            if (curve == null)
            {
                curve = new AnimationCurve();
                curve.AddKey(new Keyframe(0, 1));
                curve.AddKey(new Keyframe(1, 1));
            }
            return curve;
        }
        set
        {
            curve = value;
        }
    }

    [MenuItem("Tests/工具/从FBX中导出动画文件")]
    public static void ExportAnimationTool()
    {
        FBXAnimationExporter window = (FBXAnimationExporter)EditorWindow.GetWindow(typeof(FBXAnimationExporter));
        window.Show();       
    }

    static void setInputPath()
    {
        if (useCustomGroupEnabled)
        {
            resourcePath = customResourcePath;
            exportPath = customExportPath;

            if (!Directory.Exists(resourcePath))
            {
                EditorUtility.DisplayDialog("Export FBX Animation", "resource path not Found!!", "Yes");
                return;
            }

            if (!Directory.Exists(exportPath))
            {
                EditorUtility.DisplayDialog("Export FBX Animation", "Export path not Found!!", "Yes");
                return;
            }
        }
        else
        {
            resourcePath = RESOURCE_CHARACTER_PATH;
            exportPath = EXPORT_CHARACTER_PATH;
        }
    }

    static void exportAnimation()
    {
        setInputPath();

        if (EditorUtility.DisplayDialog("Export FBX Animation?",
                       "Are you sure you want to export FBX Animation on " + resourcePath, "Yes", "Cancel"))
        {
            filePathDict.Clear();
            getAllFBX(resourcePath);
            foreach (KeyValuePair<string, List<string>> file in filePathDict)
            {
                List<string> tmpList = file.Value as List<string>;
                for (int i = 0; i < tmpList.Count; i++)
                {
                    string path = file.Key + "/" + tmpList[i];                    
                    Debug.Log("ExportAnimationTool path = " + path);
                    exportAnimation(path);
                }
            }

            if (filePathDict.Count > 0)
            {
                EditorUtility.DisplayDialog("Export FBX Animation", "Finish!!", "Yes");
            }
        }
    }

    static void exportAnimationBySelectedFBX()
    {
        setInputPath();

        if (EditorUtility.DisplayDialog("Export FBX Animation?",
                      "Are you sure you want to export FBX Animation to " + exportPath, "Yes", "Cancel"))
        {
            string fileList = string.Empty;
            int count = 0;

            foreach (Object obj in Selection.objects)
            {
                if (obj.GetType() == typeof(GameObject))
                {
                    string path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
                    Debug.Log(obj.name + " , path = " + path);// +" , obj.GetType() = " + obj.GetType());
                    exportAnimation(path);
                    fileList += obj.name + " \r\n ";
                    count++;
                }
            }

            if (count > 0)
            {
                EditorUtility.DisplayDialog("Export FBX Animation", fileList + " Finish!!", "Yes");
            }
            else
            {
                EditorUtility.DisplayDialog("Export FBX Animation", "no selected FBX or no file to export", "Yes");
            }
        }
    }

    /// <summary>
    /// 主要導出FBX腳本
    /// </summary>
    /// <param name="assetPath"></param>
    static void exportAnimation(string assetPath)
    {
        assetPath = phasePath(assetPath);

        // 先偷改FBX
        if (useGravityWeightCurve)
        {
            ModelImporter modelImporter = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            ModelImporterClipAnimation[] clipAnimations = modelImporter.clipAnimations;
            SerializedObject so = new SerializedObject(modelImporter);
            SerializedProperty clips = so.FindProperty("m_ClipAnimations");
            for (int i = 0; i < clipAnimations.Length; i++)
            {
                SerializedProperty curves = clips.GetArrayElementAtIndex(i).FindPropertyRelative("curves");
                curves.arraySize = 1;
                curves.GetArrayElementAtIndex(0).FindPropertyRelative("curve").animationCurveValue = gravityWeightCurve;
                curves.GetArrayElementAtIndex(0).FindPropertyRelative("name").stringValue = "GravityWeight";
            }

            so.ApplyModifiedProperties();
            AssetDatabase.ImportAsset(assetPath);
            AssetDatabase.Refresh();
        }

        // 取出AnimationClip (有可能有複數的Clips)
        List<AnimationClip> AnimationClipList = new List<AnimationClip>();

        Object[] assetRepresentationsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
        foreach (Object assetRepresentation in assetRepresentationsAtPath)
        {
            AnimationClip animationClip = assetRepresentation as AnimationClip;

            if (animationClip != null)
            {
                AnimationClipList.Add(animationClip);
            }
        }

        int count = AnimationClipList.Count;
        if (count <= 0)
        {
            Debug.LogError(assetPath + "AnimationClip count 0");
            return;
        }

        for (int a = 0; a < count; a++)
        {
            AnimationClip orgClip = AnimationClipList[a];
            AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(orgClip);

            string folderPath = assetPath.Replace(resourcePath, "");
            string[] folderList = folderPath.Split('/');
            int num = folderList.Length;
            if (num > 0 && (folderList[num - 1].Contains(FEX_FBX) || folderList[num - 1].Contains(FEX_FBX.ToUpper())))
            {
                string fileName = folderList[num - 1];
                folderList[num - 1] = "";
                folderPath = folderPath.Replace(fileName, "");
            }

            // Create folder
            DirectoryInfo dirInfo = new DirectoryInfo(exportPath);
            if (num > 0)
            {
                string folderName = "";
                for (int i = 0; i < num; ++i)
                {
                    if (folderList[i] != "")
                    {
                        folderName = folderList[i];
                        DirectoryInfo[] dirArr = dirInfo.GetDirectories(folderName);
                        if (dirArr.Length == 0)
                        {
                            dirInfo = dirInfo.CreateSubdirectory(folderName);
                        }
                        else
                        {
                            dirInfo = dirArr[0];
                        }
                    }
                }
            }            

            //Save the clip
            string file = exportPath + folderPath + orgClip.name + FEX_ANIM;

            AnimationClip placeClip = new AnimationClip();
            //if (Resources.Load(file))
            if (System.IO.File.Exists(file))
            {
                //AssetDatabase.DeleteAsset(file);

                AnimationClip oldClip = (AnimationClip)AssetDatabase.LoadAssetAtPath(file, typeof(AnimationClip));
                EditorUtility.CopySerialized(orgClip, oldClip);

                AnimationUtility.SetAnimationClipSettings(oldClip, clipSettings);
            }
            else
            {
                EditorUtility.CopySerialized(orgClip, placeClip);
                AssetDatabase.CreateAsset(placeClip, file);
            }

            AssetDatabase.Refresh();
            AssetDatabase.ImportAsset(file, ImportAssetOptions.ImportRecursive);
        }
    }

    static void getAllFBX(string path)
    {
        if (path.EndsWith("/"))
        {
            path = path.TrimEnd('/');
        }

        DirectoryInfo dirInfo = new DirectoryInfo(path);
        DirectoryInfo[] dirArr = dirInfo.GetDirectories();
        if (dirArr.Length > 0)
        {
            for (int i = 0; i < dirArr.Length; i++)
            {
                getAllFBX(path + "/" + dirArr[i].Name);
            }
        }

        FileInfo[] fileInf = dirInfo.GetFiles("*" + FEX_FBX);
        foreach (FileInfo fileInfo in fileInf)
        {
            //Debug.Log("fileInfo.Name = " + fileInfo.Name);
            addFilePath(path, fileInfo.Name);
        }

    }

    static void addFilePath(string key, string value)
    {
        if (!filePathDict.ContainsKey(key))
        {
            List<string> list = new List<string>();
            list.Add(value);
            filePathDict.Add(key, list);
        }
        else
        {
            filePathDict[key].Add(value);
        }
    }

    static string phasePath(string path, bool addFinal = false)
    {
        if (path == "")
        {
            return "";
        }

        path.Trim();
        int index = path.IndexOf("Assets/");
        path = path.Remove(0, index);

        if (addFinal)
        {
            path += "/";
        }

        //Debug.Log("assetPath = " + path + " , index = " + index);

        return path;
    }

    static bool checkCustomPath()
    {
        if (useCustomGroupEnabled)
        {
            if (customResourcePath == null || customResourcePath.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "Resource Path is empty", "OK");
                return false;
            }

            if (customExportPath == null || customExportPath.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "Export Path is empty", "OK");
                return false;
            }
        }

        return true;
    }       

    void OnGUI()
    {
        GUILayout.Label("Export FBX Animation", EditorStyles.boldLabel);

        GUILayout.Label("Default Resource Path : " + RESOURCE_CHARACTER_PATH);
        GUILayout.Label("Default Export Path : " + EXPORT_CHARACTER_PATH);

        
        useCustomGroupEnabled = EditorGUILayout.BeginToggleGroup("Use Custom Path", useCustomGroupEnabled);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 160;
        EditorGUIUtility.fieldWidth = 300;
        customResourcePath = EditorGUILayout.TextField("Custom Resource Path", customResourcePath);
        if (GUILayout.Button("Resource Path"))
        {
            //Debug.Log(Application.dataPath);
            //customResourcePath = EditorUtility.SaveFilePanelInProject("Save Expression Clip", "", "anim", null);
            customResourcePath = EditorUtility.OpenFolderPanel("Resource Path", Application.dataPath + "/" + OPEN_FOLDER_PANEL_DEFAULT_PATH, "");
            customResourcePath = phasePath(customResourcePath, true);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        customExportPath = EditorGUILayout.TextField("Custom Export Path", customExportPath);
        if (GUILayout.Button("Export Path"))
        {
            //customExportPath = EditorUtility.SaveFilePanelInProject("Save Expression Clip", "", "anim", null);
            customExportPath = EditorUtility.SaveFolderPanel("Export Path", Application.dataPath + "/" + SAVE_FOLDER_PANEL_DEFAULT_PATH, "");
            customExportPath = phasePath(customExportPath, true);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndToggleGroup();

        useGravityWeightCurve = EditorGUILayout.BeginToggleGroup("Set GravityWeight Curve", useGravityWeightCurve);
        gravityWeightCurve = EditorGUILayout.CurveField(gravityWeightCurve);
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("用於轉出整個文件夾:");
        if (GUILayout.Button("Export FBX Animation"))
        {
            bool result = checkCustomPath();
            if (result)
            {
                exportAnimation();
                EditorApplication.ExecuteMenuItem("File/Save Project");
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("用於轉出選中的檔案:");
        if (GUILayout.Button("Export Selected FBX Animation"))
        {
            bool result = checkCustomPath();
            if (result)
            {
                exportAnimationBySelectedFBX();
                EditorApplication.ExecuteMenuItem("File/Save Project");
            }
        }
    }

}
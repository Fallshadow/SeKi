using UnityEngine;

namespace ASeKi.Utility
{
    // 主要提供接口：异步加载/加载/实例化 --> 预制体、SO
    public static class LoadResources
    {
        // 加载该路径预制体  路径  检查是否存在  是否打印错误（是否严重）
        public static GameObject LoadPrefab(string path, bool checkExisted = false, bool showErrorLog = true)
        {
            Object obj = Resources.Load(path);
            if(checkExisted)
            {
                if(obj == null)
                {
                    if(showErrorLog)
                    {
                        debug.PrintSystem.LogError($"LoadResources.LoadPrefab Path = {path} , Not Found!");
                    }
                    else
                    {
                        debug.PrintSystem.Log($"LoadResources.LoadPrefab Path = {path} , Not Found!");
                    }
                    return null;
                }
            }
            return InstantiateObject(obj);
        }

        // 加载该路径预制体  路径  父物体  检查是否存在  是否打印错误（是否严重）
        public static GameObject LoadPrefab(string path, Transform parent, bool checkExisted = false,bool showErrorLog = true)
        {
            Object obj = Resources.Load(path);
            if(checkExisted)
            {
                if(obj == null)
                {
                    if(showErrorLog)
                    {
                        debug.PrintSystem.LogError($"LoadResources.LoadPrefab Path = {path} , Not Found!");
                    }
                    else
                    {
                        debug.PrintSystem.Log($"LoadResources.LoadPrefab Path = {path} , Not Found!");
                    }
                    return null;
                }
            }
            return InstantiateObject(obj, parent);
        }

        public static GameObject InstantiateObject(Object obj)
        {
            return Object.Instantiate(obj) as GameObject;
        }

        // 注意，该函数会手动重置物体的各项信息
        public static GameObject InstantiateObject(Object obj, Transform parent)
        {
            GameObject go = InstantiateObject(obj);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }

        // 從指定的路徑裡讀取出特定的資料結構 主要用在讀取config scriptableObject
        public static T LoadAsset<T>(string strPath, bool logErrorIfNull = true) where T : Object
        {
            Object obj = Resources.Load(strPath, typeof(T));
            if(obj == null && logErrorIfNull)
            {
                debug.PrintSystem.LogError($"Load asset fail, path: {strPath}");
            }

            return obj as T;
        }

        public static ResourceRequest LoadAssetAsync(string strPath)
        {
            return Resources.LoadAsync(strPath);
        }
    }
}

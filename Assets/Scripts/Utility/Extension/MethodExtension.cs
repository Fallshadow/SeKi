using System;
using System.Collections.Generic;
using UnityEngine;

public static class MethodExtension
{
    #region UnityComponent

    public static T GetOrAddComponent<T>(this Component comp) where T : Component
    {
        T result = comp.GetComponent<T>();
        if(result == null)
        {
            result = comp.gameObject.AddComponent<T>();
        }
        return result;
    }

    // 复制组件内容
    public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        Type type = comp.GetType();
        if(type != other.GetType())
            return null; // type mis-match
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Default | System.Reflection.BindingFlags.DeclaredOnly;
        System.Reflection.PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach(var pinfo in pinfos)
        {
            if(pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }

        System.Reflection.FieldInfo[] finfos = type.GetFields(flags);
        foreach(var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }

    public static void SetActive(this Component comp, bool value, Action action = null)
    {
        if(comp == null)
        {
            return;
        }
        if(comp.gameObject.activeSelf != value)
        {
            comp.gameObject.SetActive(value);
        }
        action?.Invoke();
    }

    #endregion

    #region UnityGameObject

    static public T GetOrAddComponent<T>(this UnityEngine.GameObject go) where T : Component
    {
        T result = go.GetComponent<T>();
        if(result == null)
        {
            result = go.AddComponent<T>();
        }
        return result;
    }

    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    #endregion

    #region SystemList

    public static bool IsNullOrEmpty<T>(this List<T> list)
    {
        return list == null || list.Count == 0;
    }

    public static void AddIfNoExist<T>(this List<T> list, T item)
    {
        if(!list.Contains(item))
        {
            list.Add(item);
        }
    }

    #endregion

    #region color

    public static string ToColorString(this Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>";
    }

    public static string ToColorString(this Color color, string str)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
    }

    #endregion

    #region Dictionary

    public static void AddIfNotContains<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
    {
        if (source.ContainsKey(key))
        {
            return;
        }
        else
        {
            source.Add(key, value);
        }
    }
    
    public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
    {
        if (source.ContainsKey(key))
        {
            source[key] = value;
        }
        else
        {
            source.Add(key, value);
        }
    }

    #endregion
    
}

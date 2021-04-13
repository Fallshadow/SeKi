using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.action
{
    public class ActionGroupAttr : Attribute
    {
        public string GroupTag;

        public ActionGroupAttr(string name)
        {
            GroupTag = name;
        }
    }
    
    public class ActionGroupSO : ScriptableObject
    {
        public List<string> ActionGroupList = new List<string>();

        public int GetActionGroupIndex(string group)
        {
            return ActionGroupList.IndexOf(group);            
        }
    }
}
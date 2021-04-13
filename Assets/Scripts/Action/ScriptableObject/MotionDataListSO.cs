using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 临时存储的动作资源列表，之后要转化为游戏实际使用的RuntimeAction资源
namespace ASeKi.action
{
    public class MotionDataListSO : ScriptableObject, ISerializationCallbackReceiver
    {
        public ActionConditionConfigSO ActionConditionConfig = null;
        public int LayerCount = 0;
        
        private const int ACTION_ID_BASE = 1000;
        private Dictionary<int, MotionData> motionDataDict = new Dictionary<int, MotionData>();        // motionID -> motionData motionID = WeaponType x 1000
        
        #region Serializable

        [SerializeField] private List<float> keys = new List<float>();
        [SerializeField] private List<MotionData> values = new List<MotionData>();
        
        public void OnBeforeSerialize()
        {
            keys = new List<float>();
            values = new List<MotionData>();

            foreach (var pair in motionDataDict)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            motionDataDict = new Dictionary<int, MotionData>();
            for (int i = 0; i != Math.Min(keys.Count, values.Count); i++)
            {
                motionDataDict.Add((int)keys[i], values[i]);
            }
        }

        #endregion

        #region API

        public void Sort()
        {
#if UNITY_EDITOR
            motionDataDict = motionDataDict.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
#endif
        }
        
        public void ClearAll()
        {
            motionDataDict.Clear();
        }

        #region 各种方式寻找动作数据

        public List<MotionData> GetMotionDatas()
        {
            return motionDataDict.Values.ToList();
        }

        public MotionData GetMotionData(string name)
        {
            List<MotionData> motionDatas = motionDataDict.Values.ToList();
            return motionDatas.Find(x => x.ActionName.Equals(name, StringComparison.Ordinal));
        }

        public MotionData GetTopMotionData()
        {
            MotionData mData = null;
            foreach (KeyValuePair<int, MotionData> pair in motionDataDict)
            {
                mData = pair.Value;
                break;
            }

            return mData;
        }

        public MotionData GetMotionData(int key)
        {
            if (motionDataDict.TryGetValue(key, out action.MotionData mData))
            {
                return mData;
            }
            return null;
        }

        public bool GetMotionData(ActionType actType, WeaponType weaponType, bool withNormal, ref List<MotionData> list)
        {
            bool find = false;

            foreach (KeyValuePair<int, MotionData> pair in motionDataDict)
            {
                MotionData data = pair.Value;

                bool filterByActType = 
                    data.ActionType == actType ||
                    actType == ActionType.ALL;
                
                bool filterByWeaponType = 
                    data.WeaponType == weaponType || 
                    (withNormal && data.WeaponType == WeaponType.None) || 
                    weaponType == WeaponType.WT_ALL;
                
                if (filterByActType && filterByWeaponType)
                {
                    if (!find)
                    {
                        find = true;
                        list.Clear();
                    }

                    list.Add(data);
                }
            }

            return find;
        }
        
        public bool GetMotionData(ActionRoleType actType, bool withNormal, ref List<MotionData> list)
        {
            string startKey = null;
                
            switch (actType)
            {
                case ActionRoleType.ART_PLAYER_SWORD:
                case ActionRoleType.ART_PLAYER_HAMMER:
                case ActionRoleType.ART_PLAYER_DUALBLADE:
                case ActionRoleType.ART_PLAYER_BOWGUN:
                {
                    startKey = $"{(int)actType + 1}_";
                }
                    break;
                case ActionRoleType.ART_PLAYER_SPEAR:
                {
                    startKey = "5_";
                }
                    break;
                case ActionRoleType.ART_PLAYER_NONE:
                case ActionRoleType.ART_PLAYER_NORMAL:
                {
                    startKey = "0_";
                }
                    break;
                default:
                    debug.PrintSystem.Log($"Unknown Type {actType}");
                    break;
            }
                
            foreach (KeyValuePair<int, MotionData> pair in motionDataDict)
            {
                bool find = false;

                MotionData data = pair.Value;

                if (data.ActionName.StartsWith(startKey))
                {
                    find = true;
                }
                    
                if (withNormal && data.ActionName.StartsWith($"0_"))
                {
                    find = true;
                }
                    
                if (find)
                {
                    list.Add(data);
                }
            }

            return !list.IsNullOrEmpty();
        }

        #endregion

        #region 新增动作数据

        public int GetLastID(WeaponType type)
        {
            int id = 0;
            foreach (KeyValuePair<int, MotionData> pair in motionDataDict)
            {
                MotionData data = pair.Value;
                if (data.WeaponType == type && id < data.ActionId)
                {
                    id = data.ActionId; //取最大
                }
            }

            return id;
        }

        private int getNewActionID(WeaponType type)
        {
            int id = GetLastID(type);
            if (id > 0)
            {
                return id + 1;
            }
            else
            {
                int baseId = (int)type * ACTION_ID_BASE;
                return baseId + id + 1;
            }
        }

        public int GetTotalMotion()
        {
            return motionDataDict.Count;
        }

        public MotionData AddNewMotionData(WeaponType type)
        {
            MotionData mData = new MotionData();
            int id = getNewActionID(type);
            mData.ActionName = $"New State {id}";
            mData.WeaponType = type;
            mData.ActionId = id;
            motionDataDict.Add(mData.ActionId, mData);
            return mData;
        }

        public MotionData AddNewMotionData(WeaponType type, string motionName)
        {
            MotionData mData = new MotionData();
            int id = getNewActionID(type);
            mData.ActionName = motionName;
            mData.WeaponType = type;
            mData.ActionId = id;
            motionDataDict.Add(mData.ActionId, mData);
            return mData;
        }
        
        #endregion

        #region 删除动作数据

        public bool DeletMotionData(int key)
        {
            if (!motionDataDict.TryGetValue(key, out MotionData targetMotionData))
            {
                Debug.Log($"Don't has current Motion id:{key}");
                return false;
            }
            bool result = motionDataDict.Remove(key);

            //TODO: Process All Condition ID
            foreach (var motionDataPair in motionDataDict)
            {
                var motionData = motionDataPair.Value;
                int nextMotionCount = motionData.NextMotionList.Count;
                
                for (var index = nextMotionCount - 1; index >= 0; index--)
                {
                    var nextMotion = motionData.NextMotionList[index];
                    if (nextMotion.Id == key)
                    {
                        Debug.Log($"Remove {motionData.ActionName}'s nextMotion {key}'");
                        motionData.NextMotionList.RemoveAt(index);
                    }
                }
            }
            return result;
        }
        
        #endregion
        
        #endregion
        
    }
}


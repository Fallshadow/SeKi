using System.Collections.Generic;

namespace ASeKi.battle
{
    public class Carrier
    {
        private ulong carrierId;
        public List<Buff> ListBuff { get; private set; } = new List<Buff>(); 
        private Dictionary<int, float> dictBuffAttachTimes = new Dictionary<int, float>();
        private static BuffFactory buffFactory = new BuffFactory();                 // 持有静态的BUFF工厂
        private const float MAX_LAST_ATTACH = -9999f;

        public Carrier(ulong carrierId)
        {
            this.carrierId = carrierId;
        }

        #region BUFF挂载时间相关

        bool checkAttachTime(int buffID, int coolingTime)
        {
            float lastAttachTime = getAttachTime(buffID);
            if(UnityEngine.Time.time - lastAttachTime < (1.0f * coolingTime / constants.ConfigSetting.MILLISECOND))
            {
                return false;
            }
            return true;
        }

        float getAttachTime(int configId)
        {
            float time = MAX_LAST_ATTACH;
            if(dictBuffAttachTimes.TryGetValue(configId, out time))
            {
                return time;
            }
            return MAX_LAST_ATTACH;
        }

        private void saveAttachTime(int configId)
        {
            dictBuffAttachTimes[configId] = UnityEngine.Time.time;
        }

        #endregion

        public uint AttachBuff(ulong sourceID, ulong carrierID, int configID, int curSkillID = 0)
        {
            ConfigTable.BuffData buffData = new ConfigTable.BuffData(configID);
            // 获取buff数据
            // 获取载体数据
            // 判别当前载体状态是否可以挂载BUFF

            // 判别当前载体BUFF池允许以何种方式处理该BUFF

            // 挂载cd没到，直接拒绝
            if(!checkAttachTime(configID,buffData.action_cd))
            {
                return 0;
            }

            // 先做是否有相同来源，相同id的情况
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].SourceID == sourceID && ListBuff[i].Config.buff_id == configID)
                {
                    return processConflict(sourceID, configID, (int)ListBuff[i].Config.self_conflict, ListBuff[i].Id, curSkillID);
                }
            }

            // 有不同来源，相同id的情况
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].SourceID != sourceID && ListBuff[i].Config.buff_id == configID)
                {
                    return processConflict(sourceID, configID, (int)ListBuff[i].Config.diff_self_conflict, ListBuff[i].Id, curSkillID);
                }
            }

            // 相同来源，相同组
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].SourceID == sourceID && ListBuff[i].Config.group_id == buffData.group_id)
                {
                    return processConflict(sourceID, configID, (int)ListBuff[i].Config.group_conflict, ListBuff[i].Id, curSkillID);
                }
            }

            // 不同来源，相同组
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].SourceID != sourceID && ListBuff[i].Config.group_id == buffData.group_id)
                {
                    return processConflict(sourceID, configID, (int)ListBuff[i].Config.diff_group_confict, ListBuff[i].Id, curSkillID);
                }
            }

            // 前方都没拒绝则加上去
            return 0;
        }

        public uint processConflict(ulong sourceID, int buffConfigId, int condition, uint otherBuffID, int curSkillId)
        {
            switch(condition)
            {
                case (int)constants.BuffConflictType.BCT_REPLACE:
                    {
                        return ReplaceBuffWithNet(sourceID, buffConfigId, otherBuffID, curSkillId);
                    }
                case (int)constants.BuffConflictType.BCT_ONLYONE:
                    {
                        break;
                    }
                case (int)constants.BuffConflictType.BCT_INDEPENDENT:
                    {
                        return AttachBuffWithNet(sourceID, buffConfigId, curSkillId);
                    }
                case (int)constants.BuffConflictType.BCT_ADDTIVE:
                    {
                        return AddtiveBuffWithNet(sourceID, buffConfigId, otherBuffID, curSkillId);
                    }
                case (int)constants.BuffConflictType.BCT_ADDTIVE_FRESH:
                    {
                        return AddAddtiveFreshCmd(sourceID, buffConfigId, otherBuffID, curSkillId);
                    }
            }
            return 0;
        }


        #region 挂载BUFF

        public uint AttachBuffWithNet(ulong source, int configId, int sourceSkillId)
        {
            uint dynamicId = buffFactory.GetDynamicBuffId(source);
            // TODO:发送到服务器
            Buff buff = AttachBuffWithoutNet(source, configId, dynamicId);
            buff.SourceSkill = sourceSkillId;
            // evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.UPDATE, carrierId, ListBuff);
            return dynamicId;
        }

        public Buff AttachBuffWithoutNet(ulong source, int configID, uint dynamicID)
        {
            // evt.EventManager.instance.Send(evt.EventGroup.BUFF,(short)evt.BuffEvent.BUFF_ATTACH,)
            Buff buff = buffFactory.CreateBuff(source, carrierId, configID, dynamicID);
            ListBuff.Add(buff);
            //AttachBuffInternal这个函数在挂载的时候无论host端还是非host端都能运行，所以设置两边信息同步
            //包含buff的挂载时间，已经sequenceId
            saveAttachTime(configID);
            buffFactory.SetMaxSequence(source, dynamicID);
            buff.Start();
            CallReCalcStatus();
            return buff;
        }

        #endregion

        #region 替换BUFF

        public uint ReplaceBuffWithNet(ulong sourceID, int buffConfigID, uint replaceBuffID, int sourceSkillId)
        {
            uint buffDynamicId = buffFactory.GetDynamicBuffId(sourceID);
            // TODO:发送到服务器
            Buff buff = ReplaceBuffWithoutNet(sourceID, buffConfigID, buffDynamicId, replaceBuffID);
            if(buff != null)
            {
                buff.SourceSkill = sourceSkillId;
            }
            evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_UPDATE, carrierId, ListBuff);
            evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_REPLACE, buffDynamicId, replaceBuffID);
            return buffDynamicId;
        }

        public Buff ReplaceBuffWithoutNet(ulong sourceID, int buffConfigID, uint buffDynamicId, uint replaceBuffID)
        {
            evt.EventManager.instance.Send<ulong, int>(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_ATTACH, carrierId, buffConfigID);
            Buff buff = buffFactory.CreateBuff(sourceID, carrierId, buffConfigID, buffDynamicId);

            bool replaced = false;
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].Id == replaceBuffID)
                {
                    buffFactory.ReleaseBuff(ListBuff[i]);
                    ListBuff[i] = buff;
                    replaced = true;
                    break;
                }
            }
            if(!replaced)
            {
                for(int i = 0; i < ListBuff.Count; i++)
                {
                    if(ListBuff[i].Config.buff_id == buffConfigID)
                    {
                        ASeKi.debug.PrintSystem.LogWarning("[Buff:Carrier]没有找到要替换的对应动态ID的buff" + replaceBuffID);
                        buffFactory.ReleaseBuff(ListBuff[i]);
                        ListBuff[i] = buff;
                        replaced = true;
                        break;
                    }
                }
            }
            if(!replaced)
            {
                ASeKi.debug.PrintSystem.LogWarning("[Buff:Carrier]没有找到要替换的对应配置ID的buff : " + buffConfigID);
                ListBuff.Add(buff);
            }
            saveAttachTime(buffConfigID);
            buff.Start();   // buff的挂载触发
            CallReCalcStatus();
            return buff;
        }


        #endregion

        #region 叠加BUFF

        public uint AddtiveBuffWithNet(ulong sourceID, int buffConfigID, uint addtivebuffDynamicId, int sourceSkillId = 0)
        {
            // TODO：向服务器发送
            Buff buff = AddtiveBuffWithoutNet(sourceID, buffConfigID, addtivebuffDynamicId);
            if(buff != null)
            {
                buff.SourceSkill = sourceSkillId;
            }
            evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_UPDATE, carrierId, ListBuff);
            return buff.Id;
        }

        public Buff AddtiveBuffWithoutNet(ulong sourceID, int buffConfigID, uint addtivebuffDynamicId)
        {
            Buff targetBuff = null;

            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].Id == addtivebuffDynamicId)
                {
                    ListBuff[i].Addtive();
                    ListBuff[i].PerformAttach();
                    targetBuff = ListBuff[i];
                    break;
                }
            }
            saveAttachTime(buffConfigID);
            CallReCalcStatus();
            return targetBuff;
        }

        #endregion

        #region 叠加刷新

        public uint AddAddtiveFreshCmd(ulong sourceID, int buffConfigId, uint buffDID, int sourceSkillId = 0)
        {
            // TODO：向服务器发送
            Buff buff = AddtiveFreshBuffInternal(sourceID, buffConfigId, buffDID);
            if(buff != null)
            {
                buff.SourceSkill = sourceSkillId;
            }
            evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_UPDATE, carrierId, ListBuff);
            return buff.Id;
        }

        public Buff AddtiveFreshBuffInternal(ulong source, int configId, uint addtiveFreshEntity)
        {
            Buff targetBuff = null;
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].Id == addtiveFreshEntity)
                {
                    ListBuff[i].AddtiveFresh();
                    ListBuff[i].PerformAttach();
                    targetBuff = ListBuff[i];
                    break;
                }
            }
            saveAttachTime(configId);
            CallReCalcStatus();
            return targetBuff;
        }

        #endregion

        #region 卸载BUFF

        // 移除所有已经死掉的buff
        public void RemoveDeadBuff()
        {
            List<uint> deleteCache = new List<uint>();

            for(int i = ListBuff.Count - 1; i >= 0; i--)
            {
                if(ListBuff[i].Config.remove_rule2 == 1)
                {
                    deleteCache.Add(ListBuff[i].Id);
                }
            }

            for(int i = 0; i < deleteCache.Count; i++)
            {
                DetachBuffWithNet(deleteCache[i], BuffDetachType.DETACH_ID);
            }
        }

        // 移除所有buff
        public void RemoveUnitAllBuff()
        {
            DetachBuffWithNet(0, BuffDetachType.DETACH_ALL);
        }

        public void CallReCalcStatus()
        {
            //不需要本机直接计算属性
            if(!BuffSourceUtility.CheckNativeLogic(carrierId))
            {
                return;
            }
            Entity unit = BattleActorManager.instance.GetActorById(carrierId);
            if(unit == null)
            {
                return;
            }

            if(unit is Monster)
            {
                battle.LogicStatusComponent logicStatusComponent = (unit as Monster).LinkC().LStatusC;
                if(logicStatusComponent != null)
                {
                    logicStatusComponent.CalcCurrentStatus();
                }

            }
            else if(unit is Hero)
            {
                if((unit as Hero).HeroLogic != null)
                {
                    (unit as Hero).HeroLogic.HeroAttrSystem.CalcCurrentStatus();
                }
            }
        }

        public void DetachBuffWithNet(uint buffID, BuffDetachType detachType)
        {
            // TODO:发送到服务端
            DetachBuffWithoutNet(buffID, detachType);
            // TODO:通知需要更新的东西evt.EventManager.instance.Send(evt.EventGroup.BUFF,(short)evt.BuffEvent.BUFF_UPDATE,)
        }

        public void DetachBuffWithoutNet(uint buffID, BuffDetachType detachType)
        {
            if(ListBuff.Count == 0)
            {
                return;
            }
            System.Func<Buff, uint, bool> func = null;
            switch(detachType)
            {
                case BuffDetachType.DETACH_ID:
                    {
                        func = (buff, info) =>
                        {
                            return buff.Id == info;
                        };
                    }
                    break;
                case BuffDetachType.DETACH_CONFIG_ID:
                    {
                        func = (buff, info) =>
                        {
                            return buff.Config.buff_id == info;
                        };
                    }
                    break;
                case BuffDetachType.DETACH_SORT:
                    {
                        func = (buff, info) =>
                        {
                            return (uint)buff.Config.buff_sort == info;
                        };
                    }
                    break;
                case BuffDetachType.DETACH_TYPE:
                    {
                        func = (buff, info) =>
                        {
                            return (uint)buff.Config.buff_type == info;
                        };
                    }
                    break;
                case BuffDetachType.DETACH_GROUP:
                    {
                        func = (buff, info) =>
                        {
                            return buff.Config.group_id == info;
                        };
                    }
                    break;
                case BuffDetachType.DETACH_ALL:
                    {
                        func = (buff, info) =>
                        {
                            return true;
                        };
                    }
                    break;
                default:
                    break;
            }
            for(int index = ListBuff.Count - 1; index >= 0; index--)
            {
                if(index >= ListBuff.Count)
                {
                    index = ListBuff.Count - 1;
                }
                if(func(ListBuff[index], buffID))
                {
                    Buff removeItem = ListBuff[index];
                    buffFactory.ReleaseBuff(removeItem);
                    ListBuff.RemoveAt(index);
                }
            }
        }

        #endregion

        #region 免疫效果

        HashSet<int> immuneEffects = new HashSet<int>();

        public bool CheckImmuneEffect(int antiEffect)
        {
            return immuneEffects.Contains(antiEffect);
        }

        public void SetImmuneEffect(int antiEffect, bool active)
        {
            if(active)
            {
                immuneEffects.Add(antiEffect);
            }
            else
            {
                immuneEffects.Remove(antiEffect);
            }
        }

        #endregion

        #region 搜查BUFF列表

        public bool IsExistBuffWithId(ulong entity, int buffId)
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].Config.buff_id == buffId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsExistBuffWithType(ulong entity, int buffType)
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if((int)ListBuff[i].Config.buff_type == buffType)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsExistBuffWithSort(ulong entity, int buffSort)
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if((int)ListBuff[i].Config.buff_sort == buffSort)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsExistBuffWithGroup(ulong entity, int buffGroup)
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].Config.group_id == buffGroup)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 转换主机

        public void ToHost()
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i] == null)
                {
                    continue;
                }
                ListBuff[i].FakeToReal();
            }
        }

        public void ToUnHost()
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i] == null)
                {
                    continue;
                }
                ListBuff[i].RealToFake();
            }
        }

        #endregion

        #region 重连

        public Buff CreateResumeBuff(ulong source, int configId, uint dynamicId, uint endBattleTime)
        {
            buffFactory.SetMaxSequence(source, dynamicId);
            ConfigTable.BuffData buffData = ConfigTable.BuffData.ByID(configId);
            if(buffData.IsNull)
            {
                return null;
            }
            int leftTime = 0;
            if(buffData.exist_time != -1)
            {
                #region 计算BUFF时间

                ////这个是战场已经经过的时间
                //uint timePass = act.game.ServerTimeManager.instance.GetReckonSecondTime(net.ServerType.BATTLE_SERVER) -
                //    (uint)(act.game.ServerTimeManager.instance.GetStartServerTime(net.ServerType.BATTLE_SERVER) / act.data.ConfigSetting.MILLISECOND);
                ////这个是buff结束的战场时间
                //endBattleTime = endBattleTime / constants.ConfigSetting.MILLISECOND;
                //leftTime = (int)(endBattleTime - timePass);

                //dictBuffAttachTimes[configId] = UnityEngine.Time.time - (buffData.exist_time / act.data.ConfigSetting.MILLISECOND - leftTime);

                #endregion

                //这个buff已经到期了，不需要再增加
                if(leftTime <= 0)
                {
                    return null;
                }
            }
            else
            {
                saveAttachTime(configId);
            }

            Buff buff = buffFactory.CreateBuff(source, carrierId, configId, dynamicId);
            ListBuff.Add(buff);
            if(leftTime > 0)
            {
                buff.ExistTime = leftTime;
            }
            return buff;
        }

        #endregion

        #region 更新检测

        List<Buff> removeBuffs = new List<Buff>();
        private float carrierTimeScale = 1f;

        public void LogicUpdate()
        {
            for(int i = ListBuff.Count - 1; i >= 0; i--)
            {
                if(ListBuff[i].Keep)
                {
                    continue;
                }
                ListBuff[i].UpdateExistTime(carrierTimeScale);
            }
            removeBuffs.Clear();
            for(int i = ListBuff.Count - 1; i >= 0; i--)
            {
                if(ListBuff[i].OverTime && !ListBuff[i].Keep)
                {
                    removeBuffs.Add(ListBuff[i]);
                }
            }

            for(int i = 0; i < removeBuffs.Count; i++)
            {
                DetachBuffWithNet(removeBuffs[i].Id, BuffDetachType.DETACH_ID);
            }
        }

        void onEntitySpeedChange(ulong targetId, float speed)
        {
            if(targetId != carrierId)
            {
                return;
            }
            carrierTimeScale = speed;
        }

        #endregion

        public void RemoveAllBuffVFX()
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i] == null)
                {
                    continue;
                }
                ListBuff[i].DisableVFXPerform();
            }
        }

        #region Net

        public bool OnRecvBuffUseEvent(ulong dynamicId, int index, int type, int value, IEnumerable<uint> ids)
        {
            Buff buff = null;
            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i] == null)
                {
                    continue;
                }
                if(ListBuff[i].Id == dynamicId)
                {
                    buff = ListBuff[i];
                    break;
                }
            }
            if(buff == null)
            {
                return false;
            }
            return buff.BuffEffect.OnRecvBuffUseEvent(index, type, value, ids);
        }

        public void EffectBuffInternal(int buffId)
        {
            if(ListBuff.Count == 0)
            {
                return;
            }

            for(int i = 0; i < ListBuff.Count; i++)
            {
                if(ListBuff[i].Id == buffId)
                {
                    if(!BuffSourceUtility.CheckNativeLogic(ListBuff[i].CarrierID))
                    {
                        ListBuff[i].Effect();
                    }
                    return;
                }
            }
        }

        #endregion

        //private ConfigTable.BuffData getConfig(int configId)
        //{
        //    ConfigTable.BuffData data = ConfigTable.BuffData.ByID(configId);
        //    if(data.IsNull)
        //    {
        //        return default(Framework.BuffData);
        //    }
        //    return data;
        //}


        public void Clear()
        {
            for(int i = 0; i < ListBuff.Count; i++)
            {
                ListBuff[i]?.Release(true);
            }
            ListBuff.Clear();
            buffFactory.Clear();
        }
    }
}

using System.Collections.Generic;

namespace ASeKi.battle
{
    public class Carrier
    {
        private ulong carrierId;
        public List<Buff> ListBuff { get; private set; } = new List<Buff>(); 
        private Dictionary<int, float> dictBuffAttachTimes = new Dictionary<int, float>();


        private static BuffFactory buffFactory = new BuffFactory();                 // 持有静态的BUFF工厂

        public Carrier(ulong carrierId)
        {
            this.carrierId = carrierId;
        }

        public uint AttachBuff(ulong source, ulong carrier, int configId, int curSkillId = 0)
        {
            ConfigTable.BuffData buffData = new ConfigTable.BuffData(configId);
            // 获取buff数据
            // 获取载体数据
            // 判别当前载体状态是否可以挂载BUFF
            // 判别当前载体BUFF池允许以何种方式处理该BUFF
            // 前方都没拒绝则加上去
            return 0;
        }

        public uint AddAttachCmd(ulong source, int configId, int sourceSkillId)
        {
            uint dynamicId = buffFactory.GetDynamicBuffId(source);
            Buff buff = AttachBuffWithoutNet(source, configId, dynamicId);
            buff.SourceSkill = sourceSkillId;
            // evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.UPDATE, carrierId, ListBuff);
            return dynamicId;
        }

        public Buff AttachBuffWithoutNet(ulong source,int configID,uint dynamicID)
        {
            // evt.EventManager.instance.Send(evt.EventGroup.BUFF,(short)evt.BuffEvent.BUFF_ATTACH,)
            Buff buff = buffFactory.CreateBuff(source, carrierId, configID, dynamicID);
            ListBuff.Add(buff);
            //AttachBuffInternal这个函数在挂载的时候无论host端还是非host端都能运行，所以设置两边信息同步
            //包含buff的挂载时间，已经sequenceId
            saveAttachTime(configID);
            return buff;
        }

        public void DetachBuffWithNet(uint buffID ,BuffDetachType detachType)
        {
            // TODO:发送到服务端
            DetachBuffWithoutNet(buffID, detachType);
            // TODO:通知需要更新的东西evt.EventManager.instance.Send(evt.EventGroup.BUFF,(short)evt.BuffEvent.BUFF_UPDATE,)
        }

        public void DetachBuffWithoutNet(uint buffID,BuffDetachType detachType)
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

        private void saveAttachTime(int configId)
        {
            dictBuffAttachTimes[configId] = UnityEngine.Time.time;
        }

        //private ConfigTable.BuffData getConfig(int configId)
        //{
        //    ConfigTable.BuffData data = ConfigTable.BuffData.ByID(configId);
        //    if(data.IsNull)
        //    {
        //        return default(Framework.BuffData);
        //    }
        //    return data;
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    // buff挂载的种类，是单个挂载还是替换还是叠加（目前只有这几种）还没有涉及到成批添加
    public enum BuffAttachType : byte
    {
        ATTACH = 1,
        REPLACE,
        ATTACH_FRESH,
        ATTACH_ADDTIVE,
    }

    // buff消除的种类，是把ID相同的消除还是把种类为X的消除等等
    public enum BuffDetachType : byte
    {
        DETACH_ID,
        DETACH_CONFIG_ID,
        DETACH_GROUP,
        DETACH_SORT,
        DETACH_TYPE,
        DETACH_ALL,
    }

    public partial class BuffManager : Singleton<BuffManager>
    {
        private Dictionary<ulong, Carrier> dictCarrier = new Dictionary<ulong, Carrier>();

        #region 初始化战斗BUFF（包括重连）

        public void CreateBattleInitialBuff()
        {
            if(!network.model.BattleModel.instance.m_BattleInfo.IsReconnect)
            {
                #region 从装备里获取BUFF
                //List<int> equipmentBuffIds = game.EquipmentManager.instance.GetPlayerPlayerEquipmentBuffInfo(false);
                //ulong source = BattleActorManager.instance.MainPlayer.ID;

                //if(equipmentBuffIds != null && equipmentBuffIds.Count != 0)
                //{
                //    for(int i = 0, count = equipmentBuffIds.Count; i < count; ++i)
                //    {
                //        if(equipmentBuffIds[i] == 0)
                //        {
                //            continue;
                //        }

                //        // debug.PrintSystem.Log($"[Buff] Equipment buff: {equipmentBuffIds[i]}");
                //        AttachBuff(source, source, equipmentBuffIds[i]);
                //    }
                //}
                #endregion

                #region 从词缀里获取BUFF
                //List<AffixInfo> affixInfos = Model.Battle.mainHeroAffixInfos;
                //for(int i = 0, iCount = affixInfos.Count; i < iCount; ++i)
                //{
                //    Framework.AffixData affixData = Framework.AffixData.ByID((int)affixInfos[i].U16DictAffixID);
                //    for(int j = 0, jCount = affixData.buff_id.Length; j < jCount; ++j)
                //    {
                //        // debug.PrintSystem.Log($"[Buff] Affix buff: {affixData.buff_id[j]}");
                //        AttachBuff(source, source, affixData.buff_id[j]);
                //    }
                //}
                //affixInfos.Clear();
                #endregion
            }

            #region 重连初始化

            //HeroData heroData = BattleActorManager.instance.MainPlayer.HeroData;
            //int hp = heroData.HP;
            //int sp = heroData.SP;
            //List<MSG_CLIENT_BATTLE_ROLE_BUFF_INFO_LIST_NOTIFY> buffInfos = Model.Battle.heroBuffInfos;
            //List<Buff> tempResumeBuffs = new List<Buff>();
            //for(int i = 0, infoCount = buffInfos.Count; i < infoCount; ++i)
            //{
            //    MSG_CLIENT_BATTLE_ROLE_BUFF_INFO_LIST_NOTIFY msg = buffInfos[i];
            //    tempResumeBuffs.Clear();
            //    //要分两步走，第一步先创建buff，第二步再让buff起效果
            //    for(int j = 0, buffCount = msg.Lst.Count; j < buffCount; ++j)
            //    {
            //        Buff buffResume = createResumeBuff(
            //            msg.Lst[j].U16SourceID,
            //            msg.U16RoleID,
            //            (int)msg.Lst[j].U32DictBuffID,
            //            msg.Lst[j].U32TargetBuffID,
            //            msg.Lst[j].U32EndBattleTime);
            //        tempResumeBuffs.Add(buffResume);
            //    }
            //    for(int j = 0, buffCount = msg.Lst.Count; j < buffCount; ++j)
            //    {
            //        Buff buffResume = tempResumeBuffs[j];
            //        if(buffResume == null)
            //        {
            //            continue;
            //        }
            //        buffResume.Resume((int)msg.Lst[j].U08Layer, msg.Lst[j].U08IsEffect == 1, false);
            //    }
            //    Carrier carrier = getCarrier(msg.U16RoleID);
            //    carrier.CallReCalcStatus();
            //    evt.EventManager.instance.Send(evt.EventGroup.BUFF, (short)evt.BuffEvent.UPDATE, (ulong)msg.U16RoleID, carrier.ListBuff);
            //}

            //heroData.SP = sp;
            //heroData.HP = hp;
            //Model.Battle.heroBuffInfos.Clear();
            
            #endregion
        }

        private Buff createResumeBuff(ulong source, ulong carrierId, int configId, uint dynamicId, uint endBattleTime)
        {
            Carrier carrier = this.getCarrier(carrierId);
            return carrier.CreateResumeBuff(source, configId, dynamicId, endBattleTime);
        }

        #endregion

        // 创建、挂载BUFF
        public uint AttachBuff(ulong source, ulong carrier, int buffID, int curSkillId = 0)
        {
            buffID = checkReplaceBuff(source, buffID);
            if(buffID == 0)
            {
                return 0;
            }
            sendAttachApply(source, carrier, buffID);
            if(buffAttachApplyData.Refuse)
            {
                return 0;
            }
            return getCarrier(carrier).AttachBuff(source, carrier, buffID, curSkillId);
        }

        #region 卸载BUFF

        #region 动态ID

        // 已知buff去卸载
        public void DetachBuff(Buff buff)
        {
            detachBuffInternal(buff.CarrierID, buff.Id, BuffDetachType.DETACH_ID);
        }

        // 已知载体ID和动态ID去卸载
        public void RemoveBuffWithDynamicId(ulong carrierID, uint dynamicId)
        {
            detachBuffInternal(carrierID, dynamicId, BuffDetachType.DETACH_ID);
        }

        // 已知动态ID去卸载
        public void RemoveBuffWithDynamicId(uint dynamicId)
        {
            uint entity = BuffCarrierUtil.GetCarrierIdByDynamicId(dynamicId);
            detachBuffInternal(entity, dynamicId, BuffDetachType.DETACH_ID);
        }

        #endregion

        #region 配置ID

        // 已知挂载者ID和配置ID去卸载
        public void RemoveBuffWithId(ulong carrierID, uint buffId)
        {
            detachBuffInternal(carrierID, buffId, BuffDetachType.DETACH_CONFIG_ID);
        }

        #endregion

        #region 种类（增益/减益）

        public void RemoveBuffWithSort(ulong carrierID, int sort)
        {
            detachBuffInternal(carrierID, (uint)sort, BuffDetachType.DETACH_SORT);
        }

        #endregion

        #region 类型

        public void RemoveBuffWithType(ulong carrierID, int type)
        {
            detachBuffInternal(carrierID, (uint)type, BuffDetachType.DETACH_TYPE);
        }

        #endregion

        #region 组

        public void RemoveBuffWithGroup(ulong carrierID, int group)
        {
            detachBuffInternal(carrierID, (uint)group, BuffDetachType.DETACH_GROUP);
        }

        #endregion

        #region 移除载体身上BUFF

        // 移除载体身上已经死掉的BUFF
        public void RemoveDeadBuff(ulong carrier)
        {
            getCarrier(carrier).RemoveDeadBuff();
        }

        // 移除载体上的所有buff
        public void RemoveUnitAllBuff(ulong carrier)
        {
            getCarrier(carrier).RemoveUnitAllBuff();
        }

        #endregion

        private void detachBuffInternal(ulong carrierID, uint info, BuffDetachType detachType,bool sendRequest = true)
        {
            Carrier carrier = getCarrier(carrierID);
            if(sendRequest)
            {
                carrier.DetachBuffWithNet(info, detachType);
            }
            else
            {
                carrier.DetachBuffWithoutNet(info, detachType);
            }
        }
        #endregion

        #region 免疫效果

        public bool CheckContainImmuneEffect(ulong targetId, int antiEffect)
        {
            Carrier carrier = getCarrier(targetId);
            return carrier.CheckImmuneEffect(antiEffect);
        }

        public void SetImmuneEffect(ulong targetId, int antiEffect, bool active)
        {
            Carrier carrier = getCarrier(targetId);
            carrier.SetImmuneEffect(antiEffect, active);
        }

        #endregion


        // 根据ID获得/创建载体
        private Carrier getCarrier(ulong carrierId)
        {
            if(!dictCarrier.TryGetValue(carrierId, out Carrier carrier))
            {
                carrier = new Carrier(carrierId);
                dictCarrier[carrierId] = carrier;
            }

            return carrier;
        }

        public void Clear()
        {
            foreach(KeyValuePair<ulong, Carrier> kvPair in dictCarrier)
            {
                kvPair.Value.Clear();
            }
            dictCarrier.Clear();
            clearReplaceBuff();
            BuffExecuterFactory.Clear();
        }

        // 检测死亡的BUFF
        public void LogicUpdate()
        {
            // NOTE: Remove time up buffs
            foreach(KeyValuePair<ulong, Carrier> kvPair in dictCarrier)
            {
                kvPair.Value.LogicUpdate();
            }
        }

        #region 功能接口

        public void RemoveAllBuffVFX()
        {
            foreach(KeyValuePair<ulong, Carrier> item in dictCarrier)
            {
                item.Value.RemoveAllBuffVFX();
            }
        }

        public List<Buff> GetCarrierBuffList(ulong carrierId)
        {
            return getCarrier(carrierId).ListBuff;
        }

        #endregion

    }
}
using UnityEngine;

namespace ASeKi.battle
{
    public class LogicStatusComponent : EntityComponent
    {
        public LogicStatusComponent(Entity entity, BattleMonsterStatus battleMonsterStatus) : base(entity)
        {
            //dataComponent = EntityObject.LinkC().MonsterDataC;
            //ltc = EntityObject.LinkC().LTranformC;

            //BaseStatusParam.Clear();

            //CurrMonsterParam = ltc.LogicGameObject.GetComponent<MonsterParam>();
            //CurrMonsterParam.CurrMonster = (Monster)entity;

            //LogicStatus = battleMonsterStatus;
            //UpdateBattleMonsterStatus(battleMonsterStatus);

            //if(!action.ActionManager.instance.IsUseActionAnimGraphsForMonster)
            //{
            //    int hashCode = CurrMonsterParam.CurrAnimator.GetHashCode();
            //    RuntimeAnimatorController controller = action.ActionSetting.GetController(hashCode, LogicStatus.actionRoleType);
            //    if(controller != null)
            //    {
            //        CurrMonsterParam.CurrAnimator.runtimeAnimatorController = controller;
            //    }
            //    else
            //    {
            //        debug.PrintSystem.LogError($"can not load controller : [{LogicStatus.actionRoleType}]");
            //    }
            //}
        }

        public void CalcCurrentStatus()
        {
            //int currentHp = CurrentStatusParam.HP;
            //int currentMaxHp = CurrentStatusParam.MaxHP;
            //CurrentStatusParam.ClearAttr();
            //// 基础属性
            //CurrentStatusParam.AddAttr(BaseStatusParam);
            //// Buff属性
            //BuffManager.instance.AddBuffStatus(EntityObject.ID, CurrentStatusParam);

            //CurrentStatusParam.Set(constant.StatusTypes.HP, currentHp);
            //// 血量按比例保存
            //CurrentStatusParam.HP = (int)(CurrentStatusParam.MaxHP * (1.0f * currentHp / currentMaxHp));
        }
    }
}

using System.Collections.Generic;


namespace ASeKi.battle
{
    public static class BuffSelectTarget
    {
        private static List<Entity> entities = new List<Entity>();

        public static void GetBuffTargets(Buff buff, List<ulong> targets, ulong targetId = 0)
        {
            targets.Clear();
            entities.Clear();

            switch((ConfigTable.BuffTargetType)buff.Config.target_type)
            {
                case ConfigTable.BuffTargetType.NONE:
                    break;
                case ConfigTable.BuffTargetType.SELF:
                    getSelfTarget(buff);
                    break;
                case ConfigTable.BuffTargetType.ENEMY:
                    break;
                case ConfigTable.BuffTargetType.FRIENDS:
                    break;
                case ConfigTable.BuffTargetType.SELF_FRIENDS:
                    break;
                case ConfigTable.BuffTargetType.FOCUS:
                    break;
                case ConfigTable.BuffTargetType.MONSTER:
                    break;
                case ConfigTable.BuffTargetType.HERO:
                    break;
                default:
                    break;
            }

            //entity数太多，需要进行排序，选择最优先的生效
            if(buff.Config.target_maxnumber != 0 && entities.Count > buff.Config.target_maxnumber)
            {
                sortTargets((ConfigTable.BuffSortTarget)buff.Config.select_rule);
            }

            for(int i = 0; i < entities.Count && i < buff.Config.target_maxnumber; i++)
            {
                bool valifySuccess = true;
                //for(int j = 0; j < buff.Config.target_conditions.Length; j++)
                //{
                //    if(buff.Config.target_conditions[j].condition_config > 0)
                //    {
                //        if(!BuffCondition.Valify(entities[i], buff.Config.target_conditions[j]))
                //        {
                //            valifySuccess = false;
                //            break;
                //        }
                //    }
                //}
                if(valifySuccess)
                {
                    if(entities[i] != null)
                    {
                        targets.Add(entities[i].ID);
                    }
                }
            }
        }

        private static void getSelfTarget(Buff buff)
        {
            Entity unit = BattleActorManager.instance.GetActorById(buff.CarrierID);
            entities.Add(unit);
        }

        static void sortTargets(ConfigTable.BuffSortTarget select_rule)
        {
            switch(select_rule)
            {
                case ConfigTable.BuffSortTarget.NONE:
                    break;
                case ConfigTable.BuffSortTarget.RANDOM:
                    break;
                case ConfigTable.BuffSortTarget.HP_MAX:
                    break;
                case ConfigTable.BuffSortTarget.HP_MIN:
                    break;
                case ConfigTable.BuffSortTarget.ATTACK_MAX:
                    break;
                case ConfigTable.BuffSortTarget.ATTACK_MIN:
                    break;
                case ConfigTable.BuffSortTarget.DEFENCE_MAX:
                    break;
                case ConfigTable.BuffSortTarget.DEFENCE_MIN:
                    break;
                default:
                    break;
            }
        }
    }
}

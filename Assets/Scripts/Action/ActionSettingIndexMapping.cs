using System.Collections.Generic;
using ASeKi.action.Data;

namespace ASeKi.action
{
    public class ActionSettingIndexMapping
    {
        // key : roleType
        //      key : actionID
        //      value : actionState 
        static Dictionary<ActionRoleType, Dictionary<int, int>> mappingActionIdDict = new Dictionary<ActionRoleType, Dictionary<int, int>>();
        
        // key : roleType
        //      key: actionState
        //      value: actionID
        static Dictionary<ActionRoleType, Dictionary<int, int>> mappingActionStateDict = new Dictionary<ActionRoleType, Dictionary<int, int>>();
        
        static readonly Dictionary<ActionRoleType, AnimatorDataSource> motionRuntimeActionDict = new Dictionary<ActionRoleType, AnimatorDataSource>();
        
        public static string PLAYER_MOTION_DATA_LIST_PATH = "Player/PlayerMotionDataListSO";
    }
}
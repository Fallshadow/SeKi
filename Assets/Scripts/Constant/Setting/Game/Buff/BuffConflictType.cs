namespace constants
{
    //冲突处理规则
    public enum BuffConflictType
    {
        BCT_NONE,

        //替换
        BCT_REPLACE = 1,

        //互斥
        BCT_ONLYONE = 2,

        //独立
        BCT_INDEPENDENT = 3,

        //叠加
        BCT_ADDTIVE = 4,

        //叠加刷新
        BCT_ADDTIVE_FRESH = 5,
    }
}
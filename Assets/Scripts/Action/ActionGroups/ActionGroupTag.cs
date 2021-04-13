using System;

namespace ASeKi.action
{
    [Flags]
    public enum ActionGroupTag : long
    {
        [ActionGroupAttr("none")] ActionGroup0 = 0,
        [ActionGroupAttr("空手-可接其他")] ActionGroup1 = 1 << 0,
        [ActionGroupAttr("空手-移动类")] ActionGroup2 = 1 << 1,
        [ActionGroupAttr("空手-不可接其他")] ActionGroup3 = 1 << 2,
    }
}
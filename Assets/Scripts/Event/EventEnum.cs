namespace ASeKi.evt
{
    // 这边是给不同模块传送讯息的，来降低耦合度
    // 事件分类以发送者为依据，主要是各个系统的manager
    // 不要把这边的事件存储在Mono或者ScriptableObject上，否则一旦更改顺序，这边值会乱
    public enum EventGroup : short
    {
        NONE = 0,
        INPUT,
        UI,
        ENTITY,
        BT,//behavior tree
        CAMERA,
        EFFECT,
        REALTIME_TALK,
        HOMELAND,
        SYSTEMSETTING,//系统设置
        LOGIN, // 登陆
        BUFF,
    }

    public enum UiEvent : short
    {
        UI_BASE_SHOW,
        UI_BASE_HIDE,
        ALL_FULL_SCREEN_CANVAS_CLOSED,                      // 所有全屏UI都关闭了
        ALL_FULL_SCREEN_CANVAS_CLOSED_BUT_DEFUALT,          // 仅仅剩下一个默认全屏UI
    }

    public enum BuffEvent : short
    {
        BUFF_ATTACH_APPLY,                                  // 申请挂载BUFF

    }
}
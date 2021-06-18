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
    
    public enum CameraEvent : short
    {
        THREE_LAYER_LOOK_CINE,
        THREE_LAYER_CUT_SCENE,                              // 切换场景
        THREE_LAYER_CLICK_POS,                              // 切换详细位置
        THREE_LAYER_INPUT_AREA,                             // 滑动区域
    }

    public enum BuffEvent : short
    {
        BUFF_ATTACH_APPLY,                                  // 申请挂载BUFF
        BUFF_ATTACH,                                        // 挂载BUFF，给对应监听者发送所需信息 暂时无用
        BUFF_UPDATE,                                        // BUFF更新事件
        BUFF_DETACH,                                        // BUFF卸载事件
        BUFF_REPLACE,                                       // BUFF替换事件
    }
}
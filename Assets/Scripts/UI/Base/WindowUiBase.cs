namespace ASeKi.ui
{
    public abstract class WindowUiBase : InteractableUiBase
    {
        public override UiOpenType OpenType { get { return UiOpenType.UOT_POP_UP; } }// 默认一般UI窗口

    }
}

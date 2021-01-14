
namespace ASeKi.battle
{
    public abstract class BuffPerform
    {
        public Buff ParentBuff = null;
        public abstract void OnAttach();
        public abstract void OnEffect();
        public abstract void OnDetach();
    }
}

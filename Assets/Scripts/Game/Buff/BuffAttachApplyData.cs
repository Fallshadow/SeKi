

namespace ASeKi.game
{
    class BuffAttachApplyData
    {
        public ulong Source = 0;
        public ulong Carrier = 0;
        public int ConfigID = 0;
        public bool Refuse
        {
            get { return refuse; }
            set 
            {
                if(value == true)
                {
                    ASeKi.debug.PrintSystem.Log("[Buff] 挂载BUFF " + ConfigID + "被拒绝!");
                }
                refuse = value; 
            }
        }
        private bool refuse = false;

        public void Reset()
        {
            Source = 0;
            Carrier = 0;
            ConfigID = 0;
            refuse = false;
        }
    }
}
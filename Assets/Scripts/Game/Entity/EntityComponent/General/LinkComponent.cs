namespace ASeKi.battle
{
    public class LinkComponent : EntityComponent
    {
        LogicStatusComponent lsc = null;

        public LinkComponent(Entity entity) : base(entity)
        {

        }

        public LogicStatusComponent LStatusC
        {
            get
            {
                if(lsc == null)
                {
                    lsc = EntityObject.GetComponent<LogicStatusComponent>();
                }
                return lsc;
            }
        }
    }
}

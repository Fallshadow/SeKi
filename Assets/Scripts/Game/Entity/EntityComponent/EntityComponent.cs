using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASeKi.battle
{
    public class EntityComponent
    {
        public Entity EntityObject = null;
        public EntityComponent(Entity e)
        {
            this.EntityObject = e;
        }
    }
}

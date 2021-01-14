using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    public class BattleActorManager : SingletonMonoBehavior<BattleActorManager>
    {
        //public Dictionary<ulong, Hero> DictPlayer { get; } = new Dictionary<ulong, Hero>();
        //public Dictionary<ulong, InteractObject> DictInteractObj { get; } = new Dictionary<ulong, InteractObject>();
        //public Dictionary<ulong, Monster> DictMonster { get; } = new Dictionary<ulong, Monster>();

        public Entity GetActorById(ulong id)
        {
            //if(DictPlayer.ContainsKey(id))
            //{
            //    return DictPlayer[id];
            //}
            //else if(DictMonster.ContainsKey(id))
            //{
            //    return DictMonster[id];
            //}
            //else if(DictInteractObj.ContainsKey(id))
            //{
            //    return DictInteractObj[id];
            //}
            return null;
        }
    }
}
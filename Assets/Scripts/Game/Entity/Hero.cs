using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    public class Hero : Entity
    {
        public HeroLogic HeroLogic { get; private set; } = new HeroLogic();

    }
}

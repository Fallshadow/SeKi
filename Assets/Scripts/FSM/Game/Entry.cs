using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    public class Entry : State<game.GameController>
    {
        public override void Enter()
        {
            initManagers();
        }

        private void initManagers()
        {

        }
    }
}
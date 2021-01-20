using System;
using UnityEngine;
using System.Collections.Generic;

namespace ASeKi.time
{
    public partial class TimeManager
    {
        public static readonly DateTime ZeroTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private void Update()
        {
            tempCurrentNode = timerLink.First;
            while(tempCurrentNode != null)
            {
                LinkedListNode<Timer> nextNode = tempCurrentNode.Next;
                tempCurrentNode.Value.Process(Time.deltaTime);
                tempCurrentNode = nextNode;
            }
        }
    }
}

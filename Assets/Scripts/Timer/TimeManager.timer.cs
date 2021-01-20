using System;
using UnityEngine;
using System.Collections.Generic;

namespace ASeKi.time
{
    public partial class TimeManager : SingletonMonoBehavior<TimeManager>
    {
        private LinkedList<Timer> timerLink = new LinkedList<Timer>();
        private LinkedListNode<Timer> tempCurrentNode = null;

        public Timer AddUpdateTimer(bool isUnique, string name, UpdateTimer.OnUpdateDelegate callback, bool autoStart = true, object obj = null)
        {
            if(isUnique)
            {
                Timer existTimer = GetUniqueTimer(name);
                if(existTimer != null)
                    return existTimer;
            }

            UpdateTimer updateTimer = new UpdateTimer(name, isUnique, callback, obj);
            if(!autoStart)
            {
                updateTimer.Pause();
            }

            return AddTimer(updateTimer);
        }

        public Timer AddCycleTimer(bool isUnique, string name, float cycle, CycleTimer.OnCycleDelegate callback, bool autoStart = true, object obj = null)
        {
            if(isUnique)
            {
                Timer existTimer = GetUniqueTimer(name);
                if(existTimer != null)
                    return existTimer;
            }

            CycleTimer cycleTimer = new CycleTimer(name, isUnique, cycle, callback, obj);
            if(!autoStart)
            {
                cycleTimer.Pause();
            }

            return AddTimer(cycleTimer);
        }

        public Timer GetUniqueTimer(string name)
        {
            tempCurrentNode = timerLink.First;
            while(tempCurrentNode != null)
            {
                if(tempCurrentNode.Value.IsUnique && tempCurrentNode.Value.Name == name)
                    return tempCurrentNode.Value;

                tempCurrentNode = tempCurrentNode.Next;
            }

            return null;
        }

        private Timer AddTimer(Timer data)
        {
            if(!timerLink.Contains(data))
            {
                timerLink.AddLast(data);
                return data;
            }

            debug.PrintSystem.LogError("[TIME] Arealy have same timer!");
            return null;
        }

        public void RemoveTimer(Timer data)
        {
            timerLink.Remove(data);
        }
    }
}

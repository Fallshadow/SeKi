using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe abstract class Motion : System.IDisposable
    {
        protected int inputPort;
        protected AnimationMixerPlayable parent;

        private AnimGraphPlayer m_player;
        protected AnimGraphPlayer player {
            get {
                return m_player;
            }
            set
            {
                m_player = value;
            }
        }

        public abstract void Preparatory(ref MotionAsset motionAsset, float timeScale);

        public void Connecting(AnimGraphPlayer player, AnimationMixerPlayable mixer, int inputPort)
        {
            this.player = player;
            this.inputPort = inputPort;
            this.parent = mixer;
            Connect();
        }

        public abstract bool isLooping { get; }
        public abstract uint duration { get; protected set; }
        public abstract void Connect();
        public abstract void Disconnected();
        public abstract void Update(float normalTime, int loopCount);
        public abstract void Dispose();
        public abstract void GetWeights(float parentWeight, int stateName, List<AnimationClipWeight> weights);
        public abstract void SetFootIK(bool useFootIK);

        public void SetWeight(float weight)
        {
            parent.SetInputWeight(inputPort, weight);
        }

        public float GetWeight()
        {
            return parent.GetInputWeight(inputPort);
        }
    }
}
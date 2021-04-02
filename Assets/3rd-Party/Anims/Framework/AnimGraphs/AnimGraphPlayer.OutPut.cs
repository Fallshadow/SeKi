using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private void InitOutPut()
        {
            AnimationPlayableUtilities.Play(animator, controlPlayable, m_Graph);
            output = (AnimationPlayableOutput) m_Graph.GetOutputByType<AnimationPlayableOutput>(0);
//            output = AnimationPlayableOutput.Create(m_Graph, "", animator);
//            //output.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);
//            output.SetSourcePlayable(controlPlayable);
        }
        
        public AnimationScriptPlayable InsertOutputJob<T>(T data) where T : struct, IAnimationJob
        {
            var playable = AnimationScriptPlayable.Create(m_Graph, data, 1);
            var currentOutput = m_Graph.GetOutput(0);
            m_Graph.Connect(currentOutput.GetSourcePlayable(), 0, playable, 0);
            playable.SetInputWeight(0, 1);
            currentOutput.SetSourcePlayable(playable);
            return playable;
        }
    }
}
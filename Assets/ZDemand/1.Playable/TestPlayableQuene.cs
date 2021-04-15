using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

// 自己创建behavior去搞
public class TestQuenePlayable : PlayableBehaviour
{
    private int m_CurrentClipIndex = -1;
    private float m_TimeToNextClip;
    private Playable mixer;

    public void Initialize(AnimationClip[] clips, Playable owner, PlayableGraph graph)
    {
        owner.SetInputCount(1);
        mixer = AnimationMixerPlayable.Create(graph, clips.Length);
        graph.Connect(mixer, 0, owner, 0);
        owner.SetInputWeight(0, 1);
        for (int clipIndex = 0; clipIndex < mixer.GetInputCount(); clipIndex++)
        {
            graph.Connect(AnimationClipPlayable.Create(graph, clips[clipIndex]), 0, mixer, clipIndex);
            mixer.SetInputWeight(clipIndex, 1.0f);
        }
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        if (mixer.GetInputCount() == 0)
            return;
        m_TimeToNextClip -= info.deltaTime;
        if (m_TimeToNextClip <= 0.0f)
        {
            m_CurrentClipIndex++;
            if (m_CurrentClipIndex >= mixer.GetInputCount())
            {
                m_CurrentClipIndex = 0;
            }

            AnimationClipPlayable currentClip = (AnimationClipPlayable)mixer.GetInput(m_CurrentClipIndex);
            currentClip.SetTime(0);
            m_TimeToNextClip = currentClip.GetAnimationClip().length;
        }

        for (int clipIndex = 0; clipIndex < mixer.GetInputCount(); clipIndex++)
        {
            if (clipIndex == m_CurrentClipIndex)
            {
                mixer.SetInputWeight(clipIndex, 1f);
            }
            else
            {
                mixer.SetInputWeight(clipIndex, 0f);
            }
        }
    }
}


public class TestPlayableQuene : MonoBehaviour
{
    public AnimationClip[] Clips;
    private PlayableGraph graph;

    private void Start()
    {
        graph = PlayableGraph.Create();
        ScriptPlayable<TestQuenePlayable> playableQuene = ScriptPlayable<TestQuenePlayable>.Create(graph);
        TestQuenePlayable testQuenePlayable = playableQuene.GetBehaviour();
        testQuenePlayable.Initialize(Clips, playableQuene, graph);
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(graph,"我的队列",GetComponent<Animator>());
        playableOutput.SetSourcePlayable(playableQuene, 0);
        graph.Play();
    }
}

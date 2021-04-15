using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class TestPlayablePauseStatus : MonoBehaviour
{
    public AnimationClip clip0;
    public AnimationClip clip1;
    public float time = 1;
    private PlayableGraph playableGraph;
    private AnimationMixerPlayable mixerPlayable;
    private AnimationClipPlayable animclip1;
    private void OnEnable()
    {
        CommonStatusController();
    }

    private void CommonStatusController()
    {
        playableGraph = PlayableGraph.Create();
        AnimationPlayableOutput animationPlayableOutput = AnimationPlayableOutput.Create(playableGraph, "testAnim", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);
        animationPlayableOutput.SetSourcePlayable(mixerPlayable);
        AnimationClipPlayable animclip0 = AnimationClipPlayable.Create(playableGraph,clip0);
        animclip1 = AnimationClipPlayable.Create(playableGraph,clip1);
        playableGraph.Connect(animclip0, 0, mixerPlayable, 0);
        playableGraph.Connect(animclip1, 0, mixerPlayable, 1);
        mixerPlayable.SetInputWeight(0, 1.0f);
        mixerPlayable.SetInputWeight(1, 1.0f);
        animclip0.Pause();
        // mixerPlayable.Pause();
        playableGraph.Play();
    }
    
    private void OnDisable()
    {
        playableGraph.Destroy();
    }

    private void Update()
    {
        animclip1.SetTime(time);// 控制播到了何等时间
    }
}

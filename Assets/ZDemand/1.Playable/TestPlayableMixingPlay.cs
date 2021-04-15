using System;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine;
using UnityEngine.Playables;

public class TestPlayableMixingPlay : MonoBehaviour
{
    public AnimationClip clip0;
    public AnimationClip clip1;
    public float clipsWeight;

    public AnimationClip diffClip0;
    public RuntimeAnimatorController diffAnimator0;
    public float diffClipAndControlWeight;
    
    private AnimationMixerPlayable mixerPlayable;
    private PlayableGraph playableGraph;

    private void Start()
    {
        commonPlayMixingTowClipAndController();
    }

    void Update()
    {
        commonPlayMixingTowClipAndControllerUpdate();
    }
    
    private void commonPlayMixingTowClips()
    {
        playableGraph = PlayableGraph.Create();
        PlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "MyAnimation", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);
        playableOutput.SetSourcePlayable(mixerPlayable);
        AnimationClipPlayable animclip0 = AnimationClipPlayable.Create(playableGraph,clip0);
        AnimationClipPlayable animclip1 = AnimationClipPlayable.Create(playableGraph,clip1);
        playableGraph.Connect(animclip0, 0, mixerPlayable, 0);
        playableGraph.Connect(animclip1, 0, mixerPlayable, 1);
        playableGraph.Play();
    }

    private void commonPlayMixingTowClipsUpdate()
    {
        clipsWeight = Mathf.Clamp01(clipsWeight);
        mixerPlayable.SetInputWeight(0,1 - clipsWeight);
        mixerPlayable.SetInputWeight(1,clipsWeight);
    }
    
    private void commonPlayMixingTowClipAndController()
    {
        playableGraph = PlayableGraph.Create();
        PlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "MyAnimation", GetComponent<Animator>());
        mixerPlayable = AnimationMixerPlayable.Create(playableGraph, 2);
        playableOutput.SetSourcePlayable(mixerPlayable);
        AnimationClipPlayable animclip0 = AnimationClipPlayable.Create(playableGraph,diffClip0);
        AnimatorControllerPlayable animator0 = AnimatorControllerPlayable.Create(playableGraph,diffAnimator0);
        playableGraph.Connect(animclip0, 0, mixerPlayable, 0);
        playableGraph.Connect(animator0, 0, mixerPlayable, 1);
        playableGraph.Play();
    }
    
    private void commonPlayMixingTowClipAndControllerUpdate()
    {
        diffClipAndControlWeight = Mathf.Clamp01(diffClipAndControlWeight);
        mixerPlayable.SetInputWeight(0,1 - diffClipAndControlWeight);
        mixerPlayable.SetInputWeight(1,diffClipAndControlWeight);
    }

    private void OnDisable()
    {
        playableGraph.Destroy();
    }
}

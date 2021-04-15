using System;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine;

public class TestPlayableCommonPlay : MonoBehaviour
{
    public AnimationClip Clip;
    private PlayableGraph playableGraph;

    private void Start()
    {
        commonPlayUtility();
    }

    private void commonPlay()
    {
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        AnimationPlayableOutput playableOutput = AnimationPlayableOutput.Create(playableGraph, "MyAnimation", GetComponent<Animator>());
        AnimationClipPlayable playable = AnimationClipPlayable.Create(playableGraph, Clip);
        playableOutput.SetSourcePlayable(playable);
        playableGraph.Play();
    }

    private void commonPlayUtility()
    {
        AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), Clip, out playableGraph);
    }
}

using System;
using UnityEngine.Playables;
using UnityEngine.Animations;
using UnityEngine.Audio;
using UnityEngine;

public class TestPlayableTwoOutPut : MonoBehaviour
{
    public AudioClip TestAudioClip;
    public AnimationClip TestAnimationClipClip;
    private PlayableGraph graph;

    private void Start()
    {
        CommonPlayTowOutput();
    }

    private void CommonPlayTowOutput()
    {
        graph = PlayableGraph.Create();
        AudioClipPlayable audioClipPlayable = AudioClipPlayable.Create(graph, TestAudioClip, true);
        AnimationClipPlayable animationClipPlayable = AnimationClipPlayable.Create(graph, TestAnimationClipClip);
        AudioPlayableOutput audioPlayableOutput = AudioPlayableOutput.Create(graph, "testAudio", GetComponent<AudioSource>());
        AnimationPlayableOutput animationPlayableOutput = AnimationPlayableOutput.Create(graph, "testAnim", GetComponent<Animator>());
        audioPlayableOutput.SetSourcePlayable(audioClipPlayable);
        animationPlayableOutput.SetSourcePlayable(animationClipPlayable);
        graph.Play();
    }

    private void OnDisable()
    {
        graph.Destroy();
    }
}

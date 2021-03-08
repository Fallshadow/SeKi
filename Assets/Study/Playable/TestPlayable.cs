using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TestPlayable : MonoBehaviour
{
    private PlayableGraph playableGraph;
    
    private void Start()
    {
        playableGraph = PlayableGraph.Create();
        
    }
}
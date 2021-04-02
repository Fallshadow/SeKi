using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionAssetMono : MonoBehaviour
{
    public MotionPlay player;

    public string stateName;
    public string[] queueStateNames;

    public void Play()
    {
        player.PlayState(this);
    }
}

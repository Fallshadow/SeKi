using Framework.AnimGraphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public unsafe class MotionPlayManager : MonoBehaviour
{
    public TextAsset streamAssets;
    public AssetMapping assetMapping;
    public GameObject[] players;

    [Range(-180f, 180f)]
    public float Angle = 0f;

    [Range(0.1f, 0.7f)]
    public float SpeedRatio = 0.1f;

    [Range(0.1f, 2f)]
    public float speed = 1f;

    [Range(0.5f, 1f)]
    public float distance = 0.5f;

    public string defaultStateName;

    private byte[] assets;
    private AnimGraphPlayer[] playerArray;
    private int angleIndex = 0;
    private int speedRatio = 0;
    private int stepDistance = 0;

    void Start()
    {
        AssetMapping.Init(assetMapping);

        playerArray = new AnimGraphPlayer[players.Length];

        for (int i = 0; i < players.Length; i++)
        {
            AnimGraphPlayerConfig config = new AnimGraphPlayerConfig();
            assets = streamAssets.bytes;
            fixed (byte* p = assets)
            {
                config.assetPtr = p;
            }

            config.animator = players[i].GetComponent<Animator>();
            var player = new AnimGraphPlayer(config);
            player.PlayState(defaultStateName);

            angleIndex = player.GetParmeterId("Angle");
            speedRatio = player.GetParmeterId("SpeedRatio");
            stepDistance = player.GetParmeterId("StepDistance");
            playerArray[i] = player;
        }
    }

    void Update()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            var player = playerArray[i];
            player.speed = speed;
            player.SetFloat(angleIndex, Angle);
            player.SetFloat(speedRatio, SpeedRatio);
            player.SetFloat(stepDistance, distance);
            player.Evaluate(Time.deltaTime);
        }

        GraphCreater.Instance.Evaluate();
    }

    private void LateUpdate()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            var player = playerArray[i];
            player.LateUpdate();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < playerArray.Length; i++)
        {
            var player = playerArray[i];
            player.Dispose();
        }
        GraphCreater.Instance.Dispose();
    }
}

using Framework.AnimGraphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public unsafe class MonsterPlay : MonoBehaviour
{
    public TextAsset streamAssets;
    public AssetMapping assetMapping;

    public string defaultStateName;

    private byte[] assets;
    private AnimGraphPlayer player;

    public bool usedIK = false;

    public void Play()
    {
        player.PlayState(defaultStateName.HashCode());
    }

    void Start()
    {
        Framework.AnimGraphs.AnimGraphCache.Instance.GetHashCode();
        AssetMapping.Init(assetMapping);

        AnimGraphPlayerConfig config = new AnimGraphPlayerConfig();
        assets = streamAssets.bytes;
        fixed (byte* p = assets)
        {
            config.assetPtr = p;
        }

        config.animator = GetComponent<Animator>();
        player = new AnimGraphPlayer(config);
        player.PlayState(defaultStateName);
    }

    void Update()
    {
        //player.speed = speed;
        player.Evaluate(Time.deltaTime);
        GraphCreater.Instance.Evaluate();

    }

    private void LateUpdate()
    {
        player.LateUpdate();
    }

    private void OnDestroy()
    {
        player.Dispose();
        GraphCreater.Instance.Dispose();
    }
}

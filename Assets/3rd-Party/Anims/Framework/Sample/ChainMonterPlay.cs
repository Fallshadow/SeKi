using Framework.AnimGraphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public unsafe class ChainMonterPlay : MonoBehaviour
{
    public Animator target;

    public TextAsset streamAssets;
    public AssetMapping assetMapping;

    [Range(0.1f, 2f)]
    public float speed = 1f;

    public string defaultStateName;

    private byte[] assets;
    private AnimGraphPlayer player;

    public bool usedIK = false;

    void Start()
    {
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
        target.Play("chainIKtarget");
    }

    void Update()
    {
        player.speed = speed;
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

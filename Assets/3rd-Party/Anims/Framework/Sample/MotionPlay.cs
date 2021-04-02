using Framework.AnimGraphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public unsafe partial class MotionPlay : MonoBehaviour
{
    public TextAsset streamAssets;
    public AssetMapping assetMapping;

    [Range(0f,1f)]
    public float timeline = 0f;

    [Range(-180f,180f)]
    public float Angle = 0f;

    [Range(0.1f, 0.7f)]
    public float SpeedRatio = 0.1f;

    [Range(0.1f,2f)]
    public float speed = 1f;

    [Range(0.5f,1f)]
    public float distance = 0.5f;

    public string defaultStateName;

    private byte[] assets;
    private AnimGraphPlayer player;
    private int angleIndex = 0;
    private int speedRatio = 0;
    private int stepDistance = 0;
    

    //private Rig rightFootRig; 

    void Start()
    {
        Framework.AnimGraphs.AnimGraphCache.Instance.GetHashCode();
        AssetMapping.Init(assetMapping);

        AnimGraphPlayerConfig config = new AnimGraphPlayerConfig();
        assets = streamAssets.bytes;
        fixed(byte* p = assets)
        {
            config.assetPtr = p;
        }

        config.animator = GetComponent<Animator>();
        player = new AnimGraphPlayer(config);
        player.PlayState(defaultStateName);

        angleIndex = player.GetParmeterId("Angle");
        speedRatio = player.GetParmeterId("SpeedRatio");
        stepDistance = player.GetParmeterId("StepDistance");
    }

    public void Play()
    {
        player.PlayState(defaultStateName);
    }

    public void PlayState(MotionAssetMono asset)
    {
        player.PlayState(asset.stateName.HashCode());
    }

    void Update()
    {

        player.speed = speed;
        player.SetFloat(angleIndex, Angle);
        player.SetFloat(speedRatio, SpeedRatio);
        player.SetFloat(stepDistance, distance);
        player.Evaluate(Time.deltaTime);
        //player.EvaluateAt(timeline);
        if (usedIK)
        {
            var nt = player.NormalizedTime(0, defaultStateName.HashCode());
            Debug.Log(nt);
        }
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

    public bool usedIK = false;
    
}

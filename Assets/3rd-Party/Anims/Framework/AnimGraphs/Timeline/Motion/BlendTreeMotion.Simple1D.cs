using Unity.Mathematics;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{

    public unsafe sealed partial class BlendTreeMotion
    {
        private void CalculateWeights1d()
        {
            float blendValue = blendParams.x;
            blendValue = math.clamp(blendValue, motionChilds[0].position.x, motionChilds[motionChilds.Length - 1].position.x);
            for (int i = 0; i < motionChilds.Length; i++)
            {
                blend.SetInputWeight(i, WeightForIndex(motionChilds, i, blendValue));
            }
        }

        private float WeightForIndex(NArray<ChildMotionAsset> thresholdArray, int index, float blendValue)
        {
            int count = thresholdArray.Length;
            float2 pos = thresholdArray[index].position;
            if (blendValue >= pos.x)
            {
                int nextIndex = index + 1;
                if (nextIndex == count)
                    return 1f;
                float2 nextPos = thresholdArray[nextIndex].position;
                if (nextPos.x < blendValue)
                {
                    return 0f;
                }
                else
                {
                    var dur = pos.x - nextPos.x;
                    if (dur != 0)
                    {
                        return (blendValue - nextPos.x) / dur;
                    }
                    else
                    {
                        return 1f;
                    }
                }
            }
            else
            {
                if (index == 0)
                    return 1f;
                int nextIndex = index - 1;
                float2 nextPos = thresholdArray[nextIndex].position;
                if (nextPos.x > blendValue)
                {
                    return 0f;
                }
                else
                {
                    var dur = pos.x - nextPos.x;
                    if (dur != 0)
                    {
                        return (blendValue - nextPos.x) / dur;
                    }
                    else
                    {
                        return 1f;
                    }
                }
            }
        }
    }
}
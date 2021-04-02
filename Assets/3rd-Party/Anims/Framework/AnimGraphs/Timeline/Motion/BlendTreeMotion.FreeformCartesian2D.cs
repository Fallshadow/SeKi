using Unity.Mathematics;
using UnityEngine.Playables;
namespace Framework.AnimGraphs
{
    public unsafe sealed partial class BlendTreeMotion
    {
        private void CalculateWeightsFreeformCartesian() {
            var blendPosition = blendParams;
            var count = motionChilds.Length;
            float* m_influences = stackalloc float[count];
            float sum = CalculateInfluence(blendPosition, m_influences);
            for (int i = 0; i < count; i++) {
                blend.SetInputWeight(i, m_influences[i] / sum);
            }
        }

        private float CalculateInfluence(float2 p, float* m_influences) {
            float sum = 0;
            for (int i = 0; i < motionChilds.Length; i++) {
                float minInfluence = float.MaxValue;
                float2 toP = p - motionChilds[i].position;
                for (int j = 0; j < motionChilds.Length; j++) {
                    if (i == j) continue;
                    float2 toJ = motionChilds[j].position - motionChilds[i].position;
                    float dot = toP.x * toJ.x + toP.y * toJ.y;
                    float magSquare = toJ.x * toJ.x + toJ.y * toJ.y;
                    float influence = magSquare != 0f ? math.max(0, 1 - dot / magSquare) : 1.0f / motionChilds.Length;
                    if (influence < minInfluence) {
                        minInfluence = influence;
                    }
                }
                m_influences[i] = minInfluence;
                sum += minInfluence;
            }
            return sum;
        }
    }
}
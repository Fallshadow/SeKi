using UnityEngine;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public class PlayerControlPlayable : PlayableBehaviour
    {
        private AnimGraphPlayer player;

        public void SetAnimGraphPlayer(AnimGraphPlayer player)
        {
            this.player = player;
        }

        public override void PrepareFrame(Playable playable, FrameData info)
        {
            //Debug.LogError($"{player.GetHashCode()} [PlayerControlPlayable] {Time.realtimeSinceStartup} {Time.frameCount}");
            player.Process();
            base.PrepareFrame(playable, info);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
        }
    }
}
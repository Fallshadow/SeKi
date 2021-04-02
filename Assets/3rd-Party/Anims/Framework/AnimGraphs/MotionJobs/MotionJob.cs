using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;

namespace Framework.AnimGraphs
{
    public abstract class MotionJob<T> where T : struct, IAnimationJob
    {
        protected T job;

        protected AnimationScriptPlayable playable;

        protected void createPlayable(AnimGraphPlayer graphPlayer)
        {
            playable = graphPlayer.InsertOutputJob(job);
        }

        public virtual void Destroy()
        {
            MotionJobUtil.RemovePlayable(playable);
        }
    }
}

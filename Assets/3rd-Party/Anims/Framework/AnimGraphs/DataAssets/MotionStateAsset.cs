using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Framework.AnimGraphs
{
    public unsafe struct MotionStateAsset 
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public int layerIndex;
            public int stateName;
            public bool writeDefaultValues;
            public bool iKOnFeet;
            public int transitionOffset;
            public int anyTransitionOffset;
            public int motionOffset;
            public int fadeInLayerTtransOffset;
            public int fadeOutLayerTtransOffset;
            public float speed;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public bool isNull => data == null;

        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        public MotionStateAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public int layerIndex => data->layerIndex;
        public int stateName => data->stateName;
        public bool writeDefaultValues => data->writeDefaultValues;
        public bool iKOnFeet => data->iKOnFeet;
        public NArray<TransitionAsset> transitions => data->transitionOffset == -1 ? default : new NArray<TransitionAsset>(dynamicPtr + data->transitionOffset, dynamicPtr, TransitionAsset.SizeOf);
        public TransitionAsset anyTransition => data->anyTransitionOffset == -1 ? default : new TransitionAsset(dynamicPtr + data->anyTransitionOffset, dynamicPtr);
        public MotionAsset motion => data->motionOffset == -1 ? default : new MotionAsset(data->motionOffset + dynamicPtr, dynamicPtr);
        public LayerTransitionAsset fadeInLayerTtransition => data->fadeInLayerTtransOffset == -1 ? default : new LayerTransitionAsset(data->fadeInLayerTtransOffset + dynamicPtr, dynamicPtr);
        public LayerTransitionAsset fadeOutLayerTtransition => data->fadeOutLayerTtransOffset == -1 ? default : new LayerTransitionAsset(data->fadeOutLayerTtransOffset + dynamicPtr, dynamicPtr);
        
        public float speed => data->speed;
    }
}

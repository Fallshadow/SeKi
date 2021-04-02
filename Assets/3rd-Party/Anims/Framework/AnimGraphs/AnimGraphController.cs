using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.AnimGraphs
{
    public unsafe struct AnimGraphController
    {
        public bool isNull => nodeIdMap == null;

        private AnimGraphAssetIDMap* nodeIdMap;
        private NArray<ParameterAsset> parameters;
        private NArray<LayerAsset> layers;

        private byte* dynamicPtr;
        private byte* root;

        public AnimGraphController(byte* ptr)
        {
            if(ptr == null)
            {
                Debug.LogError($"AnimGraphController初始化失败");
                nodeIdMap = null;
                parameters = default;
                layers = default;
                dynamicPtr = null;
                root = null;
                return;
            }
            root = ptr;
            AnimGraphHead* head = (AnimGraphHead*)ptr;
            dynamicPtr = head->dynamicOffset + ptr;
            nodeIdMap = (AnimGraphAssetIDMap*)(head->nodeMappingOffset + ptr);
            parameters = new NArray<ParameterAsset>(ptr + head->parametersOffset, dynamicPtr, ParameterAsset.SizeOf);
            layers = new NArray<LayerAsset>(ptr + head->layersOffset, dynamicPtr, LayerAsset.SizeOf);
        }

        public NArray<ParameterAsset> GetParametersAsset()
        {
            return parameters;
        }

        public NArray<LayerAsset> GetLayerAssets()
        {
            return layers;
        }

        public NodeAsset GetNodeAsset(int stateName)
        {
            var offset = nodeIdMap->Find(stateName);
            if (offset == -1)
                return default;
            return new NodeAsset(offset + root, dynamicPtr);
        }
    }
}
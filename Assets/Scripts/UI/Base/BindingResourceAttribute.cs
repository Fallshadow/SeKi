using System;
using act.UIRes;

namespace ASeKi.ui
{
    public class BindingResourceAttribute : Attribute
    {
        public UiAssetIndex AssetId { get; private set; }

        public BindingResourceAttribute(UiAssetIndex assetId)
        {
            AssetId = assetId;
        }
    }
}

using System;

namespace ASeKi.ui
{
    public class BindingResourceAttribute : Attribute
    {
        public string AssetId { get; private set; }

        public BindingResourceAttribute(string assetId)
        {
            AssetId = assetId;
        }
    }
}

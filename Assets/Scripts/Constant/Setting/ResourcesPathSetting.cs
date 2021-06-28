namespace constants
{
    // 资源路径常量
    public static class ResourcesPathSetting
    {
        public const string ASSET_FOLDER = "Assets/";
        public const string RESOURCES_FOLDER = "ResourceSeki/";
        public const string RESOURCES_PC_FOLDER = "ResourceSeki_PC/";
        public const string RESOURCES_ANDROID_FOLDER = "ResourceSeki_Android/";
        public const string RESOURCES_IOS_FOLDER = "ResourceSeki_IOS/";
        
        #region Resources Path

        #region SO Path
        
        public const string SO_PATTERN = "*.asset";
        public const string SO_FOLDER = "ScriptableObject/";
        public const string SO_ASSET_PATH = ASSET_FOLDER + RESOURCES_FOLDER + SO_FOLDER;
        public const string SO_EXT = ".asset";
        public const string CC_FREE_LOOK_CAMERA_POINT_CONFIG_SO_PATH = SO_ASSET_PATH + "CCFreeLookCameraPointConfigSO" + SO_EXT;
        
        #endregion
        
        #endregion

        // Texture
        public const string DefaultSpriteUiTexture = "PicMiss";

    }
}
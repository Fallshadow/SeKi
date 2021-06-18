namespace act.AssetBundleCore
{
    public interface IAsyncAssetProxy
    {
        int GetHash();
        int GetABHash();
        void Release();
        int GetTypeHash();
        IAsyncAssetHandler GetHandler();
        void Addreference();
        void DefReference();
        int GetReference();
        bool IsFinish();
        void Destroy();
    }
}
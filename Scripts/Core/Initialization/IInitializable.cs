namespace PixelEngine.Core.Initialization
{
    public interface IInitializable
    {
        public void EarlyInitialize() { }
        public void InitializeAsNew() { }
        public void InitializeAsLoaded() { }
        public void LateInitialize() { }
        public void Uninitialize() { }
    }
}
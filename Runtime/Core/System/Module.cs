namespace GiraffeStar
{
    public abstract class Module : IModule
    {
        protected virtual void OnRegister() {}
        protected virtual void OnUnregister() {}
        protected virtual void Update() { }
        protected virtual void LateUpdate() { }
        protected virtual void FixedUpdate() { }
    }
}
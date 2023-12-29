namespace RockEngine.Common.Utils
{
    public class EngineEmptyWindow
    {

        public bool Exists;
        public event Action? OnRender;
        public event Action? OnUpdate;
        public event Action? OnClosing;

        public unsafe EngineEmptyWindow()
        {
        }

        public void Render()
        {
            OnRender?.Invoke();
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }
        public void Closing()
        {
            OnClosing?.Invoke();
        }
    }
}

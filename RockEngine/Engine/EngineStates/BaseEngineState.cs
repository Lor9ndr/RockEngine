namespace RockEngine.Engine.EngineStates
{
    public abstract class BaseEngineState
    {
        public abstract string Key { get; }

        public abstract void OnEnterState();
        public abstract void OnExitState();
        public abstract void OnUpdateState();
    }
}

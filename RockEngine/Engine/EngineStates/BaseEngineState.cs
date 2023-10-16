namespace RockEngine.Engine.EngineStates
{
    internal abstract class BaseEngineState
    {
        internal abstract string Key { get; }

        public abstract void OnEnterState();
        public abstract void OnExitState();
        public abstract void OnUpdateState();


    }
}

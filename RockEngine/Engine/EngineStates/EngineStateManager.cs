using RockEngine.Utils;

namespace RockEngine.Engine.EngineStates
{
    internal static class EngineStateManager
    {
        private static BaseEngineState? _baseEngineState;
        public static Dictionary<string, BaseEngineState> _states = new Dictionary<string, BaseEngineState>();

        public static void RegisterStates(params BaseEngineState[] states)
        {
            foreach (var item in states)
            {
                _states.Add(item.Key, item);
            }
            SetNextState(_states.Keys.First());
        }

        public static string GetCurrentStateKey() => _baseEngineState.Key;

        public static void SetNextState(string key)
        {
            Logger.AddWarn($"Changing state to:{key}");
            _baseEngineState?.OnExitState();
            _baseEngineState = _states[key];
            _baseEngineState.OnEnterState();
        }

        public static void UpdateState()
        {
            _baseEngineState?.OnUpdateState();
        }
    }
}

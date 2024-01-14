using RockEngine.Common.Utils;
using RockEngine.ECS;

namespace RockEngine.Engine.EngineStates
{
    public static class EngineStateManager
    {
        private static BaseEngineState? _baseEngineState;
        public static Dictionary<string, BaseEngineState> _states = new Dictionary<string, BaseEngineState>();
        private static Memento _memento = new Memento();
        private static readonly Stack<Memento> _undoStack = new Stack<Memento>();
        private static readonly Stack<Memento> _redoStack = new Stack<Memento>();

        public static void RegisterStates(params BaseEngineState[ ] states)
        {
            foreach(var item in states)
            {
                _states.Add(item.Key, item);
            }
            SetNextState(states[0].Key);
        }

        public static string GetCurrentStateKey() => _baseEngineState.Key;

        public static void SetNextState(string key)
        {
            Logger.AddWarn($"Changing state to:{key}");

            if(_baseEngineState is DevepolerEngineState)
            {
                _memento.SaveState(Scene.CurrentScene);
                _undoStack.Push(_memento);
                _redoStack.Clear();
            }
            else if(_baseEngineState is PlayEngineState)
            {
                _memento.RestoreState(Scene.CurrentScene);
            }

            _baseEngineState?.OnExitState();
            _baseEngineState = _states[key];
            _baseEngineState.OnEnterState();

        }

        public static void UpdateState()
        {
            _baseEngineState?.OnUpdateState();
        }

        public static void Undo()
        {
            if(_undoStack.Count > 0)
            {
                var currentState = new Memento();
                currentState.SaveState(Scene.CurrentScene);
                _redoStack.Push(currentState);

                _memento = _undoStack.Pop();
                _memento.RestoreState(Scene.CurrentScene);
            }
        }

        public static void Redo()
        {
            if(_redoStack.Count > 0)
            {
                var currentState = new Memento();
                currentState.SaveState(Scene.CurrentScene);
                _undoStack.Push(currentState);

                _memento = _redoStack.Pop();
                _memento.RestoreState(Scene.CurrentScene);
            }
        }
    }
}
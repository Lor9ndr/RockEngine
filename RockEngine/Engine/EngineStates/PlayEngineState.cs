namespace RockEngine.Engine.EngineStates
{
    internal sealed class PlayEngineState : BaseEngineState
    {
        internal override string Key => "play";

        public override void OnEnterState()
        {
            foreach (var item in Scene.CurrentScene.GetGameObjects())
            {
                item.OnStart();
            }
        }

        public override void OnExitState()
        {
            foreach (var go in Scene.CurrentScene.GetGameObjects())
            {
                foreach (var component in go.GetComponents())
                {
                    component.OnDestroy();
                }
            }
        }

        public override void OnUpdateState()
        {
            foreach (var item in Scene.CurrentScene.GetGameObjects())
            {
                item.Update();
            }
        }
    }
}

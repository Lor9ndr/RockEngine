using RockEngine.Engine;
using RockEngine.Engine.ECS;

namespace RockEngine.Engine.EngineStates
{
    internal sealed class DevepolerEngineState : BaseEngineState
    {
        internal override string Key => "dev";

        public override void OnEnterState()
        {
            foreach (var gameObject in Scene.CurrentScene?.GetGameObjects())
            {
                foreach (var component in gameObject.GetComponents())
                {
                    if (component is EngineRigidBody)
                    {
                        continue;
                    }
                    component.OnStart();

                }
            }
        }

        public override void OnExitState()
        {
            foreach (var go in Scene.CurrentScene?.GetGameObjects())
            {
                foreach (var component in go.GetComponents())
                {
                    component.OnDestroy();
                }
            }
        }

        public override void OnUpdateState()
        {
            foreach (var item in Scene.CurrentScene?.GetGameObjects())
            {
                item.Update();
            }
        }
    }
}

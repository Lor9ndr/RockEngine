using BulletSharp;
using BulletSharp.Math;

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
                    if(component is EngineRigidBody rb)
                    {
                        rb.ForceActivationState(ActivationState.DisableSimulation);
                        rb.SetMassProps(0, new Vector3(0));
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
                item.UpdateOnDevelpmentState();
            }
        }
    }
}

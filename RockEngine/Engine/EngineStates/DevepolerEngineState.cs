using BulletSharp;
using BulletSharp.Math;

using RockEngine.Engine.ECS;

namespace RockEngine.Engine.EngineStates
{
    public sealed class DevepolerEngineState : BaseEngineState
    {
        public override string Key => "dev";

        public override void OnEnterState()
        {
            foreach (var gameObject in Scene.CurrentScene?.GetGameObjects())
            {
                foreach (var component in gameObject.GetComponents())
                {
                    if(component is EngineRigidBody rb)
                    {
                        rb.CollisionShape.CalculateLocalInertia(0, out var inertia);
                        rb.AngularVelocity = new Vector3(0);
                        rb.LinearVelocity = new Vector3(0);
                        rb.SetMassProps(0, inertia);
                        rb.ForceActivationState(ActivationState.DisableSimulation);
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

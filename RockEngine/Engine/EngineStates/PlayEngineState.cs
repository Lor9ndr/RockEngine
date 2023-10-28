using BulletSharp;

using RockEngine.Engine.ECS;

namespace RockEngine.Engine.EngineStates
{
    public sealed class PlayEngineState : BaseEngineState
    {
        public override string Key => "play";

        public override void OnEnterState()
        {
            foreach (var item in Scene.CurrentScene.GetGameObjects())
            {
                foreach(var component in item.GetComponents())
                {
                    if(component is EngineRigidBody rb)
                    {
                        rb.ActivationState = ActivationState.ActiveTag;
                        rb.CollisionFlags &= ~CollisionFlags.None;
                        rb.ForceActivationState(ActivationState.ActiveTag);

                    }
                    component.OnStart();
                }
            }
        }

        public override void OnExitState()
        {
            foreach (var go in Scene.CurrentScene.GetGameObjects())
            {
                foreach (var component in go.GetComponents())
                {
                    if(component is EngineRigidBody rb)
                    { // Disable physics for the rigidbody
                        rb.ActivationState = ActivationState.DisableSimulation;
                        rb.CollisionFlags |= CollisionFlags.None;
                        rb.ForceActivationState(ActivationState.DisableSimulation);

                    }
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

using RockEngine.ECS;

namespace RockEngine.Engine.EngineStates
{
    public sealed class PlayEngineState : BaseEngineState
    {
        public override string Key => "play";

        public override void OnEnterState()
        {
            foreach(var item in Scene.CurrentScene.GetGameObjects())
            {
                foreach(var component in item.GetComponents())
                {
                    if(component is EngineRigidBody rb)
                    {
                        /* rb.ForceActivationState(ActivationState.ActiveTag);
                         rb.ForceActivationState(ActivationState.DisableDeactivation);
                         rb.ForceActivationState(ActivationState.ActiveTag);
                         rb.Activate(true);

                         rb.SetMassProps(rb.Mass, new BulletSharp.Math.Vector3(0));*/
                    }
                    component.OnStart();
                }
            }
        }

        public override void OnExitState()
        {
            foreach(var go in Scene.CurrentScene.GetGameObjects())
            {
                foreach(var component in go.GetComponents())
                {
                    component.OnDestroy();
                }
            }
        }

        public override void OnUpdateState()
        {
            foreach(var item in Scene.CurrentScene.GetGameObjects())
            {
                item.Update();
            }
        }
    }
}

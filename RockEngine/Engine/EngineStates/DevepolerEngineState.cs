﻿using RockEngine.ECS;

namespace RockEngine.Engine.EngineStates
{
    public sealed class DevepolerEngineState : BaseEngineState
    {
        public override string Key => "dev";

        public override void OnEnterState()
        {
            foreach(var gameObject in Scene.CurrentScene?.GetGameObjects())
            {
                var components = gameObject.GetComponents();
                foreach(var component in components)
                {
                    if(component is EngineRigidBody rb)
                    {
                        /*   rb.ForceActivationState(ActivationState.DisableSimulation);
                           rb.LinearVelocity = Vector3.Zero;
                           rb.AngularVelocity = Vector3.Zero;*/
                    }
                    component.OnStart();

                }
            }
        }

        public override void OnExitState()
        {
            foreach(var go in Scene.CurrentScene?.GetGameObjects())
            {
                foreach(var component in go.GetComponents())
                {
                    component.OnDestroy();
                }
            }
        }

        public override void OnUpdateState()
        {
            Scene.CurrentScene?.Update();
           
        }
    }
}

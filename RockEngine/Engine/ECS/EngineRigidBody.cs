using BulletSharp;

using RockEngine.Editor;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Engine.ECS
{
    public sealed class EngineRigidBody : RigidBody, IComponent
    {
        [UI]
        public float Mass;

        [UI]
        public ActivationState State { get=>this.ActivationState; set=> this.ActivationState = value; }

        public EngineRigidBody(RigidBodyConstructionInfo constructionInfo)
            : base(constructionInfo)
        {
            Mass = 1;
        }

        public GameObject? Parent { get; set; }

        public int Order => 0;

        public void OnDestroy()
        {
            
        }

        /// <summary>
        /// This function activates RigidBody on 
        /// </summary>
        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }

        public void OnUpdateDevelepmentState()
        {
        }

        public object GetState()
        {
            return new EngineRigidBodyState() { Mass = Mass };
        }

        public void SetState(object state)
        {
            var rb = (EngineRigidBodyState)state;
            Mass = rb.Mass;
        }
    }
}

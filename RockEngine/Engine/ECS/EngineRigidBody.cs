using BulletSharp;

using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Engine.ECS
{
    internal sealed class EngineRigidBody : RigidBody, IComponent
    {
        public EngineRigidBody(RigidBodyConstructionInfo constructionInfo)
            : base(constructionInfo)
        {
        }

        public GameObject? Parent { get; set; }

        public int Order => 0;

        public void OnDestroy()
        {
            ActivationState = ActivationState.DisableSimulation;
        }

        /// <summary>
        /// This function activates RigidBody on 
        /// </summary>
        public void OnStart()
        {
            ActivationState = ActivationState.ActiveTag;
            Activate();

        }

        public void OnUpdate()
        {
        }
    }
}

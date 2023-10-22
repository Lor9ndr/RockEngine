using BulletSharp;

using RockEngine.Editor;
using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Engine.ECS
{
    internal sealed class EngineRigidBody : RigidBody, IComponent
    {

        [UI]
        public float Mass;

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
    }
}

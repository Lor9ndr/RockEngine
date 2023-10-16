using BulletSharp;
using BulletSharp.Math;

using RockEngine.Engine.ECS.GameObjects;

namespace RockEngine.Engine.ECS
{
    internal sealed class BoxCollider : BoxShape, IComponent
    {
        public BoxCollider(Vector3 boxHalfExtents)
            : base(boxHalfExtents)
        {
        }

        public GameObject? Parent { get; set; }

        public int Order => 0;

        public void OnDestroy()
        {
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
        }
    }
}

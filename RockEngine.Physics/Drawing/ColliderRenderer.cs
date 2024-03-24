using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Physics.Colliders;

namespace RockEngine.Physics.Drawing
{
    public abstract class ColliderRenderer
    {
        public abstract void DrawLine(Vector3 startVertex, Vector3 endVertex, Vector4 color);

        public abstract void DrawAABB(AABB aabb, Vector4 color);
        public abstract void DrawOBB(OBB obb, Vector4 color);
        public abstract void Render();
        public abstract void Update();
    }
}

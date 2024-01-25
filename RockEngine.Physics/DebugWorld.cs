using OpenTK.Mathematics;

using RockEngine.Physics.Colliders;

namespace RockEngine.Physics
{
    public sealed class DebugWorld
    {
        public IWorldRenderer WorldRenderer;

        public DebugWorld(IWorldRenderer worldRenderer)
        {
            WorldRenderer = worldRenderer;
        }

        public void DrawCollider(Collider collider, Vector3 position, Vector3 color)
        {
            if(collider.IsConvex)
            {
                WorldRenderer.DrawConvex(collider.GetVertices(), position, color);
            }
            else
            {
                if(collider is BoxCollider bx)
                {
                    WorldRenderer.DrawCube(position, bx.Max - bx.Min, color);
                }
                else if(collider is SphereCollider sphere)
                {
                    WorldRenderer.DrawSphere(position, sphere.Radius, color);
                }
            }
        }

        public void DrawCollider(Collider collider, Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color)
        {
            if(collider.IsConvex)
            {
                WorldRenderer.DrawConvex(collider.GetVertices(), position, scale, rotation, color);
            }
            else
            {
                if(collider is BoxCollider bx)
                {
                    WorldRenderer.DrawCube(position, bx.Max - bx.Min, rotation, color);
                }
                else if(collider is SphereCollider sphere)
                {
                    WorldRenderer.DrawSphere(position, sphere.Radius, color);
                }
            }
        }

        public void DrawCollider(Collider collider, Vector3 position, Quaternion rotation, Vector3 color)
        {
            if(collider.IsConvex)
            {
                WorldRenderer.DrawConvex(collider.GetVertices(), position, Vector3.One, rotation, color);
            }
            else
            {
                if(collider is BoxCollider bx)
                {
                    WorldRenderer.DrawCube(position, bx.Max - bx.Min, rotation, color);
                }
                else if(collider is SphereCollider sphere)
                {
                    WorldRenderer.DrawSphere(position, new Vector3(sphere.Radius), rotation, color);
                }
            }
        }

        public void DrawCollider(Collider collider, Vector3 position, Quaternion rotation)
        {
            var color = collider.WasCollided ? Vector3.UnitX:Vector3.UnitY;
            if(collider.IsConvex)
            {
                WorldRenderer.DrawConvex(collider.GetVertices(), position, Vector3.One, rotation, color);
            }
            else
            {
                if(collider is BoxCollider bx)
                {
                    WorldRenderer.DrawCube(position, bx.Max - bx.Min, rotation, color);
                }
                else if(collider is SphereCollider sphere)
                {
                    WorldRenderer.DrawSphere(position, new Vector3(sphere.Radius), rotation, color);
                }
            }
        }

        public void Update()
        {
            WorldRenderer.Update();
        }

        public void Render()
        {
            WorldRenderer.Render();
        }
    }
}

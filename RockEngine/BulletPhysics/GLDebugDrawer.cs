using BulletSharp;
using BulletSharp.Math;

using RockEngine.DI;
using RockEngine.OpenGL.Vertices;
using RockEngine.Utils;

using OpenMath = OpenTK.Mathematics;

namespace RockEngine.BulletPhysics
{
    internal sealed class GLDebugDrawer : DebugDraw
    {
        public override DebugDrawModes DebugMode { get; set; }
        public DebugRenderer RenderObjectsManager { get; set; }

        public GLDebugDrawer(DebugRenderer renderer)
        {
            RenderObjectsManager = renderer;
        }

        private void RenderLine(Vector3 from, Vector3 to, Vector3 color)
        {
            var colord = color;

            RenderObjectsManager.DebugRenderVertices.AddRange(new Vertex3D[ ]
            {
                new Vertex3D((OpenMath.Vector3)from, (OpenMath.Vector3)color),
                new Vertex3D((OpenMath.Vector3)to, (OpenMath.Vector3)color)
            });

        }

        public override void DrawLine(ref Vector3 from, ref Vector3 to, ref Vector3 color)
            => RenderLine(from, to, color);

        public override void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 color, double alpha)
        {
            DrawTriangle(v0, v1, v2, color);
        }

        public void DrawTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 color)
        {

            RenderObjectsManager.DebugRenderVertices.AddRange(new Vertex3D[ ]
            {
                new Vertex3D((OpenMath.Vector3)v0,(OpenMath.Vector3)color),
                new Vertex3D((OpenMath.Vector3)v1,(OpenMath.Vector3)color),
                new Vertex3D((OpenMath.Vector3)v2,(OpenMath.Vector3)color),
            });
        }

        public override void Draw3DText(ref Vector3 location, string textString)
        {
            throw new NotImplementedException();
        }

        public override void ReportErrorWarning(string warningString)
        {
            Logger.AddError(warningString);
        }
    }
}

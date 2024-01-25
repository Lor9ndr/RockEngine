using OpenTK.Mathematics;

using System.Drawing;

namespace RockEngine.Physics
{
    public abstract class BaseWorldRenderer : IWorldRenderer
    {
        public virtual void DrawConvex(Vector3[] vertices, Vector3 position, Vector3 color)
        {
            for(int i = 0; i < vertices.Length - 1; i++)
            {
                DrawLine(vertices[i] + position, vertices[i+1] + position, color);
            }
        }

        public virtual void DrawConvex(Vector3[] vertices, Matrix4 transform, Vector3 color)
        {
            var position = transform.ExtractTranslation();
            var rotation = transform.ExtractRotation();
            var scale = transform.ExtractScale();
            vertices = vertices.Select(s=> Vector3.Transform(s, rotation) * scale + position).ToArray();
            DrawConvex(vertices, color);
        }

        public virtual void DrawConvex(Vector3[] vertices, Vector3 color)
        {
            for(int i = 0; i < vertices.Length - 1; i++)
            {
                DrawLine(vertices[i], vertices[i + 1], color);
            }
        }

        public void DrawConvex(Vector3[] vertices, Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color)
        {
            DrawConvex(vertices.Select(s => Vector3.Transform(s, rotation) * scale + position).ToArray(), color);
        }

        public virtual void DrawCube(Vector3 center, Vector3 size, Vector3 color)
        {
            // Calculate half size for each dimension
            Vector3 halfSize = size / 2;

            // Calculate the positions of the 8 corners of the cube
            Span<Vector3> corners = stackalloc Vector3[8];
            corners[0] = center + new Vector3(-halfSize.X, -halfSize.Y, -halfSize.Z);
            corners[1] = center + new Vector3(halfSize.X, -halfSize.Y, -halfSize.Z);
            corners[2] = center + new Vector3(halfSize.X, halfSize.Y, -halfSize.Z);
            corners[3] = center + new Vector3(-halfSize.X, halfSize.Y, -halfSize.Z);
            corners[4] = center + new Vector3(-halfSize.X, -halfSize.Y, halfSize.Z);
            corners[5] = center + new Vector3(halfSize.X, -halfSize.Y, halfSize.Z);
            corners[6] = center + new Vector3(halfSize.X, halfSize.Y, halfSize.Z);
            corners[7] = center + new Vector3(-halfSize.X, halfSize.Y, halfSize.Z);

            // Draw the 12 edges of the cube
            DrawLine(corners[0], corners[1],color);
            DrawLine(corners[1], corners[2],color);
            DrawLine(corners[2], corners[3],color);
            DrawLine(corners[3], corners[0],color);
            DrawLine(corners[4], corners[5],color);
            DrawLine(corners[5], corners[6],color);
            DrawLine(corners[6], corners[7],color);
            DrawLine(corners[7], corners[4],color);
            DrawLine(corners[0], corners[4],color);
            DrawLine(corners[1], corners[5],color);
            DrawLine(corners[2], corners[6],color);
            DrawLine(corners[3], corners[7],color);
        }

        public virtual void DrawCube(Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color)
        {
            Vector3 halfSize = scale / 2;

            // Calculate the positions of the 8 corners of the cube
            Span<Vector3> corners = stackalloc Vector3[8];
            corners[0] = position + Vector3.Transform(new Vector3(-halfSize.X, -halfSize.Y, -halfSize.Z),rotation);;
            corners[1] = position + Vector3.Transform(new Vector3(halfSize.X, -halfSize.Y, -halfSize.Z),rotation);
            corners[2] = position + Vector3.Transform(new Vector3(halfSize.X, halfSize.Y, -halfSize.Z),rotation);
            corners[3] = position + Vector3.Transform(new Vector3(-halfSize.X, halfSize.Y, -halfSize.Z),rotation);
            corners[4] = position + Vector3.Transform(new Vector3(-halfSize.X, -halfSize.Y, halfSize.Z),rotation);
            corners[5] = position + Vector3.Transform(new Vector3(halfSize.X, -halfSize.Y, halfSize.Z),rotation);
            corners[6] = position + Vector3.Transform(new Vector3(halfSize.X, halfSize.Y, halfSize.Z),rotation);
            corners[7] = position + Vector3.Transform(new Vector3(-halfSize.X, halfSize.Y, halfSize.Z),rotation);
            DrawLine(corners[0], corners[1], color);
            DrawLine(corners[1], corners[2], color);
            DrawLine(corners[2], corners[3], color);
            DrawLine(corners[3], corners[0], color);
            DrawLine(corners[4], corners[5], color);
            DrawLine(corners[5], corners[6], color);
            DrawLine(corners[6], corners[7], color);
            DrawLine(corners[7], corners[4], color);
            DrawLine(corners[0], corners[4], color);
            DrawLine(corners[1], corners[5], color);
            DrawLine(corners[2], corners[6], color);
            DrawLine(corners[3], corners[7], color);
        }

        public abstract void DrawLine(Vector3 start, Vector3 endPoint, Vector3 color);

        public virtual void DrawSphere(Vector3 center, float radius, Vector3 color)
        {
            // Draw latitude lines
            float latitudeLines = 10000;
            float longitudeLines = 10000;
            for(int i = 0; i <= latitudeLines; i++)
            {
                float theta = i * MathF.PI / latitudeLines;
                float y = radius * (float)Math.Cos(theta);
                float r = radius * (float)Math.Sin(theta);

                Vector3 previousPoint = new Vector3(r, y, 0) + center;
                for(int j = 1; j <= longitudeLines; j++)
                {
                    float phi = j * 2 * MathF.PI / longitudeLines;
                    Vector3 point = new Vector3(r * (float)Math.Cos(phi), y, r * (float)Math.Sin(phi)) + center;
                    DrawLine(previousPoint, point, color);
                    previousPoint = point;
                }
            }

            // Draw longitude lines
            for(int i = 0; i < longitudeLines; i++)
            {
                float phi = i * 2 * MathF.PI / longitudeLines;
                Vector3 previousPoint = new Vector3(radius * (float)Math.Cos(phi), radius, radius * (float)Math.Sin(phi)) + center;
                for(int j = 1; j <= latitudeLines; j++)
                {
                    float theta = j * MathF.PI / latitudeLines;
                    float y = radius * (float)Math.Cos(theta);
                    float r = radius * (float)Math.Sin(theta);
                    Vector3 point = new Vector3(r * (float)Math.Cos(phi), y, r * (float)Math.Sin(phi)) + center;
                    DrawLine(previousPoint, point, color);
                    previousPoint = point;
                }
            }
        }

        public void DrawSphere(Vector3 position, Vector3 scale, Quaternion rotation, Vector3 color)
        {
        }

        public abstract void Render();
       

        public abstract void Update();
      
    }
}

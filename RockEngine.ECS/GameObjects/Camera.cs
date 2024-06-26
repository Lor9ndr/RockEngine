﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using RockEngine.Common;
using RockEngine.Common.Editor;
using RockEngine.Common.Utils;
using RockEngine.ECS.Figures;
using RockEngine.Rendering;
using RockEngine.Rendering.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.ECS.GameObjects
{
    public class Camera : IComponent, IRenderable
    {
        protected CameraData _cameraData;
        public const int MAX_FOV = 120;

        protected float _fov = MathHelper.PiOver2;

        // Rotation around the X axis (radians)
        protected float _pitch = -MathHelper.PiOver2;

        protected Vector3 _right = Vector3.UnitX;

        protected Vector3 _up = Vector3.UnitY;
        protected float _yaw = -MathHelper.PiOver2; // Without this you would be started rotated 90 degrees right

        private Vector3 _front;
        public float AspectRatio { get; set; }
        public float Far { get; set; } = 10000.0f;

        // The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        // previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance
        [UI]
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 45f, MAX_FOV);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }
        public float Near { get; set; } = 0.1f;

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public Vector3 Right => _right;

        public Vector3 Up => _up;

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public Vector3 Front
        {
            get => _front;
            set
            {
                _front = value;
                UpdateVectors();
            }
        }

        public GameObject Parent { get; set; }

        public int Order => 0;

        public Camera(float aspectRatio)
        {
            AspectRatio = aspectRatio;
            UpdateVectors();
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, Near, Far);

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Parent!.Transform.Position, Parent.Transform.Position + Front, _up);

        public void Render(IRenderingContext context)
        {
            _cameraData.SendData(context);
        }

        public void UpdateVectors()
        {
            // First the front matrix is calculated using some basic trigonometry
            _front = new Vector3(MathF.Cos(_pitch) * MathF.Cos(_yaw), MathF.Sin(_pitch), MathF.Cos(_pitch) * MathF.Sin(_yaw));

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            _front = Vector3.Normalize(Front);

            // Calculate both the right and the up vector using cross product
            // Note that we are calculating the right from the global up, this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera
            _right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, Front));
        }

        public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
            Parent!.Transform.Position = eye;
            _front = Vector3.Normalize(target - eye);
            _up = up;
        }

        public void LookAt(GameObject gameObject)
        {
            LookAt(Parent.Transform.Position, gameObject.Transform.Position, Vector3.UnitY);
        }

        public void MoveTowardsTarget(float distance)
        {
            Vector3 direction = _front; // Use the front vector calculated in the LookAt function
            Vector3 newPosition = Parent.Transform.Position + direction * distance;
            Parent.Transform.Position = newPosition;
        }

        public void MoveToTarget(float distance, Vector3 target)
        {
            Vector3 direction = Vector3.Normalize(target - Parent.Transform.Position);
            float currentDistance = Vector3.Distance(target, Parent.Transform.Position);

            distance = Math.Min(distance, currentDistance - distance);
            Vector3 newPosition = Parent.Transform.Position + direction * distance;
            Parent.Transform.Position = newPosition;
        }

        public void OnStart()
        {
            _cameraData = new CameraData();
        }

        public virtual void OnUpdate()
        {
            _cameraData.Projection = GetProjectionMatrix();
            _cameraData.View = GetViewMatrix();
            _cameraData.ViewPos = Parent.Transform.Position;
        }

        public Ray ScreenPointToRay(Vector2 mousePosition)
        {
            // Get the current viewport dimensions
            int[ ] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            // Create a normalized device coordinate from the screen position
            Vector3 ndc = new Vector3(
                2.0f * mousePosition.X / viewport[2] - 1.0f,
                1.0f - 2.0f * mousePosition.Y / viewport[3],
                0.0f
            );

            // Create a view-space coordinate by unprojecting the normalized device coordinate
            Matrix4 viewMatrix = GetViewMatrix();
            Matrix4 projectionMatrix = GetProjectionMatrix();

            Matrix4 inverseProjection = Matrix4.Invert(projectionMatrix);
            Matrix4 inverseView = Matrix4.Invert(viewMatrix);

            Vector4 ndc4 = new Vector4(ndc.X, ndc.Y, ndc.Z, 1.0f); // Convert to Vector4

            Vector4 viewSpace = MathUtil.Transform(ndc4, inverseProjection);
            viewSpace /= viewSpace.W;
            viewSpace = MathUtil.Transform(viewSpace, inverseView);

            // Create a ray from the camera position to the view-space coordinate
            Vector3 rayOrigin = Parent.Transform.Position;
            Vector3 rayDirection = Vector3.Normalize(viewSpace.Xyz - rayOrigin);

            return new Ray(rayOrigin, rayDirection);
        }

        public void OnDestroy()
        {
        }

        public dynamic GetState()
        {
            return new
            {
                _cameraData = _cameraData,
                _fov = _fov,
                _front = _front,
                _pitch = _pitch,
                _right = _right,
                _up = _up,
                _yaw = _yaw
            };
        }

        public void SetState(dynamic state)
        {
            _cameraData = state._cameraData;
            _fov = state._fov;
            _front = state._front;
            _pitch = state._pitch;
            _yaw = state._yaw;
            _right = state._right;
            _up = state._up;
        }
    }
}

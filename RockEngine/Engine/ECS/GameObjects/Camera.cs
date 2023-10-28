using OpenTK.Mathematics;

using RockEngine.OpenGL;
using RockEngine.OpenGL.Buffers.UBOBuffers;

using RockEngine.Editor;

namespace RockEngine.Engine.ECS.GameObjects
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

        public GameObject? Parent { get; set; }

        public int Order => 0;

        public Camera(float aspectRatio)
        {
            _cameraData = new CameraData();
            AspectRatio = aspectRatio;
            UpdateVectors();
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, Near, Far);

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Parent!.Transform.Position, Parent.Transform.Position + Front, _up);

        public void Render()
        {
            _cameraData.SendData();
            Parent.Transform.ShouldBeUpdated = false;
        }

        public virtual void RenderOnEditorLayer()
        {
            //Render();
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
        }

        public virtual void OnUpdate()
        {
            _cameraData.Projection = GetProjectionMatrix();
            _cameraData.View = GetViewMatrix();
            _cameraData.ViewPos = Parent.Transform.Position;
        }

        public void OnDestroy()
        {
        }

        public virtual void OnUpdateDevelepmentState()
        {
            _cameraData.Projection = GetProjectionMatrix();
            _cameraData.View = GetViewMatrix();
            _cameraData.ViewPos = Parent.Transform.Position;
        }
    }
}

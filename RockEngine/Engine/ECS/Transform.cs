using OpenTK.Mathematics;

using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL;

using RockEngine.Editor;
using RockEngine.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.Engine.ECS
{
    public class Transform : IComponent, IRenderable
    {

        public Quaternion RotationQuaternion;

        [UI] public Vector3 Position;

        [UI] public Vector3 Scale;

        [UI]
        public Vector3 Rotation
        {
            get => RotationQuaternion.ToEulerAngles();
            set => RotationQuaternion = Quaternion.FromEulerAngles(value);
        }

        public GameObject? Parent { get => _parent; set => _parent = value; }

        private Matrix4 _model;

        public int Order => 0;

        internal bool ShouldBeUpdated = true;

        private readonly HashSet<Transform> _childTransforms;
        private GameObject? _parent;

        public void AddChildTransform(Transform t)
        {
            _childTransforms.Add(t);
        }
        public void RemoveChildTransform(Transform t)
        {
            _childTransforms.Remove(t);
        }
        public void ClearChildrenTransforms()
        {
            _childTransforms.Clear();
        }

        public Transform()
        {
            _childTransforms = new HashSet<Transform>();
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
        }

        public Transform(Vector3 position, Vector3 rotation = default)
            : this()
        {
            Position = position;
            Rotation = rotation;
            Scale = new Vector3(1);
        }
        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
            : this(position, rotation)
        {
            Scale = scale;
        }

        public Matrix4 GetModelMatrix()
        {
            var t = Matrix4.CreateTranslation(Position);
            var r = Matrix4.CreateFromQuaternion(RotationQuaternion);
            var s = Matrix4.CreateScale(Scale);
            return r * s * t;
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.FromAxisAngle(axis, angle);
            Rotation = rotation * Rotation;
        }

        public void OnStart()
        {
        }

        public void OnUpdate()
        {
            _model = GetModelMatrix();
        }

        public void OnDestroy()
        {
        }

        public void Render()
        {
            new TransformData(_model).SendData();
        }

        public void RenderOnEditorLayer()
        {
            new TransformData(_model).SendData();
        }
    }
}

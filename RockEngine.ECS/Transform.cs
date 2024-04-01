
using OpenTK.Mathematics;

using RockEngine.Rendering;

namespace RockEngine.ECS
{
    public class Transform : IComponent, IRenderable
    {
        public Quaternion RotationQuaternion;

        public Vector3 Position;

        public Vector3 Scale;

        public Vector3 Rotation
        {
            get => RotationQuaternion.ToEulerAngles() * (180f / MathF.PI);
            set => RotationQuaternion = Quaternion.FromEulerAngles(value * (MathF.PI / 180f));
        }

        public GameObject Parent { get => _parent; set => _parent = value; }

        public int Order => 0;

        private List<Transform> _childTransforms;
        private GameObject _parent;
        private Matrix4 _modelMatrix;

        public Transform()
        {
            _childTransforms = new List<Transform>();
            Position = Vector3.Zero;
            RotationQuaternion = Quaternion.Identity;
            Scale = Vector3.One;
            _modelMatrix = Matrix4.Identity;
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

        public Matrix4 GetUpdatedModelMatrix()
        {
            Matrix4 model;
            var rb = Parent.GetComponent<EngineRigidBody>();
            if(rb is not null)
            {
                Position = rb.Position;
                RotationQuaternion = rb.Rotation;
            }
            var t = Matrix4.CreateTranslation(Position);
            var r = Matrix4.CreateFromQuaternion(RotationQuaternion);
            var s = Matrix4.CreateScale(Scale);

            model = s * r * t;

            return model;
        }

        public void Rotate(Vector3 axis, float angle)
        {
            Quaternion rotation = Quaternion.FromAxisAngle(axis, MathHelper.DegreesToRadians(angle));
            RotationQuaternion = rotation * RotationQuaternion;
        }

        public void OnStart()
        {
            var rb = Parent.GetComponent<EngineRigidBody>();
            if(rb is not null)
            {
                rb.Position = Position;
            }
            //_transformData = new TransformData();

        }

        public void OnUpdate()
        {
            _modelMatrix = GetUpdatedModelMatrix();
        }

        public void Render(IRenderingContext context)
        {
            //_transformData.SendData(context);
        }

        public Matrix4 GetModelMatrix() => _modelMatrix;

        public void AddChildTransform(Transform t)
            => _childTransforms.Add(t);

        public void RemoveChildTransform(Transform t)
            => _childTransforms.Remove(t);

        public void ClearChildrenTransforms()
            => _childTransforms.Clear();

        public List<Transform> GetChildTransforms()
            => _childTransforms;

        public void OnDestroy()
        {
        }

        public dynamic GetState()
        {
            return new
            {
                Position = Position,
                RotationQuaternion = RotationQuaternion,
                Scale = Scale,
                _childTransforms = _childTransforms,
                _parent = _parent,
            };
        }

        public void SetState(dynamic state)
        {
            Position = state.Position;
            Scale = state.Scale;
            RotationQuaternion = state.RotationQuaternion;
            Scale = state.Scale;
            _childTransforms = state._childTransforms;
            _parent = state._parent;
        }
    }
}

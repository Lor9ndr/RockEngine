﻿using OpenTK.Mathematics;

using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL;

using RockEngine.Editor;
using RockEngine.OpenGL.Buffers.UBOBuffers;
using BulletSharp;

namespace RockEngine.Engine.ECS
{
    public class Transform : IComponent, IRenderable
    {
        private TransformData _transformData;

        public Quaternion RotationQuaternion;

        [UI] public Vector3 Position;

        [UI] public Vector3 Scale;

        [UI]
        public Vector3 Rotation
        {
            get => RotationQuaternion.ToEulerAngles() * (180f / MathF.PI);
            set => RotationQuaternion = Quaternion.FromEulerAngles(value * (MathF.PI / 180f));
        }

        public GameObject? Parent { get => _parent; set => _parent = value; }

        public int Order => 0;

        internal bool ShouldBeUpdated = true;

        private readonly HashSet<Transform> _childTransforms;
        private GameObject? _parent;

        public void AddChildTransform(Transform t) 
            => _childTransforms.Add(t);

        public void RemoveChildTransform(Transform t) 
            => _childTransforms.Remove(t);

        public void ClearChildrenTransforms() 
            => _childTransforms.Clear();

        public HashSet<Transform> GetChildTransforms() 
            => _childTransforms;

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
            var rb = Parent.GetComponent<EngineRigidBody>();
            var model = s * r * t;
            if(rb is not null && rb.ActivationState != ActivationState.DisableSimulation)
            {
                model *= ((Matrix4)rb.WorldTransform).ClearScale();
            }

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
            if(rb is not null && rb.ActivationState != ActivationState.DisableSimulation)
            {
                rb.WorldTransform = _transformData.Model;
            }
        }

        public void OnUpdate()
        {
            _transformData = new TransformData();

            _transformData.Model = GetModelMatrix();
        }

        public void OnDestroy()
        {
        }

        public void Render()
        {
            _transformData.SendData();
        }

        public void RenderOnEditorLayer()
        {
            _transformData.SendData();
        }

        public void OnUpdateDevelepmentState()
        {
            _transformData = new TransformData();

            _transformData.Model = GetModelMatrix();
        }
    }
}

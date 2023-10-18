using OpenTK.Mathematics;

using RockEngine.Engine.ECS;

namespace RockEngine.Tests
{
    [TestFixture]
    public class TransformTests
    {
        private Transform _transform;

        [SetUp]
        public void Setup()
        {
            _transform = new Transform();
        }

        [Test]
        public void AddChildTransform_ShouldAddChildTransform()
        {
            // Arrange
            var childTransform = new Transform();

            // Act
            _transform.AddChildTransform(childTransform);

            // Assert
            CollectionAssert.Contains(_transform.GetChildTransforms(), childTransform);
        }

        [Test]
        public void RemoveChildTransform_ShouldRemoveChildTransform()
        {
            // Arrange
            var childTransform = new Transform();
            _transform.AddChildTransform(childTransform);

            // Act
            _transform.RemoveChildTransform(childTransform);

            // Assert
            CollectionAssert.DoesNotContain(_transform.GetChildTransforms(), childTransform);
        }

        [Test]
        public void ClearChildrenTransforms_ShouldClearChildrenTransforms()
        {
            // Arrange
            var childTransform1 = new Transform();
            var childTransform2 = new Transform();
            _transform.AddChildTransform(childTransform1);
            _transform.AddChildTransform(childTransform2);

            // Act
            _transform.ClearChildrenTransforms();

            // Assert
            CollectionAssert.IsEmpty(_transform.GetChildTransforms());
        }
        [Test]
        public void GetModelMatrix_ShouldReturnCorrectModelMatrix()
        {
            // Arrange
            _transform.Position = new Vector3(1, 2, 3);
            _transform.Rotation = new Vector3(45, 0, 0);
            _transform.Scale = new Vector3(2, 2, 2);

            // Calculate the expected modelMatrix
            var translationMatrix = Matrix4.CreateTranslation(_transform.Position);
            var rotationMatrix = Matrix4.CreateFromQuaternion(_transform.RotationQuaternion);
            var scaleMatrix = Matrix4.CreateScale(_transform.Scale);

            var expectedModelMatrix = scaleMatrix * rotationMatrix * translationMatrix;

            // Act
            var modelMatrix = _transform.GetModelMatrix();

            // Assert
            Assert.That(modelMatrix, Is.EqualTo(expectedModelMatrix));
        }
    }
}

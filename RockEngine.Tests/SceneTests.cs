using RockEngine.ECS;

namespace RockEngine.Tests
{
    [TestFixture]
    public class SceneTests
    {
        [Test]
        public void AddGameObject_ShouldAddGameObjectToScene()
        {
            // Arrange
            var scene = new Scene("TestScene", "test/path", Guid.NewGuid());
            var gameObject = new GameObject();

            // Act
            scene.Add(gameObject);

            // Assert
            Assert.That(scene.GetGameObjects(), Does.Contain(gameObject));
        }

        [Test]
        public void AddChild_ShouldAddChildToParentGameObject()
        {
            // Arrange
            var scene = new Scene("TestScene", "test/path", Guid.NewGuid());
            var parent = new GameObject();
            var child = new GameObject();

            // Act
            scene.AddChild(parent, child);

            // Assert
            Assert.That(parent.Children, Does.Contain(child));
        }

        [Test]
        public void RemoveChild_ShouldRemoveChildFromParentGameObject()
        {
            // Arrange
            var scene = new Scene("TestScene", "test/path", Guid.NewGuid());
            var parent = new GameObject();
            var child = new GameObject();
            scene.AddChild(parent, child);

            // Act
            scene.RemoveChild(parent, child);

            // Assert
            Assert.That(parent.Children, Does.Not.Contain(child));
        }
    }
}

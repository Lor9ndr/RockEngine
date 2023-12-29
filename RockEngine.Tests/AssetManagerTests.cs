using RockEngine.ECS;
using RockEngine.Common.Vertices;
using RockEngine.Rendering.OpenGL.Settings;
using RockEngine.Rendering.OpenGL.Shaders;
using RockEngine.ECS.Assets;

namespace RockEngine.Tests
{
    [TestFixture]
    public class AssetManagerTests
    {
        private const string TestAssetsPath = "TestAssets";

        [SetUp]
        public void SetUp()
        {
            // Create a temporary directory for test assets
            Directory.CreateDirectory(TestAssetsPath);
            var scene = AssetManager.CreateScene("TestScene", TestAssetsPath, Guid.Empty);
            AssetManager.CreateProject("TestProject", TestAssetsPath, Guid.Empty, scene);
        }

        [TearDown]
        public void TearDown()
        {
            // Delete the temporary directory and its contents
            Directory.Delete(TestAssetsPath, true);
        }

        [Test]
        public void CreateMaterialAsset_ShouldCreateAndSaveMaterialAsset()
        {
            // Arrange
            var expectedPath = Path.Combine(TestAssetsPath, "Material.asset");

            // Act
            var asset = AssetManager.CreateMaterialAsset(TestAssetsPath);

            // Assert
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset, Is.InstanceOf<Material>());
            Assert.That(asset.Name, Is.EqualTo("Material"));
            Assert.That(asset.Path, Is.EqualTo(TestAssetsPath));
            Assert.That(asset.ID, Is.Not.EqualTo(default(Guid)));
            Assert.That(File.Exists(expectedPath), Is.True);
        }

        [Test]
        public void CreateMesh_ShouldCreateAndSaveMeshAsset()
        {
            // Arrange
            var indices = new int[ ] { 0, 1, 2 };
            var vertices = new Vertex3D[ ] { new Vertex3D(), new Vertex3D(), new Vertex3D() };
            var expectedPath = Path.Combine(TestAssetsPath, "Mesh.asset");

            // Act
            var asset = AssetManager.CreateMesh(ref indices, ref vertices, "Mesh", TestAssetsPath);

            // Assert
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset, Is.InstanceOf<Mesh>());
            Assert.That(asset.Name, Is.EqualTo("Mesh"));
            Assert.That(asset.Path, Is.EqualTo(TestAssetsPath));
            Assert.That(asset.ID, Is.Not.EqualTo(default(Guid)));
            Assert.That(asset.Vertices, Is.EqualTo(vertices));
            Assert.That(asset.Indices, Is.EqualTo(indices));
            Assert.That(asset.ID, Is.Not.EqualTo(default(Guid)));
            Assert.That(File.Exists(expectedPath), Is.True);
        }

        [Test]
        public void CreateTexture_ShouldCreateAndSaveTextureAsset()
        {
            // Arrange
            var size = new OpenTK.Mathematics.Vector2i(256, 256);
            var settings = new TextureSettings();
            var expectedPath = Path.Combine(TestAssetsPath, "Texture.asset");

            // Act
            var asset = AssetManager.CreateTexture(size, settings, "Texture", TestAssetsPath);

            // Assert
            Assert.That(asset, Is.Not.Null);
            Assert.That(asset, Is.InstanceOf<Texture>());
            Assert.That(asset.Name, Is.EqualTo("Texture"));
            Assert.That(asset.Path, Is.EqualTo(TestAssetsPath));
            Assert.That(asset.ID, Is.Not.EqualTo(default(Guid)));
            Assert.That(File.Exists(expectedPath), Is.True);
        }

        [Test]
        public void LoadAssetFromFile_ShouldLoadMaterialAssetFromJsonFile()
        {
            // Arrange
            var filePath = Path.Combine(TestAssetsPath, "Material.asset");
            var assetToSave = new Material(ShaderProgram.GetOrCreate("", new VertexShader())TestAssetsPath, "Material", Guid.NewGuid());
            AssetManager.SaveAssetToFile(filePath, assetToSave);

            // Act
            var loadedAsset = AssetManager.LoadAssetFromFile<Material>(filePath);

            // Assert
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(loadedAsset, Is.InstanceOf<Material>());
            Assert.That(loadedAsset.Name, Is.EqualTo("Material"));
            Assert.That(loadedAsset.Path, Is.EqualTo(TestAssetsPath));
            Assert.That(loadedAsset.ID, Is.EqualTo(assetToSave.ID));
           
        }

        [Test]
        public void LoadAssetFromFile_ShouldLoadMeshAssetFromJsonFile()
        {
            // Arrange
            var filePath = Path.Combine(TestAssetsPath, "Material.asset");
            var assetToSave = new Material(TestAssetsPath, "Material", Guid.NewGuid());
            AssetManager.SaveAssetToFile(filePath, assetToSave);

            // Act
            var loadedAsset = AssetManager.LoadAssetFromFile<Material>(filePath);

            // Assert
            Assert.That(loadedAsset, Is.Not.Null);
            Assert.That(loadedAsset, Is.InstanceOf<Material>());
            Assert.That(loadedAsset.Name, Is.EqualTo("Material"));
            Assert.That(loadedAsset.Path, Is.EqualTo(TestAssetsPath));
            Assert.That(loadedAsset.ID, Is.EqualTo(assetToSave.ID));
        }

        [Test]
        public void SaveAssetToFile_ShouldSaveAssetToJsonFile()
        {
            // Arrange
            var filePath = Path.Combine(TestAssetsPath, "Material.asset");
            var assetToSave = new Material(TestAssetsPath, "Material", Guid.NewGuid());

            // Act
            AssetManager.SaveAssetToFile(filePath, assetToSave);

            // Assert
            Assert.That(File.Exists(filePath), Is.True);
            var savedJson = File.ReadAllText(filePath);
            Assert.That(savedJson, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void GetFilePath_ShouldReturnCorrectFilePath()
        {
            // Arrange
            var asset = new Material(TestAssetsPath, "Material", Guid.NewGuid());

            // Act
            var filePath = AssetManager.GetFilePath(asset);

            // Assert
            var expectedPath = Path.Combine(TestAssetsPath, "Material.asset");
            Assert.That(filePath, Is.EqualTo(expectedPath));
        }
    }
}

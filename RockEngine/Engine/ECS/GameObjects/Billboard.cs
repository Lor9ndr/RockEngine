using OpenTK.Mathematics;

using RockEngine.OpenGL;
using RockEngine.OpenGL.Shaders;
using RockEngine.OpenGL.Textures;
using RockEngine.OpenGL.Vertices;

namespace RockEngine.Engine.ECS.GameObjects
{
    internal sealed class Billboard : IComponent, IRenderable, IDisposable
    {
        private readonly AShaderProgram _shader;
        private readonly Texture _billboardTexture;
        private readonly Camera _camera;
        private readonly Sprite _sprite;
        private static readonly Vertex2D[] _vertices = new Vertex2D[]
        {
            new Vertex2D(new Vector2(-0.5f, -0.5f), new Vector2(0)),
            new Vertex2D(new Vector2(0.5f, -0.5f), new Vector2(1.0f, 0.0f)),
            new Vertex2D(new Vector2(0.5f, 0.5f), new Vector2(1)),
            new Vertex2D(new Vector2(-0.5f, 0.5f), new Vector2(0.0f, 1.0f))
        };

        public GameObject? Parent { get;set;}

        public int Order => 0;

        public Billboard(Texture2D billboardTexture, Camera camera)
        {
            _billboardTexture = billboardTexture;
            _camera = camera;
            _sprite = new Sprite(_billboardTexture, in _vertices);
            _shader = ShaderProgram.GetOrCreate("Lit2DShader", new VertexShader("Resources\\Shaders\\Screen\\Screen.vert"), new FragmentShader("Resources\\Shaders\\Screen\\Screen.frag"));
        }

        public void Render()
        {
            var view = _camera.GetViewMatrix();
            var proj = _camera.GetProjectionMatrix();
            var model = new Vector4(Parent!.Transform.Position,1.0f);
            var billboardMatrix = proj * view * model;
            billboardMatrix /= billboardMatrix.W;
            if(billboardMatrix.Z < 0.0f)
            {
                return;
            }
            _sprite.Render();
        }

        public  void RenderOnEditorLayer()
        {
           
        }

        public void OnStart()
        {
            throw new NotImplementedException();
        }

        public void OnUpdate()
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OnUpdateDevelepmentState()
        {
            throw new NotImplementedException();
        }

        public object GetState()
        {
            throw new NotImplementedException();
        }

        public void SetState(object state)
        {
            throw new NotImplementedException();
        }
    }
}

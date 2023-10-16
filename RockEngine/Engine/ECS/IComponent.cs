using Newtonsoft.Json;

using RockEngine.Engine.ECS.GameObjects;
using RockEngine.OpenGL;
using RockEngine.OpenGL.Buffers.UBOBuffers;

namespace RockEngine.Engine.ECS
{
    public interface IComponent
    {
        /// <summary>
        /// Ref to the component parent, so it is attached to that gameobject
        /// </summary>
        [JsonIgnore]
        GameObject? Parent { get; set; }

        [JsonIgnore]
        int Order { get; }

        void OnStart();

        void OnUpdate();

        void OnDestroy();
    }
    public interface IComponentRenderable<T> : IComponent, IRenderable where T : struct, IUBOData<T>
    {
        T GetUBOData();
    }
}

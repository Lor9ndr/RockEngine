using RockEngine.Common.Utils;
using RockEngine.ECS;

namespace RockEngine.ECS.Assets.Converters
{
    public sealed class GameObjectConverter : IConverter<GameObject>
    {
        public void Write(GameObject data, BinaryWriter writer)
        {
            var components = data.GetComponents();
            writer.Write(data.Name);
            writer.Write(components.Count);
            foreach(var component in components)
            {
                if(component is IAsset asset)
                {
                    writer.Write(asset.ID.ToByteArray());
                }
                else
                {
                    WriteNonAssetComponent(component, writer);
                }
            }
            // TODO:Продумать запись дочерних игровых объектов
            /*foreach (var item in data.Children)
            {

            }*/
        }

        private void WriteNonAssetComponent<T>(T component, BinaryWriter writer) where T : IComponent
        {

        }

        public GameObject Read(BinaryReader reader)
        {
            var name = reader.ReadString();
            var countComponents = reader.ReadInt32();
            var components = new IComponent[countComponents];
            for(int i = 0; i < countComponents; i++)
            {
                var id = new Guid(reader.ReadBytes(16));
                var component = (IComponent?)AssetManager.GetAssetByID(id);
                Check.IsNull(component);
                components[i] = component!;
            }
            GameObject go = new GameObject(name, components);
            return go;

        }
    }
}

using RockEngine.Assets;

using System.Runtime.InteropServices;

namespace RockEngine.Editor.ImguiEditor.DragDrop
{
    internal struct DragDropAsset
    {
        public static int Size = Marshal.SizeOf<DragDropAsset>();
        public Guid ID;
        public AssetType AssetType;
        public DragDropAsset(Guid id, AssetType type)
        {
            ID = id;
            AssetType = type;
        }
    }
}

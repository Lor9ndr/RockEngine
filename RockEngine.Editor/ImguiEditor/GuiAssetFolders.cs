using RockEngine.ECS.Assets;

namespace RockEngine.Editor.ImguiEditor
{
    internal static class GuiAssetFolderManager
    {
        public static void RenameAsset(IAsset asset, string newName)
        {
            string oldFilePath = AssetManager.GetFilePath(asset);
            string newFilePath = Path.Combine(Path.GetDirectoryName(oldFilePath), newName + AssetManager.EXTENSION_NAME);

            if(File.Exists(newFilePath))
            {
                throw new InvalidOperationException("An asset with the same name already exists.");
            }

            File.Move(oldFilePath, newFilePath);
            asset.Name = newName;
        }

        public static void MoveAsset(IAsset asset, string newPath)
        {
            string oldFilePath = AssetManager.GetFilePath(asset);
            string newFilePath = Path.Combine(newPath, asset.Name + AssetManager.EXTENSION_NAME);

            if(File.Exists(newFilePath))
            {
                throw new InvalidOperationException("An asset with the same name already exists in the destination folder.");
            }

            File.Move(oldFilePath, newFilePath);
            asset.Path = newPath;
        }

        public static void DeleteAsset(IAsset asset)
        {
            string filePath = AssetManager.GetFilePath(asset);
            File.Delete(filePath);
            AssetManager.Assets.Remove(asset);
        }
    }
}

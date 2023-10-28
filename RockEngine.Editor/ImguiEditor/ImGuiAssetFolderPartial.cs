using FontAwesome.Constants;

using ImGuiNET;

using RockEngine.Assets;
using RockEngine.Editor.ImguiEditor;
using RockEngine.Utils;

using System.Runtime.InteropServices;

namespace RockEngine.Rendering.Layers
{
    public partial class ImGuiRenderer
    {
        private IAsset _selectedAsset;
        private string _currentFileSelected;
        private string selectedFolder;

        partial void DisplayAssetFolders()
        {
            if(ImGui.Begin("Folder Window"))
            {
                ImGui.Columns(2);
                DisplayFolderTree(Project.CurrentProject.Path);
                ImGui.NextColumn();
                DisplayAssets();
                ImGui.NextColumn();
                ImGui.End();
            }
        }

        partial void DisplayAssets()
        {
            if(!string.IsNullOrEmpty(selectedFolder))
            {
                DirectoryInfo directory = new DirectoryInfo(selectedFolder);

                if(!directory.Exists)
                {
                    throw new DirectoryNotFoundException("Directory not found: " + selectedFolder);
                }

                foreach(var file in directory.GetFiles())
                {

                    ImGui.Indent();
                    ImguiHelper.FaIconText(FA.FILE);
                    ImGui.SameLine();
                    if(ImGui.Selectable(file.Name, _currentFileSelected == file.FullName, ImGuiSelectableFlags.AllowOverlap))
                    {
                        if(ImGui.IsItemFocused())
                        {
                            _currentFileSelected = file.FullName;
                            _selectedAsset = AssetManager.GetAssetByPath(_currentFileSelected);
                        }
                    }

                    if(ImGui.BeginDragDropSource())
                    {
                        if(_selectedAsset != null)
                        {
                            ImGui.SetDragDropPayload("SELECTED_ASSET_PAYLOAD", _selectedAsset.ID.ToIntPtr(), (uint)Marshal.SizeOf<Guid>());
                            ImguiHelper.FaIconText(FA.FILE);

                        }
                        ImGui.EndDragDropSource();
                    }

                    ImGui.Unindent();
                }
            }
        }

        private void DisplayFolderTree(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            if(!directory.Exists)
            {
                throw new DirectoryNotFoundException("Directory not found: " + path);
            }

           /* if(selectedFolder == directory)
            {
                bool isSelected = ImGui.Selectable(directory.Name, selectedFolder == directory.FullName, ImGuiSelectableFlags.AllowDoubleClick);
            }*/
            bool isNodeOpen = ImGui.TreeNode(directory.Name);
            
            if(ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("SELECTED_ASSET_PAYLOAD", ImGuiDragDropFlags.AcceptPeekOnly);
                unsafe
                {
                    if((nint)payload.NativePtr != nint.Zero && payload.Data != IntPtr.Zero)
                    {
                        var assetID = Marshal.PtrToStructure<Guid>(payload.Data);
                        var asset = AssetManager.GetAssetByID(assetID);
                        if(asset != null && payload.Delivery)
                        {
                            GuiAssetFolderManager.MoveAsset(asset, directory.FullName);
                        }
                    }
                }
                ImGui.EndDragDropTarget();
            }
            if(ImGui.IsItemFocused())
            {
                selectedFolder = directory.FullName;
            }
         

            if(isNodeOpen)
            {
                foreach(var subdirectory in directory.GetDirectories())
                {
                    DisplayFolderTree(subdirectory.FullName);
                }
               

                ImGui.TreePop();
            }
        }
    }
}

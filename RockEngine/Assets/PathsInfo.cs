namespace RockEngine.Assets
{
    public static class PathsInfo
    {
        public static PathInfo ENGINE_DIRECTORY =>  Directory.GetCurrentDirectory();
        public static PathInfo PROJECT_PATH => Project.CurrentProject.Path;
        public static PathInfo PROJECT_ASSETS_PATH => PROJECT_PATH + @"\Assets\";
    }

    public static class PathConstants
    {
        public const string RESOURCES = "Resources";
        public const string SHADERS = "Shaders";
        public const string PICKING = "Picking";
        public const string DEBUG = "Dbg";
        public const string SCREEN = "Screen";
        public const string SELECTED_OBJECT = "SelectedObject";
        public const string GIZMO = "Gizmo";
    }
    public record PathInfo(string Path)
    {
        public static PathInfo operator / (PathInfo p, string path) => new PathInfo(p.Path + "/" + path);
        public static implicit operator string(PathInfo p) => p.Path;
        public static implicit operator PathInfo(string p) => new PathInfo(p);
    }
}

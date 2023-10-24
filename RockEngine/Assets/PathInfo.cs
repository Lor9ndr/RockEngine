namespace RockEngine.Assets
{
    internal static class PathInfo
    {
        public static string ENGINE_DIRECTORY => Directory.GetCurrentDirectory();
        public static string PROJECT_PATH => Project.CurrentProject.Path;
        public static string PROJECT_ASSETS_PATH => PROJECT_PATH + @"\Assets\";
    }

    public static class PathConstants
    {
        public const string RESOURCES = "Resources";
        public const string SHADERS = "Shaders";
        public const string PICKING = "Picking";
        public const string DEBUG = "Dbg";
        public const string SCREEN = "Screen";
        public const string SELECTED_OBJECT = "SelectedObject";
    }
}

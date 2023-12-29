using RockEngine.Common;

namespace RockEngine.ECS
{
    public static class PathsInfo
    {
        public static PathInfo ENGINE_DIRECTORY => Directory.GetCurrentDirectory();
        public static PathInfo PROJECT_PATH => Project.CurrentProject.Path;
        public static PathInfo PROJECT_ASSETS_PATH => PROJECT_PATH + @"\Assets\";
    }
}

namespace RockEngine.Common
{
    public static class PathConstants
    {
        public static PathInfo RESOURCES = "Resources";
        public static PathInfo SHADERS = "Shaders";
        public static PathInfo PICKING = "Picking";
        public static PathInfo DEBUG = "Dbg";
        public static PathInfo SCREEN = "Screen";
        public static PathInfo SELECTED_OBJECT = "SelectedObject";
        public static PathInfo GIZMO = "Gizmo";
    }
    public record PathInfo(string Path)
    {
        public static PathInfo operator /(PathInfo p, string path) => new PathInfo(p.Path + "/" + path);
        public static PathInfo operator +(PathInfo p, PathInfo p2) => new PathInfo(p.Path + "/" + p2.Path);
        public static implicit operator string(PathInfo p) => p.Path;
        public static implicit operator PathInfo(string p) => new PathInfo(p);
    }
}

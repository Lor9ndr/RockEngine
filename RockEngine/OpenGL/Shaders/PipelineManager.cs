using RockEngine.OpenGL;

namespace RockEngine.OpenGL.Shaders
{
    internal static class PipelineManager
    {
        public static Dictionary<string, Dictionary<RenderType, Pipeline>> _pipelines = new Dictionary<string, Dictionary<RenderType, Pipeline>>();

        public static Pipeline CurrentPipeline => Pipeline.CurrentPipeline;

        public static void AddPipeLine(Pipeline p)
        {
            if (_pipelines.TryGetValue(p.Name, out var value))
            {
                value.Add(p.RenderType, p);
            }
            else
            {
                _pipelines.Add(p.Name,
                    new Dictionary<RenderType, Pipeline>()
                    {
                        {
                           p.RenderType, p
                        }
                    });
            }
        }

        public static Pipeline GetPipeline(string name, RenderType type)
        {
            return _pipelines[name].FirstOrDefault(s => s.Key.HasFlag(type)).Value;
        }

        public static Pipeline GetCurrentPipeline(RenderType type)
        {
            return _pipelines[CurrentPipeline.Name].FirstOrDefault(s => s.Key.HasFlag(type)).Value;
        }
    }
}

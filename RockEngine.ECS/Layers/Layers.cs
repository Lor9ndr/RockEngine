using RockEngine.Rendering;

using System.Collections;

namespace RockEngine.ECS.Layers
{
    public sealed class Layers : IEnumerable<ALayer>
    {
        private List<ALayer> _layers;

        public Layers()
        {
            _layers = new List<ALayer>();
        }
        public void AddLayer(ALayer layer)
        {
            _layers.Add(layer);
            _layers = _layers.OrderBy(s => s.Order).ToList();
        }
        public void RemoveLayer(ALayer layer)
        {
            _layers.Remove(layer);
        }

        public void OnRender(Scene scene)
        {
            foreach(var item in _layers)
            {
                 item.OnRender(scene);
            }
        }

        public T? GetLayer<T>() where T : ALayer
        {
            return _layers.OfType<T>().FirstOrDefault();
        }

        public IEnumerator<ALayer> GetEnumerator()
            => _layers.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}

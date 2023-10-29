﻿using RockEngine.DI;
using RockEngine.Engine;

using System.Collections;

namespace RockEngine.Rendering.Layers
{
    public sealed class Layers : IEnumerable<ALayer>
    {
        private List<ALayer> _layers;
        public static Layer CurrentLayer;

        public Layers()
        {
            _layers = IoC.GetAll<ALayer>().ToList();
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

        public void OnRender()
        {
            foreach (var item in _layers)
            {
                CurrentLayer = item.Layer;
                item.OnRender();
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
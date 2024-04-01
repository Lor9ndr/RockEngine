namespace RockEngine.Rendering.Renderers
{
    public interface IRenderer<T> 
    {
        public void Update(T item);
        public void Render(IRenderingContext context, T item);
    }
}

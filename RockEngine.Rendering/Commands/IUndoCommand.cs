namespace RockEngine.Rendering.Commands
{
    public interface IUndoCommand : ICommand
    {
        public void Undo();
    }
}

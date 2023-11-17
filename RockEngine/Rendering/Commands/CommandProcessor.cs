
namespace RockEngine.Rendering.Commands
{
    public class CommandProcessor
    {
        private readonly Queue<ICommand> _commands = new Queue<ICommand>();

        public void Enqueue(ICommand command)
        {
            _commands.Enqueue(command);
        }
       
        public void ProcessCommands()
        {
            while(_commands.Any())
            {
                _commands.Dequeue().Execute();
            }
        }
    }
}

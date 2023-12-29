namespace RockEngine.Rendering.Commands
{
    internal readonly struct ApiCommand : ICommand
    {
        private readonly Action Action;

        public ApiCommand(Action action)
        {
            Action = action;
        }

        public void Execute()
        {
            Action.Invoke();
        }
    }
}

using RockEngine.Rendering.Commands;

using System.Collections.Concurrent;

namespace RockEngine.Rendering
{
    public static class OpenGLDispatcher
    {
        private static readonly ConcurrentQueue<Action> _renderQueue = new ConcurrentQueue<Action>();
        private static readonly ConcurrentQueue<Action> _updateQueue = new ConcurrentQueue<Action>();
        private static readonly object queueLock = new object();

        public static void RenderEnqueue(Action command)
        {
            lock(queueLock)
            {
                _renderQueue.Enqueue(command);
            }
        }
        public static void UpdateEnqueue(Action command)
        {
            lock(queueLock)
            {
                _updateQueue.Enqueue(command);
            }
        }

        public static void ExecuteRenderCommands()
        {
            lock(queueLock)
            {
                while(!_renderQueue.IsEmpty)
                {
                    if(_renderQueue.TryDequeue(out var command))
                    {
                        command();
                    }
                }
            }
        }
        public static void ExecuteUpdateCommands()
        {
            lock(queueLock)
            {
                while(!_updateQueue.IsEmpty)
                {
                    if(_updateQueue.TryDequeue(out var command))
                    {
                        command();
                    }
                }
            }
        }
    }
}

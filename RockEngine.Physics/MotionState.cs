
namespace RockEngine.Physics
{
    public class MotionState
    {
        public float SleepThreshold = 1f;
        public ActivationState State { get; private set; } = ActivationState.Active;

        public void Sleep()
        {
            State = ActivationState.Sleeping;
        }

        public void WakeUp()
        {
            State = ActivationState.Active;
        }
    }
}

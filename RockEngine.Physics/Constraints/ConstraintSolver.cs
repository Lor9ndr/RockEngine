namespace RockEngine.Physics.Constraints
{
    public class ConstraintSolver
    {
        private readonly List<IConstraint> _constraints = new List<IConstraint>();

        public void AddConstraint(IConstraint constraint)
        {
            _constraints.Add(constraint);
        }

        public void SolveConstraints(float deltaTime)
        {
            foreach(var constraint in _constraints)
            {
                constraint.ApplyConstraint(deltaTime);
            }
        }
    }
}
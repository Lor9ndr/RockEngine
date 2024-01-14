using OpenTK.Mathematics;

namespace RockEngine.Physics.Colliders
{
    internal class ConvexCollider : Collider
    {
        /*  public override bool CheckCollision(BoxCollider otherCollider, out CollisionResult result)
          {
              // Initialize the simplex
              List<Vector3> simplex = new List<Vector3>();

              // Get the initial direction
              Vector3 direction = Vector3.One;

              // Start the GJK algorithm
              while(true)
              {
                  // Add a new point to the simplex
                  simplex.Add(Support(MinkowskiDifference(in GetVertices(), in otherCollider.GetVertices()), direction));

                  // If the last point added to the simplex doesn't pass the origin in the direction of d,
                  // then the Minkowski Sum doesn't contain the origin and there is no collision
                  if(Vector3.Dot(simplex.Last(), direction) <= 0)
                  {
                      result = new CollisionResult();
                      return false;
                  }
                  else
                  {
                      // If the simplex contains the origin we have a collision
                      if(ContainsOrigin(simplex, ref direction))
                      {
                          result = new CollisionResult();
                          return true;
                      }
                  }
              }
          }
          // Support function for the GJK algorithm
          private Vector3 Support(List<Vector3> shape, Vector3 direction)
              => shape.OrderByDescending(point => Vector3.Dot(point, direction)).First();

          // Function to calculate the Minkowski difference
          private List<Vector3> MinkowskiDifference(in Vector3[ ] verticesA, in Vector3[ ] verticesB)
          {
              List<Vector3> result = new List<Vector3>();

              foreach(Vector3 pointA in verticesA)
              {
                  foreach(Vector3 pointB in verticesB)
                  {
                      result.Add(pointA - pointB);
                  }
              }

              return result;
          }

          private bool ContainsOrigin(List<Vector3> simplex, ref Vector3 direction)
          {
              // Get the last point added to the simplex
              Vector3 a = simplex.Last();

              // Compute AO (same as -A)
              Vector3 ao = -a;

              if(simplex.Count == 3)
              {
                  // The simplex is a triangle
                  Vector3 b = simplex[1];
                  Vector3 c = simplex[0];

                  // Compute the edges
                  Vector3 ab = b - a;
                  Vector3 ac = c - a;

                  // Compute the normals
                  Vector3 abPerp = Vector3.Cross(Vector3.Cross(ab, ao), ab);
                  Vector3 acPerp = Vector3.Cross(Vector3.Cross(ac, ao), ac);

                  // Check if the origin is in one of the regions defined by the edges
                  if(Vector3.Dot(abPerp, ao) > 0)
                  {
                      // The origin is in the region defined by AB
                      simplex.Remove(c);
                      direction = abPerp;
                  }
                  else if(Vector3.Dot(acPerp, ao) > 0)
                  {
                      // The origin is in the region defined by AC
                      simplex.Remove(b);
                      direction = acPerp;
                  }
                  else
                  {
                      // The origin is in the region defined by the triangle
                      return true;
                  }
              }
              else
              {
                  // The simplex is a line segment
                  Vector3 b = simplex[0];

                  // Compute the edge
                  Vector3 ab = b - a;

                  // Compute the normal
                  Vector3 abPerp = Vector3.Cross(Vector3.Cross(ab, ao), ab);

                  // The new direction is perp to AB towards the origin
                  direction = abPerp;
              }

              return false;
          }

          public override bool CheckCollision(SphereCollider otherCollider, out CollisionResult result)
          {
              result = new CollisionResult();
              return false;
          }

          public override ref Vector3[ ] GetVertices()
          {
              throw new NotImplementedException();
          }*/
        public override CollisionResult CheckCollision(BoxCollider otherCollider)
        {
            throw new NotImplementedException();
        }

        public override CollisionResult CheckCollision(SphereCollider otherCollider)
        {
            throw new NotImplementedException();
        }

        public override ref Vector3[ ] GetVertices()
        {
            throw new NotImplementedException();
        }
    }
}

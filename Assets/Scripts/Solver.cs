using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solver
{
   public float kDamping = 0.98f;
   private List<Body> _bodies;
   private Collider[] _colliders;
   
   public Solver(Collider[] colliders)
   {
      _bodies = new List<Body>();
      _colliders = colliders;
   }

   public void AddBody(Body b)
   {
      _bodies.Add(b);
   }

   public void Solve()
   {
      
         // Applying gravity
         ApplyForces();

         // Update velocities with forces
         foreach (Body b in _bodies)
         {
            for (int i = 0; i < b.NumParticles; i++)
            {
               b.Velocities[i] += Time.deltaTime * (b.Forces[i] / b.Masses[i]);
            }
         }

         // Damping velocities
         DampVelocities();
         
         // Update projected positions
         foreach (Body b in _bodies)
         {
            for (int i = 0; i < b.NumParticles; i++)
            {
               b.Projected[i] = b.Positions[i] + Time.deltaTime * b.Velocities[i];
            }
         }

         // Generate collisions Constraints
         GenerateCollisionsConstraints();

         // Project constraints
         ProjectConstraints();
         
         
         // Update positions and velocities
         foreach (Body b in _bodies)
         {
            for (int i = 0; i < b.NumParticles; i++)
            {
               b.Velocities[i] += Time.deltaTime * b.Forces[i] / b.Masses[i];
               b.Positions[i] = b.Projected[i];
            }
         }
   }

   public void ApplyForces()
   {
      foreach (Body b in _bodies)
      {
         Vector3 g = Vector3.down * 9.81f;
         for (int i = 0; i < b.NumParticles; i++)
         {
            b.Forces[i] = g;
         }
      }
   }

   public void DampVelocities()
   {
      foreach (Body b in _bodies)
      {
         /*// Temp easy damping
         for (int i = 0; i < b.NumParticles; i++)
         {
            b.Velocities[i] *= 0.98f;
         }*/
         // Compute centered position and velocity
         Vector3 xcm = Vector3.zero;
         Vector3 vcm = Vector3.zero;
         float m = 0;
         for (int i = 0; i < b.NumParticles; i++)
         {
            xcm += b.Positions[i] * b.Masses[i];
            vcm += b.Velocities[i] * b.Masses[i];
            m += b.Masses[i];
         }

         xcm /= m;
         vcm /= m;
         
         // Compute L
         Vector3 L = Vector3.zero;
         for (int i = 0; i < b.NumParticles; i++)
         {
            // ri x mivi
            L += Vector3.Cross(xcm - b.Positions[i], b.Masses[i] * b.Velocities[i]);
         }
         
         // Compute I
         Matrix4x4 I = Matrix4x4.identity;
         for (int i = 0; i < b.NumParticles; i++)
         {
            Vector3 ri = xcm - b.Positions[i];
            // Matrix3x3  doesn't exist so we use a 4x4 as an homogeneous matrix
            Matrix4x4 rtilde = Matrix4x4.zero;
            rtilde.m01 = ri.z;
            rtilde.m02 = ri.y;
            rtilde.m12 = ri.x;
            rtilde.m10 = -rtilde.m01;
            rtilde.m20 = -rtilde.m02;
            rtilde.m21 = -rtilde.m12;
            rtilde.m33 = 1;
            
            Matrix4x4 massScalar = Matrix4x4.Scale(Vector3.one* b.Masses[i]);
            massScalar.m33 = 1;
            
            Matrix4x4 temp = rtilde * rtilde.transpose * massScalar;
            I.m00 += temp.m00;
            I.m01 += temp.m01;
            I.m02 += temp.m02;
            I.m10 += temp.m10;
            I.m11 += temp.m11;
            I.m12 += temp.m12;
            I.m20 += temp.m20;
            I.m21 += temp.m21;
            I.m22 += temp.m22;       
         }
         
         // Compute omega
         Vector3 w = I.inverse * L ;

         /*Debug.Log ("I: ");
         Debug.Log (I);
         Debug.Log ("L: ");
         Debug.Log (L);
         Debug.Log (" omega :");
         Debug.Log ( w );*/
         // Damp velocities
         for (int i = 0; i < b.NumParticles; i++)
         {
            Vector3 deltaV = vcm + Vector3.Cross(w, xcm - b.Positions[i]) - b.Velocities[i];
            b.Velocities[i] += kDamping * deltaV;
         }   
      }

      

   }

   public void ProjectConstraints()
   {
      for (int i = 0; i < 4; i++)
      {
         foreach (Body b in _bodies)
         {
            foreach (Constraint c in b.StaticConstraints)
            {
               c.SolveConstraint(i);
            }
            
            foreach (Constraint c in b.Constraints)
            {
               c.SolveConstraint(i);
            }
               
            foreach (Constraint c in b.CollConstraints)
            {
               c.SolveConstraint(i);
            }
         }
      }
   }

   public void GenerateCollisionsConstraints()
   {
      foreach (Body b in _bodies)
      {
         b.ResetCollConstraints();
         for (int i = 0; i < b.NumParticles; i++)
         {
            foreach (Collider c in _colliders)
            {
               Vector3 direction = b.Projected[i] - b.Positions[i];
               Vector3 rayOffset = direction.normalized * 0.2f;
               Debug.DrawLine(b.Positions[i], b.Projected[i], Color.blue);
               // Debug.Log($"Pos: {b.Positions[i]}, Proj: {b.Projected[i]}");
               float distance = direction.magnitude;
               if (distance > 0)
               {
                  Ray ray = new Ray(b.Positions[i]-rayOffset, direction.normalized);
                  RaycastHit hit;
                  if (c.Raycast(ray, out hit, distance+rayOffset.magnitude))
                  {
                     Vector3 qc = hit.point;
                     Vector3 n = hit.normal;
                     // Debug.Log($"HIT qc:{qc} n:{n}");
                     CollisionConstraint collConstraint = new CollisionConstraint(b, i, n, qc);
                     b.AddCollConstraint(collConstraint);
                  }
               }
            }
         }
      }
   }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Body
{
    public int NumParticles;
    public float ParticlesRadius;
    public float[] Masses;
    public float[] InvMasses;
    public Vector3[] Positions;
    public Vector3[] Projected;
    public Vector3[] Velocities;
    public Vector3[] Forces;
    public List<Constraint> Constraints;
    public List<CollisionConstraint> CollConstraints;
    public List<Constraint> StaticConstraints;

    public Body(int numParticles)
    {
        NumParticles = numParticles;
        ParticlesRadius = 0.5f;
        Masses = new float[NumParticles];
        InvMasses = new float[NumParticles];
        Positions = new Vector3[NumParticles];
        Projected = new Vector3[NumParticles];
        Velocities = new Vector3[NumParticles];
        Forces = new Vector3[NumParticles];
        Constraints = new List<Constraint>();
        CollConstraints = new List<CollisionConstraint>();
        StaticConstraints = new List<Constraint>();
    }

    public void AddConstraint(Constraint c)
    {
        Constraints.Add(c);
    }
    
    public void AddCollConstraint(CollisionConstraint c)
    {
        CollConstraints.Add(c);
    }
    
    public void AddStaticConstraint(StaticConstraint c)
    {
        StaticConstraints.Add(c);
    }

    public void ResetCollConstraints()
    {
        CollConstraints = new List<CollisionConstraint>();
    }
}

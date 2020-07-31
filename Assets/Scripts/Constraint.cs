using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Constraint
{
    // Range from 0 to 1
    protected float _stiffness;
    protected int[] _indices;
    protected Body _body;
    
    public Constraint(Body body)
    {
        _body = body;
        _stiffness = 0.5f;
    }

    public abstract void SolveConstraint(int nIteration);
}

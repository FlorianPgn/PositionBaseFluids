using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StaticConstraint : Constraint
{
    private Vector3[] _positions;
    
    public StaticConstraint(Body body, int[] indices) : base(body)
    {
        _indices = indices;
        _positions = new Vector3[indices.Length];
        _stiffness = 1f;
        for (int i = 0; i < indices.Length; i++)
        {
            _positions[i] = _body.Positions[indices[i]];
        }
    }

    public override void SolveConstraint(int nIteration, float deltaTime)
    {
        for (int i = 0; i < _indices.Length; i++)
        {
            _body.Positions[_indices[i]] = _positions[i];
            _body.Projected[_indices[i]] = _positions[i];
        }
    }
}

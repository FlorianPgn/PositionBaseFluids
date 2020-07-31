using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionConstraint : Constraint
{
    private Vector3 _n;
    private Vector3 _qc;
    private int i => _indices[0];
    public CollisionConstraint(Body body, int index, Vector3 n, Vector3 qc) : base(body)
    {
        _indices = new []{index};
        _stiffness = 1f;
        _n = n;
        _qc = qc;
    }

    public override void SolveConstraint(int nIteration)
    {
        // Inequality constraint
        float cp = Vector3.Dot(_body.Projected[i] - _qc, _n);
        if (cp < 0)
        {
            float kCorr = (1 - Mathf.Pow(1 - _stiffness, 1/(float) nIteration));
            _body.Projected[i] += _n * (-cp * kCorr);
            _body.Velocities[i] = _n * -cp / Time.fixedDeltaTime;
        }
    }
}

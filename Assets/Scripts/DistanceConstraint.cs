using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceConstraint : Constraint
{
    private float _d;
    private int _i0 => _indices[0];
    private int _i1 => _indices[1];
    public DistanceConstraint(Body body, int i0, int i1) : base(body)
    {
        _stiffness = 0.3f;
        _indices = new [] {i0, i1};
        _d = (_body.Positions[i0] - _body.Positions[i1]).magnitude;
    }

    public override void SolveConstraint(int nIteration)
    {
        Vector3 l = _body.Positions[_i0] - _body.Positions[_i1];
        Vector3 deltaP = (l.magnitude - _d) * l / l.magnitude;
        float corr = (1 - Mathf.Pow(1 - _stiffness, 1/(float) nIteration));
        _body.Projected[_i0] += deltaP * (-_body.InvMasses[_i0] / (_body.InvMasses[_i0] + _body.InvMasses[_i1]) * corr);
        _body.Projected[_i1] += deltaP * (_body.InvMasses[_i1] / (_body.InvMasses[_i0] + _body.InvMasses[_i1]) * corr);
    }
}

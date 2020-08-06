using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeConstraint : Constraint
{
    private Vector3[] _relativePositions;
    private Vector3 _centerMass;
    public ShapeConstraint(Body b) : base(b)
    {
        _relativePositions = new Vector3[b.NumParticles];
        _stiffness = 0.3f;
        
        for (int i = 0; i < b.NumParticles; i++)
        {
            _centerMass += b.Positions[i];
        }

        _centerMass /= b.NumParticles;
        
        for (int i = 0; i < b.NumParticles; i++)
        {
            _relativePositions[i] = b.Positions[i] - _centerMass;
        }  
        
        
    }

    public override void SolveConstraint(int nIteration, float deltaTime)
    {
        Vector3 cm = Vector3.zero;
        for (int i = 0; i < _body.NumParticles; i++)
        {
            cm += _body.Projected[i];
        }
        
        cm /= _body.NumParticles;

        for (int i = 0; i < _body.NumParticles; i++)
        {
            float kCorr = (1 - Mathf.Pow(1 - _stiffness, 1/(float) nIteration));
            Vector3 target = cm + _relativePositions[i];
            _body.Projected[i] += (target - _body.Projected[i]) * (kCorr);
        } 
    }
}

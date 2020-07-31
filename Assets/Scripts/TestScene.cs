using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    private Body _body1;
    private Body _body2;
    private Solver _solver;

    public Collider[] Colliders;

    public GameObject ParticlePrefab;

    private Transform[] _particles;
    // Start is called before the first frame update
    void Start()
    {
        _body1 = new Body(1);
        _body1.Positions[0] = Vector3.zero;
        // _body1.Positions[1] = Vector3.right;
        _body1.Masses[0] = 1f;
        _body1.InvMasses[0] = 1f;
        // _body1.Masses[1] = 1f;
        // _body1.InvMasses[1] = 1f;
        // StaticConstraint sConstraint = new StaticConstraint(_body1, new []{0});
        // Constraint distConstraint = new DistanceConstraint(_body1, 0, 1);
        // _body1.AddStaticConstraint(sConstraint);
        // _body1.AddConstraint(distConstraint);
        
        _body2 = BodyBuilder.CreateCube(transform.position + Vector3.right * 2, 1, 10);
        //StaticConstraint staticConstraint = new StaticConstraint(_body2, new []{26});
        // _body2.AddStaticConstraint(staticConstraint);
        
        _solver = new Solver(Colliders);
        _solver.AddBody(_body1);
        _solver.AddBody(_body2);
        
        
        _particles = new Transform[_body2.NumParticles];
        for (int i = 0; i < _body2.NumParticles; i++)
        {
            _particles[i] = Instantiate(ParticlePrefab, _body2.Positions[i], Quaternion.identity).transform;
            _particles[i].localScale = Vector3.one * _body2.ParticlesRadius;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < _body2.NumParticles; i++)
        {
            _particles[i].position = _body2.Positions[i];
        }
        _solver.Solve();
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.red;
        if (_body2 != null)
        {
            foreach (var pos in _body2.Positions)
            {
                Gizmos.DrawSphere(pos, _body2.ParticlesRadius);
            }
        }*/
        
        Gizmos.color = Color.green;
        if (_body1 != null)
        {
            foreach (var pos in _body1.Positions)
            {
                Gizmos.DrawSphere(pos, 0.05f);
            }
        }
    }
}

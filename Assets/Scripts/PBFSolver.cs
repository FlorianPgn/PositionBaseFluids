using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PBFSolver
{
    public int SolverIterations = 4;
    public float EpsilonRelax = 120.0f; // 60
    
    private const float _kPressureCorr = 0.01f;
    private const float _kernelRadius = 0.3f;
    private const float _kernelRadiusSqr = _kernelRadius * _kernelRadius;
    private readonly float _invRestDensity;
    private Vector3[] _deltaP;
    private float[] _lambdas;
    private Vector3 _g = Vector3.down * 9.81f;
    private float _deltaT;
    
    private Kernel _kernel;
    private FluidBody _body;
    private Collider[] _colliders;
    private List<int>[] _neighbors;
    public PBFSolver(Collider[] colliders)
    {
        _colliders = colliders;
        _kernel = new Kernel(_kernelRadius);
        _invRestDensity = 1 / FluidBody.RestDensity;
    }

    public void SetBody(FluidBody b)
    {
        _body = b;
        _neighbors = new List<int>[_body.NumParticles];
        _deltaP = new Vector3[_body.NumParticles];
        _lambdas = new float[_body.NumParticles];
    }
    
    public void Solve(float deltaT)
    {
        Debug.Log("-------NEW STEP-------");
        _deltaT = deltaT;
        // Applying gravity
        ApplyForces();

        // Update velocities with forces and projected positions
        for (int i = 0; i < _body.NumParticles; i++)
        {
            _body.Velocities[i] += _body.Forces[i] * (deltaT * _body.InvMasses[i]);
            _body.Projected[i] = _body.Positions[i] + deltaT * _body.Velocities[i];
        }
        
        // Find neighbors
        FindNeighbors();

        // Solve
        for (int iter = 0; iter < SolverIterations; iter++)
        {
            Debug.Log($"-------ITERATION {iter}-------");
            // Calculate lambda
            ComputeLambda();
            
            Debug.Log("Density : "+_body.Densities.Sum());
            
            // Calculate deltaPi
            ComputeDeltaP();
            
            // Generate collisions Constraints
            GenerateCollisionsConstraints();

            // Project constraints
            ProjectConstraints();
            
            // Update projected positions
            for (int i = 0; i < _body.NumParticles; i++)
            {
                _body.Projected[i] += _deltaP[i];
            }
        }
        

        // Update positions and velocities
        for (int i = 0; i < _body.NumParticles; i++)
        {
            _body.Velocities[i] = (_body.Projected[i]-_body.Positions[i])/deltaT;
            // Apply vorticity and viscosity
            // TODO
            _body.Positions[i] = _body.Projected[i];
        }
    }
    
    public void ApplyForces()
    {
        // Apply gravity
        for (int i = 0; i < _body.NumParticles; i++)
        {
            _body.Forces[i] = _g;
        }
    }

    public void FindNeighbors()
    {
        for (int i = 0; i < _body.NumParticles; i++)
        {
            _neighbors[i] = new List<int>();
            for (int j = 0; j < _body.NumParticles; j++)
            {
                if (i==j)
                    continue;
                if ((_body.Positions[j] - _body.Positions[i]).sqrMagnitude <= _kernelRadiusSqr*4f)
                {
                    _neighbors[i].Add(j);
                }
            }
        }
    }

    public void ComputeLambda()
    {
        for (int i = 0; i < _body.NumParticles; i++)
        {
            float gradCSum = 0;
            Vector3 gradCi = Vector3.zero;
            _body.Densities[i] = 0;
            foreach (int j in _neighbors[i])
            {
                Vector3 r = _body.Positions[i] - _body.Positions[j];
                _body.Densities[i] += _body.Masses[i] * _kernel.Poly6(r);
                Vector3 gradCj = _invRestDensity * -_kernel.SpikyDeriv(r);
                gradCSum += Vector3.Dot(gradCj, gradCj);
                gradCi += gradCj;
            }

            gradCSum += gradCi.sqrMagnitude;
                
            // Density constraint
            float C = _invRestDensity * _body.Densities[i] - 1;
            _lambdas[i] = -C / (gradCSum - EpsilonRelax);
        }
    }

    public void ComputeDeltaP()
    {
        Vector3 pointInKernel = Vector3.up * (_kernelRadius * 0.1f);
        for (int i = 0; i < _body.NumParticles; i++)
        {
            _deltaP[i] = Vector3.zero;
            foreach (int j in _neighbors[i])
            {
                Vector3 r = _body.Positions[i] - _body.Positions[j];
                float corr = -_kPressureCorr *
                             Mathf.Pow(_kernel.Poly6(r) / _kernel.Poly6(pointInKernel), 4);
                // Debug.Log(corr);
                // _deltaP[i] += (Mathf.Max(0, _lambdas[i]) + Mathf.Max(0, _lambdas[j]) + corr) * _kernel.SpikyDeriv(r);
                _deltaP[i] += (_lambdas[i] + _lambdas[j] + corr) * _kernel.SpikyDeriv(r);
            }
            _deltaP[i] *= _invRestDensity;
        }
    }
    
    public void GenerateCollisionsConstraints()
    {
        _body.ResetCollConstraints();
        for (int i = 0; i < _body.NumParticles; i++)
        {
            foreach (Collider c in _colliders)
            {
                Vector3 direction = _body.Projected[i] - _body.Positions[i];
                float distance = direction.magnitude;
                Vector3 normalizeDir = direction / distance;
                Vector3 rayOffset = normalizeDir * 0.2f;
                // if (i == 0 || i == 1 || i == 2 || i == 9 || i == 10 || i == 11 || i == 18 || i == 19 || i == 20)
                Debug.DrawLine(_body.Positions[i], _body.Projected[i], Color.blue);
                if (distance > Mathf.Epsilon)
                {
                    Ray ray = new Ray(_body.Positions[i]-rayOffset, normalizeDir);
                    RaycastHit hit;
                    
                    if (c.Raycast(ray, out hit, distance+rayOffset.magnitude + _body.ParticlesRadius))
                    {
                        Vector3 qc = hit.point;
                        Vector3 n = hit.normal;
                        CollisionConstraint collConstraint = new CollisionConstraint(_body, i, n, qc);
                        _body.AddCollConstraint(collConstraint);
                    }
                }
            }
        }
    }
    
    public void ProjectConstraints()
    {
        for (int i = 0; i < SolverIterations; i++)
        {
            foreach (CollisionConstraint c in _body.CollConstraints)
            {
                c.SolveConstraint(i, _deltaT);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidTestScene : MonoBehaviour
{
    private PBFSolver _solver;
    private FluidBody _fluidBody;
    private Transform[] _particles;
    
    public Collider[] Colliders;
    public GameObject ParticlePrefab;
    public int NumDirections = 250;
    
    // Start is called before the first frame update
    void Start()
    {
        _fluidBody = new FluidBody(Vector3.up*0.6f, 1f, 10);
        _particles = new Transform[_fluidBody.NumParticles];
        for (int i = 0; i < _fluidBody.NumParticles; i++)
        {
            _particles[i] = Instantiate(ParticlePrefab, _fluidBody.Positions[i], Quaternion.identity).transform;
            _particles[i].localScale = Vector3.one * _fluidBody.ParticlesRadius;
            _particles[i].name = "Particle " + i;
        }
        _solver = new PBFSolver(Colliders);
        _solver.SetBody(_fluidBody);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < _fluidBody.NumParticles; i++)
        {
            _particles[i].position = _fluidBody.Positions[i];
        }
        _solver.Solve(0.008f);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = System.Random;

public class FluidBody : Body
{
    public float[] Densities;
    public float[] Pressures;
    public const float Viscosity = 250.0f;
    public const float RestDensity = 1000.0f;

    private static Vector3[] PossibleDirections = Helper.GetDirections(250);
    public FluidBody(Vector3 pos, float width, int particlesPerUnit) : base((int) Mathf.Pow(particlesPerUnit * width, 3))
    {
        Densities = new float[NumParticles];
        Pressures = new float[NumParticles];
        ParticlesRadius = 1.0f / particlesPerUnit;
        float mass = RestDensity * (4.0f / 3.0f) * Mathf.PI * ParticlesRadius * ParticlesRadius * ParticlesRadius;
        int nbParticlesPerRow = (int) (particlesPerUnit * width);
        Random rand = new Random();
        for (int i = 0; i < nbParticlesPerRow; i++)
        {
            for (int j = 0; j < nbParticlesPerRow; j++)
            {
                for (int k = 0; k < nbParticlesPerRow; k++)
                {
                    Vector3 p = new Vector3(
                        pos.x-width/2.0f+(i/(float)particlesPerUnit),
                        pos.y-width/2.0f+(j/(float)particlesPerUnit), 
                        pos.z-width/2.0f+(k/(float) particlesPerUnit));
                    int index = k + j * nbParticlesPerRow + i * nbParticlesPerRow * nbParticlesPerRow;
                    Positions[index] = p;
                    Masses[index] = mass;
                    InvMasses[index] = 1 / Masses[index];
                    // Velocities[index] = PossibleDirections[rand.Next(0, 249)]*0.02f;
                }
            }   
        }
    }

    
}

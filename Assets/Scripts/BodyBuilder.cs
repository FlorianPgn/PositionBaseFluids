using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BodyBuilder
{
    public static Body CreateCube(Vector3 pos, int width=1, int particlesPerUnit=10)
    {
        int nbParticles = 8;
        Body b = new Body(nbParticles);
        b.ParticlesRadius = 0.1f;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2; k++)
                {
                    Vector3 p = new Vector3(
                        pos.x-width/2.0f+i,
                        pos.y-width/2.0f+j, 
                        pos.z-width/2.0f+k);
                    int index = k + j * 2 + i * 4;
                    b.Positions[index] = p;
                    b.Masses[index] = 1f;
                    b.InvMasses[index] = 1 / b.Masses[index];
                }
            }   
        }
        /*int nbParticles = (int) Mathf.Pow(particlesPerUnit * width, 3);
        Body b = new Body(nbParticles);
        b.ParticlesRadius = 1.0f / particlesPerUnit;
        for (int i = 0; i < particlesPerUnit * width; i++)
        {
            for (int j = 0; j < particlesPerUnit * width; j++)
            {
                for (int k = 0; k < particlesPerUnit * width; k++)
                {
                    Vector3 p = new Vector3(
                        pos.x+(i/(float)particlesPerUnit),
                        pos.y+(j/(float)particlesPerUnit), 
                        pos.z+(k/(float) particlesPerUnit));
                    int index = k + j * particlesPerUnit * width + i * particlesPerUnit * width * particlesPerUnit * width;
                    b.Positions[index] = p;
                    b.Masses[index] = 1f;
                    b.InvMasses[index] = 1 / b.Masses[index];
                }
            }   
        }*/
        ShapeConstraint c = new ShapeConstraint(b);
        b.AddConstraint(c);
        
        return b;
    }
}

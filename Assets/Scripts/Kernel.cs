using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kernel
{
    private float _radius;
    private float _radiusSqr;
    private readonly float _poly6; 
    private readonly float _spikyDeriv; 
    
    public Kernel(float kernelRadius)
    {
        _radius = kernelRadius;
        _radiusSqr = _radius * _radius;
        _poly6 = 315 / (64 * Mathf.PI * Mathf.Pow(_radius, 9));
        _spikyDeriv = 45 / (Mathf.PI * Mathf.Pow(_radius, 6));
    }

    public float Poly6(Vector3 pos)
    {
        float rSq = pos.sqrMagnitude;
        // if 0 ≤ r ≤ h then (_radiusSqr - rSq)^3 else 0
        return Mathf.Max(0, _poly6*Mathf.Pow(_radiusSqr - rSq, 3));
    }

    // Debrun’s spiky kernel gradient
    public Vector3 SpikyDeriv(Vector3 pos)
    {
        float r = pos.magnitude;
        // if 0 ≤ r ≤ h then (_radius - r)^3 else 0
        float grad = Mathf.Max(0, Mathf.Pow(_radius - r, 3));
        return _spikyDeriv * grad * pos / r;
    }
}

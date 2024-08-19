using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShapeModifier : Singleton<ParticleShapeModifier>
{
    public ParticleSystem fog;

    public void FogMoveMent()
    {
        var shape = fog.shape;

        shape.radius += 0.1f;

    }

}


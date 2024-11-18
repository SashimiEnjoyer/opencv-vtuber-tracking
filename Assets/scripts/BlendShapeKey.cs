using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendShapeKey 
{
    int index;
    float weight;

    public BlendShapeKey(int _index, float _weight)
    {
        index = _index;
        weight = _weight;
    }
}

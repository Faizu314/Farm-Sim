using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomatoSeed : PlantElement
{

    protected override void Function(float deltaTime)
    {
        ExchangeCompoundsWithSustainer(deltaTime);
        ExchangeCompoundsWithOffspring(deltaTime);
    }

    protected override void Grow(float deltaTime)
    {
        throw new System.NotImplementedException();
    }
    protected override void Morph()
    {
        throw new System.NotImplementedException();
    }
    protected override void Offspring()
    {
        throw new System.NotImplementedException();
    }
}

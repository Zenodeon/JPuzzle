using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    public static float RangedMapClamp(float value, float InMinimum, float InMaximum, float OutMinimum, float OutMaximum)
    {
        var InRange = InMaximum - InMinimum;
        var OutRange = OutMaximum - OutMinimum;
        var finalValue = ((value - InMinimum) * OutRange / InRange) + OutMinimum;

        return finalValue;
    }
}

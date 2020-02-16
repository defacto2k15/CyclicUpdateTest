using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LongUpdateObject : MonoBehaviour
{
    public int IterationCount;

    void Start()
    {
        
    }

    void Update()
    {
        double sum = Random.Range(1,1000000);
        for (int i = 0; i < IterationCount; i++)
        {
            sum = Math.Pow(sum, 1 + Random.Range(0.999f, 1.001f));
        }

        name = sum.ToString();
    }
}
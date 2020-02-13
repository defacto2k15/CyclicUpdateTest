using System;
using System.Collections.Generic;
using UnityEngine;

public class CyclicUpdateRegistry : BaseCyclicLoopRegistry 
{
    private static CyclicUpdateGroup _updateGroup;
    private static BaseCyclicLoopRegistry instance;

    public static void RegisterUpdateMethod(Action updateAction, MonoBehaviour component, float timeBetweenUpdates)
    {
        BaseCyclicLoopRegistry.RegisterUpdateMethod(updateAction, component, timeBetweenUpdates, ref _updateGroup);
    }

    public static void RemoveUpdateMethod(Action updateAction, MonoBehaviour component)
    {
        BaseCyclicLoopRegistry.RemoveUpdateMethod(updateAction, component, ref _updateGroup);
    }

    void Update()
    {
        Loop(ref instance, ref _updateGroup);
    }

    void OnDestroy()
    {
        base.MyOnDestroy(ref instance);

    }

    void OnDisable()
    {
        base.MyOnDisable(ref instance);
    }
}
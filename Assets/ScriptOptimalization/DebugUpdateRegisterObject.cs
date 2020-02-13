using UnityEngine;
using UnityEditor;
using System;

public class DebugUpdateRegisterObject : MonoBehaviour
{
    public bool UpdateName = true;
    public float TimeBetweenUpdates;
    private float LastUpdateTime;

    public float TimeBetweenLateUpdates;
    private float LastLateUpdateTime;

    private float _updateDeltaTimeSum;
    private int _updateCount;

    private float _lateUpdateDeltaTimeSum;
    private int _lateUpdateCount;


    void Start()
    {
        if (TimeBetweenUpdates > 0) { CyclicUpdateRegistry.RegisterUpdateMethod(MyUpdate, this, TimeBetweenUpdates); };
        if (TimeBetweenLateUpdates > 0) { CyclicLateUpdateRegistry.RegisterUpdateMethod(MyLateUpdate, this, TimeBetweenLateUpdates); };
    }

    private void MyUpdate()
    {
        if(LastUpdateTime > 0)
        {
            _updateCount++;
            _updateDeltaTimeSum += Time.time - LastUpdateTime;
        }
        //Debug.Log($"Update:{name} Delta:{Time.time-LastUpdateTime - TimeBetweenUpdates} Proc:{100 * (Time.time - LastUpdateTime)/ TimeBetweenUpdates}%");
        LastUpdateTime= Time.time;
        UpdateObjectName();
    }


    private void MyLateUpdate()
    {
        if (LastLateUpdateTime > 0)
        {
            _lateUpdateCount++;
            _lateUpdateDeltaTimeSum += Time.time - LastLateUpdateTime;
        }
        //Debug.Log($"LateUpdate:{name} Delta:{Time.time-LastLateUpdateTime - TimeBetweenLateUpdates} Proc:{100 * (Time.time - LastLateUpdateTime) / TimeBetweenLateUpdates}%");
        LastLateUpdateTime = Time.time;
        UpdateObjectName();
    }

    private void UpdateObjectName()
    {
        var avgUpdate = _updateDeltaTimeSum / _updateCount;
        var avgLateUpdate = _lateUpdateDeltaTimeSum / _lateUpdateCount;

        if (UpdateName)
        {
            name = $"Update perc: {100 * avgUpdate / TimeBetweenUpdates}% LatePerc {100 * avgLateUpdate / TimeBetweenLateUpdates}%";
        }
    }

    void OnDestroy()
    {
        if(TimeBetweenUpdates > 0 ){CyclicUpdateRegistry.RemoveUpdateMethod(MyUpdate,this);}
        if(TimeBetweenLateUpdates > 0 ){CyclicLateUpdateRegistry.RemoveUpdateMethod(MyLateUpdate,this);}
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.PlayerLoop;
using UnityEngine.Profiling;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;
using Random = System.Random;

public class BaseCyclicLoopRegistry  : MonoBehaviour
{
    private bool _amISingleton;

    protected void Loop(ref BaseCyclicLoopRegistry singletonInstance, ref CyclicUpdateGroup group)
    {
        MakeSureGroupWasCreated( ref group);
        if (!_amISingleton)
        {
            if (singletonInstance == null)
            {
                _amISingleton = true;
                singletonInstance = this;
            }
        }
        if (_amISingleton)
        {
            ProcessUpdateLoop(group);
        }
    }

    private void ProcessUpdateLoop(CyclicUpdateGroup group)
    {
        group.Loop();
    }

    protected  void MyOnDestroy(ref BaseCyclicLoopRegistry instance)
    {
        YieldBeingSingleton(ref instance);
    }

    protected  void MyOnDisable(ref BaseCyclicLoopRegistry instance)
    {
        YieldBeingSingleton(ref instance);
    }

    private void YieldBeingSingleton(ref BaseCyclicLoopRegistry instance)
    {
        if (_amISingleton)
        {
            _amISingleton = false;
            instance = null;
        }
    }

    protected  static void RegisterUpdateMethod(Action updateAction, MonoBehaviour component, float timeBetweenUpdates, ref CyclicUpdateGroup group)
    {
        MakeSureGroupWasCreated( ref group);
        group.RegisterUpdateMethod(updateAction, component, timeBetweenUpdates);
    }

    protected static void RemoveUpdateMethod(Action updateAction, MonoBehaviour component, ref CyclicUpdateGroup group )
    {
        MakeSureGroupWasCreated( ref group);
        group.RemoveUpdateMethod(component, updateAction);
    }

    private static void MakeSureGroupWasCreated(ref CyclicUpdateGroup group)
    {
        if (group == null)
        {
            group = new CyclicUpdateGroup();
        }
    }
}

public class CyclicUpdateRegisterOrder
{
    public MonoBehaviour Component;
    public Action UpdateAction;
    public float TimeBetweenUpdates;
    public bool OrderIsCancelled;
}

public class UpdateeWithMethod 
{
    public MonoBehaviour Component;
    public Action UpdateAction;
    public float UpdateOffset;
}


public class CyclicUpdateGroup
{
    private Dictionary<Type, PerTypeCyclicUpdateBag> _perTypeDictionary;
    private Random _random;

    public CyclicUpdateGroup()
    {
        _perTypeDictionary = new Dictionary<Type, PerTypeCyclicUpdateBag>();
        _random = new Random(1);
    }

    public void RegisterUpdateMethod(Action updateAction, MonoBehaviour component, float timeBetweenUpdates)
    {
        if (!_perTypeDictionary.ContainsKey(component.GetType()))
        {
            _perTypeDictionary[component.GetType()] = new PerTypeCyclicUpdateBag(_random, timeBetweenUpdates);
        }

        var bag = _perTypeDictionary[component.GetType()];
        Assert.AreApproximatelyEqual(bag.TimeBetweenUpdates, timeBetweenUpdates, 0.01f);
        bag.AddNewUpdatee(component, updateAction);
    }

    public void RemoveUpdateMethod(MonoBehaviour component, Action updateAction)
    {
        _perTypeDictionary[component.GetType()].RemoveUpdatee(updateAction);
    }

    public void Loop()
    {
        foreach (var bag in _perTypeDictionary.Values)
        {
            bag.Loop();
        }
    }
}


public class PerTypeCyclicUpdateBag
{
    private Random _random;
    private float _timeBetweenUpdates;
    private SortedList<UpdateOffsetWithAction, UpdateeWithMethod> _updatees;
    private Dictionary<Action, UpdateOffsetWithAction> _actionToKey;
    private int? _currentIndex;

    public PerTypeCyclicUpdateBag(Random random, float timeBetweenUpdates)
    {
        _random = random;
        _timeBetweenUpdates = timeBetweenUpdates;
        _updatees = new SortedList<UpdateOffsetWithAction, UpdateeWithMethod>();
        _actionToKey = new Dictionary<Action, UpdateOffsetWithAction>();
    }

    public float TimeBetweenUpdates => _timeBetweenUpdates;

    public void AddNewUpdatee(MonoBehaviour component, Action updateAction)
    {
        var updateOffset = (float)(_random.NextDouble() * _timeBetweenUpdates);
        var key = new UpdateOffsetWithAction()
        {
            Action = updateAction,
            UpdateOffset = updateOffset
        };
        Assert.IsFalse(_actionToKey.ContainsKey(updateAction));
        _actionToKey[updateAction] = key;

        var updatee = new UpdateeWithMethod()
        {
            Component = component,
            UpdateOffset = updateOffset,
            UpdateAction = updateAction,
        };
        _updatees.Add(key, updatee);

        if (_updatees.Count == 1)
        {
            Assert.IsFalse(_currentIndex.HasValue);
            _currentIndex = 0;
        }
        else
        {
            Assert.IsTrue(_currentIndex.HasValue);
            var indexOfNewUpdatee = _updatees.IndexOfKey(key);
            if (indexOfNewUpdatee <= _currentIndex.Value)
            {
                _currentIndex++;
            }
            else
            {
                //updatee.NextUpdateTime = CalculateNextUpdateTime(updateOffset, 0);
            }
        }
    }

    public void RemoveUpdatee(Action updateAction)
    {
        var key = _actionToKey[updateAction];
        _actionToKey.Remove(updateAction);

        var updateeIndex = _updatees.IndexOfKey(key);
        _updatees.RemoveAt(updateeIndex);
        if (_updatees.Count == 0)
        {
            _currentIndex = null;
        }
        else
        {
            if (_currentIndex.Value > updateeIndex)
            {
                _currentIndex--;
            }
        }
    }

    private bool once = false;
    private float _previousUpdateTime;
    public void Loop()
    {
        var olderUpdateTime = _previousUpdateTime;
        var currentUpdateTime = Time.time;
        _previousUpdateTime = currentUpdateTime;
        if (_updatees.Count == 0)
        {
            return;
        }

        if (!once)
        {
            foreach (var v in _updatees.Keys)
            {
                UnityEngine.Debug.Log(v.UpdateOffset);
            }

            once = true;
        }

        List<Action> actionsToDelete=null;
        var newCurrentIndex = _currentIndex;

        var currentCycleIndex = CalculateCycleIndex(currentUpdateTime);
        var currentCycleOffset = CalculateOffset(currentUpdateTime);
        var previousCycleIndex = CalculateCycleIndex(olderUpdateTime);
        var previousCycleOffset = CalculateOffset(olderUpdateTime);

        bool isPositioningPass = false;
        if (currentCycleIndex > previousCycleIndex + 1)
        {
            isPositioningPass = true;
        }else if (currentCycleIndex == previousCycleIndex + 1)
        {
            if (currentCycleOffset > previousCycleOffset)
            {
                isPositioningPass = true;
            }
        }

        var timesUpdateWasCalled = 0;
        var currentElementOffset = _updatees.ElementAt(_currentIndex.Value).Value.UpdateOffset;
        bool weExpectOverflow = isPositioningPass && currentElementOffset < currentCycleOffset;

        for (int i = 0; i < _updatees.Count; i++)
        {
            var idx = (i + _currentIndex.Value) % _updatees.Count;
            if (idx == 0 && i > 0)
            {
                weExpectOverflow = false;
            }

            newCurrentIndex = idx;
            var element = _updatees.ElementAt(idx).Value;

            if (element.Component == null)
            {
                if (actionsToDelete == null)
                {
                    actionsToDelete=new List<Action>();
                }
                actionsToDelete.Add(element.UpdateAction);
            }
            else
            {
                var elementUpdateOffset = element.UpdateOffset;
                if (isPositioningPass)
                {
                    if (elementUpdateOffset < currentCycleOffset || weExpectOverflow)
                    {

                    }
                    else
                    {
                        break;
                    }
                }else 
                {
                    if (currentCycleOffset > previousCycleOffset)
                    {
                        if (elementUpdateOffset >= previousCycleOffset && elementUpdateOffset < currentCycleOffset)
                        {
                            timesUpdateWasCalled++;
                            element.UpdateAction();
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (elementUpdateOffset > previousCycleOffset || elementUpdateOffset < currentCycleOffset)
                        {
                            timesUpdateWasCalled++;
                            element.UpdateAction();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }

        if (isPositioningPass)
        {
            foreach (var updateeWithMethod in _updatees)
            {
                timesUpdateWasCalled++;
                updateeWithMethod.Value.UpdateAction();
            }
        }

        //UnityEngine.Debug.Log($"Cycle: {currentCycleIndex} CurOff:{currentCycleOffset} PrevCyc:{previousCycleIndex} PrefOffs:{previousCycleOffset} Index:{_currentIndex.Value} OldI:{oldIndex} TM:{times++} ");
        if (timesUpdateWasCalled > 0)
        {
            UnityEngine.Debug.Log("TimesWeUpdated: " + timesUpdateWasCalled);

        }

        _currentIndex = newCurrentIndex;

        actionsToDelete?.ForEach(RemoveUpdatee);
    }

    private int CalculateCycleIndex(float time)
    {
        return Mathf.FloorToInt(time/ _timeBetweenUpdates);
    }

    private float CalculateOffset(float time)
    {
        return time - CalculateCycleIndex(time) * _timeBetweenUpdates;
    }
}

public class UpdateOffsetWithAction : IComparable<UpdateOffsetWithAction>
{
    public float UpdateOffset;
    public Action Action;

    public int CompareTo(UpdateOffsetWithAction other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        var result1 = UpdateOffset.CompareTo(other.UpdateOffset);
        if (result1 == 0)
        {
            return Action.GetHashCode().CompareTo(other.Action.GetHashCode());
        }
        else
        {
            return result1;
        }
    }
}

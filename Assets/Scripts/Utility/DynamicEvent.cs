using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Reflection;

public class DynamicAction
{
    public MethodInfo method;
    public UnityEngine.Object target;

    public void Invoke(object[] parameters)
    {
        Debug.LogFormat("Invoking {0} with {1}", method.Name, parameters[0]);
        method.Invoke(target, parameters);
    }
}

/// <summary>
/// a variation on the UnityEvent class which WILL pass a parameter to it's listeners
/// </summary>
/// <typeparam name="T">parameter type</typeparam>
/// <remarks>This entire class is a workaround for UnityEvent.Invoke ALWAYS using the parameter set up in the Inspector and ignoring the value that is passed to it via code.</remarks>
[Serializable]
public class DynamicEvent<T> : UnityEvent<T>
{
    protected DynamicAction[] peristantCallbacks;

    /// <summary>
    /// Invoke our listeners with a parameterised value
    /// </summary>
    /// <param name="value">value to be passed to our listeners</param>
    
    public void DynamicInvoke(T value)
    {
        if (peristantCallbacks == null)
        {
            var paramTypes = new System.Type[] { typeof(T) };
            int listenerCount = GetPersistentEventCount();
            peristantCallbacks = new DynamicAction[listenerCount];

            if (listenerCount > 0)
            {

                for (int i = listenerCount - 1; i > -1; i--)
                {
                    peristantCallbacks[i] = new DynamicAction() { target = GetPersistentTarget(i) };

                    peristantCallbacks[i].method = UnityEventBase.GetValidMethodInfo(
                        peristantCallbacks[i].target, 
                        GetPersistentMethodName(i), 
                        paramTypes
                    );

                    SetPersistentListenerState(i, UnityEventCallState.Off);
                }
            }
        }

        if(peristantCallbacks.Length > 0)
        {
            var parameters = new object[] { value };

            foreach (DynamicAction callback in peristantCallbacks)
            {
                callback.Invoke(parameters);
            }
        }

        Invoke(value);
    }

};

[Serializable]
public class DynamicEvent<T0,T1> : UnityEvent<T0,T1>
{
    protected DynamicAction[] peristantCallbacks;

    /// <summary>
    /// Invoke our listeners with a parameterised value
    /// </summary>
    /// <param name="value">value to be passed to our listeners</param>

    public void DynamicInvoke(T0 valueZero, T1 valueOne)
    {
        if (peristantCallbacks == null)
        {
            var paramTypes = new System.Type[] { typeof(T0), typeof(T1) };
            int listenerCount = GetPersistentEventCount();
            peristantCallbacks = new DynamicAction[listenerCount];

            if (listenerCount > 0)
            {

                for (int i = listenerCount - 1; i > -1; i--)
                {
                    peristantCallbacks[i] = new DynamicAction() { target = GetPersistentTarget(i) };

                    peristantCallbacks[i].method = UnityEventBase.GetValidMethodInfo(
                        peristantCallbacks[i].target,
                        GetPersistentMethodName(i),
                        paramTypes
                    );

                    SetPersistentListenerState(i, UnityEventCallState.Off);
                }
            }
        }

        if (peristantCallbacks.Length > 0)
        {
            var parameters = new object[] { valueZero, valueOne };

            foreach (DynamicAction callback in peristantCallbacks)
            {
                callback.Invoke(parameters);
            }
        }

        Invoke(valueZero, valueOne);
    }

};
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DamageableEvent : UnityEvent<Damageable> { }

public class Damageable : MonoBehaviour {

    public bool DestroyOnBreak;

    public DamageableEvent OnBreak = new DamageableEvent();
    public DamageableEvent OnDamage = new DamageableEvent();
    public DamageableEvent OnRepair = new DamageableEvent();

    public float MaxValue;
    public float Value;

    public GameObject DebrisPrefab;

    public enum DamageState
    {
        Unknown,
        Broken,
        Damaged,
        Ideal
    }

    public DamageState State()
    {
        if (Value == MaxValue) return DamageState.Ideal;
        if (Value <= 0f) return DamageState.Broken;

        return DamageState.Damaged;
    }

    public DamageState Damage(float ChangeValue)
    {
        if (ChangeValue == 0f) return State();

        float newValue = Value - ChangeValue;

        if (ChangeValue < 0)
        {
            if (newValue > MaxValue) newValue = MaxValue;
        } else
        {
            if (newValue < 0f) newValue = 0f;
        }

        byte bEvent = 0;

        if(Value > 0 && newValue == 0f)
        {
            bEvent = 1;
        } else
        {

            if (Value > newValue)
            {
                bEvent = 2;
            }

            if (Value < newValue)
            {
                bEvent = 3;
            }
        }

        Value = newValue;

        switch (bEvent)
        {
            case 1:
                Breaking();

                break;
            case 2:
                OnRepair.Invoke(this);
                break;
            case 3:
                OnDamage.Invoke(this);
                break;
        }

        return State();
    } //End Damage

    private void Breaking()
    {
        OnBreak.Invoke(this);

        if (DebrisPrefab != null)
        {
            Debug.LogFormat("Spawning debris at {0}", transform.position);
            var debris = GameObject.Instantiate(DebrisPrefab); //, transform.position, transform.rotation); //, transform.parent);
            debris.transform.position = transform.position;
            debris.transform.rotation = transform.rotation;
        }

        if (DestroyOnBreak)
        {
            gameObject.SetActive(false);
            Destroy(gameObject, 0.01f);
        }
    }
}

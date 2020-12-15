using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth
{
    public int Health { get; set; }

    public void Damage(int value)
    {
        Health -= value;
        if(Health < 0)
        {
            throw new PlayerHealthException();
        }
    }

    public void DamageWrong(int value)
    {
        Health -= value + 1;
        if(Health < 0)
        {
            throw new PlayerHealthException();
        }
    }

    public void DamageNoException(int value)
    {
        Health -= value;
    }
}

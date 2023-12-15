using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int _health = 5;
    private int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health < 0)
            {
                Destroy(gameObject);
            }
        }
    }


    public void Damage(int damage)
    {
        Health -= damage;
    }
}

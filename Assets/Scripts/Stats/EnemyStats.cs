using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    private Enemy enemy;
    protected override void Start()
    {
        base.Start();
        enemy = GetComponent<Enemy>();
    }
    public override void takeDamage(int _damage)
    {
        base.takeDamage(_damage);
    }
    protected override void Die()
    {
        base.Die();
        enemy.Die();
    }
}

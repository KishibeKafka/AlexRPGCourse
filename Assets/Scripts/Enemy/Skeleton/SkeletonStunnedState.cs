using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    private Enemy_Skeleton enemy;
    private bool landed;
    public SkeletonStunnedState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Skeleton _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.fx.InvokeRepeating("RedColorBlink", 0, .2f);

        landed = false;
        stateTimer = enemy.stunDuration;
        rb.velocity = new Vector2(-enemy.facingDir * enemy.stunDirection.x,enemy.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.fx.Invoke("CancelColorChange", 0);
    }

    public override void Update()
    {
        base.Update();
        if (!enemy.isGroundDetected())
            landed = true;
        if (landed && enemy.isGroundDetected())
            enemy.SetVelocity(0,rb.velocity.y);

        if (stateTimer < 0)
            stateMachine.ChangeState(enemy.idleState);
    }
}

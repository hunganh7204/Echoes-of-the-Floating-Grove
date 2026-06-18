using UnityEngine;

public class Eagle : EnemyBase
{
    protected override void Start()
    {
        patrolState = new FlyingPatrolState();
        chaseState = new FlyingChaseState();
        returnState = new FlyingReturnState();
        attackState = new FlyingAttackState();
        base.Start();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiHealth : BaseStats
{
    AiAgent modifiedAiAgent;

    protected override void OnStart()
    {
        modifiedAiAgent = GetComponent<AiAgent>();
    }

    protected override void OnDeath(Vector3 _direction)
    {
        AiDeathState deathState = modifiedAiAgent.GetAiStateMachine.GetState(AiStateId.Death) as AiDeathState;
        modifiedAiAgent.GetAiStateMachine.ChangeState(AiStateId.Death);
        modifiedAiAgent.AiWeapon.SetFiring(false);
        Destroy(this.gameObject, 8f);
    }

    protected override void OnDamage(Vector3 _direction)
    {

    }

}

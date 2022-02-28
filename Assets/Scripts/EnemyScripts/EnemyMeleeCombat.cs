using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeCombat : MonoBehaviour
{

    Animator animator;

    int meleeAnimId;

    [SerializeField] AnimationClip[] clips;

    [SerializeField] float attackTime;
    const string meleeAnimName = "Standing Melee Attack Horzontal";
    [SerializeField] Animation combatAnim;

    void Start()
    {
        animator = GetComponent<Animator>();
        meleeAnimId = Animator.StringToHash("MAttack");

    }


    public void Attack(BaseStats _targetStats)
    {
        StartCoroutine(AttackAnimation(attackTime, _targetStats));
    }

    IEnumerator AttackAnimation(float _animTime, BaseStats _targetStats)
    {
        animator.SetTrigger(meleeAnimId);
        Soundmanager.PlaySound(Soundmanager.Sound.MeleeWoosh, transform.position);
        yield return new WaitForSeconds(2.2f);
        _targetStats.TakeDamage(2f);
    }
}

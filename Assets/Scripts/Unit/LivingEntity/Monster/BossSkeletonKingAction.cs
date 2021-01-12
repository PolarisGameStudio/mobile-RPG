﻿using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using System;

public class BossSkeletonKingAction : MonsterAction
{
    enum AttackType { AIR_ATTACK , ATTACK1,ATTACK2, BLACKHOLE, JUMP_ATTACK , SUMMON}

    [SerializeField] private GameObject attackPos;
    //[SerializeField] private Transform _baseMeleeAttackPos;
    //[SerializeField] private GameObject _baseMeleeAttackPrefab;

    AttackType attackType;
    [SerializeField] private GameObject JumpSkillRange;

    [SerializeField] private GameObject AttackRange;
    [SerializeField] private GameObject AttackEffect;

    [SerializeField] private GameObject AirSkillRange;
    [SerializeField] private GameObject AirSkillEffect;

    [SerializeField] private GameObject BlackHoleRange;
    [SerializeField] private GameObject BlackHoleEffect;

    [SerializeField] private GameObject skeleton_grunt;
    [SerializeField] private GameObject skeleton_sword;

    [SerializeField] float defalutAtkCastingTime;
    [SerializeField] float blackHoleCastingTime;
    [SerializeField] float summonCastingTime;
    [SerializeField] float AirSkillCastingTime;
    

    List<Transform> ProjectileList = new List<Transform>();

    private GameObject currentTarget;

    private float velocity;
    private float angle;

    string currentAnimation;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _findRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }


    protected override void DoAttack()
    {
     
        StopCoroutine(_attackCoroutine);
        _attackCoroutine = null;
        _readyCast = false;

        MakeEffect();

        _navMeshAgent.isStopped = false;
        _navMeshAgent.acceleration = 8f;
        CameraManager.Instance.ShakeCamera(3, 1, 0.5f);
        currentTarget = _target;
        ChangeState(MONSTER_STATE.STATE_TRACE);

    }

    private void MakeEffect()
    {

        switch (attackType)
        {        
            case AttackType.ATTACK1: case AttackType.ATTACK2: case AttackType.JUMP_ATTACK:
                DefalutAttackEffect();
                break;         
            case AttackType.SUMMON:              
                break;
            default:
                break;
        }
    }
    private void DefalutAttackEffect()
    {      
        transform.LookAt(_target.transform.position);
        BossAttack atk = ObjectPoolManager.Instance.GetObject(AttackEffect).GetComponent<BossAttack>();
        atk.SetParent(gameObject , attackPos.transform);
        atk.PlayAttackTimer(0.5f);
        atk.OnLoad(gameObject, attackPos);

    }

    private void AirAttackEffect(Transform target)
    {
        BossAttack atk = ObjectPoolManager.Instance.GetObject(AirSkillEffect).GetComponent<BossAttack>();
        atk.SetParent(gameObject , target);
        atk.PlayAttackTimer(0.5f);
        atk.OnLoad(currentTarget, currentTarget);
    }

    private void BlackHoleAttackEffect(Transform target)
    {
        BossAttack atk = ObjectPoolManager.Instance.GetObject(BlackHoleEffect).GetComponent<BossAttack>();
        atk.SetParent(gameObject, target);
        atk.PlayAttackTimer(4.5f);
        atk.OnLoad(currentTarget, currentTarget);

    }
    protected void ComboAttack()
    {
        //애니메이터 호출용
        _monster.myAnimator.SetTrigger("Attack1");
        currentAnimation = "Attack1";

    }
    protected override void SetAttackAnimation()
    {
        
        switch (attackType)
        {           
            case AttackType.ATTACK1:
                _monster.myAnimator.SetTrigger("Attack0");
                currentAnimation = "Attack0";
                break;
            case AttackType.ATTACK2:
                _monster.myAnimator.SetTrigger("Attack1");
                currentAnimation = "Attack1";
                break;                      
            case AttackType.SUMMON:
                _monster.myAnimator.SetTrigger("Summon");
                currentAnimation = "Summon";
                break;
            default:
                break;
        }
       
    }

    protected override void SpawnStart()
    {
        _isImmune = true; // 스폰 중 무적
        _monster.myAnimator.SetTrigger("Spawn");
    }
    protected override void SpawnExit()
    {
        base.SpawnExit();
        currentTarget = _target;
    }
    private void AttackCorotineInit()
    {
        _attackCoroutine = null;
        ChangeState(MONSTER_STATE.STATE_TRACE);
    }

    protected override void CastStart()
    {

        // 플레이어가 기절상태나 넘어짐 상태면 우선 공격 모션 2개 있음.
        // if(_target.getState? == 기절) attackType = AttackType.~~~~

        //if (_attackCoroutine != null) Invoke("AttackCorotineInit", 1.5f);

        

        if(Vector3.Distance(transform.position , _target.transform.position) <= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.isStopped = true;
            _monster.myAnimator.SetTrigger("Idle");
        }
        else
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_target.transform.position);
            _monster.myAnimator.SetTrigger("Walk");
        }
       
        int proc = UnityEngine.Random.Range(0, 100);

        if (proc <= 25)
        {
            _castTime = defalutAtkCastingTime;
            attackType = AttackType.ATTACK1;
        }
        else if (proc <= 50)
        {
            _castTime = defalutAtkCastingTime;
            attackType = AttackType.ATTACK2;
        }
        else if (proc <= 75)
        {
            _castTime = AirSkillCastingTime;
            attackType = AttackType.AIR_ATTACK;
        }
        else if (proc <= 100)
        {
            _castTime = blackHoleCastingTime;
            attackType = AttackType.BLACKHOLE;
        }

    }

    protected override void DoCastingAction()
    {
        _cntCastTime += Time.deltaTime;
        _bar.CastUpdate();

        if (attackType != AttackType.ATTACK2 && attackType != AttackType.ATTACK1)
        {
            _navMeshAgent.isStopped = true;
            if (!_monster.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("HoldAttack"))
                _monster.myAnimator.SetTrigger("HoldAttack");
        }

        if (_cntCastTime >= _castTime)
        {
            _cntCastTime = 0;
            _readyCast = true;
            ChangeState(MONSTER_STATE.STATE_ATTACK);
        }

    }

    protected override void LookTarget()
    {

    }

    protected override void SetAttackType()
    {
        if (_readyCast) return;
    }

    public override void InitState()
    {
        _currentState = MONSTER_STATE.STATE_NULL;
        ChangeState(MONSTER_STATE.STATE_IDLE);
        _navMeshAgent.enabled = true;
        _navMeshAgent.speed = _monster.speed;
        currentTarget = _target;

    }

    protected override void AttackExit()
    {
    }

    public override void MoveToTarget()
    {

        _navMeshAgent.SetDestination(_target.transform.position);

        if (Vector3.Distance(_target.transform.position, _monster.transform.position) < _attackRange)
        {
            ChangeState(MONSTER_STATE.STATE_CAST);            
        }

    }

    protected override IEnumerator AttackTarget()
    {

        while (true)
        {
            yield return null;

            AttackAction();
            
            yield return new WaitForSeconds(_attackSpeed);
            SetAttackAnimation();
            
            // 사운드 재생

            yield return new WaitForSeconds(_monster.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);

            _readyCast = false;
            //if (!_readyCast && ToCast()) break;
            ChangeState(MONSTER_STATE.STATE_TRACE);

            break;
            
        }
    }

    private void AttackAction()
    {
        if (attackType != AttackType.JUMP_ATTACK)
            _monster.myAnimator.SetTrigger("Walk");
        _navMeshAgent.SetDestination(_target.transform.position);

        switch (attackType)
        {
            case AttackType.AIR_ATTACK:
                StartCoroutine(AirAction());
                break;
            case AttackType.ATTACK1: case AttackType.ATTACK2:
                StartCoroutine(DefalutAttackAction());
                break;
            case AttackType.BLACKHOLE:
                StartCoroutine(BlackHoleAction());
                break;
            case AttackType.JUMP_ATTACK:
                StartCoroutine(JumpAttackAction());
                break;
            case AttackType.SUMMON:
                break;
            default:
                break;
        }

    }

    private IEnumerator JumpAttackAction()
    {
        _navMeshAgent.stoppingDistance = 0f;
        currentTarget = _target;
        _navMeshAgent.SetDestination(_target.transform.position);
        _navMeshAgent.isStopped = false;
        _navMeshAgent.speed = _monster.speed * 1.5f;
        _navMeshAgent.acceleration = 500f;
        transform.LookAt(_target.transform.position);

        _monster.myAnimator.SetTrigger("Jump");
        currentAnimation = "Jump";

        yield return new WaitForSeconds(_monster.myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        _navMeshAgent.acceleration = 8f;
        _navMeshAgent.speed = _monster.speed;
        _navMeshAgent.stoppingDistance = 3f;
    }

    private IEnumerator AirAction()
    {

        _monster.myAnimator.SetTrigger("HoldAttack");
        yield return null;

        for (int i = 0; i < 10; i++)
        {
            GameObject range = ObjectPoolManager.Instance.GetObject(AirSkillRange);
            range.GetComponent<BossSkillRange>().RemovedRange(gameObject, _attackSpeed);
            range.transform.position = new Vector3(UnityEngine.Random.Range(transform.position.x - 20, transform.position.x + 20), _target.transform.position.y, UnityEngine.Random.Range(transform.position.z - 20, transform.position.z + 20));
            ProjectileList.Add(range.transform);
        }

        yield return new WaitForSeconds(_attackSpeed);

        foreach (Transform item in ProjectileList)
        {
            AirAttackEffect(item);
        }

        ProjectileList.Clear();

        DoAttack();
    }

    private IEnumerator DefalutAttackAction()
    {       
        yield return null;

        _navMeshAgent.SetDestination(_target.transform.position);

        GameObject range = ObjectPoolManager.Instance.GetObject(AttackRange);
        range.GetComponent<BossSkillRange>().RemovedRange(gameObject, _attackSpeed);
        range.GetComponent<BossSkillRange>().setFollow();

    }

    private IEnumerator BlackHoleAction()
    {

        _monster.myAnimator.SetTrigger("HoldAttack");
        yield return null;

        GameObject range = ObjectPoolManager.Instance.GetObject(BlackHoleRange);
        range.GetComponent<BossSkillRange>().RemovedRange(_target, _attackSpeed);
        range.transform.position = _target.transform.position;

        yield return new WaitForSeconds(_attackSpeed);

        BlackHoleAttackEffect(range.transform);

        DoAttack();
    }


    protected override void TraceStart()
    {
        StopCoroutine(_attackCoroutine);
        _attackCoroutine = null;
        _monster.myAnimator.ResetTrigger("Walk");

        if(Vector3.Distance(transform.position , _target.transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _monster.myAnimator.SetTrigger("Idle");
        }
        else
        {
        _monster.myAnimator.SetTrigger("Walk");
        }
        _navMeshAgent.speed = _monster.speed * 1.5f;
        _navMeshAgent.isStopped = false;
    }
    protected override void TraceUpdate()
    {
        MoveToTarget();
    }

    protected override void AttackStart()
    {
        if (!_readyCast && ToCast()) return;
        else
        {
            if (_attackCoroutine == null)
                _attackCoroutine = StartCoroutine(AttackTarget());
            //else
            //    ChangeState(MONSTER_STATE.STATE_IDLE);
        }
        
    }

    public override void Damaged(float dmg, bool SetAnimation = false)
    {
        if (MasteryManager.Instance.currentMastery.currentMasteryChoices[5] == 1)
        {
            base.Damaged(dmg * 1.2f, false);

        }
        else
        {
            base.Damaged(dmg, false);
        }
    }

    protected override void AttackUpdate() { }

    protected override void RigidExit()
    {
        base.RigidExit();
        _attackCoroutine = null;
    }
    protected override void StunExit()
    {
        base.StunExit();
        _attackCoroutine = null;

    }
    protected override void FallStart()
    {
        Debug.Log("넘어짐 면역");
    }
    protected override void FallExit() { }

    protected override void KillStart()
    {
        StopAllCoroutines();
        _monster.myAnimator.SetTrigger("Laugh");
        transform.LookAt(_target.transform.position);
    }

    protected override void KillUpdate()
    {
        if (!_monster.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Laugh"))
        {
            _monster.myAnimator.SetTrigger("Laugh");
        }
    }
}

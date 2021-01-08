﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack: MonoBehaviour
{
    [SerializeField] protected Collider _collider;
    [SerializeField] protected GameObject _particleEffectPrefab;
    [SerializeField] private int targetNumber;
    protected HashSet<GameObject> _attackedTarget;
    protected GameObject _baseParent;

    protected bool _isParentPlayer;
    [SerializeField] protected bool _useFixedDmg;
    [SerializeField] protected bool _useMeleeDmg;

    [SerializeField] protected float _damage;

    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
        _attackedTarget = new HashSet<GameObject>();
    }

    protected virtual void OnEnable()
    {
        GameObject Effect = ObjectPoolManager.Instance.GetObject(_particleEffectPrefab);
        Effect.transform.position = Player.Instance.skillPoint.position;
        Effect.transform.rotation = Quaternion.LookRotation(Player.Instance.transform.forward);
        if(Player.Instance.currentCombo == 1)
        {
            Effect.transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        else if(Player.Instance.currentCombo == 2)
        {
            Effect.transform.Rotate(new Vector3(0f, 0f, 180));
        }
        else
        {
            Effect.transform.Rotate(new Vector3(0f, 0f, 270f));
        }

        if (_attackedTarget != null)
        {
            _attackedTarget.Clear();
        }
    }

    public void SetParent(GameObject parent)
    {
        _baseParent = parent;
        if (parent.GetComponent<Player>() != null) _isParentPlayer = true;
        else _isParentPlayer = false;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (CanCollision(other))
        {
            if (_attackedTarget.Count <= targetNumber)
            {
                _attackedTarget.Add(other.gameObject);
            }
        }
    }

    private bool CanCollision(Collider other)
    {
        if ((_isParentPlayer && (other.CompareTag("Boss") || other.CompareTag("Monster"))) || // 부모가 플레이어 && 충돌체가 적
            (!_isParentPlayer && other.CompareTag("Player"))) // 부모가 적 && 충돌체가 플레이어
        {
            if (!_attackedTarget.Contains(other.gameObject)) return true;
        }

        return false;
    }

    public void SetEnableCollider(bool active)
    {
        _collider.enabled = active;
    }

    public void PlayAttackTimer(float time)
    {
        StartCoroutine(SetColliderTimer(time));
    }

    private IEnumerator SetColliderTimer(float time)
    {
        _collider.enabled = true;
        yield return new WaitForSeconds(time);
        _collider.enabled = false;
        ObjectPoolManager.Instance.ReturnObject(gameObject);
    }
}
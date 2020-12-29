﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleInputNamespace;
using UnityEngine.EventSystems;
public class Player : LivingEntity
{
    public static Player Instance;
    
    private bool avoidButtonClick = false;
    [SerializeField] private float avoid_power = 10f;
    private float count = 0f;
    private State saveState = null;

    private bool AttackButtonClick = false;
    private bool SkillA_ButtonClick = false;
    private bool SkillB_ButtonClick = false;
    private bool SkillC_ButtonClick = false;

    //[SerializeField] public GameObject skillAEffect;
    //[SerializeField] public GameObject skillBEffect;
    //[SerializeField] public GameObject skillCEffect;
    //[SerializeField] public GameObject attackEffect;
    
    [SerializeField] public Transform firePoint;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    [SerializeField] private Transform cam;

    private Vector3 direction;
    private Vector3 moveDir;
    [SerializeField] private Quaternion rotateAngle;

    public Joystick joystick = null;
    
    float horizontal;
    float vertical;
    bool isdead = false;
    PartSelection selection;
    FaceCam faceCam;

    Dictionary<string, int> playerItems = new Dictionary<string, int>();

    public WeaponManager weaponManager;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        weaponManager.SetWeapon("SWORD");
        faceCam = GameObject.Find("PlayerFaceCam").GetComponent<FaceCam>();
        selection = GetComponent<PartSelection>();
        selection.Start();
        selection.Init();
        faceCam.Init(transform.Find("PlayerAvatar").gameObject);
        //joystick = GameObject.Find("Joystick").GetComponent<Joystick>();
    }

    protected override void Update()
    {
        // if(joystick == null) joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        if (joystick == null) joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();

        if (!GameManager.Instance.isInteracting) // 상호작용 중이지 않을 때
        {
            if(Input.GetKeyDown(KeyCode.O))
            {
                weaponManager.SetWeapon("WAND");
            }

            PlayerAvoidance();

            PlayerMove();

            weaponManager.UpdateWeapon();

            selection.Update();

            if (_hp <= 0 && !isdead)
            {
                MyStateMachine.SetState("DIE");
                isdead = true;
            }
        }

        CheckInteractObject();
    }

    public void PlayerMove()
    {
        horizontal = joystick.GetX_axis().value;
        vertical = joystick.GetY_axis().value;

        direction = new Vector3(horizontal, 0, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        MyStateMachine.UpdateState();
    }

    public bool GetMove()
    {
        return joystick.getHold();
    }

    public void PlayerSkillA()
    {
        MyStateMachine.SetState("SKILL_A");
        GameObject skill = Instantiate(weaponManager.GetWeapon().SkillAEffect);
        skill.transform.position = firePoint.position;
        skill.transform.rotation = transform.rotation; 
    }
    public void PlayerSkillB()
    {
        MyStateMachine.SetState("SKILL_B");
        GameObject skill = Instantiate(weaponManager.GetWeapon().SkillBEffect);
        skill.transform.position = firePoint.position;
        skill.transform.rotation = transform.rotation;

    }
    public void PlayerSkillC()
    {
        MyStateMachine.SetState("SKILL_C");
        GameObject skill = Instantiate(weaponManager.GetWeapon().SkillCEffect);
        skill.transform.position = firePoint.position;
        skill.transform.rotation = transform.rotation;
    }

    public bool GetAttackButton()
    {
        return AttackButtonClick;
    }

    public void SetAttackButton(bool attackbutton)
    {
        AttackButtonClick = attackbutton;
        if(AttackButtonClick == true)
        {
            GameObject skill = Instantiate(weaponManager.GetWeapon().AttackEffect);
            skill.transform.position = firePoint.position;
            skill.transform.rotation = transform.rotation;
        }
    }

    public void PlayerAvoidance()
    {
        if (avoidButtonClick)
        {
            count += 1f;

            if (count > avoid_power)
            {
                avoidButtonClick = false;
                count = 0f;
            }
            if (direction == Vector3.zero) characterController.Move(moveDir * speed * Time.deltaTime * avoid_power);

            else characterController.Move(moveDir * speed * Time.deltaTime * avoid_power);
        }
    }
    public bool GetAvoidance()
    {
        return avoidButtonClick;
    }
    public void SetAvoidButton(bool avoidbutton)
    {
        avoidButtonClick = avoidbutton;
    }
    protected override void InitObject()
    {
        //임의 값
        _initHp = 10;
        _hp = 10;
        MyAnimator = new Animator();
        MyAnimator = GetComponent<Animator>();

        MyStateMachine = new StateMachine();
        State Idle = new StateIdle_TestPlayer(this);
        State Move = new StateMove_TestPlayer(this);
        State Attack = new StateAttack_TestPlayer(this);
        State Die = new StateDie_TestPlayer(this);
        State Avoid = new StateAvoid_TestPlayer(this);
        State SkillA = new StateSkillA_TestPlayer(this);
        State SkillB = new StateSkillB_TestPlayer(this);
        State SkillC = new StateSkillC_TestPlayer(this);

        weaponManager = new WeaponManager();

        Weapon Sword = new Sword();
        Weapon Dagger = new Dagger();
        Weapon GreatSword = new GreatSword();
        Weapon Blunt = new Blunt();
        Weapon Staff = new Staff();
        Weapon Wand = new Wand();

        MyStateMachine.AddState("IDLE", Idle);
        MyStateMachine.AddState("MOVE", Move);
        MyStateMachine.AddState("ATTACK", Attack);
        MyStateMachine.AddState("DIE", Die);
        MyStateMachine.AddState("AVOID", Avoid);
        MyStateMachine.AddState("SKILL_A", SkillA);
        MyStateMachine.AddState("SKILL_B", SkillB);
        MyStateMachine.AddState("SKILL_C", SkillC);

        weaponManager.AddWeapon("SWORD", Sword);
        weaponManager.AddWeapon("DAGGER", Dagger);
        weaponManager.AddWeapon("GREATSWORD", GreatSword);
        weaponManager.AddWeapon("BLUNT", Blunt);
        weaponManager.AddWeapon("STAFF", Staff);
        weaponManager.AddWeapon("WAND", Wand);

        MyStateMachine.SetState("IDLE");
    }

    public void CheckInteractObject()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.9f);
            for(int i=0; i<colliders.Length; i++)
            {
                if(colliders[i].GetComponent<NonLivingEntity>())
                {
                    colliders[i].GetComponent<NonLivingEntity>().Interaction();
                    break;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item itemInfo = other.gameObject.GetComponent<Item>();
            Debug.Log(itemInfo.itemName + " 아이템 획득!");
            if (playerItems.ContainsKey(itemInfo.name))
            {
                playerItems.Add(itemInfo.name, playerItems[itemInfo.itemName] + 1);
            }
            else
            {
                playerItems.Add(itemInfo.name, 1);
            }
            Destroy(other.gameObject);
        }
    }
}

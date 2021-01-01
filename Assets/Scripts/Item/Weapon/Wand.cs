﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Wand : Weapon
{
    // Start is called before the first frame update
    public Wand()
    {
        damage = 10f;
        speed = 1.5f;
        masteryLevel = 1;
        outfitGrade = 0;
        AttackEffect = Resources.Load<GameObject>("Prefab/PlayerEffect/Wand Attack");
        SkillAEffect = Resources.Load<GameObject>("Prefab/PlayerEffect/Wand Skill A");
        SkillBEffect = Resources.Load<GameObject>("Prefab/PlayerEffect/Wand Skill B");
        SkillCEffect = Resources.Load<GameObject>("Prefab/PlayerEffect/Wand Skill C");
        WeaponAnimation = Resources.Load<RuntimeAnimatorController>("Animation/Animator/Player/Wand Animator");

    }

    // Update is called once per frame
    public override void Update()
    {
        OutfitGradeCheck();
        if (Input.GetKeyDown(KeyCode.P))
        {
            masteryLevel++;
        }
    }
}

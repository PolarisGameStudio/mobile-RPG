﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Debuff { //디버프 추상클래스

    protected LivingEntity target; //타겟
    private float duration; //지속시간
    protected float elapsed; //시간
    
    public Debuff(LivingEntity target, float duration) //디버프
    {
        this.target = target;
        this.duration = duration;
    }
    public virtual void Update() //디버프 업데이트
    {
        elapsed += Time.deltaTime;
        if (elapsed >= duration)
        {
            Remove();
        }
    }
    public virtual void Remove() //디버프 삭제
    {
        if (target != null)
        {
            target.speed = target.MAXspeed;     
            target.DebuffManager.RemoveDebuff(this);
        }
    }
        
}

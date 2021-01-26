﻿////////////////////////////////////////////////////
/*
    File CardEffect.cs
    class CardEffect
    
    담당자 : 이신홍, 김기정

    데이터를 파싱하고, 실질적인 이펙트의 작동을 관리한다.
*/
////////////////////////////////////////////////////

using System.Collections.Generic;

public class CardEffect
{
    // 기본 데이터
    public CardEffectData effectData;

    // 파싱 데이터
    public float[] gradeNum;        // 레벨 별 증가량 정보

    // 기타 데이터
    public int level;               // 레벨 정보
    List<float> originalValues;     // 적용 전의 카드 효과 데이터



    ////////// 베이스 //////////

    public CardEffect(CardEffectData data)
    {
        effectData = data;
        originalValues = new List<float>();
    }





    ////////// 데이터 //////////

    /// <summary>
    /// EffectData Class 파싱 작업
    /// </summary>
    public void ParsingEffectData()
    {
        gradeNum = new float[3];
        gradeNum[0] = effectData.gradeOneValue;
        gradeNum[1] = effectData.gradeTwoValue;
        gradeNum[2] = effectData.gradeThreeValue;
    }



    /// <summary>
    // 카드 시작 효과
    /// </summary>
    public virtual void StartEffect()
    {
        if (effectData.id == 2) // 대쉬 길이 감소
        {
            originalValues.Add(Player.Instance.dashSpeed);
            Player.Instance.dashSpeed -= Player.Instance.dashSpeed * (gradeNum[level] / 100.0f);
        }
        else if (effectData.id == 3) // 대쉬 길이 증가
        {
            originalValues.Add(Player.Instance.dashSpeed);
            Player.Instance.dashSpeed += Player.Instance.dashSpeed * (gradeNum[level] / 100.0f);
        }
        else if (effectData.id == 4) // 소모 스테미너 감소
        {
            originalValues.Add(StatusManager.Instance.finalStatus.dashStamina);
            StatusManager.Instance.finalStatus.dashStamina -= StatusManager.Instance.finalStatus.dashStamina * gradeNum[level] / 100.0f;
        }
        else if (effectData.id == 5) // 소모 스테미너 증가
        {
            originalValues.Add(StatusManager.Instance.finalStatus.dashStamina);
            StatusManager.Instance.finalStatus.dashStamina += StatusManager.Instance.finalStatus.dashStamina * gradeNum[level] / 100.0f;
        }
        else if (effectData.id == 6) // 대쉬 속도 증가
        {
            originalValues.Add(Player.Instance.dashSpeed);
            Player.Instance.dashSpeed *= (gradeNum[level] / 100.0f);
        }
        else if (effectData.id == 15) // 플레이어의 데미지 증가
        {
            originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
            StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
        }
        else if (effectData.id == 21) // 남은 적 체력에 비례한 플레이어의 추가 데미지 (미구현)
        {
            originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
            StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
        }

        else if (effectData.id == 26) // 플레이어의 체력이 100%일 때 데미지 증가
        {
            if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
            {
                originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
                StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
            }
        }
        else if (effectData.id == 30) // 스킬 사용 이후 1회 평타 강화 (미구현)
        {
            if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
            {
                originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
                StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
            }
        }
        else if (effectData.id == 31) // 적 체력 80% 이상일 때, 적 받는 데미지 증가 (미구현)
        {
            if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
            {
                originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
                StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
            }
        }
        else if (effectData.id == 38) // 피격 받았을 때 플레이어 가하는 데미지 2초간 증가 (미구현)
        {
            if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
            {
                originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
                StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
            }
        }
        else if (effectData.id == 40) // 스테미너 감소
        {
            originalValues.Add(StatusManager.Instance.finalStatus.maxStamina);
            StatusManager.Instance.finalStatus.maxStamina -= gradeNum[level];
        }
        else if (effectData.id == 43) // 스테미너 증가
        {
            originalValues.Add(StatusManager.Instance.finalStatus.maxStamina);
            StatusManager.Instance.finalStatus.maxStamina += gradeNum[level];
        }
        else if (effectData.id == 53) // 플레이어 가하는 데미지 & 받는 데미지 증가
        {
            originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
            originalValues.Add(StatusManager.Instance.finalStatus.armor);
            StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
            StatusManager.Instance.finalStatus.armor -= StatusManager.Instance.finalStatus.armor * gradeNum[level] / 100.0f;
        }
        else if (effectData.id == 58) // 회피 소모 스테미너 감소
        {
            originalValues.Add(StatusManager.Instance.finalStatus.dashStamina);
            StatusManager.Instance.finalStatus.dashStamina -= StatusManager.Instance.finalStatus.dashStamina * gradeNum[level] / 100.0f;
        }
        else if (effectData.id == 59) // 플레이어 받는 데미지 증가
        {
            originalValues.Add(StatusManager.Instance.finalStatus.armor);
            StatusManager.Instance.finalStatus.armor -= StatusManager.Instance.finalStatus.armor * gradeNum[level] / 100.0f;
        }
        else if (effectData.id == 61) // 플레이어 체력 증가
        {
            originalValues.Add(StatusManager.Instance.finalStatus.maxHp);
            StatusManager.Instance.finalStatus.maxHp += StatusManager.Instance.finalStatus.maxHp * gradeNum[level] / 100.0f;
        }

        CardManager.Instance.activeEffects.Add(this); // 활성화 리스트에 이펙트 추가
    }

    public virtual void UpdateEffect()
    {
        if (effectData.id == 26) // 플레이어의 체력이 100%일 때 데미지 증가
        {
            if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
            {
                if (originalValues.Count == 0)
                    originalValues.Add(StatusManager.Instance.finalStatus.attackDamage);
                StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
                StatusManager.Instance.finalStatus.attackDamage += StatusManager.Instance.finalStatus.attackDamage * gradeNum[level] / 100.0f;
            }
            else
            {
                if (originalValues.Count > 0)
                    StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
            }
        }
    }

    public virtual void EndEffect()
    {
        if (originalValues.Count > 0)
        {
            if (effectData.id == 2) // 대쉬 길이 감소
            {
                Player.Instance.dashSpeed = originalValues[0];
            }
            else if (effectData.id == 3) // 대쉬 길이 증가
            {
                Player.Instance.dashSpeed = originalValues[0];
            }
            else if (effectData.id == 4) // 소모 스테미너 감소
            {
                StatusManager.Instance.finalStatus.dashStamina = originalValues[0];
            }
            else if (effectData.id == 5) // 소모 스테미너 증가
            {
                StatusManager.Instance.finalStatus.dashStamina = originalValues[0];
            }
            else if (effectData.id == 6) // 대쉬 속도 증가
            {
                Player.Instance.dashSpeed = originalValues[0];
            }
            else if (effectData.id == 15) // 플레이어의 데미지 증가
            {
                StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
            }
            else if (effectData.id == 21) // 남은 적 체력에 비례한 플레이어의 추가 데미지 (미구현)
            {
                StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
            }
            else if (effectData.id == 26) // 플레이어의 체력이 100%일 때 데미지 증가
            {
                StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
            }
            else if (effectData.id == 30) // 스킬 사용 이후 1회 평타 강화 (미구현)
            {
                if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
                {
                    StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
                }
            }
            else if (effectData.id == 31) // 적 체력 80% 이상일 때, 적 받는 데미지 증가 (미구현)
            {
                if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
                {
                    StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
                }
            }
            else if (effectData.id == 38) // 피격 받았을 때 플레이어 가하는 데미지 2초간 증가 (미구현)
            {
                if (Player.Instance.Hp == StatusManager.Instance.finalStatus.maxHp)
                {
                    StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
                }
            }
            else if (effectData.id == 40) // 스테미너 감소
            {
                StatusManager.Instance.finalStatus.maxStamina = originalValues[0];
            }
            else if (effectData.id == 43) // 스테미너 증가
            {
                StatusManager.Instance.finalStatus.maxStamina = originalValues[0];
            }
            else if (effectData.id == 53) // 플레이어 가하는 데미지 & 받는 데미지 증가
            {
                StatusManager.Instance.finalStatus.attackDamage = originalValues[0];
                StatusManager.Instance.finalStatus.armor = originalValues[1];
            }
            else if (effectData.id == 58) // 회피 소모 스테미너 감소
            {
                StatusManager.Instance.finalStatus.dashStamina = originalValues[0];
            }
            else if (effectData.id == 59) // 플레이어 받는 데미지 증가
            {
                StatusManager.Instance.finalStatus.armor = originalValues[0];
            }
            else if (effectData.id == 61) // 플레이어 체력 증가
            {
                StatusManager.Instance.finalStatus.maxHp = originalValues[0];
            }
        }
        CardManager.Instance.activeEffects.Remove(this);
    }

    /// <summary>
    /// Effect 정보를 반환한다.
    /// </summary>
    public string GetDescription()
    {
        string description = effectData.description;
        description = description.Replace(";", effectData.effectName);
        description = description.Replace(":", gradeNum[level].ToString());

        return description;
    }
}
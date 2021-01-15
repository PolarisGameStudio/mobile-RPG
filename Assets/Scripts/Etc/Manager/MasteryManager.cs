﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using CSVReader;

[System.Serializable]
public class WeaponSkillLevel : ICloneable
{
    public int autoAttackLevel;
    public int skillALevel;
    public int skillBLevel;
    public int skillCLevel;

    public WeaponSkillLevel()
    {
        autoAttackLevel = 1;
        skillALevel = 1;
        skillBLevel = 1;
        skillCLevel = 1;
    }

    public object Clone()
    {
        WeaponSkillLevel copy = MemberwiseClone() as WeaponSkillLevel;
        return copy;
    }
}

[System.Serializable]
public class CurrentMastery : ICloneable
{
    [Header("숙련도 및 마스터리 레벨")]
    public int currentMasteryLevel;
    public int currentSwordMasteryLevel;
    public int currentDaggerMasteryLevel;
    public int currentBluntMasteryLevel;
    public int currentStaffMasteryLevel;
    public int currentWandMasteryLevel;

    [Header("무기별 경험치 획득 상황")]
    public int currentSwordMasteryExp;
    public int currentDaggerMasteryExp;
    public int currentBluntMasteryExp;
    public int currentStaffMasteryExp;
    public int currentWandMasteryExp;

    [Header("무기별 스킬 해제 상황")]
    public bool currentSwordSkillBReleased;
    public bool currentSwordSkillCReleased;
    public bool currentDaggerSkillBReleased;
    public bool currentDaggerSkillCReleased;
    public bool currentBluntSkillBReleased;
    public bool currentBluntSkillCReleased;
    public bool currentWandSkillBReleased;
    public bool currentWandSkillCReleased;
    public bool currentStaffSkillBReleased;
    public bool currentStaffSkillCReleased;

    public List<int> currentMasteryChoices;

    public CurrentMastery()
    {
        currentMasteryLevel = 50;
        currentSwordMasteryLevel = 30;
        currentDaggerMasteryLevel = 1;
        currentBluntMasteryLevel = 1;
        currentStaffMasteryLevel = 1;
        currentWandMasteryLevel = 30;

        currentSwordMasteryExp = 0;
        currentDaggerMasteryExp = 0;
        currentBluntMasteryExp = 0;
        currentStaffMasteryExp = 0;
        currentWandMasteryExp = 0;

        currentSwordSkillBReleased = true;
        currentSwordSkillCReleased = true;
        currentDaggerSkillBReleased = false;
        currentDaggerSkillCReleased = false;
        currentBluntSkillBReleased = false;
        currentBluntSkillCReleased = false;
        currentWandSkillBReleased = true;
        currentWandSkillCReleased = true;
        currentStaffSkillBReleased = false;
        currentStaffSkillCReleased = false;
        currentMasteryChoices = new List<int>(new int[10]);
    }

    public object Clone()
    {
        CurrentMastery copy = MemberwiseClone() as CurrentMastery;
        copy.currentMasteryChoices = new List<int>();
        copy.currentMasteryChoices.AddRange(currentMasteryChoices);
        return copy;
    }
}

[CSVReader.Data("id")]
public class PlayerMasteryData
{
    public int id;
    public int level;
    public string masteryName;
    public string masteryDescription;
}

public class MasteryManager : SingletonBase<MasteryManager>
{
    public CurrentMastery currentMastery;
    public WeaponSkillLevel[] weaponSkillLevel;
    //public WeaponSkillLevel swordSkillLevel;
    //public WeaponSkillLevel daggerSkillLevel;
    //public WeaponSkillLevel bluntSkillLevel;
    //public WeaponSkillLevel wandSkillLevel;
    //public WeaponSkillLevel staffSkillLevel;
    public Dictionary<int,PlayerMasteryData> masteryDictionary;

    public void InitMasteryManager()
    {
        weaponSkillLevel = new WeaponSkillLevel[5];
        Table masteryTable = CSVReader.Reader.ReadCSVToTable("CSVData/MasteryDatabase");
        masteryDictionary = masteryTable.TableToDictionary<int, PlayerMasteryData>();
        LoadCurrentMastery();
        LoadSkillLevel();
    }

    public void SaveCurrentMastery()
    {
        if (currentMastery == null) currentMastery = new CurrentMastery();
        string jsonData = JsonUtility.ToJson(currentMastery, true);
        string path = Path.Combine(Application.persistentDataPath, "playerCurrentMastery.json");
        File.WriteAllText(path, jsonData);
    }

    public void LoadCurrentMastery()
    {
        PlayerPrefs.DeleteKey("LoadCurrentMasteryCount");
        PlayerPrefs.SetInt("LoadCurrentMasteryCount", PlayerPrefs.GetInt("LoadCurrentMasteryCount", 0));
        if (PlayerPrefs.GetInt("LoadCurrentMasteryCount") == 0)
        {
            //Debug.log("최초 마스터리 데이터 로드 실행입니다.");
            PlayerPrefs.SetInt("LoadCurrentMasteryCount", 1);
            SaveCurrentMastery();
            PlayerPrefs.Save();
        }

        string path = Path.Combine(Application.persistentDataPath, "playerCurrentMastery.json");
        string jsonData = File.ReadAllText(path);
        if (jsonData == null) return;
        currentMastery = JsonUtility.FromJson<CurrentMastery>(jsonData);
    }

    public void SaveSkillLevel()
    {
        for (int i = 0; i < 5; i++)
        {
            if (weaponSkillLevel[i] == null) weaponSkillLevel[i] = new WeaponSkillLevel();
            string jsonData = JsonUtility.ToJson(weaponSkillLevel[i], true);
            string path = Path.Combine(Application.persistentDataPath, "weaponSKillLevel" + i + ".json");
            File.WriteAllText(path, jsonData);
        }
        //string swordSkillData = JsonUtility.ToJson(currentMastery, true);
        //string path = Path.Combine(Application.persistentDataPath, "playerCurrentMastery.json");
        //File.WriteAllText(path, jsonData);
    }

    public void LoadSkillLevel()
    {
        PlayerPrefs.DeleteKey("LoadWeaponSkillCount");
        PlayerPrefs.SetInt("LoadWeaponSkillCount", PlayerPrefs.GetInt("LoadWeaponSkillCount", 0));
        if (PlayerPrefs.GetInt("LoadWeaponSkillCount") == 0)
        {
            //Debug.log("최초 스킬 레벨 데이터 로드 실행입니다.");
            PlayerPrefs.SetInt("LoadWeaponSkillCount", 1);
            SaveSkillLevel();
            PlayerPrefs.Save();
        }
        for (int i = 0; i < 5; i++)
        {
            string path = Path.Combine(Application.persistentDataPath, "weaponSKillLevel" + i + ".json");
            string jsonData = File.ReadAllText(path);
            if (jsonData == null) return;
            weaponSkillLevel[i] = JsonUtility.FromJson<WeaponSkillLevel>(jsonData);
        }

    }
    /// <summary>
    /// 무기 숙련도 증가
    /// </summary>
    /// <param name="weapon">무기. 
    /// 종류별로 string값 입력
    /// sword : 한손검
    /// dagger : 단검
    /// blunt : 둔기
    /// staff : 전투 지팡이
    /// wand : 지팡이
    /// </param>
    public void incrementMasteryLevel(string weapon)
    {
        if (currentMastery.currentMasteryLevel < 50)
            currentMastery.currentMasteryLevel++;
        switch (weapon)
        {
            case "sword":
                currentMastery.currentSwordMasteryLevel++;
                currentMastery.currentSwordMasteryExp = 0;
                break;
            case "dagger":
                currentMastery.currentDaggerMasteryLevel++;
                currentMastery.currentDaggerMasteryExp = 0;
                break;
            case "blunt":
                currentMastery.currentBluntMasteryLevel++;
                currentMastery.currentBluntMasteryExp = 0 ;
                break;
            case "staff":
                currentMastery.currentStaffMasteryLevel++;
                currentMastery.currentStaffMasteryExp = 0;
                break;
            case "wand":
                currentMastery.currentWandMasteryLevel++;
                currentMastery.currentWandMasteryExp = 0;
                break;
        }
        SaveCurrentMastery();
    }

    public void incrementSkillLevel(string weapon, string skillName)
    {
        switch (weapon)
        {
            case "sword":
                switch (skillName)
                {
                    case "autoAttack":
                        weaponSkillLevel[0].autoAttackLevel++;
                        break;
                    case "skillA":
                        weaponSkillLevel[0].skillALevel++;
                        break;
                    case "skillB":
                        weaponSkillLevel[0].skillBLevel++;
                        break;
                    case "skillC":
                        weaponSkillLevel[0].skillCLevel++;
                        break;
                }
                break;
            case "dagger":
                switch (skillName)
                {
                    case "autoAttack":
                        weaponSkillLevel[1].autoAttackLevel++;
                        break;
                    case "skillA":
                        weaponSkillLevel[1].skillALevel++;
                        break;
                    case "skillB":
                        weaponSkillLevel[1].skillBLevel++;
                        break;
                    case "skillC":
                        weaponSkillLevel[1].skillCLevel++;
                        break;
                }
                break;
            case "blunt":
                switch (skillName)
                {
                    case "autoAttack":
                        weaponSkillLevel[2].autoAttackLevel++;
                        break;
                    case "skillA":
                        weaponSkillLevel[2].skillALevel++;
                        break;
                    case "skillB":
                        weaponSkillLevel[2].skillBLevel++;
                        break;
                    case "skillC":
                        weaponSkillLevel[2].skillCLevel++;
                        break;
                }
                break;
            case "wand":
                switch (skillName)
                {
                    case "autoAttack":
                        weaponSkillLevel[3].autoAttackLevel++;
                        break;
                    case "skillA":
                        weaponSkillLevel[3].skillALevel++;
                        break;
                    case "skillB":
                        weaponSkillLevel[3].skillBLevel++;
                        break;
                    case "skillC":
                        weaponSkillLevel[3].skillCLevel++;
                        break;
                }
                break;
            case "staff":
                switch (skillName)
                {
                    case "autoAttack":
                        weaponSkillLevel[4].autoAttackLevel++;
                        break;
                    case "skillA":
                        weaponSkillLevel[4].skillALevel++;
                        break;
                    case "skillB":
                        weaponSkillLevel[4].skillBLevel++;
                        break;
                    case "skillC":
                        weaponSkillLevel[4].skillCLevel++;
                        break;
                }
                break;
        }
        SaveSkillLevel();
    }
    public void UpdateCurrentExp()
    {
        if (WeaponManager.Instance != null)
        {
            switch (WeaponManager.Instance.GetWeapon().name)
            {
                case "sword":
                    if (WeaponManager.Instance.GetWeapon().exp != currentMastery.currentSwordMasteryExp)
                    {
                        WeaponManager.Instance.GetWeapon().exp = currentMastery.currentSwordMasteryExp;
                    }
                    break;
                case "wand":
                    if (WeaponManager.Instance.GetWeapon().exp != currentMastery.currentWandMasteryExp)
                    {
                        WeaponManager.Instance.GetWeapon().exp = currentMastery.currentWandMasteryExp;
                    }
                    break;
                case "dagger":
                    if (WeaponManager.Instance.GetWeapon().exp != currentMastery.currentDaggerMasteryExp)
                    {
                        WeaponManager.Instance.GetWeapon().exp = currentMastery.currentWandMasteryExp;
                    }
                    break;
                case "blunt":
                    if (WeaponManager.Instance.GetWeapon().exp != currentMastery.currentBluntMasteryExp)
                    {
                        WeaponManager.Instance.GetWeapon().exp = currentMastery.currentBluntMasteryExp;
                    }
                    break;
                case "staff":
                    if (WeaponManager.Instance.GetWeapon().exp != currentMastery.currentStaffMasteryExp)
                    {
                        WeaponManager.Instance.GetWeapon().exp = currentMastery.currentStaffMasteryExp;
                    }
                    break;
            }
            SaveCurrentMastery();
        }
    }

    public void SetMastery(int choice)
    {
        currentMastery.currentMasteryChoices.Add(choice);
    }
}

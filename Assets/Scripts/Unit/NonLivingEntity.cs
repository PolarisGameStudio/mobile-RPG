﻿using UnityEngine;

/// <summary>
/// Object 및 NPC에 사용되는 클래스
/// </summary>
public class NonLivingEntity : Unit
{
    [SerializeField] private bool _isNpc;

    private TalkManager _talkManager;   // 토크매니저 캐싱
    private TalkChecker _myTalkChecker; // 해당 NPC의 토크체커 캐싱

    [SerializeField] private GameObject canQuestSquare;
    [SerializeField] private GameObject canEndSquare;

    private void Start()
    {
        _talkManager = TalkManager.Instance;
        _myTalkChecker = _talkManager.talkCheckers[_id];
    }

    private void Update()
    {
        if (_myTalkChecker.canFinishQuest)
        {
            canEndSquare.SetActive(true);
            canQuestSquare.SetActive(false);
        }

        else
        {
            canEndSquare.SetActive(false);

            if (_myTalkChecker.canStartQuest || _myTalkChecker.canProcressQuest)
            {
                canQuestSquare.SetActive(true);
            }

            else
            {
                canQuestSquare.SetActive(false);
            }

        }
    }

    /// <summary>
    /// 가까운 오브젝트에 대해 상호작용을 실시한다.
    /// </summary>
    public void Interaction()
    {
        GameManager.Instance.isInteracting = true; // 상호작용 중 설정
        _myTalkChecker.Talk();
    }
}

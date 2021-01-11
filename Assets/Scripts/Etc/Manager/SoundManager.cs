﻿using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonBase<SoundManager>
{
    // 오디오 경로
    private const string    _bgmPath            = "Sound/BGM/";
    private const string    _effectPath         = "Sound/Effect/";

    private const int       _audioCount         = 30; // 최대 동시재생 오디오 개수.

    // 사운드 컨테이너
    private GameObject      _bgmContainer1;
    private GameObject      _bgmContainer2;
    private GameObject[]    _effectContainer;

    // 각 사운드의 음량
    public float _masterVolume  = 1.0f;
    public float _bgmVolume     = 1.0f;
    public float _effectVolume  = 1.0f;
    public float _uiVolume      = 1.0f;

    // distance 비례 음량
    private const float     _minDistanceVolume  = 15; // 거리가 이만큼 될때부터 소리가 작아지기 시작
    
    public void InitSoundManager()
    {
        InitSound();
    }

    public void UpdateSoundManager()
    {
    }
    

    /// 초기 사운드들을 배치하도록 한다.
    public void InitSound()
    {
        _effectContainer = new GameObject[_audioCount];

        for (int i = 0; i < _audioCount; i++)
        {
            _effectContainer[i] = ResourceManager.Instance.Instantiate("EffectContainer" + i, "Prefab/Etc/Container/AudioSource", transform);
        }

        _bgmContainer1 = ResourceManager.Instance.Instantiate("bgmPlayer1", "Prefab/Etc/Container/AudioSource", transform);
        _bgmContainer2 = ResourceManager.Instance.Instantiate("bgmPlayer2", "Prefab/Etc/Container/AudioSource", transform);
        _bgmContainer1.GetComponent<SoundInfor>().SetInfor(SoundType.BGM, 0);
        _bgmContainer2.GetComponent<SoundInfor>().SetInfor(SoundType.BGM, 0);
    }

    // BGM을 재생시킨다. 기본적으로 FadeIn / Out이 적용된다.
    public AudioSource PlayBGM(string soundPath, float vol = 1.0f, bool startImmediatly = false)
    {
        AudioSource startSource;
        AudioSource endSource;
        
        if (_bgmContainer1.GetComponent<AudioSource>().isPlaying)
        {
            startSource = _bgmContainer2.GetComponent<AudioSource>();
            endSource = _bgmContainer1.GetComponent<AudioSource>();
            _bgmContainer2.GetComponent<SoundInfor>().SetInfor(SoundType.BGM, Time.realtimeSinceStartup);
        }

        else
        {
            startSource = _bgmContainer1.GetComponent<AudioSource>();
            endSource = _bgmContainer2.GetComponent<AudioSource>();
            _bgmContainer1.GetComponent<SoundInfor>().SetInfor(SoundType.BGM, Time.realtimeSinceStartup);
        }

        startSource.clip = Resources.Load(_bgmPath + soundPath) as AudioClip;
        startSource.loop = true;

        if(startImmediatly)
        {
            // 현재 재생중인 BGM 즉시 중지시킴
            StopCoroutine("FadeOutBGM");
            endSource.Stop();

            // 재생할 BGM 즉시 FadeIn
            StopCoroutine("FadeInBGM");
            StartCoroutine(FadeInBGM(startSource, 2));
        }

        else // 현재 재생중인 음악 FadeOut으로 정지 후, 재생할 음악 FadeIn으로 재생
        {
            StopCoroutine("FadeOutBGM");
            StartCoroutine(FadeOutBGM(endSource, 2, startSource, true));
        }

        startSource.pitch = 1;
        startSource.GetComponent<AudioReverbFilter>().enabled = false;
        startSource.GetComponent<AudioLowPassFilter>().enabled = false;
        startSource.GetComponent<AudioHighPassFilter>().enabled = false;
        startSource.GetComponent<AudioChorusFilter>().enabled = false;
        startSource.GetComponent<AudioEchoFilter>().enabled = false;

        return startSource;
    }

    // BGM을 정지시킨다. 별도의 입력이 없으면 fadeOut으로 정지시킨다.
    public void PauseBGM(bool stopImmediatly = false)
    {
        AudioSource sourceA = _bgmContainer1.GetComponent<AudioSource>();
        AudioSource sourceB = _bgmContainer2.GetComponent<AudioSource>();

        if (sourceA.isPlaying)
        {
            if (stopImmediatly) sourceA.Stop();
            else
            {
                StopCoroutine("FadeOutBGM");
                StopCoroutine(FadeOutBGM(sourceA, 2));
            }
        }

        if (sourceB.isPlaying)
        {
            if (stopImmediatly) sourceB.Stop();
            else
            {
                StopCoroutine("FadeOutBGM");
                StopCoroutine(FadeOutBGM(sourceB, 2));
            }
        }
    }

    // BGM을 서서히 재생시킨다.
    private IEnumerator FadeInBGM(AudioSource fadeInTarget, float period)
    {
        fadeInTarget.Play();
        fadeInTarget.volume = 0;

        float curTime = 0;
        while (true)
        {
            curTime += Time.deltaTime;

            fadeInTarget.volume = _masterVolume * _bgmVolume * Mathf.Lerp(0, 1, curTime / period);

            if (curTime >= period)
            {
                break;
            }

            yield return null;
        }
    }

    // BGM을 서서히 정지시킨다. isChange가 true일 경우, 정지 후에 타겟 BGM을 서서히 재생시킨다.
    private IEnumerator FadeOutBGM(AudioSource fadeOutTarget, float period, AudioSource fadeInTarget = null, bool isChange = false)
    {
        float curTime = 0;

        while (true)
        {
            curTime += Time.deltaTime;

            fadeOutTarget.volume = _masterVolume * _bgmVolume * Mathf.Lerp(1, 0, curTime / period);

            if (curTime >= period)
            {
                break;
            }

            yield return null;
        }

        fadeOutTarget.volume = 0;
        fadeOutTarget.Stop();

        if(isChange && fadeInTarget != null)
        {
            StopCoroutine("FadeInBGM");
            StartCoroutine(FadeInBGM(fadeInTarget, period));
        }
    }

    // SoundContainer 중 적합한 사운드 컨테이너 Index를 찾아 반환한다.
    private int FindEffectContainerPosition()
    {
        bool findPos = false;
        int targetPos = -1;

        for (int i = 0; i < _audioCount; i++) // 전체를 돌며 정지된 사운드나 빈 clip을 찾는다.
        {
            AudioSource currentSource = _effectContainer[i].GetComponent<AudioSource>();
            if (currentSource.clip == null || !currentSource.isPlaying)
            {
                findPos = true;
                targetPos = i;
            }
        }

        if (!findPos) // 모두 재생중인 상태 -> 가장 플레이한지 오래된 clip을 찾는다.
        {
            float minStartTime = float.MaxValue;

            for (int i = 0; i < _audioCount; i++)
            {
                AudioSource currentSource = _effectContainer[i].GetComponent<AudioSource>();
                SoundInfor currentInfor = currentSource.GetComponent<SoundInfor>();

                if (currentSource.GetComponent<SoundInfor>().StartTime < minStartTime)
                {
                    minStartTime = currentInfor.StartTime;
                    targetPos = i;
                }
            }
        }

        return targetPos;
    }

    // Effect / UI 사운드를 재생하도록 한다.
    public AudioSource PlayEffect(SoundType type, string soundPath, float volume = 1.0f, float distance = 0f, bool isLoop = false)
    {
        int targetPos = FindEffectContainerPosition();

        AudioSource targetSource = _effectContainer[targetPos].GetComponent<AudioSource>();

        switch (type)
        {
            case SoundType.EFFECT:
                targetSource.clip = Resources.Load(_effectPath + soundPath) as AudioClip;
                targetSource.volume = _masterVolume * _effectVolume * volume * ((distance > _minDistanceVolume) ? (_minDistanceVolume / distance) : 1); // distance가 최소 distanceVolume보다 크면 거리에 비례한 적은 소리를 배출하도록 한다.
                _effectContainer[targetPos].GetComponent<SoundInfor>().SetInfor(SoundType.EFFECT, Time.realtimeSinceStartup);
                break;
            case SoundType.UI:
                targetSource.clip = Resources.Load(_effectPath + soundPath) as AudioClip;
                targetSource.volume = _masterVolume * _uiVolume * volume; // ui는 어디서든 volume만큼의 소리를 낸다.
                _effectContainer[targetPos].GetComponent<SoundInfor>().SetInfor(SoundType.UI, Time.realtimeSinceStartup);
                break;
        }

        targetSource.Play();
        targetSource.loop = isLoop;
        targetSource.pitch = 1;
        targetSource.GetComponent<AudioReverbFilter>().enabled = false;
        targetSource.GetComponent<AudioLowPassFilter>().enabled = false;
        targetSource.GetComponent<AudioHighPassFilter>().enabled = false;
        targetSource.GetComponent<AudioChorusFilter>().enabled = false;
        targetSource.GetComponent<AudioEchoFilter>().enabled = false;

        return targetSource;
    }

    /// <summary>
    /// 사운드의 pitch를 조절
    /// </summary>
    public void SetPitch(AudioSource source, float pitch)
    {
        pitch = source.pitch;
    }

    /// <summary>
    /// 사운드에 에코를 추가
    /// </summary>
    /// <param name="delay">10~5000, default 500</param>
    /// <param name="decayRatio">0~1, default 0.5</param>
    public void SetEcho(AudioSource source, float delay = 500f, float decayRatio = 0.5f, float dryMix = 1f, float wetMix = 1f)
    {
        AudioEchoFilter echo = source.GetComponent<AudioEchoFilter>();
        echo.enabled = true;
        echo.delay = delay;
        echo.decayRatio = decayRatio;
        echo.dryMix = dryMix;
        echo.wetMix = wetMix;
    }

    /// <summary>
    /// 사운드에 리버브를 추가
    /// </summary>
    public void SetAudioReverbEffect(AudioSource source, AudioReverbPreset preset)
    {
        source.GetComponent<AudioReverbFilter>().enabled = true;
        source.GetComponent<AudioReverbFilter>().reverbPreset = preset;
    }

    /// <summary>
    /// 사운드에 Lowpass를 추가
    /// </summary>
    /// <param name="cutoffFrequency"> Default 5007 </param>
    /// <param name="lowpassResonance"> Default 1, Range 1~10 </param>
    public void SetLowpass(AudioSource source, float cutoffFrequency = 5007.7f, float lowpassResonance = 1f)
    {
        AudioLowPassFilter lowPass = source.GetComponent<AudioLowPassFilter>();
        lowPass.enabled = true;
        lowPass.cutoffFrequency = cutoffFrequency;
        lowPass.lowpassResonanceQ = lowpassResonance;
    }

    /// <summary>
    /// 사운드에 Highpass를 추가
    /// </summary>
    /// <param name="cutoffFrequency"> Default 5000 </param>
    /// <param name="highpassResonance"> Default 1, Range 1~10 </param>
    public void SetHighpass(AudioSource source, float cutoffFrequency = 5000f, float highpassResonance = 1f)
    {
        AudioHighPassFilter highPass = source.GetComponent<AudioHighPassFilter>();
        highPass.enabled = true;
        highPass.cutoffFrequency = cutoffFrequency;
        highPass.highpassResonanceQ = highpassResonance;
    }

    /// <summary>
    /// 사운드에 코러스 필터를 추가.
    /// </summary>
    /// <param name="delay">0~100, default 40</param>
    /// <param name="rate">0~20, default 20</param>
    /// <param name="depth">0~1, default 0.03</param>
    public void SetChorusFilter(AudioSource source, float delay = 40f, float rate = 0.8f, float depth = 0.03f, float dryMix = 0.5f, float wetMix1 = 0.5f, float wetMix2 = 0.5f, float wetMix3 = 0.5f)
    {
        AudioChorusFilter chorus = source.GetComponent<AudioChorusFilter>();
        chorus.enabled = true;
        chorus.delay = delay;
        chorus.rate = rate;
        chorus.depth = depth;
        chorus.dryMix = dryMix;
        chorus.wetMix1 = wetMix1;
        chorus.wetMix2 = wetMix2;
        chorus.wetMix3 = wetMix3;
    }

    // BGM의 볼륨을 조절
    public void ChangeBGMVolume(float volume)
    {
        if (volume <= 0) volume = 0.0001f;

        if (_bgmContainer1.GetComponent<AudioSource>().isPlaying) _bgmContainer1.GetComponent<AudioSource>().volume *= (volume / _bgmVolume);
        if(_bgmContainer2.GetComponent<AudioSource>().isPlaying) _bgmContainer2.GetComponent<AudioSource>().volume *= (volume / _bgmVolume);
        _bgmVolume = volume;
    }

    // Effect의 볼륨을 조절
    public void ChangeEffectVolume(float volume)
    {
        if (volume <= 0) volume = 0.0001f;

        for (int i=0; i<_audioCount; i++)
        {
            if(_effectContainer[i].GetComponent<SoundInfor>().SoundType == SoundType.EFFECT && _effectContainer[i].GetComponent<AudioSource>().isPlaying)
                _effectContainer[i].GetComponent<AudioSource>().volume *= (volume / _effectVolume);
        }

        _effectVolume = volume;
    }

    // UI의 볼륨을 조절
    public void ChangeUIVolume(float volume)
    {
        if (volume <= 0) volume = 0.0001f;

        for (int i = 0; i < _audioCount; i++)
        {
            if (_effectContainer[i].GetComponent<SoundInfor>().SoundType == SoundType.UI && _effectContainer[i].GetComponent<AudioSource>().isPlaying)
                _effectContainer[i].GetComponent<AudioSource>().volume *= (volume / _uiVolume);
        }

        _uiVolume = volume;
    }

    // 마스터 볼륨을 조절
    public void ChangeMasterVolume(float volume)
    {
        if (volume <= 0) volume = 0.0001f;
        _bgmContainer1.GetComponent<AudioSource>().volume *= (volume / _masterVolume);
        _bgmContainer2.GetComponent<AudioSource>().volume *= (volume / _masterVolume);

        for (int i = 0; i < _audioCount; i++)
        {
            if (_effectContainer[i].GetComponent<AudioSource>().isPlaying)
                _effectContainer[i].GetComponent<AudioSource>().volume *= (volume / _masterVolume);
        }

        _masterVolume = volume;
    }
}
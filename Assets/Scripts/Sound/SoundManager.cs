using JollyPanda.LastFlag.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USound
{

    [RequireComponent(typeof(SoundEventsListener))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<SoundManager>();

                    if (_instance == null)
                    {
                        Debug.LogError("No SoundManager found in the scene. Please add one.");
                    }
                }

                return _instance;
            }
        }
        private static SoundManager _instance;

        [SerializeField] private SoundTag splashAudioClip;
        [SerializeField] private bool playSplashMusicOnStart = true;

        [Header("Volume Settings")]
        private float backgroundVolume = 1f;
        private float sfxVolume = 1f;

        [Header("Background Audio Clips")]
        [SerializeField] private SoundTag homeAudioClip;
        [SerializeField] private SoundTag gameAudioClip;

        [Header("Fade")]
        [SerializeField] private float fadeDuration = 2f;

        private SoundTag currentBG;
        private SoundTag nextBG;

        private readonly List<SoundTag> registeredSounds = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            backgroundVolume = SaveSystem.GetBackgroundVolume();
            sfxVolume = SaveSystem.GetSfxVolume();

            currentBG = null;
            nextBG = null;

            if (playSplashMusicOnStart && splashAudioClip != null)
            {
                StartCoroutine(PlaySplashThenHome());
            }
            else
            {
                PlayHomeBackground();
            }
        }

        private IEnumerator PlaySplashThenHome()
        {
            yield return FadeInOnly(splashAudioClip);

            AudioSource splashSource = splashAudioClip.GetAudioSource();
            while (splashSource.isPlaying)
            {
                yield return null;
            }

            FadeToBackground(homeAudioClip);
        }

        private void Start()
        {
            ApplyVolumes();
        }

        internal void PlayHomeBackground()
        {
            if (currentBG == homeAudioClip && currentBG.GetAudioSource().isPlaying)
                return;

            FadeToBackground(homeAudioClip);
        }

        internal void PlayGameBackground()
        {
            if (currentBG == gameAudioClip && currentBG.GetAudioSource().isPlaying)
                return;

            FadeToBackground(gameAudioClip);
        }

        private void FadeToBackground(SoundTag newBackground)
        {
            if (newBackground == null)
                return;

            if (newBackground == currentBG)
                return;

            nextBG = newBackground;
            var nextSource = nextBG.GetAudioSource();
            nextSource.volume = 0f;
            nextSource.Play();

            if (currentBG != null)
            {
                StartCoroutine(FadeOutIn(currentBG, nextBG));
            }
            else
            {
                StartCoroutine(FadeInOnly(nextBG));
            }

            currentBG = nextBG;
        }

        private IEnumerator FadeOutIn(SoundTag fromTag, SoundTag toTag)
        {
            float timer = 0f;
            AudioSource from = fromTag.GetAudioSource();
            AudioSource to = toTag.GetAudioSource();

            while (timer < fadeDuration)
            {
                float t = timer / fadeDuration;
                from.volume = Mathf.Lerp(backgroundVolume, 0f, t);
                to.volume = Mathf.Lerp(0f, backgroundVolume, t);
                timer += Time.deltaTime;
                yield return null;
            }

            from.Stop();
            from.volume = 0f;
            to.volume = backgroundVolume;
        }

        private IEnumerator FadeInOnly(SoundTag toTag)
        {
            float timer = 0f;
            AudioSource to = toTag.GetAudioSource();

            while (timer < fadeDuration)
            {
                float t = timer / fadeDuration;
                to.volume = Mathf.Lerp(0f, backgroundVolume, t);
                timer += Time.deltaTime;
                yield return null;
            }

            to.volume = backgroundVolume;
        }

        public void PlaySFX(AudioSource sfxSource, AudioClip clip)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(clip);
        }

        public void SetBackgroundVolume(float value)
        {
            backgroundVolume = Mathf.Clamp01(value);

            ApplyVolumes();
        }

        public void SetSFXVolume(float value)
        {
            sfxVolume = Mathf.Clamp01(value);

            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            if (currentBG != null)
                UpdateSoundTagVolume(currentBG);

            foreach (var tag in registeredSounds)
            {
                UpdateSoundTagVolume(tag);
            }
        }

        internal void RegisterSound(SoundTag tag)
        {
            if (!registeredSounds.Contains(tag))
                registeredSounds.Add(tag);

            UpdateSoundTagVolume(tag);
        }

        private void UpdateSoundTagVolume(SoundTag tag)
        {
            if (tag.SoundType == SoundType.SFX)
                tag.SetVolume(sfxVolume);
            else if (tag.SoundType == SoundType.Background)
                tag.SetVolume(backgroundVolume);
        }

        public void Save()
        {
            SaveSystem.UpdateBackgroundVolume(backgroundVolume);
            SaveSystem.UpdateSfxVolume(sfxVolume);
        }
    }
}

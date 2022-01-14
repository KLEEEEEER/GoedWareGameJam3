using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private AudioSource _mainAudioSource;

        private static SoundManager _instance;
        public static SoundManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _mainAudioSource.clip = _backgroundMusic;
            _mainAudioSource.Play();
        }
    }
}
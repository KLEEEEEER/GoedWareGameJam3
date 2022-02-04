using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _footstepsSounds;
        [SerializeField] private AudioSource _footstepsSource;
        [SerializeField] private float _timeBetweenFootsteps = 0.3f;

        private float _timeFromPreviousFootstepPlayed = 0f;

        public void PlayFootstepSounds(Vector2 movementDirection)
        {
            if (movementDirection.normalized.sqrMagnitude > 0f && _timeFromPreviousFootstepPlayed >= _timeBetweenFootsteps)
            {
                AudioClip randomFootstep = _footstepsSounds[Random.Range(0, _footstepsSounds.Length)];
                _footstepsSource.clip = randomFootstep;
                _footstepsSource.pitch = Random.Range(0.8f, 1.2f);
                _footstepsSource.Play();
                _timeFromPreviousFootstepPlayed = 0f;
            }
            else
            {
                _timeFromPreviousFootstepPlayed += Time.deltaTime;
            }
        }
    }
}
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Settings/Player/Create settings")]
    public class PlayerSettingsSO : ScriptableObject
    {
        [SerializeField] private float _speed = 1f;
        public float Speed => _speed;
    }
}
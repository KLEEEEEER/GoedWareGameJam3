using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    [RequireComponent(typeof(PlayerFSM))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerLedgeChecker : MonoBehaviour
    {
        [SerializeField] private Transform _ledgeChecker;
        [SerializeField] private Transform _ledgeCheckerHead;
        [SerializeField] private float _ledgeCheckerDistance = 2f;
        [SerializeField] private LayerMask _groundLayerMask;
        
        private PlayerFSM _playerFSM;
        private PlayerMovement _playerMovement;

        RaycastHit hit;
        RaycastHit ledgeCheckerHit;
        public Vector3 LedgeNormal => hit.normal;

        private void Awake()
        {
            _playerFSM = GetComponent<PlayerFSM>();
            _playerMovement = GetComponent<PlayerMovement>();
        }

        public bool CheckLedgeInFront()
        {
            if (Physics.Raycast(_ledgeCheckerHead.position, _ledgeCheckerHead.forward, out hit, _ledgeCheckerDistance, _groundLayerMask))
            {
                if (Physics.Raycast(_ledgeChecker.position, _ledgeChecker.forward, out ledgeCheckerHit, _ledgeCheckerDistance, _groundLayerMask))
                {
                    return false;
                }
                return true;
            }

            return false;
        }

        public void Climb()
        {
            _playerMovement.Climb(hit);
        }

        private void OnDrawGizmos()
        {
            if (_ledgeCheckerHead == null || _ledgeChecker == null) return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_ledgeCheckerHead.position, _ledgeCheckerHead.position + _ledgeCheckerHead.forward * _ledgeCheckerDistance);
            Gizmos.DrawLine(_ledgeChecker.position, _ledgeChecker.position + _ledgeChecker.forward * _ledgeCheckerDistance);
        }
    }
}
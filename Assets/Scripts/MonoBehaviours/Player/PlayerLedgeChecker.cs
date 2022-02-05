using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Player
{
    public class PlayerLedgeChecker : MonoBehaviour
    {
        [SerializeField] private Transform _ledgeChecker;
        [SerializeField] private Transform _ledgeCheckerHead;
        [SerializeField] private float _ledgeCheckerDistance = 2f;
        [SerializeField] private LayerMask _groundLayerMask;

        private void FixedUpdate()
        {
            RaycastHit hit;
            if (Physics.Raycast(_ledgeCheckerHead.position, _ledgeCheckerHead.forward, out hit, _ledgeCheckerDistance, _groundLayerMask))
            {
                if (Physics.Raycast(_ledgeChecker.position, _ledgeChecker.forward, _ledgeCheckerDistance, _groundLayerMask))
                {
                    return;
                }

                PerformClimb(hit);
            }
        }

        private void PerformClimb(RaycastHit hit)
        {
            Debug.Log($"Climbing right now! Hit with {hit.collider.gameObject.name}");
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
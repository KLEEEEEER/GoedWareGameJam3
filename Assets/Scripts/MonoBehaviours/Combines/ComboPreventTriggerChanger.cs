using GoedWareGameJam3.Core.Interfaces;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    [RequireComponent(typeof(Renderer))]
    public class ComboPreventTriggerChanger : MonoBehaviour, IInteractable
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ComboPreventTrigger _trigger;
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Start()
        {
            SetScreenMaterial(_trigger.CurrentMaterial);
        }

        public void Interact()
        {
            _trigger.ChangeType();
            _audioSource.Play();
            SetScreenMaterial(_trigger.CurrentMaterial);
        }

        private void SetScreenMaterial(Material material)
        {
            Material[] tempMaterials = _renderer.materials;
            tempMaterials[2] = material;
            _renderer.materials = tempMaterials;
        }
    }
}
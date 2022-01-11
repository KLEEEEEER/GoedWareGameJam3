using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    [CreateAssetMenu(fileName = "ComboPreventingChangerRulesSettings", menuName = "Settings/ComboPreventingChanger/Create settings")]
    public class ComboPreventingChangerRulesSettingsSO : ScriptableObject
    {
        [SerializeField] private ComboPreventingChangerRule[] _rules;
        public ComboPreventingChangerRule[] Rules => _rules;
    }

    [System.Serializable]
    public struct ComboPreventingChangerRule
    {
        [SerializeField] private ComboObject.Type _type;
        public ComboObject.Type Type => _type;

        [SerializeField] private Material _material;
        public Material Material => _material;
    }
}
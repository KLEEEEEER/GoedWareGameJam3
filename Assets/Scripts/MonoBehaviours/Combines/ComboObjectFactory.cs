using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.MonoBehaviours.Combines
{
    public class ComboObjectFactory : MonoBehaviour
    {
        [SerializeField] private ComboObjectPrefab[] _prefabs;

        Dictionary<ComboObject.Type, GameObject> _prefabsDictionary = new Dictionary<ComboObject.Type, GameObject>();

        private static ComboObjectFactory _instance;
        public static ComboObjectFactory Instance => _instance;

        private void Awake()
        {
            _instance = this;

            foreach (ComboObjectPrefab _prefab in _prefabs)
            {
                _prefabsDictionary.Add(_prefab.Type, _prefab.Prefab);
            }
        }

        public void Create(ComboObject.Type type, Vector3 position)
        {
            if (!_prefabsDictionary.ContainsKey(type)) return;

            Instantiate(_prefabsDictionary[type], position, Quaternion.identity);
        }
    }

    [System.Serializable]
    public struct ComboObjectPrefab
    {
        [SerializeField] private ComboObject.Type _type;
        public ComboObject.Type Type => _type;
        [SerializeField] private GameObject _prefab;
        public GameObject Prefab => _prefab;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoedWareGameJam3.MonoBehaviours
{
    public class InitializeScene : MonoBehaviour
    {
        private void Start()
        {
            SceneManager.LoadScene(1);
            Debug.Log("Initialized");
        }
    }
}
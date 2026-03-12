using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yakanashe.Wiper;

public class EasySceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private Transition transition;

    private void Start()
    {
        transition.Out(0.1f);
    }

    public void Transition()
    {
        transition.In(1, () => SceneManager.LoadScene(sceneName));
    }
}
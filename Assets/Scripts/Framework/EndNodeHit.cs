using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndNodeHit : MonoBehaviour
{
    public void switchScene(string name)
    {
        StartCoroutine(SwitchSceneRoutine(name));
    }

    private IEnumerator SwitchSceneRoutine(string name)
    {
        yield return new WaitForSecondsRealtime(2);
        SceneManager.LoadScene(name);
        //transition here
    }
}

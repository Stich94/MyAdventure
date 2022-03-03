using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    [SerializeField] CanvasGroup iamgeCanvas;

    string trigger = "Start";

    public void LoadNextLevel()
    {
        // GameObject.DontDestroyOnLoad(this.gameObject);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int _levelIndex)
    {
        iamgeCanvas.alpha = 1f;
        // Play Animation
        transition.SetTrigger(trigger);

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        SceneManager.LoadScene(_levelIndex);

    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(LoadLevel(0));
    }
}

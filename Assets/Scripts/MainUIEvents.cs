using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainUIEvents : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.5f;
    [SerializeField] private SpriteRenderer blackboard;

    public void GoToMenu()
    {
        // animation
        Vector3 endPos = new Vector3(blackboard.transform.position.x, 0, blackboard.transform.position.z);
        blackboard.transform.DOMove(endPos, transitionTime).OnComplete(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }
}

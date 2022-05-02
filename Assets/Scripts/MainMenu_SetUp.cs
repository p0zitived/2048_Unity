using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_SetUp : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown1;
    [SerializeField] private TMP_Dropdown dropdown2;
    [Header("Transition")]
    [SerializeField] private float transitionTime = 0.5f;
    [SerializeField] private SpriteRenderer blackboard;

    public void StartGame(Button bt)
    {
        Destroy(bt);
        Global.width = int.Parse(dropdown1.options[dropdown1.value].text);
        Global.height = int.Parse(dropdown2.options[dropdown2.value].text);

        // animation
        Vector3 endPos = new Vector3(blackboard.transform.position.x, 0, blackboard.transform.position.z);
        blackboard.transform.DOMove(endPos, transitionTime).OnComplete(() =>
        {
            SceneManager.LoadScene("Game");
        });
    }

    public void exitGame()
    {
        Application.Quit();
    }
}

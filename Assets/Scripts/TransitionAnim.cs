using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnim : MonoBehaviour
{
    [Header("Transition")]
    [SerializeField] private float transitionTime = 5f;
    [SerializeField] private SpriteRenderer blackboard;

    private void Start()
    {
        Vector3 endPos = new Vector3(blackboard.transform.position.x, 20, blackboard.transform.position.z);
        blackboard.transform.DOMove(endPos, transitionTime);
    }
}

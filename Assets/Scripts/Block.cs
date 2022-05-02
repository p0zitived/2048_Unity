using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isMargening = false;
    public Vector2 Pos => transform.position;
    public Node node;
    public int Value;
    [SerializeField] private TextMeshPro _text;
    [SerializeField] private SpriteRenderer _render;
    public void Init(BlockType blockType)
    {
        Color typecolor = blockType.Color;
        typecolor.a = 1;

        Value = blockType.Value;
        _render.color = typecolor;
        _text.text = blockType.Value.ToString();
    }

    public void SetBlock(Node newNode)
    {
        if (node != null) node.OccupiedBlock = null;
        node = newNode;
        node.OccupiedBlock = this;
    }
}

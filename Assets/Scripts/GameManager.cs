using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [Header("Other")]
    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private float _travelTime = 0.2f;

    [SerializeField] private List<BlockType> _types;

    private List<Node> _nodes;
    private List<Block> _blocks;

    private GameState _state;
    private int _round = 0;

    // methods
    private BlockType GetBlockTypeByValue(int value) => _types.First(aux => aux.Value == value);
    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState)
        {
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2:1);
                break;
            case GameState.WaitingInput: break;
            case GameState.Moving: break;
            case GameState.Win: break;
            case GameState.Lose: break;
        }
    }

    private void Start()
    {
        // setup size
        _width = Global.width;
        _height = Global.height;

        ChangeState(GameState.GenerateLevel);
        ChangeState(GameState.SpawningBlocks);
        ChangeState(GameState.WaitingInput);
    }
    private void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(Vector2.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(Vector2.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(Vector2.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(Vector2.down);
    }

    private void Move(Vector2 dir)
    {
        ChangeState(GameState.Moving);
        // economiste foarte multe linii de code . Daca ordonam deodate si pe x si pe y logica nu se schimba . Daca trebuie invers facem revers la lista ordonata
        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();

        foreach (var block in orderedBlocks)
        {
            bool marging = false; // experimental
            var next = block.node;
            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);
                if (possibleNode != null)
                {
                    // experimental
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.Value == block.Value && !possibleNode.OccupiedBlock.isMargening)
                    {
                        possibleNode.OccupiedBlock.isMargening = true;
                        _blocks.Remove(block);
                        block.node.OccupiedBlock = null;
                        block.transform.DOMove(possibleNode.Pos, _travelTime).OnComplete(() =>
                        {
                            Destroy(block.gameObject);
                            possibleNode.OccupiedBlock.Init(GetBlockTypeByValue(possibleNode.OccupiedBlock.Value * 2));

                            possibleNode.OccupiedBlock.isMargening = false;
                        });
                        
                        marging = true;
                        // in nodul creiam
                    }
                    // experimental end;
                    if (possibleNode.OccupiedBlock == null) next = possibleNode;
                }
            } while (next != block.node);

            if (!marging) // experimental
                block.transform.DOMove(block.node.Pos, _travelTime);
        }
        StartCoroutine(waitTime(_travelTime));
        audioSource.Play();
    }
    private Node GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(a => a.Pos == pos);
    } 
    private void GenerateGrid()
    {
        _nodes = new List<Node>();
        _blocks = new List<Block>();

        for (int i=0;i<_width;i++)
        {
            for (int j=0;j<_height;j++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(i, j), Quaternion.identity);
                _nodes.Add(node);
            }
        }

        var center = new Vector2((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);

        Camera.main.transform.position = new Vector3(center.x ,center.y, Camera.main.transform.position.z);
    }
    private void SpawnBlocks(int amount)
    {
        var freeNodes = _nodes.Where(node => node.OccupiedBlock == null).OrderBy(aux => Random.value).ToList();

        foreach (var node in freeNodes.Take(amount))
        {
            var block = Instantiate(_blockPrefab,node.Pos,Quaternion.identity);
            block.Init(GetBlockTypeByValue(Random.value >= 0.8f ? 4 : 2));
            block.SetBlock(node);

            _blocks.Add(block);
        }

        if (freeNodes.Count() == 0)
        {
            if (checkIfLose())
            {
                GetComponent<MainUIEvents>().GoToMenu();
            }
        }
    }
    private IEnumerator waitTime(float time)
    {
        yield return new WaitForSeconds(time);
        ChangeState(GameState.SpawningBlocks);
        ChangeState(GameState.WaitingInput);
    }
    
    private bool checkIfExistMove(Vector2 dir)
    {
        bool existMove = false;

        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y).ToList();
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();
        foreach (var block in orderedBlocks)
        {
            var next = block.node;
            do
            {
                block.SetBlock(next);

                var possibleNode = GetNodeAtPosition(next.Pos + dir);
                if (possibleNode != null)
                {
                    if (possibleNode.OccupiedBlock != null && possibleNode.OccupiedBlock.Value == block.Value && !possibleNode.OccupiedBlock.isMargening)
                    {
                        existMove = true;
                    }
                }
            } while (next != block.node);
        }

        return existMove;
    }
    private bool checkIfLose()
    {
        if (checkIfExistMove(Vector2.up) || checkIfExistMove(Vector2.down) || checkIfExistMove(Vector2.left) || checkIfExistMove(Vector2.right))
        {
            return false;
        }
        else
            return true;
    }
}

[System.Serializable]
public struct BlockType
{
    public int Value;
    public Color Color;
}

public enum GameState
{
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}
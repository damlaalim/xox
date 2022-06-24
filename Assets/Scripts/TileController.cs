using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.WSA;

public class TileController : MonoBehaviour, IPointerDownHandler
{
    public TileState MyState { get; set; }

    public Vector2 coordinate;

    [SerializeField] private SpriteRenderer mySpriteRenderer;
    
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    
    [SerializeField] private Color xColor;
    [SerializeField] private Color oColor;

    [SerializeField] private Animator tileAnimator;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (MyState != TileState.None)
            return;

        var manager = GameManager.Instance;
        
        if (!manager.IsStartGame)
            return;

        var state = manager.MyTileState;
        
        SetState(state);
        manager.MyTileState = manager.MyTileState == TileState.O ? TileState.X : TileState.O;

        var (hasWinner, tileState) = manager.HasWinner();

        if (hasWinner)
        {
            manager.IsStartGame = false;

            IEnumerator Do()
            {
                yield return new WaitForSeconds(1.5f);
                manager.OnGameOver(tileState);
            }

            StartCoroutine(Do());
        }
        else
        {
            if (!manager.HasNoneTile())
                manager.OnGameOver(TileState.None);
        }
    }

    public void SetState(TileState state)
    {
        MyState = state;
        mySpriteRenderer.color = state == TileState.X ? xColor : oColor;
        mySpriteRenderer.sprite = state == TileState.X ? xSprite : oSprite;
        
        AudioManager.Instance.Play(AudioType.Click);
        tileAnimator.SetTrigger("choose");

        if (state == TileState.None) mySpriteRenderer.sprite = null;
    }

    public TileController GetNextTile(Direction direction)
    {
        var nextTileCoordinate = coordinate;

        switch (direction)
        {
            case Direction.Up:
                nextTileCoordinate.y++;
                break;
            case Direction.UpRight:
                nextTileCoordinate.y++;
                nextTileCoordinate.x++;
                break;
            case Direction.Right:
                nextTileCoordinate.x++;
                break;
            case Direction.DownRight:
                nextTileCoordinate.x++;
                nextTileCoordinate.y--;
                break;
            case Direction.Down:
                nextTileCoordinate.y--;
                break;
            case Direction.DownLeft:
                nextTileCoordinate.y--;
                nextTileCoordinate.x--;
                break;
            case Direction.Left:
                nextTileCoordinate.x--;
                break;
            case Direction.UpLeft:
                nextTileCoordinate.x--;
                nextTileCoordinate.y++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        return GameManager.Instance.ListTileController.Find(tile => tile.coordinate == nextTileCoordinate);
    }
}

public enum TileState
{
    None,
    X,
    O
}

public enum Direction
{
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left,
    UpLeft
}
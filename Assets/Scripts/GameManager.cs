using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.WSA;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public TileState MyTileState { get; set; }
    
    public int Turn { get; set; }

    public List<TileController> ListTileController => listTileController;
    [SerializeField] private List<TileController> listTileController;
    
    public bool IsStartGame { get; set; }

    public Canvas canvasChoose;
    [SerializeField] private GameObject chooseSection;
    [SerializeField] private GameObject gameSection;
    
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI textStart;
    [SerializeField] private TextMeshProUGUI textWinner;
    [SerializeField] private Button buttonRestart;

    [SerializeField] private GameObject endSection;
    [SerializeField] private GameObject draw;
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject winX;
    [SerializeField] private GameObject winO;

    [SerializeField] private Animator endGameAnimator;
    
    [SerializeField] private LineRenderer lineRenderer;
    
    private void Awake()
    {
        Instance = this;
    }

    private void PopulateEndSection(TileState winner)
    {
        gameSection.SetActive(false);
        endSection.SetActive(true);

        var isWin = winner != TileState.None;
        draw.SetActive(!isWin);
        win.SetActive(isWin);
        winX.SetActive(winner==TileState.X);
        winO.SetActive(winner==TileState.O);
        
        endGameAnimator.SetTrigger(isWin ? "Win" : "Draw");
        AudioManager.Instance.Play(isWin ? AudioType.Win : AudioType.Draw);
    }
    
    public void OnChoose(TileState state)
    {
        MyTileState = state;
        canvasChoose.enabled = false;
        chooseSection.SetActive(false);
        gameSection.SetActive(true);
        IsStartGame = true;
    }
    
    public void OnStartGame()
    {
        IsStartGame = true;
        canvas.enabled = false;

        buttonRestart.gameObject.SetActive(false);
        textWinner.enabled = false;
        endSection.SetActive(false);
    }

    public void OnClick_RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
    
    public void OnGameOver(TileState state)
    {
        IsStartGame = false;
        canvas.enabled = true;

        PopulateEndSection(state);

        canvas.enabled = true;
    }

    public (bool, TileState) HasWinner()
    {
        foreach (var tile in listTileController)
        {
            if (tile.MyState == TileState.None) continue;
            
            foreach (var direction in Enum.GetValues(typeof(Direction)))
            {
                var next = tile.GetNextTile((Direction) direction);
                if (!next) continue;
                
                if (next.MyState != tile.MyState) continue;

                var lastTile = next.GetNextTile((Direction) direction);
                if(!lastTile) continue;
                
                if(lastTile.MyState != tile.MyState) continue;

                LineAnimation(new []{tile.transform.position,lastTile.transform.position});
                return (true, tile.MyState);
            }
        }

        return (false, TileState.None);
    }

    private void LineAnimation(Vector3[] points)
    {
        IEnumerator Do()
        {
            lineRenderer.gameObject.SetActive(true);
                        
            float passed = 0;
            float time = 1;
            var init = points[0];
            var target = points[1];
            lineRenderer.SetPosition(0,init);
            
            AudioManager.Instance.Play(AudioType.Line);
            
            while (passed<time)
            {
                passed += Time.deltaTime;
                var current = Vector3.Lerp(init, target, passed / time);
                lineRenderer.SetPosition(1, current);	
                yield return null;
            }

            lineRenderer.gameObject.SetActive(false);
            yield return new WaitForSeconds(1);
        }

        StartCoroutine(Do());
    }
    
    public bool HasNoneTile()
    {
        return listTileController.Exists(x => x.MyState == TileState.None);
    }
}
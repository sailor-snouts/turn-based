using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController player1;
    [SerializeField] private PlayerController player2;
    private PlayerController currentPlayer;
    private static GameManager instance = null;

    public void Awake()
    {
        if (GameManager.instance && GameManager.instance != this)
        {
            DestroyImmediate(this.gameObject);
            return;
        }

        GameManager.instance = this;
    }

    public void Start()
    {
        this.currentPlayer = this.player1;
        this.player1.BeginTurn();
    }

    public static GameManager GetInstance()
    {
        return GameManager.instance;
    }

    public void NextPlayer()
    {
        this.currentPlayer = this.GetOtherPlayerController(this.currentPlayer);
    }

    public PlayerController GetOtherPlayerController(PlayerController player)
    {
        return player == this.player1 ? this.player2 : this.player1;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using System;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public enum GameState
{
    WaitingForPlayers,
    InProgress,
    GameOver
}

public class GameManager : NetworkBehaviour
{
    // Singleton instance of GameManager
    public static GameManager Instance { get; private set; }

    private readonly List<PlayerController> _players = new();
    private readonly Dictionary<PlayerController, PlayerChoice> _playerChoices = new();

    [SyncVar(hook = nameof(OnGameStateChanged))]
    public GameState CurrentGameState = GameState.WaitingForPlayers;



    #region Unity Callbacks

    /// <summary>
    /// Add your validation code here after the base.OnValidate(); call.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }

    // NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.
    void Awake()
    {
        // TODO: I commented this out because I'm having issues and not sure if this is what's causing it
        // will bring it back later maybe

        // Ensure only one instance of GameManager exists
        // if (Instance == null)
        // {
        Instance = this;
        // }
        // else
        // {
        //     Destroy(gameObject); // Destroy duplicate instances
        // }
    }

    void Start()
    {
        // moved this to Start to heed the warning about DontDestroyOnLoad
        // but not sure if this would work correctly in a multiplayer context
        // DontDestroyOnLoad(gameObject); // Keep GameManager across scenes
    }

    #endregion

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();

        // Initialize game state
        if (_players.Count < 2)
        {
            CurrentGameState = GameState.WaitingForPlayers;
        }

        // should I register the gamemanager instance here??? aaaaaaaaa idk
    }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() { }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() { }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion

    #region Server Only Methods

    [Server]
    public void RegisterPlayer(PlayerController player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);

            // Check if we can start the game
            if (_players.Count == 2 && CurrentGameState == GameState.WaitingForPlayers)
            {
                StartRound();
            }
        }
    }

    [Server]
    public void StartRound()
    {
        CurrentGameState = GameState.InProgress;
        // Reset player choices
        _playerChoices.Clear();

        foreach (var player in _players)
        {
            player.Choice = PlayerChoice.None;
        }
    }

    [Server]
    public void PlayerMadeChoice(PlayerController player, PlayerChoice choice)
    {
        if (CurrentGameState != GameState.InProgress)
        {
            Debug.LogWarning($"Player {player.PlayerName} tried to make a choice while the game is not in progress.");
            return;
        }

        if (!_players.Contains(player))
        {
            Debug.LogWarning($"Player {player.PlayerName} is not registered in the game.");
            return;
        }


        // Store the player's choice
        _playerChoices[player] = choice;
        Debug.Log($"Player {player.PlayerName} made choice: {choice}");

        // Check if all players have made their choices
        if (_playerChoices.Count == _players.Count)
        {
            DetermineRoundWinner();
        }

    }

    [Server]
    public void DetermineRoundWinner()
    {
        if (CurrentGameState != GameState.InProgress) return;

        CurrentGameState = GameState.GameOver; // Set to GameOver temporarily to prevent further actions

        PlayerController p1 = _players[0];
        PlayerController p2 = _players[1];
        PlayerChoice choice1 = _playerChoices.ContainsKey(p1) ? _playerChoices[p1] : PlayerChoice.None;
        PlayerChoice choice2 = _playerChoices.ContainsKey(p2) ? _playerChoices[p2] : PlayerChoice.None;

        // Sync choices to players
        p1.Choice = choice1;
        p2.Choice = choice2;

        string result = "";

        // Determine the winner
        if (choice1 == PlayerChoice.Rock && choice2 == PlayerChoice.Scissors ||
            choice1 == PlayerChoice.Scissors && choice2 == PlayerChoice.Paper ||
            choice1 == PlayerChoice.Paper && choice2 == PlayerChoice.Rock)
        {
            result = $"Round Winner: {p1.PlayerName}";
            p1.PlayerScore++;
        }
        else if (choice1 == choice2)
        {
            result = "It's a tie!";
        }
        else
        {
            result = $"Round Winner: {p2.PlayerName}";
            p2.PlayerScore++;
        }

        RpcShowResults(result, p1.PlayerName, p1.Choice, p2.PlayerName, p2.Choice);
        Invoke(nameof(StartRound), 3f); // Restart the round after 3 seconds
    }

    #endregion

    #region Client RPCs

    [ClientRpc]
    public void RpcShowResults(string result, string player1Name, PlayerChoice player1Choice, string player2Name, PlayerChoice player2Choice)
    {
        Debug.Log($"RPC Show Results: {result}");

        string choicesMessage = $"{player1Name} played {player1Choice}\n{player2Name} played {player2Choice}";

        UIManager.Instance.ShowRoundResults(result + "\n" + choicesMessage);
    }

    #endregion

    #region SyncVar Hooks
    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        if (!NetworkClient.active || UIManager.Instance == null)
        {
            return;
        }

        Debug.Log($"Game state changed from {oldState} to {newState}");

        UIManager.Instance.UpdateStateText(newState.ToString());

        if (newState == GameState.InProgress)
        {
            UIManager.Instance.ShowGameUI(true);
        }
        else
        {
            UIManager.Instance.ShowGameUI(false);
        }
    }
    #endregion
}

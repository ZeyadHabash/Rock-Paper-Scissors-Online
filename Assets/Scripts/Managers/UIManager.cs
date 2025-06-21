using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Button rockButton;
    public Button paperButton;
    public Button scissorsButton;
    public GameObject choicePanel;
    public TextMeshProUGUI gameStateText;
    public TextMeshProUGUI resultText;


    void Awake()
    {
        // Ensure only one instance of UIManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        // Add listeners to the buttons to call the command on the local player
        rockButton.onClick.AddListener(() => MakeChoice(PlayerChoice.Rock));
        paperButton.onClick.AddListener(() => MakeChoice(PlayerChoice.Paper));
        scissorsButton.onClick.AddListener(() => MakeChoice(PlayerChoice.Scissors));
    }

    void MakeChoice(PlayerChoice choice)
    {
        // Find the local player object and call its command
        PlayerController localPlayer = NetworkClient.localPlayer.GetComponent<PlayerController>();
        if (localPlayer != null)
        {
            localPlayer.CmdMakeChoice(choice);
            ShowGameUI(false); // Hide buttons after making a choice
            gameStateText.text = "Waiting for opponent...";
        }
    }

    public void UpdateStateText(string text)
    {
        gameStateText.text = text;
    }

    public void ShowRoundResults(string text)
    {
        resultText.text = text;
    }

    public void ShowGameUI(bool show)
    {
        choicePanel.SetActive(show);
        // When showing choice buttons, clear the previous result
        if (show)
        {
            resultText.text = "";
        }
    }
}
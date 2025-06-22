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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    void Start()
    {
        rockButton.onClick.AddListener(() => MakeChoice(PlayerChoice.Rock));
        paperButton.onClick.AddListener(() => MakeChoice(PlayerChoice.Paper));
        scissorsButton.onClick.AddListener(() => MakeChoice(PlayerChoice.Scissors));
    }

    public void DebugLog(string message)
    {
        Debug.Log(message);
    }

    void MakeChoice(PlayerChoice choice)
    {
        Debug.Log($"Making choice: {choice}");
        PlayerController localPlayer = NetworkClient.localPlayer.GetComponent<PlayerController>();
        if (localPlayer != null)
        {
            localPlayer.CmdMakeChoice(choice);
            ShowGameUI(false);
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
        if (show)
        {
            resultText.text = "";
        }
    }
}
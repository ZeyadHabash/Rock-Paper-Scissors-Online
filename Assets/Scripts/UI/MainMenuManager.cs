using UnityEngine;
 
 public class MainMenuManager : MonoBehaviour
 {
     [Header("UI Panels")]
     [Tooltip("The main menu panel that is shown on start.")]
     public GameObject mainMenuPanel;
     [Tooltip("The panel for creating a new room.")]
     public GameObject createRoomPanel;
     [Tooltip("The panel for joining an existing room.")]
     public GameObject joinRoomPanel;
 

     public void ShowCreateRoomPanel()
     {
         if (mainMenuPanel != null)
         {
             mainMenuPanel.SetActive(false);
         }
         if (createRoomPanel != null)
         {
             createRoomPanel.SetActive(true);
         }
     }
 

     public void ShowJoinRoomPanel()
     {
         if (mainMenuPanel != null)
         {
             mainMenuPanel.SetActive(false);
         }
         if (joinRoomPanel != null)
         {
             joinRoomPanel.SetActive(true);
         }
     }
 

     public void ShowMainMenuPanel()
     {
         if (createRoomPanel != null)
         {
             createRoomPanel.SetActive(false);
         }
         if (joinRoomPanel != null)
         {
             joinRoomPanel.SetActive(false);
         }
         if (mainMenuPanel != null)
         {
             mainMenuPanel.SetActive(true);
         }
     }
 

     public void QuitGame()
     {
         Debug.Log("Quit button pressed. Application will close in a build.");
         Application.Quit();
     }
 }
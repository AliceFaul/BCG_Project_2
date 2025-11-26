using UnityEngine;

public class UIManagerSkillTree : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject inGameMenu;     
    public GameObject skillTreeUI;    

    private bool isSkillTreeOpen = false;
    private bool isInGameMenuOpen = false;

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
           
            if (isSkillTreeOpen)
            {
                BackToInGameMenu();
            }
            
            else if (isInGameMenuOpen)
            {
                CloseInGameMenu();
            }
           
            else
            {
                OpenInGameMenu();
            }
        }
    }

    public void OpenInGameMenu()
    {
        inGameMenu.SetActive(true);
        skillTreeUI.SetActive(false);
        isInGameMenuOpen = true;
        isSkillTreeOpen = false;
        Time.timeScale = 0f;
    }

    public void CloseInGameMenu()
    {
        inGameMenu.SetActive(false);
        skillTreeUI.SetActive(false);
        isInGameMenuOpen = false;
        isSkillTreeOpen = false;
        Time.timeScale = 1f;
    }

    public void OpenSkillTree()
    {
        inGameMenu.SetActive(false);
        skillTreeUI.SetActive(true);
        isSkillTreeOpen = true;
        isInGameMenuOpen = false;
        Time.timeScale = 0f;
    }

    public void BackToInGameMenu()
    {
        inGameMenu.SetActive(true);
        skillTreeUI.SetActive(false);
        isSkillTreeOpen = false;
        isInGameMenuOpen = true;
        Time.timeScale = 0f;
    }
}

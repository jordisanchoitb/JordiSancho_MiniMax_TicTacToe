using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI textStatusGame;

    private void Start()
    {
        if (SceneManager.sceneCount > 1)
        {
            if (SceneManager.GetSceneAt(1).name.Equals("EndGame"))
                AssignStateGame();
        }
    }
    public void ChoiceCirclePlay()
    {
        GameObject tmp1 = GameManager.Instance.token1; 
        GameObject tmp2 = GameManager.Instance.token2;

        GameManager.Instance.token1 = tmp2;
        GameManager.Instance.token2 = tmp1;

        SceneManager.UnloadSceneAsync("ChoicePlayerToken");
        SceneManager.LoadScene("ChoicePlayerStart", LoadSceneMode.Additive);
    }

    public void ChoiceCrossPlay()
    {
        SceneManager.UnloadSceneAsync("ChoicePlayerToken");
        SceneManager.LoadScene("ChoicePlayerStart", LoadSceneMode.Additive);
    }

    public void ChoiceMeStart()
    {
        GameManager.Instance.state = States.CanMove;
        SceneManager.UnloadSceneAsync("ChoicePlayerStart");
        GameManager.Instance.isGameStarted = true;
    }

    public void ChoiceIAStart()
    {
        GameManager.Instance.state = States.CantMove;
        SceneManager.UnloadSceneAsync("ChoicePlayerStart");
        GameManager.Instance.isGameStarted = true;
        GameManager.Instance.RandomAI();
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
        GameManager.Instance.isGameStarted = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void AssignStateGame()
    {
        switch (GameManager.Instance.whoWin)
        {
            case 1:
                textStatusGame.text = "You Win!";
                textStatusGame.color = Color.green;
                break;
            case -1:
                textStatusGame.text = "You Lose";
                textStatusGame.color = Color.red;
                break;
            case 0:
                textStatusGame.text = "Draw";
                textStatusGame.color = Color.yellow;
                break;
        }
    }
}

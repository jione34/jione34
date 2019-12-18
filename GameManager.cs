using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartButton;
    public GameObject UIExitButton;

    void Start()
    {
        
    }

    public void NestStage() //스테이지 넘기기
    {
        if(stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();
            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("게임 끝");
            UIRestartButton.SetActive(true);
            UIExitButton.SetActive(true);
            Text ButtonText = UIRestartButton.GetComponentInChildren<Text>();
            ButtonText.text = "Clear!";
            ViewButton();
        }

    }

    public void HealthDown() //HP 관리
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(0, 0, 0, 0.1f);
        }

        else
        {
            UIhealth[0].color = new Color(0, 0, 0, 0.1f);
            player.OnDie();
            Debug.Log("게임 끝");
            UIRestartButton.SetActive(true);
            UIExitButton.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //낙사
        if (collision.gameObject.tag == "Player")
        {
            //리스폰
            if (health > 1)
            {
                PlayerReposition();
            }
            HealthDown();
        }
    }

    void ViewButton()
    {
        UIRestartButton.SetActive(true);
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(57, 0, 0);
        player.VelocityZero();
    }

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
}

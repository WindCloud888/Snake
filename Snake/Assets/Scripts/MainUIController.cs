using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour {

	private static MainUIController _instance;
	public static MainUIController Instance
	{
		get
		{
			return _instance;
		}
	}

	public bool hasBorder=true;
	public int score = 0;
	public int length = 0;
	public Text msgText;
	public Text scoreText;
	public Text lengthText;
	public Image pauseImage;
	public Sprite[] pauseSprite; 
	public Image bgImage;
	private Color tempColor;
	public bool isPause = false;
	public void UpdateUI(int s=5,int l = 1)
    {
		score += s;
		length += l;
		scoreText.text = "得分：\n" + score;
		lengthText.text = "长度：\n" + length;
    }


	public void Pause()
    {
		isPause = !isPause;
		if (isPause)
        {
			Time.timeScale = 0;
			pauseImage.sprite=pauseSprite[1];
        }
		else
        {
			Time.timeScale = 1;
			pauseImage.sprite = pauseSprite[0];
        }
    }

	// Use this for initialization

	void Awake()
    {
		_instance = this;
    }
	void Start () {
        if (PlayerPrefs.GetInt("border", 1) == 0)
        {
			hasBorder = false;
			foreach(Transform t in bgImage.gameObject.transform)
            {
				t.gameObject.GetComponent<Image>().enabled = false;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        switch (score / 100)
        {
			case 0:
			case 1:
			case 2:
				break;
			case 3:
			case 4:
				ColorUtility.TryParseHtmlString("#CCEEFFFF", out tempColor);  //将颜色的十六进制形式转换为Unity的Color结构体形式
				bgImage.color = tempColor;
				msgText.text = "阶段" + 2;
				SnakeHead.Instance.velocity = 0.3f;
				break;
			case 5:
			case 6:
				ColorUtility.TryParseHtmlString("#CCFFDBFF", out tempColor);
				bgImage.color = tempColor;
				msgText.text = "阶段" + 3;
				SnakeHead.Instance.velocity = 0.25f;
				break;
			case 7:
			case 8:
				ColorUtility.TryParseHtmlString("#EBFFCCFF", out tempColor);
				bgImage.color = tempColor;
				msgText.text = "阶段" + 4;
				SnakeHead.Instance.velocity = 0.2f;
				break;
			case 9:
			case 10:
				ColorUtility.TryParseHtmlString("#FFF3CCFF", out tempColor);
				bgImage.color = tempColor;
				msgText.text = "阶段" + 5;
				SnakeHead.Instance.velocity = 0.16f;
				break;
			case 11:
				ColorUtility.TryParseHtmlString("#FFDACCFF", out tempColor);
				bgImage.color = tempColor;
				msgText.text = "无尽阶段" ;
				SnakeHead.Instance.velocity = 0.12f;
				break;

        }
	}
	public void Home()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(0);
	}
}

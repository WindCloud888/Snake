using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


using UnityEngine;
using System.Linq;

public class SnakeHead : MonoBehaviour
{
    private static SnakeHead _instance;
    public static SnakeHead Instance
    {
        get
        {
            return _instance;
        }
    }

    public List<Transform> bodyList = new List<Transform>();
    public float velocity = 0.35f;
    public int step;
    private int x;
    private int y;
    private Vector3 headPos;
    private Transform canvas;  
    private bool isDie;


    public AudioClip eatClip;
    public AudioClip dieClip;
    public GameObject bodyPrefab;
    public Sprite[] bodySprites = new Sprite[2];
    public GameObject dieEffect;

    private void Awake()
    {
        _instance = this;
        canvas = GameObject.Find("Canvas").transform;
        //通过Resources.Load(string path)方法加载资源，path的书写不需要写Resources/以及文件扩展名
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(PlayerPrefs.GetString("sh", "sh02"));
        bodySprites[0] = Resources.Load<Sprite>(PlayerPrefs.GetString("sb01", "sb0201"));
        bodySprites[1] = Resources.Load<Sprite>(PlayerPrefs.GetString("sb02", "sb0201"));

    }
    void Start()
    {
        InvokeRepeating("Move", 0, velocity);
        x = 0;
        y = step;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)&&MainUIController.Instance.isPause!=true&&isDie==false)
        {
            CancelInvoke();
            if (velocity >= 0.3f) 
            InvokeRepeating("Move", 0, velocity - 0.2f); 
            else if (velocity < 0.3f && velocity >= 0.2f)
            {
                InvokeRepeating("Move", 0, 0.08f);
            }
            else
            {
                InvokeRepeating("Move", 0, velocity-0.08f);
            }
         }
        if(Input.GetKeyUp(KeyCode.Space)&& MainUIController.Instance.isPause != true && isDie == false)
        {
            CancelInvoke();
            InvokeRepeating("Move", 0, velocity);
        }
        if (Input.GetKey(KeyCode.W)&&y!=-step && MainUIController.Instance.isPause != true && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            x = 0; y = step;
        }  
        if (Input.GetKey(KeyCode.S)&&y!=step && MainUIController.Instance.isPause != true && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 180);
            x = 0; y = -step;
        }
        if (Input.GetKey(KeyCode.A)&&x!=step && MainUIController.Instance.isPause != true && isDie == false)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 90);
            Debug.Log(123);
            x = -step; y = 0;
        }
        if (Input.GetKey(KeyCode.D)&&x!=-step && MainUIController.Instance.isPause != true)
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, -90);
            x = step; y = 0;
        }
    }

    void Move()
    {
        headPos = gameObject.transform.localPosition;//保存下来蛇头移动前的位置
        gameObject.transform.localPosition = new Vector3(headPos.x + x, headPos.y + y, headPos.z);//蛇头向期望位置移动
        if (bodyList.Count > 0)
        {
            ////由于我们是双色蛇，此方法弃用
            //bodyList.Last().localPosition = headPos;    //将蛇尾移动到蛇头移动前的位置
            //bodyList.Insert(0, bodyList.Last());            //将蛇尾在List中的位置更新到最前
            //bodyList.RemoveAt(bodyList.Count - 1);    //移除List最末尾的蛇尾引用

            //由于我们是双色蛇，故采用此方法
            for(int i=bodyList.Count-2;i>=0;i--)               //从后往前开始移动蛇身
            {
                bodyList[i + 1].localPosition = bodyList[i].localPosition;          //每一个蛇身都移动到它前面一个节点的位置
            }
            bodyList[0].localPosition = headPos;                          //第一个蛇身移动到蛇头移动前的位置 
        }

    }

    void Grow()
    {
        AudioSource.PlayClipAtPoint(eatClip, Vector3.zero);
        int index = (bodyList.Count % 2 == 0) ? 0 : 1;
        //GameObject body = Instantiate(bodyPrefab);
        GameObject body = Instantiate(bodyPrefab,new Vector3(2000,2000,0),Quaternion.identity);
        body.GetComponent<Image>().sprite = bodySprites[index];
        body.transform.SetParent(canvas,false);
        bodyList.Add(body.transform);

     }
    void Die()
    {
        AudioSource.PlayClipAtPoint(dieClip,new Vector3(0,0,-10));
        headPos = gameObject.transform.localPosition;
        CancelInvoke();
        isDie = true;
        Debug.Log(headPos);
        GameObject dieEff=Instantiate(dieEffect,headPos, Quaternion.identity);
       dieEff.transform.SetParent(canvas, false);
        PlayerPrefs.SetInt("lastl", MainUIController.Instance.length); 
        PlayerPrefs.SetInt("lasts", MainUIController.Instance.score);
        if (PlayerPrefs.GetInt("bests", 0)< MainUIController.Instance.score)
        {
            PlayerPrefs.SetInt("bestl", MainUIController.Instance.length);
            PlayerPrefs.SetInt("bests", MainUIController.Instance.score);
        }
        StartCoroutine(GameOver(1.5f));
    } 

    IEnumerator GameOver(float t)
    {
        yield return new WaitForSeconds(t);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if(collision.gameObject.CompareTag("Food"))
        if (collision.tag == "Food")
        {
            Destroy(collision.gameObject);
            MainUIController.Instance.UpdateUI();
            Grow();
            FoodMaker.Instance.MakeFood((Random.Range(0, 100) < 20) ? true : false);
            //if (Random.Range(0, 100) < 20)
            //{
            //    FoodMaker.Instance.MakeFood(true);
            //}
            //else
            //{
            //    FoodMaker.Instance.MakeFood(false);
            //} 
        }
        else if (collision.gameObject.CompareTag("Reward"))
        {  
            Destroy(collision.gameObject);
            MainUIController.Instance.UpdateUI(Random.Range(5,15)*10);
            Grow();
           // FoodMaker.Instance.MakeFood(false);
        }
        else if (collision.gameObject.CompareTag("Body"))
        {
            Die();
        }
        else
        {
            if (MainUIController.Instance.hasBorder)
            {
                Die();
            }
            else
            {
                switch (collision.gameObject.name)
                {
                    case "Up":
                        transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y + 30, transform.localPosition.z);
                        break;
                    case "Down":
                        transform.localPosition = new Vector3(transform.localPosition.x, -transform.localPosition.y - 30, transform.localPosition.z);
                        break;
                    case "Left":
                        transform.localPosition = new Vector3(-transform.localPosition.x + 180, transform.localPosition.y, transform.localPosition.z);
                        break;
                    case "Right":
                        transform.localPosition = new Vector3(-transform.localPosition.x + 240, transform.localPosition.y, transform.localPosition.z);
                        break;

                }
            } 
        }
    }
    }


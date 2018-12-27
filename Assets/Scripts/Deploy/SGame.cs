using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Advertisements;

public class SGame : MonoBehaviour
{
    private enum GameState
    {
        UNDEFINED,
        READY,
        PLAYING,
        PAUSED,
        GAMEOVER
    };

    private UnityEngine.Object oZombie = null;

    private GameObject[] targets = null;
    private GameObject pStart = null;
    private Button bStart = null;
    private Button bMenu = null;
    private GameObject pDialog = null;
    private Button bYes = null;
    private Button bNo = null;
    private Text tLife = null;
    private Text tReload = null;
    private Text tBullet = null;
    private GameObject pEnd = null;
    private Button bOk = null;
    private Button bReload = null;
    private Button bBomb = null;
    private Text tBomb = null;
    private Text tScore = null;

    private GameState state = GameState.UNDEFINED;
    private Dictionary<GameState, Action> updates = null;

    private float cooldown = float.NaN;
    private float timer = float.NaN;
    private System.Random rand = null;

    private int life = -1;

    private int maxBullet = 0;
    private int damage = 0;
    private int ammo = 0;
    private int bullets = 0;
    private bool isReloading = false;
    private float reloadTime = float.NaN;

    private int bomb = -1;
    private int score = -1;

    private static int mGameTrial = 0;

    private void Awake()
    {
        this.oZombie = Resources.Load("Prefabs/Zombie");

        this.targets = GameObject.FindGameObjectsWithTag("Target");
        Array.Sort(this.targets, delegate (GameObject _1, GameObject _2)
        {
            return _1.name.CompareTo(_2.name);
        });
        this.pStart = GameObject.Find("Canvas/PStart");
        var obj = GameObject.Find("Canvas/PStart/Button");
        this.bStart = obj.GetComponent<Button>();
        this.bStart.onClick.AddListener(this.OnClickStart);

        obj = GameObject.Find("Canvas/PMenu/Button");
        this.bMenu = obj.GetComponent<Button>();
        this.bMenu.onClick.AddListener(this.OnClickMenu);

        this.pDialog = GameObject.Find("Canvas/PDialog");
        obj = GameObject.Find("Canvas/PDialog/Yes");
        this.bYes = obj.GetComponent<Button>();
        this.bYes.onClick.AddListener(this.OnClickYes);
        obj = GameObject.Find("Canvas/PDialog/No");
        this.bNo = obj.GetComponent<Button>();
        this.bNo.onClick.AddListener(this.OnClickNo);
        this.pDialog.SetActive(false);

        obj = GameObject.Find("Canvas/PLife/Text");
        this.tLife = obj.GetComponent<Text>();

        this.pEnd = GameObject.Find("Canvas/PEnd");
        obj = GameObject.Find("Canvas/PEnd/Button");
        this.bOk = obj.GetComponent<Button>();
        this.bOk.onClick.AddListener(this.OnClickOk);
        this.pEnd.SetActive(false);

        obj = GameObject.Find("Canvas/PReload/Button");
        this.bReload = obj.GetComponent<Button>();
        this.bReload.onClick.AddListener(this.OnClickReload);

        obj = GameObject.Find("Canvas/PReload/Button/Text");
        this.tReload = obj.GetComponent<Text>();

        obj = GameObject.Find("Canvas/PBullet/Text");
        this.tBullet = obj.GetComponent<Text>();

        obj = GameObject.Find("Canvas/PBomb/Button");
        this.bBomb = obj.GetComponent<Button>();
        this.bBomb.onClick.AddListener(this.OnClickBomb);
        obj = GameObject.Find("Canvas/PBomb/Button/Text");
        this.tBomb = obj.GetComponent<Text>();

        obj = GameObject.Find("Canvas/PScore/Text");
        this.tScore = obj.GetComponent<Text>();

        this.state = GameState.READY;

        this.updates = new Dictionary<GameState, Action>();
        this.updates.Add(GameState.READY, this.UpdateReady);
        this.updates.Add(GameState.PLAYING, this.UpdatePlaying);
        this.updates.Add(GameState.PAUSED, this.UpdatePaused);
        this.updates.Add(GameState.GAMEOVER, this.UpdateGameover);

        this.cooldown = 2.0f;
        this.timer = 0.0f;
        this.rand = new System.Random();

        this.life = 3;
        this.tLife.text = "♡ " + this.life.ToString();

        this.maxBullet = 30;
        this.damage = 35;
        this.ammo = 10;
        this.bullets = 30;
        this.reloadTime = 1.5f;
    
        this.tReload.text = "R :" + this.ammo.ToString();
        this.tBullet.text = this.bullets.ToString() + "/" + this.maxBullet.ToString();

        this.bomb = 3;
        this.tBomb.text = "B " + this.bomb.ToString();
        this.score = 0;
        this.tScore.text = "SCORE " + this.score.ToString();
    }

    #region Functions to update

    private void Update()
    {
        this.updates[this.state]();
    }

    private void UpdateReady()
    {
        ;
    }

    private void UpdatePlaying()
    {
        this.InputPlaying();
        this.CheckSpawn();
        this.UpdateScore();
    }

    private void UpdatePaused()
    {
        ;
    }

    private void UpdatePanel(bool _isTarget, GameObject _obj)
    {
        if (_isTarget)
        {
            Shoot(_obj);
        }
        else
        {
            foreach (GameObject go in this.targets)
            {
                Image image = go.GetComponent<Image>();
                image.color = Color.white;
            }
        }
    }

    private void UpdateGameover()
    {
        // 게임을 5번 플레이 했다면, 광고를 시청하게 한다.
        if (mGameTrial == 5)
        {
            Advertisement.Show();
            mGameTrial = 0;
        }
    }

    private void OnClickOk()
    {
        SceneManager.LoadScene("Main");
    }

    #endregion

    #region Functions to process input

    private void OnClickStart()
    {
        this.pStart.SetActive(false);
        this.state = GameState.PLAYING;
    }

    private void InputPlaying()
    {
        bool isTargetPressed = false;
        GameObject targetObject = null;

        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (var go in raycastResults)
                {
                    if (go.gameObject.tag == "Target")
                    {
                        isTargetPressed = true;
                        targetObject = go.gameObject;
                        break;
                    }
                }
            }
        }

        this.UpdatePanel(isTargetPressed, targetObject);
    }

    private void OnClickMenu()
    {
        if (this.state == GameState.PLAYING)
        {
            this.state = GameState.PAUSED;
            this.pDialog.SetActive(true);
        }
    }

    private void OnClickReload()
    {
        if (this.state == GameState.PLAYING)
        {
            // 타임 딜레이 기능 추가 할 것.
            // 리로드 중에는 리로드가 되어서는 안됨.
            if (!isReloading)
            {
                Reload();
            }
            
        }
    }

    private void OnClickYes()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnClickNo()
    {
        this.state = GameState.PLAYING;
        this.pDialog.SetActive(false);
    }

    #endregion

    #region Functions to spawn zombie
    
    private void CheckSpawn()
    {
        this.timer += Time.deltaTime;

        if(this.timer >= this.cooldown)
        {
            this.SpawnZombie();
            this.timer = 0.0f;
        }
    }

    private void SpawnZombie()
    {
        int[] order = new int[9];
        for(int i = 0; i < 9; ++i)
        {
            order[i] = i;
        }
        for(int i = 8; i >= 0; --i)
        {
            int num = this.rand.Next(i + 1);
            int temp = order[num];
            order[num] = order[i];
            order[i] = temp;
        }

        for(int i = 0; i < 9; ++i)
        {
            GameObject target = this.targets[order[i]];

            if(target.transform.childCount == 0)
            {
                GameObject zombie = Instantiate(this.oZombie, target.transform) as GameObject;
                zombie.transform.SetParent(target.transform);
                break;
            }
        }
    }

    public void OnHit()
    {
        if (this.state == GameState.PLAYING)
        {
            this.life -= 1;
            this.tLife.text = "♡ " + this.life.ToString();

            if (this.life <= 0)
            {
                // 게임이 끝나면, 게임 시도 횟수를 증가한다.
                ++mGameTrial;

                this.state = GameState.GAMEOVER;
                this.pEnd.SetActive(true);
            }
        }
    }

    #endregion

    #region Functions for gun system

    public void Shoot(GameObject _obj)
    {
        if (this.state == GameState.PLAYING)
        {
            if (this.bullets > 0)
            {
                Image image = _obj.GetComponent<Image>();
                image.color = Color.red;

                if (_obj.transform.childCount > 0)
                {
                    GameObject zombie = _obj.gameObject.transform.GetChild(0).gameObject;
                    zombie.GetComponent<SZombie>().getDamaged(this.damage);
                }

                bullets--;
                this.tBullet.text = this.bullets.ToString() + "/" + this.maxBullet.ToString();
            }
        }
    }

    public void Reload()
    {
        if (this.state == GameState.PLAYING)
        {
            if (this.ammo > 0)
            {
                this.ammo--;
                bullets = maxBullet;
                this.tReload.text = "R :" + this.ammo.ToString();
                this.tBullet.text = this.bullets.ToString() + "/" + this.maxBullet.ToString();
            }
        }
    }

    #endregion

    #region Functions for item

    private void OnClickBomb()
    {
        if(this.state == GameState.PLAYING)
        {
            if (this.bomb > 0)
            {
                this.bomb = this.bomb - 1;
                this.tBomb.text = "B " + this.bomb.ToString();

                this.ProcessBomb();
            }
        }
    }

    private void ProcessBomb()
    {
        foreach (GameObject target in this.targets)
        {
            if(target.transform.childCount > 0)
            {
                Destroy(target.gameObject.transform.GetChild(0).gameObject);
            }
        }
    }

    #endregion

    #region Functions for score

    public void IncreaseScore(int _s)
    {
        this.score += _s;
    }

    private void UpdateScore()
    {
        this.tScore.text = "SCORE " + this.score.ToString();
    }

    #endregion
}
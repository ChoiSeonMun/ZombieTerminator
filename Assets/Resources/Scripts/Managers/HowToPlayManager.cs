using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class HowToPlayManager : GameManager
{
    public GameObject dialog = null;

    private Text mDialogText = null;
    private Button mDialogButton = null;

    private delegate void checkDelegate();
    private checkDelegate checkFunction = null;

    public void OnGreeting()
    {
        pauseTutorial();
        mDialogText.text = "Welcome to tutorial !\nPress OK to process";
        mDialogButton.onClick.AddListener(OffGreeting);
    }

    public void OffGreeting()
    {
        resumeTutorial();
        checkFunction = checkZombieSpawn;
    }

    public void OnWarningZombie()
    {
        pauseTutorial();
        mDialogText.text = "Zombie is approaching to you !\nPress it to attack !\nMissing it means losing life !";
        mDialogButton.onClick.AddListener(OffWarningZombie);
    }

    public void OffWarningZombie()
    {
        resumeTutorial();
        checkFunction = checkZombieDead;
    }

    public void OnKillingZombie()
    {
        pauseTutorial();
        mDialogText.text = "Got some score, look above !\nLost some ammo, look below !\n";
        mDialogButton.onClick.AddListener(OffKillingZombie);
    }

    public void OffKillingZombie()
    {
        resumeTutorial();
        checkFunction = checkLowAmmo;
    }

    public void OnLowAmmo()
    {
        pauseTutorial();
        mDialogText.text = "You have low ammo !\nPress bullet to reload !\nBomb button will help you, too !";
        mDialogButton.onClick.AddListener(OffLowAmmo);
    }

    public void OffLowAmmo()
    {
        resumeTutorial();
        checkFunction = checkFeverGuage;
    }

    public void OnWarningGuage()
    {
        pauseTutorial();
        mDialogText.text = "Whenever zombie dies, fever \nguage increases, look above !\nFever guage is reset, \nwhenever you hit !";
        mDialogButton.onClick.AddListener(OffWarningGuage);
    }

    public void OffWarningGuage()
    {
        resumeTutorial();
        checkFunction = checkFeverOn;
    }

    public void OnWarningFever()
    {
        pauseTutorial();
        mDialogText.text = "Fever guage is full !\nFever mode has been started !\nYou do not get damage !\nAnd one shot one kill !\nEven no reload delay !";
        mDialogButton.onClick.AddListener(OffWarningFever);
    }

    public void OffWarningFever()
    {
        resumeTutorial();
        checkFunction = checkFeverOff;
    }

    public void OnGoodbye()
    {
        pauseTutorial();
        mDialogText.text = "Fever mode ended !\nNow you returned to normal !\nTutorial completed, \nCongratulations !";
        mDialogButton.onClick.AddListener(OffGoodbye);
    }

    public void OffGoodbye()
    {
        resumeTutorial();
        checkFunction = checkRealEnd;
    }

    public void OnRealEnd()
    {
        pauseTutorial();
        mDialogText.text = "You do not have to stay here !\nPlease go back !";
        mDialogButton.onClick.AddListener(OffRealEnd);
    }

    public void OffRealEnd()
    {
        SceneManager.LoadScene("Main");
    }

    new void Awake()
    {
        base.Awake();

        mDialogText = dialog.GetComponentInChildren<Text>();
        mDialogButton = dialog.GetComponentInChildren<Button>();
    }

    new void Start()
    {
        base.Start();

        OnGreeting();
    }

    new void Update()
    {
        base.Update();

        if (checkFunction != null)
        {
            checkFunction.Invoke();
        }
    }

    private void pauseTutorial()
    {
        Time.timeScale = 0.0f;
        checkFunction = null;
        mDialogButton.onClick.RemoveAllListeners();
        dialog.SetActive(true);
    }

    private void resumeTutorial()
    {
        Time.timeScale = 1.0f;
        dialog.SetActive(false);
    }

    private void checkZombieSpawn()
    {
        ButtonExtension[] buttons = eventManager.spawnManager.targetButtons;

        foreach(ButtonExtension button in buttons)
        {
            if(button.transform.childCount > 0)
            {
                Zombie zombie = button.transform.GetChild(0).GetComponent<Zombie>();

                if(zombie != null)
                {
                    OnWarningZombie();
                    break;
                }
            }
        }
    }

    private void checkZombieDead()
    {
        ButtonExtension[] buttons = eventManager.spawnManager.targetButtons;

        foreach (ButtonExtension button in buttons)
        {
            if (button.transform.childCount > 0)
            {
                GameObject child = button.transform.GetChild(0).gameObject;
                
                if(child.name == "CrackAnimation(Clone)")
                {
                    OnKillingZombie();
                    break;
                }
            }
        }
    }

    private void checkLowAmmo()
    {
        int bullets = eventManager.gun.BulletCur;

        if(bullets <= 5)
        {
            OnLowAmmo();
        }
    }

    private void checkFeverGuage()
    {
        int feverGuage = eventManager.fever.FeverCount;

        if(feverGuage >= 15)
        {
            OnWarningGuage();
        }
    }

    private void checkFeverOn()
    {
        bool feverOn = eventManager.fever.IsFeverOn;

        if(feverOn == true)
        {
            OnWarningFever();
        }
    }

    private void checkFeverOff()
    {
        bool feverOn = eventManager.fever.IsFeverOn;

        if(feverOn == false)
        {
            OnGoodbye();
        }
    }

    private void checkRealEnd()
    {
        string score = eventManager.player.scoreText.text;

        if(int.Parse(score) > 2000)
        {
            OnRealEnd();
        }
    }
};
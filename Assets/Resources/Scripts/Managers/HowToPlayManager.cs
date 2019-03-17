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

    private Text dialogText = null;
    private Button dialogButton = null;

    private delegate void checkDelegate();
    private checkDelegate checkFunction = null;

    public void GreetingOn()
    {
        pauseTutorial();
        dialogText.text = "Welcome to tutorial !\nPress OK to process";
        dialogButton.onClick.AddListener(GreetingOff);
    }

    public void GreetingOff()
    {
        resumeTutorial();
        checkFunction = checkZombieSpawn;
    }

    public void ZombieWarnOn()
    {
        pauseTutorial();
        dialogText.text = "Zombie is approaching to you !\nPress it to attack !\nMissing it means losing life !";
        dialogButton.onClick.AddListener(ZombieWarnOff);
    }

    public void ZombieWarnOff()
    {
        resumeTutorial();
        checkFunction = checkZombieDead;
    }

    public void ZombieDeadOn()
    {
        pauseTutorial();
        dialogText.text = "Got some score, look above !\nLost some ammo, look below !\n";
        dialogButton.onClick.AddListener(ZombieDeadOff);
    }

    public void ZombieDeadOff()
    {
        resumeTutorial();
        checkFunction = checkLowAmmo;
    }

    public void LowAmmoOn()
    {
        pauseTutorial();
        dialogText.text = "You have low ammo !\nPress bullet to reload !\nBomb button will help you, too !";
        dialogButton.onClick.AddListener(LowAmmoOff);
    }

    public void LowAmmoOff()
    {
        resumeTutorial();
        checkFunction = checkFeverGuage;
    }

    public void GuageWarnOn()
    {
        pauseTutorial();
        dialogText.text = "Whenever zombie dies, fever \nguage increases, look above !\nFever guage is reset, \nwhenever you hit !";
        dialogButton.onClick.AddListener(GuageWarnOff);
    }

    public void GuageWarnOff()
    {
        resumeTutorial();
        checkFunction = checkFeverOn;
    }

    public void FeverWarnOn()
    {
        pauseTutorial();
        dialogText.text = "Fever guage is full !\nFever mode has been started !\nYou do not get damage !\nAnd one shot one kill !\nEven no reload delay !";
        dialogButton.onClick.AddListener(FeverWarnOff);
    }

    public void FeverWarnOff()
    {
        resumeTutorial();
        checkFunction = checkFeverOff;
    }

    public void GoodbyeOn()
    {
        pauseTutorial();
        dialogText.text = "Fever mode ended !\nNow you returned to normal !\nTutorial completed, \nCongratulations !";
        dialogButton.onClick.AddListener(GoodbyeOff);
    }

    public void GoodbyeOff()
    {
        resumeTutorial();
        checkFunction = checkRealEnd;
    }

    public void RealEndOn()
    {
        pauseTutorial();
        dialogText.text = "You do not have to stay here !\nPlease go back !";
        dialogButton.onClick.AddListener(RealEndOff);
    }

    public void RealEndOff()
    {
        SceneManager.LoadScene("Main");
    }

    new void Awake()
    {
        base.Awake();

        dialogText = dialog.GetComponentInChildren<Text>();
        dialogButton = dialog.GetComponentInChildren<Button>();
    }

    new void Start()
    {
        base.Start();

        GreetingOn();
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
        dialogButton.onClick.RemoveAllListeners();
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
                    ZombieWarnOn();
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
                    ZombieDeadOn();
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
            LowAmmoOn();
        }
    }

    private void checkFeverGuage()
    {
        int feverGuage = eventManager.fever.FeverCount;

        if(feverGuage >= 15)
        {
            GuageWarnOn();
        }
    }

    private void checkFeverOn()
    {
        bool feverOn = eventManager.fever.IsFeverOn;

        if(feverOn == true)
        {
            FeverWarnOn();
        }
    }

    private void checkFeverOff()
    {
        bool feverOn = eventManager.fever.IsFeverOn;

        if(feverOn == false)
        {
            GoodbyeOn();
        }
    }

    private void checkRealEnd()
    {
        string score = eventManager.player.scoreText.text;

        if(int.Parse(score) > 2000)
        {
            RealEndOn();
        }
    }
};
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EventManager : MonoBehaviour
{
    public Player player = null;
    public Gun gun = null;
    public Fever fever = null;
    public Bomb bomb = null;
    public GameManager gameManager = null;
    public SpawnManager spawnManager = null;
    public LevelManager levelManager = null;
    // EState 이벤트
    public UnityEvent pauseEvent = new UnityEvent();
    public UnityEvent resumeEvent = new UnityEvent();
    public UnityEvent endEvent = new UnityEvent();
    // Fever 이벤트
    public UnityEvent feverOnEvent = new UnityEvent();
    public UnityEvent feverOffEvent = new UnityEvent();
    // Level 이벤트
    public UnityEvent levelUpEvent = new UnityEvent();

    void Awake()
    {
        //
        pauseEvent.AddListener(gun.BlockReload);
        pauseEvent.AddListener(bomb.BlockBomb);
        pauseEvent.AddListener(levelManager.BlockUpdate);
        pauseEvent.AddListener(spawnManager.StopTargets);
        pauseEvent.AddListener(fever.PauseFever);
        //
        resumeEvent.AddListener(gun.GrantReload);
        resumeEvent.AddListener(bomb.GrantBomb);
        resumeEvent.AddListener(levelManager.GrantUpdate);
        resumeEvent.AddListener(spawnManager.ResumeTargets);
        resumeEvent.AddListener(fever.ResumeFever);
        //
        endEvent.AddListener(gameManager.EndGame);
        endEvent.AddListener(spawnManager.StopTargets);
        endEvent.AddListener(player.ReportScore);
        //
        feverOnEvent.AddListener(gun.PowerUp);
        feverOnEvent.AddListener(spawnManager.SpawnSpeedUp);
        feverOnEvent.AddListener(levelManager.BlockUpdate);
        //
        feverOffEvent.AddListener(gun.PowerDown);
        feverOffEvent.AddListener(spawnManager.SpawnSpeedDown);
        feverOffEvent.AddListener(spawnManager.DestroyZombies);
        feverOffEvent.AddListener(levelManager.GrantUpdate);
        //
        levelUpEvent.AddListener(spawnManager.DecreaseSpawnCooldown);
    }
}

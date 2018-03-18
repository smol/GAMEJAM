using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaveHandler : NetworkBehaviour
{

    public enum SpawnersAction
    {
        spawn,
        next,
        count

    }

    [System.Serializable]
    public class Wave
    {
        public int numberOfEnemy = 0;
        public List<GameObject> spawnedEnemy = new List<GameObject>();
    }


    public GameObject enemy;
    public int enemyPerDifficultyPoint = 3;

    //Synchronized variables
    [SyncVar] int synchronizedWaveNumber;

    public int waveNumber = 0;

    private GameManager gameMAnager;
    private Spawner[] spawnPoints;
    private Wave currentWave;
    private int currentEnnemyNumber;
    private float nextUpdate = 1f;
    private SpawnersAction action = SpawnersAction.next;
    private bool spawning;

    void Start() {
        if (isServer)
        {
            spawnPoints = GameObject.FindObjectsOfType<Spawner>();
            gameMAnager = GameObject.FindObjectOfType<GameManager>();
        }

    }

    void Update() {
        if (isServer)
        {
            Debug.Log("Je suis le serveur je tente un spawn");
            if (Time.time >= nextUpdate)
            {
                Debug.Log("Il est temps de spawn");
                nextUpdate = Mathf.FloorToInt(Time.time) + 1;
                HandleCurrentState();
                SendWaveNumber();
            }
        }
        else
        {
            waveNumber = synchronizedWaveNumber;
        }
    }

    private void HandleCurrentState() {
        //Debug.Log("Doing " + action.ToString());
        switch (action) {
            case SpawnersAction.spawn:
                if (spawning) {
                    SpawnWave();
                    spawning = false;
                } 
                break;
            case SpawnersAction.next:
                gameMAnager.IncreaseDifficultyLvl();
                WaveCompleted();
                spawning = true;
                break;
            case SpawnersAction.count:
                EnemyIsAlive();
                break;
        }
    }

    private void WaveCompleted() {
        currentWave = new Wave();
        currentWave.numberOfEnemy = enemyPerDifficultyPoint * (gameMAnager.GetDifficultyLvl() + 1);
        waveNumber += 1;

        action = SpawnersAction.spawn;
    }

    void EnemyIsAlive() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //Debug.Log("It remain " + enemies.Length + " enemies");
        if (enemies.Length == 0) {
            action = SpawnersAction.next;
        }
    }

    void SpawnWave() {
        //for (int i = 0; i < currentWave.numberOfEnemy; i++) {
        currentEnnemyNumber = 0;
        Invoke("Spawn", 0.5f);
    }

    public void Spawn() {
        if (currentEnnemyNumber < currentWave.numberOfEnemy) {
            GameObject o = Instantiate(enemy, spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);
            o.transform.parent = this.transform;
            Debug.Log("SPAWN !!!!!");
            NetworkServer.Spawn(o);
            currentEnnemyNumber++;
            Invoke("Spawn", 0.5f);
        } else {
            action = SpawnersAction.count;
        }
    }

    //Network function

    //Client
    [Client]
    void SendWaveNumber()
    {
        CmdSendWaveNumberToServer(waveNumber);
    }


    //Command
    [Command]
    void CmdSendWaveNumberToServer(int waveNumber)
    {
        synchronizedWaveNumber = waveNumber;
    }

}

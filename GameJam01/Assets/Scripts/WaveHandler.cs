using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaveHandler : NetworkBehaviour {

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

	public int waveNumber = 0;

	private GameManager gameMAnager;
	private Spawner[] spawnPoints;
	private Wave currentWave;
	private float nextUpdate = 1f;
	private SpawnersAction action = SpawnersAction.next;

	void Start () {
		spawnPoints = GameObject.FindObjectsOfType<Spawner> ();
		gameMAnager = GameObject.FindObjectOfType<GameManager> ();
	}

	void Update () {
		if(Time.time>=nextUpdate){
			nextUpdate=Mathf.FloorToInt(Time.time)+1;
			HandleCurrentState();
		}
	}

	private void HandleCurrentState() {
		Debug.Log ("Doing " + action.ToString());
		switch (action) {
			case SpawnersAction.spawn:
				SpawnWave ();
				break;
			case SpawnersAction.next:
				gameMAnager.IncreaseDifficultyLvl ();
				WaveCompleted ();
				break;
			case SpawnersAction.count:
				EnemyIsAlive ();
				break;
		}
	}

	private void WaveCompleted () {
		currentWave = new Wave ();
		currentWave.numberOfEnemy = enemyPerDifficultyPoint*(gameMAnager.GetDifficultyLvl()+1);
		waveNumber += 1;

		action = SpawnersAction.spawn;
	}

	void EnemyIsAlive () {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		Debug.Log ("It remain " + enemies.Length + " enemies");
		if (enemies.Length == 0) {
			action = SpawnersAction.next;
		}
	}

	void SpawnWave () {
		for (int i = 0; i < currentWave.numberOfEnemy; i++) {
			GameObject o = Instantiate (enemy, spawnPoints [Random.Range (0, spawnPoints.Length)].transform.position, Quaternion.identity);
			o.transform.parent = this.transform;
		}
		action = SpawnersAction.count;
	}
		
}
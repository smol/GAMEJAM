using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour {

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

	private Spawner[] spawnPoints;
	private Wave currentWave;
	private float nextUpdate = 1f;
	private SpawnersAction action = SpawnersAction.next;

	void Start () {
		spawnPoints = GameObject.FindObjectsOfType<Spawner> ();
	}

	void Update () {
		if(Time.time>=nextUpdate){
			nextUpdate=Mathf.FloorToInt(Time.time)+1;
			HandleCurrentState();
		}
	}

	private void HandleCurrentState() {
		switch (action) {
			case SpawnersAction.spawn:
				SpawnWave ();
				break;
			case SpawnersAction.next:
				WaveCompleted ();
				break;
			case SpawnersAction.count:
				EnemyIsAlive ();
				break;
		}
	}

	private void WaveCompleted () {
		currentWave = new Wave ();
		currentWave.numberOfEnemy = 1;
		action = SpawnersAction.spawn;
	}

	void EnemyIsAlive () {
		Debug.Log ("It remain" + currentWave.spawnedEnemy.Count + "enemies");
		if (currentWave.spawnedEnemy.Count == 0) {
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
using System.Collections.Generic;
using Enemies;
using Players;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public LayerMask raycast;

	public GameObject boi;
	public GameObject bigBoi;
	public GameObject flyingBoi;
	public GameObject suicideBoi;
	public GameObject sniperBoi;
	public GameObject barrel;

	private int maxNormalBois  = 4;
	private int maxFlyingBois  = 3;
	private int maxBigBois 	   = 2;
	private int maxSuicideBois = 10;
	private int maxSniperBois  = 2;

	public int normalBois, flyingBois, bigBois, suicideBois, sniperBois;

	public static List<GameObject> enemies;

	public static EnemyController instance;

	private float totalTimer;
	
	private float nTimer;
	private float newTime = 4.5f;
	private float minTime = 3f;
	
	private float bTimer;
	private float bNewTime = 8.5f;
	private float bMinTime = 5.5f;
	
	private float fTimer;
	private float fNewTime = 10.5f;
	private float fMinTime = 6.5f;
	
	private float suTimer;
	private float suNewTime = 18.5f;
	
	private float snTimer;
	private float snNewTime = 14.5f;

	private float barrelMax = 30f;
	private float barrelMin = 7f;

	public bool spawn;

	private void RestartTimers() {
		totalTimer = 0f;
		newTime = 3f;
		bNewTime = 6f;
		fNewTime = 7f;
		suNewTime = 8f;
		snNewTime = 10f;

		nTimer = newTime;
		bTimer = 0.1f;
		fTimer = 0.1f;
		suTimer = 0.1f;
		snTimer = 0.1f;
	}
	
	private void Start() {
		enemies = new List<GameObject>();
		instance = this;
	}

	public void StartEnemies() {
		ClearEnemies();
		RestartTimers();
		enemies = new List<GameObject>();
		spawn = true;
		Invoke("SpawnBarrel", Random.Range(10, 20));
	}
	
	void Update () {
		if (!spawn) return;
		totalTimer += Time.deltaTime;
		
		//normal bois
		if(boi != null) {
			nTimer -= Time.deltaTime;
			if (nTimer <= 0)
				SpawnBoi();
		}

		//big bois
		if(bigBoi != null) {
			if (totalTimer < 20) return;
			bTimer -= Time.deltaTime;
			if (bTimer <= 0)
				SpawnBigBoi();
		}

		//Flying bois
		if(flyingBoi != null) {
			if (totalTimer < 40) return;
			fTimer -= Time.deltaTime;
			if (fTimer <= 0) 
				SpawnFlyingBoi();
		}
		
		//Suicide bois
		if(suicideBoi != null) {
			if (totalTimer < 60) return;
			suTimer -= Time.deltaTime;
			if (suTimer <= 0)
				SpawnSuicideBoi();
		}
		
		//Sniper bois
		if(sniperBoi != null) {
			if (totalTimer < 80) return;
			snTimer -= Time.deltaTime;
			if (snTimer <= 0)
				SpawnSniperBoi();
		}
	}

	private void SpawnBoi() {
		if (newTime > minTime) {
			newTime -= 0.05f;
		}
		
		nTimer = newTime;
		if (normalBois >= maxNormalBois) return;
		
		normalBois++;
		SpawnOneBoi(boi);
	}

	private void SpawnBigBoi() {
		if (bNewTime > bMinTime) {
			bNewTime -= 0.05f;
		}
		
		bTimer = bNewTime;
		if (bigBois >= maxBigBois) return;
		
		bigBois++;
		SpawnOneBoi(bigBoi);
	}

	private void SpawnOneBoi(GameObject boiToSpawn) {
		//float y = MapGenerator.floorPos.y + 6;
		float y = -6.5f;
		if(Random.Range(0f, 1f) < 0.4f && MapGenerator.instance.hasMiddle) y = 5f;

		// Below is used if Ground2 is enabled
		/*float x = 0f;
		if(MapGenerator.instance.selectedGround == 0) x = Random.Range(-16f, 16f);
		else if(MapGenerator.instance.selectedGround == 1) {
			if(Random.Range(0f, 1f) > 0.5f) x = Random.Range(-16f, 6f);
			else x = Random.Range(6f, 16f);
		}*/

		float x = Random.Range(-16f, 16f);
		if(y == 5f) x = Random.Range(-3.5f, 3.5f);

		Vector2 spawnPos = new Vector2(x,y);
		//if birdboi
		if (boiToSpawn.GetComponent(typeof(Actor)) is FlyingEnemy) {
			y = Random.Range(-7.5f, 9f);
			spawnPos = new Vector2(x,y);
		}
		else {
			RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, 50, raycast);
			if (hit.collider != null) {
				if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground")) {
					SpawnOneBoi(boiToSpawn);
					return;
				}
			}
		}
		
		Instantiate(boiToSpawn, spawnPos, transform.rotation);
	}
	
	private void SpawnFlyingBoi() {
		if (fNewTime > fMinTime) {
			fNewTime -= 0.05f;
		}
		
		fTimer = fNewTime;
		if (flyingBois >= maxFlyingBois) return;
		
		flyingBois++;
		SpawnOneBoi(flyingBoi);
	}
	
	private void SpawnSuicideBoi() {
		if (suNewTime > fMinTime) {
			suNewTime -= 0.05f;
		}
		
		suTimer = suNewTime;
		if (suicideBois >= maxSuicideBois) return;
		
		suicideBois++;
		SpawnOneBoi(suicideBoi);
	}
	
	private void SpawnSniperBoi() {
		if (snNewTime > fMinTime) {
			snNewTime -= 0.05f;
		}
		
		snTimer = snNewTime;
		if (sniperBois >= maxSniperBois) return;
		
		sniperBois++;
		SpawnOneBoi(sniperBoi);
	}

	public void ClearEnemies() {
		for (int i = 0; i < enemies.Count; i++) {
			print("destroying: " + enemies[i]);
			Destroy(enemies[i]);
						
		}

		normalBois = 0;
		bigBois = 0;
		flyingBois = 0;
		sniperBois = 0;
		suicideBois = 0;
		CancelInvoke();
	}

	public void KillAll() {
		GameObject[] gos = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //will return an array of all GameObjects in the scene
		foreach(GameObject go in gos)
		{
			if(go.layer==LayerMask.NameToLayer("Enemy") && go.CompareTag("Actor")) {
				if(go.name == "Player") return;
				Actor enemyScript = go.GetComponent(typeof(Actor)) as Actor;
				enemyScript.Damage(1000);
			}
		} 
	}

	private void SpawnBarrel() {
		if (barrelMax > barrelMin + 1)
			barrelMax -= 1f;
		
		float x = Random.Range(-16f, 16f);
		float y = 15;

		Vector3 euler = new Vector3(0, 0, Random.Range(0, 360));

		if(barrel != null) /*GameObject newBarrel = Instantiate(barrel, new Vector2(x, y), Quaternion.Euler(euler));*/ Instantiate(barrel, new Vector2(x, y), Quaternion.Euler(euler));
		
		Invoke("SpawnBarrel", Random.Range(barrelMin, barrelMax));
	}
}

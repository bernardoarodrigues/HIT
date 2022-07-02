using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Audio;
using DefaultNamespace;
using Players;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

	public GameObject pistol;
	public GameObject currentWeapon;
	public GameObject normalBullet;
	public Button pistolBtn, rifleBtn, shotgunBtn, sniperBtn, jumpBtn, dashBtn, damageBtn, reloadBtn, healthBtn, protectBtn, fullammoBtn, nukeBtn;
	public TextMeshProUGUI displayCoin;
    public Slider hpSlider;
	public Image nukeFx;
		
	// Player prefab
	public GameObject player;
	public GameObject playerUI;
	public GameObject menuUI;
	public GameObject gameOverUI;
	public GameObject nukeUI;

	public static GameObject currentPlayer;
	public static Player currentPlayerScript;

	public static int health = 1000;
	protected int maxHealth = 1000;

    public static int playerMoney;

	public static bool inGame;

	public PostProcessProfile ppProfile;

	private static Game instance;
	
	private int newDashes;
	private int newJumps;
    private float damageIncrease;
    private float reloadDecrease;

	public static string special;

	private GameObject bullet;

	private bool boughtWeapon, boughtUpg, boughtSpecial;

	void Start () {
		instance = this;
		currentWeapon = pistol;
		EnterLobby();
	}

	private void Update() {
		if (inGame && currentPlayer == null && !gameOverUI.activeInHierarchy) {
			Invoke("GameOver", 1f);
			inGame = false;
		}
        else if(inGame && currentPlayer != null) {
            hpSlider.value = currentPlayerScript.Health;
        }

		if(nukeUI.activeSelf && nukeFx.color.a >= 0f) {
			var tempColor = nukeFx.color;
			tempColor.a -= Time.deltaTime / 4;
			nukeFx.color = tempColor;
			if(nukeFx.color.a <= 0f) {
				nukeUI.SetActive(false);
				currentPlayerScript.SetOnSpecial(false);
			}
		} 
	}

	public void EnterLobby() {
		ppProfile.GetSetting<DepthOfField>().enabled.value = true;		

		playerUI.SetActive(false);
		gameOverUI.SetActive(false);
		menuUI.SetActive(true);

		health = 1000;
		PlayerPrefs.SetInt("money", 10000);
        playerMoney = PlayerPrefs.GetInt("money");
        displayCoin.text = playerMoney.ToString();
		special = "";
		inGame = false;

		StartCoroutine(WaitScripts());

		ScoreController.instance.score = 0;

		ResetUpgrades();

		if(MapGenerator.instance != null) MapGenerator.instance.CleanMap();
		EnemyController.instance.ClearEnemies();
		Camera.main.orthographicSize = 2.5f;
		//AudioManager.Play("Lobby");
	}

	public void StartGame() {
		if(!boughtWeapon) return;

		inGame = true;

		ppProfile.GetSetting<DepthOfField>().enabled.value = false;	
		playerUI.SetActive(true);
		menuUI.SetActive(false);
		gameOverUI.SetActive(false);

        hpSlider.maxValue = maxHealth;
		
		GameObject newPlayer = Instantiate(player, transform.position, transform.rotation);
		newPlayer.name = "Player";
		currentPlayer = newPlayer;
		currentPlayerScript = currentPlayer.GetComponent(typeof(Player)) as Player;

		//Perform the upgrades
		UpgradePlayer();
		
		AudioManager.Play("Song");
		//AudioManager.Play("Start");
		
		MapGenerator.instance.GenerateMap();
		EnemyController.instance.StartEnemies();
	}
	
	private void UpgradePlayer() {
        // Gives gun to player
        Destroy(currentPlayerScript.gun);
		currentPlayerScript.GiveWeapon(currentWeapon);

		// Jumps and Dashes
		currentPlayerScript.maxJumps = newJumps;
        currentPlayerScript.maxDashes = newDashes;

        // Damage
        currentPlayerScript.weaponScript.damage += currentPlayerScript.weaponScript.damage * damageIncrease;

        // Reload
        currentPlayerScript.weaponScript.reloadTime -= currentPlayerScript.weaponScript.reloadTime * reloadDecrease;
		
		// Bullet
		currentPlayerScript.weaponScript.bullet = bullet;

		// Kills
		currentPlayerScript.specialPoints = 0;

		// Special
		SpecialController.instance.StartSpecial();
	}
	
	// Resets Player upgrades
	private void ResetUpgrades() {
		currentWeapon = pistol;
		newJumps = 2;
        newDashes = 2;
        damageIncrease = 0f;
        reloadDecrease = 0f;
		health = 1000;

        UpdateWeapon();
		UpdateUpg();
		UpdateSpecial();
		
		//other
		bullet = normalBullet;

		displayCoin.text = "Moedas: " + PlayerPrefs.GetInt("money").ToString();
	}

	public void BuyWeapon(GameObject w) {
        int takeMoney = 0;
        if(w.name == "Pistol") takeMoney = 0;
        else if(w.name == "Shotgun") takeMoney = 500;
		else if(w.name == "Rifle") takeMoney = 1000;
        else if(w.name == "Sniper") takeMoney = 2000;

        currentWeapon = w;
		boughtWeapon = true;
		TakeMoney(takeMoney);
		DisableWeaponButtons();
	}

	private void DisableWeaponButtons() {
		pistolBtn.interactable = false;
		rifleBtn.interactable = false;
		shotgunBtn.interactable = false;
		sniperBtn.interactable = false;
	}

	public void UpdateWeapon() {
		pistolBtn.interactable = (playerMoney >= 0);
		shotgunBtn.interactable = (playerMoney >= 500);
		rifleBtn.interactable = (playerMoney >= 1000);
		sniperBtn.interactable = (playerMoney >= 2000);
	}

	public void BuyUpgrade(String u) {
		if(!boughtWeapon) return;
        
		int takeMoney = 0;
        if(u == "Jump") {
			takeMoney = 300;
			newJumps = 3;
		} 
        else if(u == "Dash") {
			takeMoney = 400;
			newDashes = 3;
		}
        else if(u == "Damage") {
			takeMoney = 1000;
			damageIncrease = 0.4f;
		}
        else if(u == "Reload") {
			takeMoney = 1500;
			reloadDecrease = 0.4f;
		} 
		boughtUpg = true;

		TakeMoney(takeMoney);
		DisableUpgradeButtons();
	}

	private void DisableUpgradeButtons() {
		jumpBtn.interactable = false;
		dashBtn.interactable = false;
		damageBtn.interactable = false;
		reloadBtn.interactable = false;
	}

    public void UpdateUpg() {
        jumpBtn.interactable = (playerMoney >= 300);
		dashBtn.interactable = (playerMoney >= 400);
		damageBtn.interactable = (playerMoney >= 1000);
		reloadBtn.interactable = (playerMoney >= 1500);
	}

	public void BuySpecial(string s) {
		if(!boughtWeapon) return;

        int takeMoney = 0;
        
        if(s == "Protect") takeMoney = 500;
		else if(s == "Health") takeMoney = 700;
        else if(s == "FullAmmo") takeMoney = 900;
        else if(s == "Nuke") takeMoney = 1100;

        special = s;
		boughtSpecial = true;
		TakeMoney(takeMoney);
		DisableSpecialButtons();
	}

	private void DisableSpecialButtons() {
		healthBtn.interactable = false;
		protectBtn.interactable = false;
		fullammoBtn.interactable = false;
		nukeBtn.interactable = false;
	}

	public void UpdateSpecial() {
		protectBtn.interactable = (playerMoney >= 500);
		healthBtn.interactable = (playerMoney >= 700);
		fullammoBtn.interactable = (playerMoney >= 900);
		nukeBtn.interactable = (playerMoney >= 1100);
	}

	public void TakeMoney(int value) {
        PlayerPrefs.SetInt("money", playerMoney - value);
		AudioManager.Play("Buy");
		displayCoin.text = "Moedas: " + (playerMoney - value);
        playerMoney = PlayerPrefs.GetInt("money");
		if(boughtWeapon && boughtUpg && boughtSpecial) return;
		if(boughtWeapon && !boughtUpg) {
        	UpdateUpg();
			UpdateSpecial();
		}
		else if(boughtWeapon && boughtUpg) {
			UpdateSpecial();
		}
	}

	public void GameOver() {
		ppProfile.GetSetting<DepthOfField>().enabled.value = true;
		SpecialController.instance.Reset();		
		playerUI.SetActive(false);
		GameOverUI.instance.UpdateScore();
		gameOverUI.SetActive(true);

		EnemyController.instance.spawn = false;
		
		AudioManager.Stop("Song");
	}

	public static void StartNukeFx() {
		instance.NukeFx();
	}

	public void NukeFx() {
		nukeUI.SetActive(true);
		var alphaFx = 1f;
		var tempColor = nukeFx.color;
		tempColor.a = alphaFx;
		nukeFx.color = tempColor;
		AudioManager.Play("Nuke");
	}

	public void RestartGame() {
		SceneManager.LoadScene("Game");
	}

	public void ReturnMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	IEnumerator WaitScripts()
	{
		yield return new WaitUntil(() => ScoreController.instance != null && EnemyController.instance != null && SpecialController.instance != null && MapGenerator.instance != null);
	}
}

using UnityEngine;
using Audio;

public class Gun : MonoBehaviour
{
    // Bullets
    public GameObject bullet;
	public GameObject enemyBullet;
	private Transform attackPoint;
	public Color shooterColor;

    // Gun Variables 
    public float bulletSpeed;
	public float fireRate;
	public float recoil;
	public float damage;
	public int amount = 1;
	public float spread;

	public int totalAmmo;
	private int currentAmmo;
	public float reloadTime;
	
    // Variables
	public bool ready = true;

	// Special
	public bool fullAmmo;

	void Start()
	{
		attackPoint = transform.Find("AttackPoint");
		shooterColor = gameObject.transform.parent.GetComponent<SpriteRenderer>().color;
		currentAmmo = totalAmmo;
		fullAmmo = false;
	}

	// Gun fire function
    public void Fire() {
		if(currentAmmo <= 0) {
			if(!fullAmmo) {
				ready = false;
				Invoke("Reload", reloadTime);
			} else {
				ready = true;
				currentAmmo = totalAmmo; 
			}
		}

		// If not ready to shoot, returns
		if (!ready) return;

		// If ready, spawn (amout) of bullets
		for (int i = 0; i < amount; i++) {
			SpawnBullet();
		}
		
		// Sets ready to false and wait to reset
		ready = false;
		if(!fullAmmo) {
			Invoke("GetReady", fireRate);
		} else {
			Invoke("GetReady", 0.05f);
		}
	}

	// Resets ready bool
    private void GetReady() {
		ready = true;
	}

	// Resets ready bool and reloads 
    private void Reload() {
		ready = true;
		currentAmmo = totalAmmo; 
	}

	// Spawns Bullet
    private void SpawnBullet() {
		GameObject newBullet;
		Vector3 spreadVector = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread));
			
		if (transform.parent.name == "Player") 
			newBullet = Instantiate(bullet, attackPoint.position, attackPoint.rotation);
		else 
			newBullet = Instantiate(enemyBullet, attackPoint.position, attackPoint.rotation);

		//Change size depending on damage
		if(damage >= 100) newBullet.transform.localScale *= (1 + ((damage*0.25f) / 15));
		else newBullet.transform.localScale *= (1 + (damage / 15));

		// Changes bullet color
		newBullet.GetComponent<SpriteRenderer>().color = shooterColor;

		// Changes trail color and width
		newBullet.GetComponent<TrailRenderer>().startColor = shooterColor;
		if(damage >= 100) newBullet.GetComponent<TrailRenderer>().widthMultiplier = (1 + ((damage*0.25f) / 15));
		else newBullet.GetComponent<TrailRenderer>().widthMultiplier = (1 + (damage / 15));
		
		if (transform.parent.name == "Player") {
			newBullet.layer = LayerMask.NameToLayer("Enemy");

			// Sets damage for the bullet
			((PlayerBullet)(newBullet.GetComponent(typeof(PlayerBullet)))).SetDamage(damage);	

			// Play sound
			AudioManager.Play("Shot");	
		}
		else {
			newBullet.layer = LayerMask.NameToLayer("Player");

			// Sets damage for the bullet
			((EnemyBullet)(newBullet.GetComponent(typeof(EnemyBullet)))).SetDamage(damage);		

			// Play sound
			//AudioManager.Play("Pistol");	
		}
		
		newBullet.GetComponent<Rigidbody2D>().velocity = (transform.right * bulletSpeed) + spreadVector;

		currentAmmo--;
	}

    public void EnemyWeapon() {
		bulletSpeed /= 3f;
	}
}

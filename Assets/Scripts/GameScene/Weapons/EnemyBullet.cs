using Players;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	// Variables
    private float damage = 1f;
	public GameObject destroyFx;
	private bool gone;

	// Called when bullet hits something
    private void OnTriggerEnter2D(Collider2D other) {
		// If Bullet hits:
		
		// Player
		if (other.gameObject.CompareTag("Actor") && other.gameObject.layer != LayerMask.NameToLayer("Enemy")) {
			Actor targetHit = other.gameObject.GetComponent(typeof(Actor)) as Actor;			
			
			//damage
			if (targetHit != null) {
				CameraShake.ShakeOnce(0.2f, 0.1f);
				targetHit.Damage(damage);
				//targetHit.GetRb().AddForce(transform.up * 500f);
			}
			Destroy(gameObject);

			if(destroyFx != null)
				Instantiate(destroyFx, transform.position, transform.rotation);
		}

		// Other bullet
		/*if (other.CompareTag("Bullet")) {
			if (transform.localScale.x <= other.transform.localScale.x) {
				Destroy(gameObject);
				return;
			}
			return;
		}*/

		// Wall
		if (other.gameObject.CompareTag("Wall")) {
			Destroy(gameObject);
		}

		// Barrel
		if (other.gameObject.CompareTag("Barrel")) {
			//((Barrel)other.GetComponent(typeof(Barrel))).Explode();
			Destroy(gameObject);
		}
		
	}

	// Sets the bullet damage
	public void SetDamage(float f) {
		damage = f;
	}
}

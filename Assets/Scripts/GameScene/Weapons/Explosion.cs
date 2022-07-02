using Players;
using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	public float damage;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Actor")) {
			Actor script = other.GetComponent(typeof(Actor)) as Actor;
			script.Damage(damage);
			StartCoroutine(destroy());
		}
	}

	IEnumerator destroy()
	{
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}

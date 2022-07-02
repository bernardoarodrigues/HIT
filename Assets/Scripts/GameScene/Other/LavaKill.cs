using Players;
using UnityEngine;

public class LavaKill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Actor")) {
			Actor script = other.GetComponent(typeof(Actor)) as Actor;
			script.Damage(script.Health);
		}
	}
}

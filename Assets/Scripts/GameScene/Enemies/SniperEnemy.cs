using Players;
using UnityEngine;

namespace Enemies {
	public class SniperEnemy : Enemy {
		private bool dead;
		
		private new void Start() {
			StartWeapon();

			base.Start();
			EnemyStart();
			
			maxMoveSpeed = 9f;
			moveSpeed *= 1.5f;
		}

		protected override void Behaviour(Vector3 playerPos) {
			float distFromPlayer = Vector2.Distance(transform.position, playerPos);

			if (distFromPlayer > 17) {
				if (playerPos.x > transform.position.x)
					Move(1);
				else
					Move(-1);
			}

			if(Random.Range(0f, 1f) < 0.7) {
				RaycastHit2D hit2D = Physics2D.Raycast(gun.transform.Find("AttackPoint").position, gun.transform.Find("AttackPoint").right, Mathf.Infinity);
				if (hit2D.collider != null && hit2D.collider.gameObject.name == "Player")
					weaponScript.Fire();
			}
		}

		protected override void OnDeathAction() {
			if (dead) return;
			dead = true;
			EnemyController.instance.sniperBois--;
		}
	}
}

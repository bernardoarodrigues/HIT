using Players;
using UnityEngine;

namespace Enemies {
	public class FlyingEnemy : Enemy {
		private bool dead;
		
		private new void Start() {
			StartWeapon();

			base.Start();
			EnemyStart();
		}
	
		protected override void Behaviour(Vector3 playerPos) {
			if(Random.Range(0, 1f) < 0.4) {
				//Move hoizontally
				if (playerPos.x > transform.position.x)
					Move(1);
				else Move(-1);
			}
		
			//Move vertically
			if(transform.position.y <= 2f)
				GetRb().AddForce(Vector2.up * Time.deltaTime * 800);

			if (Random.Range(0, 1f) < 0.02f)
				weaponScript.Fire();
		}

		protected override void OnDeathAction() {
			if (dead) return;
			dead = true;
			EnemyController.instance.flyingBois--;
		}

	
	}
}

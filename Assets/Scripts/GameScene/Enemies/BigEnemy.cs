using Players;
using UnityEngine;

namespace Enemies {
	public class BigEnemy : Enemy {
		private bool dead;
		
		private new void Start() {
			StartWeapon();
			
			base.Start();
			EnemyStart();
		}

		protected override void Behaviour(Vector3 playerPos) {
			if(Random.Range(0, 1f) < 0.1) {
				if (playerPos.x > transform.position.x)
					Move(1);
				else Move(-1);
			}

			if (Random.Range(0, 1f) < 0.0025)
				Jump();

			if (Random.Range(0, 1f) < 0.05f)
				weaponScript.Fire();
		}

		protected override void OnDeathAction() {
			if (dead) return;
			dead = true;
			EnemyController.instance.bigBois--;
		}
	}
}

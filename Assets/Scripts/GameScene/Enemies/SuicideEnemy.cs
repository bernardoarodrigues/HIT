//using Audio;
using Players;
using UnityEngine;

namespace Enemies {
	public class SuicideEnemy : Enemy {

		private bool dead;
		
		private bool readyToJump = true;
		public GameObject explosion;
		
		private new void Start() {
			StartWeapon();

			base.Start();
			jumpForce *= 0.65f;
			maxMoveSpeed = 6f;
			moveSpeed *= 0.5f;
			maxJumps = 2;
			Destroy(gun);
			EnemyStart();
		}
		
		protected override void Behaviour(Vector3 playerPos) {
			if (playerPos.x > transform.position.x)
				Move(1);
			else Move(-1);

			if (playerPos.y > (transform.position.y + 1.5f) && readyToJump) {
				Jump();
				readyToJump = false;
				Invoke("JumpCoolDown", 0.3f);
			}
			
			if(Vector2.Distance(transform.position, playerPos) < 1.2f)
				Kill(false);
		}

		protected override void OnDeathAction() {
			if (dead) return;
			dead = true;
			EnemyController.instance.suicideBois--;
			//AudioManager.Play("SuicideExplosion");
			Instantiate(explosion, transform.position, transform.rotation);
			//CameraShake.ShakeOnce(0.5f, 1.2f);
		}

		private void JumpCoolDown() {
			readyToJump = true;
		}
	}
}

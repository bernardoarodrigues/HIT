using UnityEngine;

namespace Players {
    public class Enemy : Actor
    {
        public GameObject[] weapons;
        public LayerMask whatIsRaycastable;

        protected void EnemyStart() {
            EnemyController.enemies.Add(gameObject);
            maxMoveSpeed = 3;
            moveSpeed = 500;
            jumpForce = 500;
            maxJumps = 2;
            gun.layer = LayerMask.NameToLayer("Enemy");
        }

        // Update is called once per frame
        protected void Update()
        {
            if(Game.currentPlayer == null) return;
            Vector3 playerPos = Game.currentPlayer.transform.position;
            
            if(gun != null) {
                // Rotates gun towards player position
                Vector3 lookPos = playerPos;
                lookPos = lookPos - transform.position;
                float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
                gun.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
            
            Behaviour(playerPos);
        }

        // Gives weapon to enemy
        protected virtual void StartWeapon() {
			if (weapons.Length == 1) {
				GiveWeapon(weapons[0]);
			}
            else {
			    GiveWeapon(weapons[Random.Range(0, weapons.Length)]);
            }
		}

        // Enemy's behaviour (walking, attack, jump, etc)
        protected virtual void Behaviour(Vector3 playerPos) {
			
		}
    }
}

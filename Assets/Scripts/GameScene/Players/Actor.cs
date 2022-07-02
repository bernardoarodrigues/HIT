using UnityEngine;
using UnityEngine.UI;
using Audio;

namespace Players {
    public class Actor : MonoBehaviour
    {
        // Components
        protected Rigidbody2D rb;
        public Transform footPos;
		public GameObject scoreFx;
		public GameObject dashFx;
		public GameObject playerExplodeFx;
		public GameObject shield;

        // Movement
        public float moveSpeed = 2700f;
        public float maxMoveSpeed = 14f;
        private bool moving = true;

        // Jump
        public float jumpForce = 500f;
        protected int currentJumps;
		public int maxJumps = 2;

		// Dash
		protected int currentDashes;
		public int maxDashes = 2;

        // Ground
		protected bool grounded;
		public LayerMask whatIsGround;

        // Health
		public float Health { get; set; }
		public float hbarHealth { get; set; }
		public float startHealth = 100f;
		private float framesFlashing = 7f;

        // Gun
        public GameObject gun;
        public Gun weaponScript;

        // Score
        private bool scored;
		public int points;

        // Player Sprite
		protected SpriteRenderer sprite;
		protected Color playerColor;
		protected Color actorColor;

		// Gun Sprite
		private SpriteRenderer gunSprite;

        // Standards
		private Vector2 standardScale;

		// Runs once
        protected void Start()
        {
            // Components
            rb = transform.GetComponent<Rigidbody2D>();
            footPos = transform.Find("FootPos");
            sprite = GetComponent<SpriteRenderer>();
			weaponScript = gun.GetComponent(typeof(Gun)) as Gun;
			gunSprite = gun.GetComponent<SpriteRenderer>();

            // Variables
            currentJumps = maxJumps;
			currentDashes = maxDashes;
            actorColor = sprite.color;
			playerColor = actorColor;
			Health = startHealth;
            standardScale = transform.localScale;
        }

		// Update used for Physics, frame-independent
        private void FixedUpdate() {
			// If player is slow, helps slowing down
			SlowDown();

			// If gun rotation changes side, flips the gun
			if(gun != null) FlipGun();

			// If player's y scale decreases "blink", slowly returns to normal
			if (transform.localScale.y < standardScale.y) 
				transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y + 0.03f);
			if (transform.localScale.y > standardScale.y) transform.localScale = new Vector2(transform.localScale.x, standardScale.y);
		}

		// Executed after Update()
        private void LateUpdate() {
			// Checks if FootPos is colliding with something
			// If so, and it's not a bullet, actor is grounded
			Collider2D col = Physics2D.OverlapCircle(footPos.position, 0.5f, whatIsGround);
			if(col != null && col.tag != "Bullet") grounded = true;
			else grounded = false;
			
			// Resets actor jumps
			if (currentJumps < maxJumps && grounded)
				currentJumps = maxJumps;

			// Resets actor dashes	
			if (currentDashes < maxDashes && grounded)
				currentDashes = maxDashes;	
		}

		// Moves actor in X axis
        protected void Move(float dir) {
			// Sets actor movement
            moving = true;

			// Gets Rigidbody2D x velocity
            float xVel = rb.velocity.x;

			// Moves actor to the right
            if (xVel < maxMoveSpeed && dir > 0) {
				rb.AddForce(moveSpeed * Time.deltaTime * Vector2.right * dir);
			}
			// Moves actor to the left
			else if (xVel > -maxMoveSpeed && dir < 0) {					
				rb.AddForce(moveSpeed * Time.deltaTime * Vector2.right * dir);
			}

            //If actor is turning around, help turn faster
			if (xVel > 0.2f && dir < 0)
				rb.AddForce(moveSpeed * 3.2f * Time.deltaTime * -Vector2.right);
			if (xVel < 0.2f && dir > 0) {
				rb.AddForce(moveSpeed * 3.2f * Time.deltaTime * Vector2.right);
			}	
        }

		// Slows down actor
        private void SlowDown() {
			// Checks if movement input is being received
            if(moving) return;

			// Slows actor if there isn't input
            if (rb.velocity.x > 0.2f)
				rb.AddForce(moveSpeed * Time.deltaTime * -Vector2.right);
			else if (rb.velocity.x < -0.2f)
				rb.AddForce(moveSpeed * Time.deltaTime * Vector2.right);
        }

		// Change actor movement state
        protected void StopMoving() {
            if(moving) {
                moving = false;
            }
        }

		// Makes actor jump
        protected void Jump() {
			// Used <= to solve bug of jumps immediately reset after jumping from ground
            if (currentJumps <= 1) return;

			// If player is jumping
			if(gameObject.name == "Player") {
				// Plays jump sound
				AudioManager.Play("Jump");
				// Decreases player height "blink"
                transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y / 4);
			}

			// Sets Rigidbody2D y velocity to 0
			rb.velocity = new Vector2(rb.velocity.x, 0);
			
			// Adds upward force to actor
			rb.AddForce(Vector2.up * jumpForce);

			// Decreases jumps
			currentJumps--;
        }

		// Flips gun in the Y axis (when actor changes mouse from one side to other)
		private void FlipGun() {
			// Gets gun z rotation
			float zRot = gun.transform.eulerAngles.z;
			
			// If gun was rotated to the left, flips it
			if(zRot >=90f && zRot <= 270f) {
				gunSprite.flipY = true;
			} 
			// Else, doesn't
			else {
				gunSprite.flipY = false;
			}
		}

		// Fires bullet
        protected void Fire() {
			// If gun isn't ready to shoot, returns
			if (!weaponScript.ready) return;

			// Fires bullet
			weaponScript.Fire();

			// Adds recoil to actor
			rb.AddForce(-gun.transform.right * weaponScript.recoil);
		}

		// Make actor receive damage
        public void Damage(float damage) {
			// Doesn't damage player if protect special is enabled
			if (gameObject.name == "Player" && Game.currentPlayerScript.protect == true) return;

			// Decreases actor's health by damage
			Health -= damage;

			// If actor's health is below 0
			if (Health <= 0) {
				// Kills actor
				if(damage == 1000) Kill(true);
				else Kill(false);

				// If an enemy was killed
				if (gameObject.layer == LayerMask.NameToLayer("Enemy")) {
					if (scored) return;
					scored = true;

					// Increases score
					ScoreController.AddScore(points);
					
					// Shows score popup
					GameObject scorePop = Instantiate(scoreFx, transform.position, Quaternion.identity);
					scorePop.GetComponentInChildren<TextMesh>().text = "+" + points;

					scored = false;
				}
			}
			// Changes actor color to white
			DamageFlash();
		}
		
		// Changes actor color to white when it's hit
		private void DamageFlash() {
			sprite.color = new Color(1, 1, 1, 1f);
			Invoke("ResetColor", Time.deltaTime * framesFlashing);
		}

		// Resets color to normal 
		private void ResetColor() {
			sprite.color = actorColor;
		}

		// Gives weapon to actor
		public void GiveWeapon(GameObject weapon) {
			// Instantiate gun
			gun = Instantiate(weapon, gun.transform.position, gun.transform.rotation);
			// Gets the gun script
			weaponScript = gun.GetComponent(typeof(Gun)) as Gun;
			// Sets the gun to be child of the actor
			gun.transform.parent = this.transform;
			gun.transform.localPosition = gun.transform.position;
		}

		// Kills actor
        public void Kill(bool isNuke) {
			// If the player was killed
			if (gameObject.name == "Player") {
				//AudioManager.Play("PlayerDeath");
				CameraShake.ShakeOnce(0.6f, 1.4f);
			}
			//else AudioManager.Play("EnemyDeath");

			Health = -1;
			
			// Runs player death fx
			if(playerExplodeFx != null && isNuke) {
				// Instantiate fx
				GameObject playerExplosion = Instantiate(playerExplodeFx, transform.position, transform.rotation);

				// Gets particle system script
				ParticleSystem ps = playerExplosion.GetComponent<ParticleSystem>();
				var main = ps.main;

				// Changes the particle color to the actor's
				main.startColor = actorColor;

				/*var trail = ps.trails;
				Gradient trailGrad = new Gradient();

				trailGrad.SetKeys(
					new GradientColorKey[] { new GradientColorKey(actorColor, 0.0f), new GradientColorKey(Color.white, 1.0f) },
					new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
				);

				trail.colorOverLifetime = trailGrad;*/
			}

			// Executes OnDeath
			OnDeathAction();

			// Increases special from player's script
			if(!Game.currentPlayerScript.onSpecial) Game.currentPlayerScript.specialPoints += 1;

			// Destroys object
			Destroy(gameObject);
		}

		// Executed when enemy dies
		protected virtual void OnDeathAction() {
			
		}

		// Function to get actor's Rigidbody 2D
        public Rigidbody2D GetRb() {
			return rb;
		}
    }
}

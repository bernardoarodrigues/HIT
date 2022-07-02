using UnityEngine;
using Audio;

namespace Players {
    public class Player : Actor
    {
        // Special
		public int specialPoints;
		public bool onSpecial;
		public bool protect;

        // Runs once
        private new void Start() {
            base.Start();

			// Starts variables
			gun.layer = LayerMask.NameToLayer("Player");
			shield = transform.Find("Shield").gameObject;
            Health = Game.health;
        }

        // Update is called once per frame
        private void Update()
        {
            // Gets movement from AD keys and moves player
            float xMove = Input.GetAxisRaw("Horizontal");
            if(xMove != 0f)
                Move(xMove);
            else
                StopMoving();

            // Adds downward force to the player
            if(Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow)) 
                DownDash();
            
            // Gets Jump from Space key
            if (Input.GetKeyDown(KeyCode.Space) | Input.GetKeyUp(KeyCode.UpArrow))
				Jump();

            // Gets Dash from LeftShift key
            if (Input.GetKeyDown(KeyCode.LeftShift))
				Dash();

            // Rotates gun towards mouse position
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 dirRelativeToPlayer = (mousePos - (Vector2) transform.position).normalized;
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
			Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
			lookPos = lookPos - transform.position;
			float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
			gun.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Gets Mouse click and Fires
            if(Input.GetKey(KeyCode.Mouse0))
                Fire();

			// Gets Special from E key
            if(Input.GetKeyDown(KeyCode.E) && Game.special != "") {
                Special();
            }
        }

        // Makes player dash
		private void Dash() {
			// If dashes == 0, doesn't execute the dash
			if (currentDashes < 1) return;

			// If player is going to the right, dashes to the right
			if(rb.velocity.x > 0f) {
				rb.AddForce(Vector2.right * 400f);
				Instantiate(dashFx, transform.position - new Vector3(-0.5f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, -90f)));
			} 
			// If player is going to the left, dashes to the left
			else if(rb.velocity.x < 0f) {
				rb.AddForce(Vector2.right * -400f);
				Instantiate(dashFx, transform.position - new Vector3(0.5f, 0f, 0f), Quaternion.Euler(new Vector3(0f, 0f, 90f)));
			}

			// Plays sound
			AudioManager.Play("Jump");

			// Decreases dashes
			currentDashes--;
		}

		// Adds downward force to player
		private void DownDash() {
			// If player is not grounded, adds downward force
			if(!grounded)
				rb.AddForce(-Vector2.up * 20f);
		}

        private void Special() {
			SetOnSpecial(true);

			if(Game.special == "Protect" && specialPoints >= 8) {
				specialPoints = 0;
				protect = true;
				shield.SetActive(true);
				Invoke("ResetProtect", 10f);
			}
			else if(Game.special == "Health" && specialPoints >= 10) {
				specialPoints = 0;
				Health = 1000;
				SetOnSpecial(false);
			}
			else if(Game.special == "FullAmmo" && specialPoints >= 8) {
				specialPoints = 0;
				weaponScript.fullAmmo = true;
				sprite.color = new Color(0, 0, 0, 255);
				actorColor = new Color(0, 0, 0, 255);
				weaponScript.shooterColor = actorColor;
				Invoke("ResetFullAmmo", 10f);
			} 
			else if(Game.special == "Nuke" && specialPoints >= 12) {
				specialPoints = 0;
				EnemyController.instance.KillAll();
				Game.StartNukeFx();
			}
		}

        // Ends protect special
		private void ResetProtect() {
			SetOnSpecial(false);
			protect = false;
			shield.SetActive(false);
		}

		// Ends fullAmmo special
		private void ResetFullAmmo() {
			SetOnSpecial(false);
			weaponScript.fullAmmo = false;
			sprite.color = playerColor;
			actorColor = playerColor;
			weaponScript.shooterColor = actorColor;
		}		

		// Sets onSpecial
		public void SetOnSpecial(bool on) {
			onSpecial = on;
		}


    }
}

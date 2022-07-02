using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players {
    public class DummyPlayer : Actor {

        // Runs once
        private new void Start() {
            base.Start();

			// Starts variables
			gun.layer = LayerMask.NameToLayer("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if(Random.Range(0, 1f) < 0.05) {
                if(Random.Range(0, 1f) < 0.25) Move(1);
                else Move(-1);
			}
			
			if (Random.Range(0, 1f) < 0.005)
				Jump();
        }
    }
}

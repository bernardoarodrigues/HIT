using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static Vector2 floorPos;
	public static Vector2 middlePos;
	public static Vector2 rightPos, leftPos;

	public GameObject[] floors;
	public GameObject[] middlePlatforms;
	public GameObject[] sidePlatforms;

	public static MapGenerator instance;

	private List<GameObject> parts;

    public int selectedGround;
    public bool hasMiddle;

	// Use this for initialization
	void Start () {
		parts = new List<GameObject>();
		instance = this;
	}

	public void GenerateMap() {
		//Clean map before generating new
		parts = new List<GameObject>();
		rightPos  = new Vector2(11.4f, 5);
		leftPos  = new Vector2(-11.4f, 5);
		floorPos  = new Vector2(0, -8.75f);
		middlePos = new Vector2(0, 0); 
        hasMiddle = false;
		
        selectedGround = 0; //Random.Range(0, floors.Length);

        if(floors.Length > 0)
		    parts.Add(Instantiate(floors[selectedGround], floorPos, transform.rotation));

        if(middlePlatforms.Length > 0) {
            if(Random.Range(0f, 1f) > 0.2f) {
                parts.Add(Instantiate(middlePlatforms[Random.Range(0, middlePlatforms.Length)], middlePos, transform.rotation));
                hasMiddle = true;
            }
        }

        if(sidePlatforms.Length > 0) {
            if (Random.Range(0, 1f) > 0.5f)
                parts.Add(Instantiate(sidePlatforms[Random.Range(0, sidePlatforms.Length)], rightPos, transform.rotation));
            if (Random.Range(0, 1f) > 0.5f)
                parts.Add(Instantiate(sidePlatforms[Random.Range(0, sidePlatforms.Length)], leftPos, transform.rotation));
        }
	}

	public void CleanMap() {
		for (int i = 0; i < parts.Count; i++)
			Destroy(parts[i]);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactController : MonoBehaviour {

    public Transform startPoint;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset () {
        transform.position = startPoint.position;
    }

    void OnTriggerEnter2D (Collider2D col) {
    Debug.Log("Hi");
        if(col.gameObject.tag == "OutBound") {
        Debug.Log("Hello");
            GameObject.Find("Main Camera").GetComponent<TapController>().GameOver();
        }
    }
}

using UnityEngine;
using System.Collections;

public class zoom : MonoBehaviour {

    //public Camera cam;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Minus))
        {
            gameObject.transform.Translate(-gameObject.transform.forward);
            gameObject.transform.Translate(-gameObject.transform.up);
            Debug.Log(gameObject.transform.position.y + ", " + gameObject.transform.position.z);
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            gameObject.transform.Translate(gameObject.transform.forward);
            gameObject.transform.Translate(gameObject.transform.up);
            Debug.Log(gameObject.transform.position.y + ", " + gameObject.transform.position.z);
        }
        
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		var moveX = Input.GetAxis("Vertical") * 6f;
		var moveY = Input.GetAxis("Horizontal") * 6f;
		var movement = new Vector3( moveY, 0, moveX) * Time.deltaTime;

		transform.Translate(movement);
    }
}

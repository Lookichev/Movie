using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifting : MonoBehaviour
{
	
    void Update()
    {
		transform.Translate(0, EnemyManager.LiftFlameSpeed * Time.deltaTime, 0);
    }

	private void OnTriggerEnter(Collider other)
	{
		var player = other.gameObject.GetComponent<Movement>();

		//Если : лава дошла до игрока
		if (player != null)
		{
			MainManager.PlayerIsDie();
		}
	}
}

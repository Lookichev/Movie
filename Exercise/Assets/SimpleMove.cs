using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    void Start()
    {
		StartCoroutine(DestroyThis());
    }

    void Update()
    {
		transform.Translate(EnemyManager.SphereSpeed * Time.deltaTime, 0, 0);
    }

	/// <summary>
	/// Пакман поймал персика
	/// </summary>
	/// <param name="collision"></param>
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.GetComponent<Movement>() != null)
			MainManager.PlayerIsDie();
	}


	private IEnumerator DestroyThis()
	{
		//Отметка времени существования сферы
		yield return new WaitForSeconds(10);

		Destroy(this.gameObject);
	}
}

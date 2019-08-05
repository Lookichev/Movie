using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Move))]
public class Leader : MonoBehaviour
{
	private Rigidbody _rigidBody;

	private Move _move;

    void Start()
    {
		_rigidBody = gameObject.GetComponent<Rigidbody>();
		_move = gameObject.GetComponent<Move>();
    }

    void Update()
    {
		//Расталкивание компаньонов с пути
		CheckWay();
	}

	/// <summary>
	/// Проверка пути на присутствие компаньонов
	/// </summary>
	private void CheckWay()
	{
		//Бросок луча с разрезом
		if (Physics.SphereCast(new Ray(transform.position, _rigidBody.velocity.normalized), 1f, out RaycastHit hit))
		{
			var companion = hit.transform.gameObject.GetComponent<CompanionFollowing>();
			//Если : цель попадания является компаньоном
			if (companion != null)
				//Если : расстояние до препятствия меньше дальности реагирования
				if (hit.distance <= _move.avoidDistance) companion.GetOut(transform.position, _rigidBody.velocity);
		}
	}
}

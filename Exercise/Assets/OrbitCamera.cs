using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	/// <summary>
	/// Положение камеры относительно игрока
	/// </summary>
	private Vector3 _offset;

    void Start()
    {
		_offset = target.position - transform.position;
    }

	private void LateUpdate()
	{
		//Сохранение начального смещения
		transform.position = target.position - _offset;
		//НАправление камеры на персонажа
		transform.LookAt(target);
	}
}

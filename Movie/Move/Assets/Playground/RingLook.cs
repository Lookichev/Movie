using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingLook : MonoBehaviour
{
	/// <summary>
	/// Цель оси
	/// </summary>
	[SerializeField]
	private Transform target;

	/// <summary>
	/// Скорость вращения
	/// </summary>
	[SerializeField]
	private float speed;

	/// <summary>
	/// Положение камеры
	/// </summary>
	private float _position = 0.0f;

	/// <summary>
	/// Расстояние камеры от цели
	/// </summary>
	private Vector3 _distance;

	private void Start()
	{
		var camera = gameObject.GetComponentInChildren<Camera>();

		camera.transform.LookAt(gameObject.transform);
	}

	void Update()
    {
		//Если : пауза, ничего не крутить
		if (Time.timeScale == 0) return;

		gameObject.transform.position = target.position;

		float delta = 0;

		if ((delta = Input.GetAxis("Mouse X")) != 0)
			delta *= -speed;
		else if ((delta = Input.GetAxis("Mouse ScrollWheel")) != 0)
		{
			delta *= speed * 2;
		}

		transform.Rotate(new Vector3(0,1,0), delta);
	}
}

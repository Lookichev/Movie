using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Слежение камеры за персонажем
/// </summary>
[RequireComponent(typeof(Camera))]
public class LookUnit : MonoBehaviour
{
	/// <summary>
	/// Смещение камеры
	/// </summary>
	private bool _Shift;

	/// <summary>
	/// Камера
	/// </summary>
	private Camera _Camera;

	/// <summary>
	/// Направление смещения камеры
	/// </summary>
	private Vector3 _ShiftNormalized;

	/// <summary>
	/// Длина шага камеры при переходе персонажа на следующую ступень
	/// </summary>
	private float _MagnitudeRange;

	/// <summary>
	/// Пройденное расстояние
	/// </summary>
	private Vector3 _Range;


    void Start()
    {
		_Camera = GetComponent<Camera>();
		_Camera.transform.LookAt(new Vector3(-2, 3, 0));
		_ShiftNormalized = new Vector3(-1, 1, 0);
		_MagnitudeRange = _ShiftNormalized.sqrMagnitude;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.W)) { _Shift = true; _Range = Vector3.zero; }
		if (_Shift)
		{
			//Смещение камеры за кадр
			var size = _ShiftNormalized * Time.deltaTime;
			//Сумма смещения камеры
			_Range += size;
			gameObject.transform.position += size;
		}

		//Если : камера прошла расстояние до следующей ступеньки - остановка
		if (_Range.sqrMagnitude >= _MagnitudeRange) _Shift = false;
	}

}

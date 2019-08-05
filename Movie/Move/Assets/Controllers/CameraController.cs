using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	/// <summary>
	/// Массив камер в ролике
	/// </summary>
	[SerializeField]
	private Camera[] camers;

	/// <summary>
	/// Номер активной камеры
	/// </summary>
	public int NumActive { get; set; }

	/// <summary>
	/// Пол
	/// </summary>
	[SerializeField]
	private GameObject _floor;
	
	/// <summary>
	/// Управление флагами
	/// </summary>
	private AutoFlag _autoTargeting;

	void Start()
	{
		_autoTargeting = _floor.GetComponent<AutoFlag>();

		NumActive = 2;

		//Проходка по массиву камер и их отключение
		foreach (var camera in camers) camera.gameObject.SetActive(false);
		//Включение выбранной камеры
		camers[NumActive].gameObject.SetActive(true);
	}

	void Update()
	{
		ActivatedCamera();
	}

	/// <summary>
	/// Активация установленной камеры
	/// </summary>
	private void ActivatedCamera()
	{
		//Если : выбранная камера активирована - ничего не делать
		if (camers[NumActive].gameObject.activeSelf) return;

		//Проходка по массиву камер и их отключение
		foreach (var camera in camers) camera.gameObject.SetActive(false);
		//Включение выбранной камеры
		camers[NumActive].gameObject.SetActive(true);
	}
}

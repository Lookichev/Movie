using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ControlScene : MonoBehaviour
{
	/// <summary>
	/// Пол
	/// </summary>
	[SerializeField]
	private GameObject floor;

	/// <summary>
	/// Контроллер
	/// </summary>
	[SerializeField]
	private GameObject controller;

	/// <summary>
	/// Камера
	/// </summary>
	private Camera _camera;

	/// <summary>
	/// Контроллер движения
	/// </summary>
	private MovementController _mover;

	
    void Start()
    {
		_mover = controller.GetComponent<MovementController>();
		_camera = gameObject.GetComponent<Camera>();
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
		//Если : нажата левая кнопка мыши - ставим флаг
		if (Input.GetMouseButtonDown(0))
		{
			var point = new Vector3(_camera.pixelWidth / 2, _camera.pixelHeight / 2, 0);
			//Генерация луча в точку прицела
			var ray = _camera.ScreenPointToRay(point);
			//Если : попадание зафиксировано
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				if (hit.transform.gameObject.Equals(floor))
				{
					_mover.CreateFlag(hit.point);
				}
			}
		}
    }

	/// <summary>
	/// Отрисовка прицела
	/// </summary>
	private void OnGUI()
	{
		int size = 12;
		var posX = _camera.pixelWidth / 2 - size / 4;
		var posY = _camera.pixelHeight / 2 - size / 4;
		GUI.Label(new Rect(posX, posY, size, size), "*");
	}
}

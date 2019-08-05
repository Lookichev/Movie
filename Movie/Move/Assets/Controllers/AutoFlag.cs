using Assets.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFlag : MonoBehaviour
{
	/// <summary>
	/// Выбор центра описания окружности
	/// </summary>
	private Source _source;
	
	/// <summary>
	/// Время смены цели
	/// </summary>
	private float _changeTargetInSec = 2;

	/// <summary>
	/// Радиус оружности границы выбора целей
	/// </summary>
	private float _range = 10f;

	/// <summary>
	/// Контроллер для флага
	/// </summary>
	private MovementController _mover;

	/// <summary>
	/// Контроллер камер
	/// </summary>
	private CameraController _cameraController;

	/// <summary>
	/// Работа таймера
	/// </summary>
	private bool _timer;



	/// <summary>
	/// Установка центра окружности автотаргетинга
	/// </summary>
	public int SetSourceAutoFlag { set => _source = value == 0 ? Source.CentreFloor : Source.Object; }

	/// <summary>
	/// Установка радиуса окружности автотаргетинга
	/// </summary>
	public string SetRadius { set => _range = Converter.ToFloat(value); }

	/// <summary>
	/// Установка времени смены цели
	/// </summary>
	public string SetTimer { set => _changeTargetInSec = Converter.ToFloat(value); }

	void Start()
    {
		_mover = gameObject.GetComponent<MovementController>();
		_cameraController = gameObject.GetComponent<CameraController>();
    }

    void Update()
    {
		//Если : камера сама устанавливает флаги - не делаем это здеся
		if (_cameraController.NumActive == 2) return;

		//Если : таймер отработал
		if (!_timer) StartCoroutine(TargetChange());
	}

	/// <summary>
	/// Смена цели объекта
	/// </summary>
	/// <returns>Задержка</returns>
	private IEnumerator TargetChange()
	{
		_timer = true;

		//Задержка
		yield return new WaitForSeconds(_changeTargetInSec);

		_mover.CreateFlag(RandomVector());

		_timer = false;
	}

	/// <summary>
	/// Возвращает случайный вектор
	/// </summary>
	/// <returns>Расположение флага</returns>
	private Vector3 RandomVector()
	{
		var vector = new Vector3(Random.Range(-_range, _range), 0, Random.Range(-_range, _range));

		switch(_source)
		{
			case Source.CentreFloor:
				return vector;
			case Source.Object:
				return vector + _mover.getTarget.transform.position;
		}

		return Vector3.zero;
	}
}

/// <summary>
/// Определение центра окружности выбора цели
/// </summary>
public enum Source
{
	/// <summary>
	/// Центр пола
	/// </summary>
	CentreFloor,

	/// <summary>
	/// Движущийся объект
	/// </summary>
	Object
}

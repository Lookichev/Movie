using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Регулирует перемещение персонажа
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
	/// <summary>
	/// Шаг персонажа в сторону
	/// </summary>
	private const float c_StepZ = 1f;


	/// <summary>
	/// Сила толчка персонажа вверх
	/// </summary>
	[SerializeField]
	private float Force = 54f;

	/// <summary>
	/// Скорость смещения по оси X
	/// </summary>
	[SerializeField]
	private float SpeedX = 1.15f;

	/// <summary>
	/// Скорость смещения по оси Z
	/// </summary>
	[SerializeField]
	private float SpeedZ = 2f;


	/// <summary>
	/// Положение персонажа на ступеньке
	/// </summary>
	/// <remarks>Изменяется от -3 до 3</remarks>
	private int _Position = 0;

	/// <summary>
	/// Очередь движений
	/// </summary>
	private Queue<TypeMovement> _Queue = new Queue<TypeMovement>(2);

	/// <summary>
	/// Физтело персонажа
	/// </summary>
	private Rigidbody _Rigidbody;


	
	/// <summary>
	/// Добавление движения в очередь
	/// </summary>
	/// <param name="move">Движение</param>
	public void AddMovement(TypeMovement move)
	{
		//Если : очередь полна - ничего не предпринимать
		if (_Queue.Count == 2) return;

		_Queue.Enqueue(move);

		//Добавлено первое движение на выполнение
		if (_Queue.Count == 1)
			SwitchMovement(_Queue.Peek());
	}




	void Start()
    {
		_Rigidbody = gameObject.GetComponent<Rigidbody>();
	}

    void Update()
    {
		//Не уверен, что вовремя паузы не начнет забиваться очередь
		if (Time.timeScale == 0) return;

		//Если : игра симулируется на ПК
		if (!MainManager.isMobileVersion)
		{
			//Если : смещение вправо
			if (Input.GetKeyDown(KeyCode.D)) AddMovement(TypeMovement.Right);
			//Если : смещение влево
			if (Input.GetKeyDown(KeyCode.A)) AddMovement(TypeMovement.Left);
			//Если : смещение вверх
			if (Input.GetKeyDown(KeyCode.W)) AddMovement(TypeMovement.Up);
		}

		//Если : персонаж неподвижен - валим отседа
		if (_Queue.Count == 0) return;

		switch (_Queue.Peek())
		{
			case TypeMovement.Left:
				SideStep(false);
				break;
			case TypeMovement.Right:
				SideStep(true);
				break;
			case TypeMovement.Up:
				FrontStep();
				break;
		}
	}

	/// <summary>
	/// Смещение персонажа в сторону
	/// </summary>
	private void SideStep(bool isRight)
	{
		if (isRight)
			//Если : смещение к точке не закончено
			if (gameObject.transform.position.z < _Position)
				gameObject.transform.Translate(0, 0, SpeedZ * Time.deltaTime);
			//Удаление "отработанного" движения
			else RemoveMovement();
		else
		//Если : смещение к точке не закончено
		if (gameObject.transform.position.z > _Position)
			gameObject.transform.Translate(0, 0, -SpeedZ * Time.deltaTime);
		//Удаление "отработанного" движения
		else RemoveMovement();
	}

	/// <summary>
	/// Смещение персонажа вперед
	/// </summary>
	private void FrontStep()
	{
		if (gameObject.transform.position.x > -MainManager.Interface.NumStep)
			gameObject.transform.Translate(-1 * SpeedX * Time.deltaTime, 0, 0);
		else RemoveMovement();
	}

	/// <summary>
	/// Удаление выполненого действия
	/// </summary>
	private void RemoveMovement()
	{
		_Queue.Dequeue();

		//Если : в очереди есть другое действие
		if (_Queue.Count == 1)
			SwitchMovement(_Queue.Peek());
	}

	/// <summary>
	/// Изменение координаты прибытия
	/// </summary>
	/// <param name="move">Движение</param>
	private void SwitchMovement(TypeMovement move)
	{
		switch (move)
		{
			case TypeMovement.Left:
				//Если : персонаж у левого края - не выполнять это движение
				if (_Position - 1 == -4) _Queue.Dequeue();
				else _Position -= 1;
				break;
			case TypeMovement.Right:
				//Если : персонаж у правого края - не выполнять это движение
				if (_Position + 1 == 4) _Queue.Dequeue();
				else _Position += 1;
				break;
			case TypeMovement.Up:
				//Если : движение вверх, то задать импульс
				_Rigidbody.velocity = Vector3.zero;
				if (move == TypeMovement.Up) _Rigidbody.AddForce(new Vector3(0, Force, 0), ForceMode.Impulse);
				MainManager.Interface.NextStep();
				break;
		}
	}
}

/// <summary>
/// Виды передвижений
/// </summary>
public enum TypeMovement
{
	/// <summary>
	/// Смещение вдоль ступеньки влево
	/// </summary>
	Left,

	/// <summary>
	/// Смещение вдоль ступеньки вправо
	/// </summary>
	Right,

	/// <summary>
	/// Смещение на верхнюю ступень
	/// </summary>
	Up
}

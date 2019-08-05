using Assets.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
	[Space]

	/// <summary>
	/// Шарик
	/// </summary>
	[SerializeField]
	private GameObject target;

	[Header("Prefabs")]

	/// <summary>
	/// Флаг цели для объекта
	/// </summary>
	[SerializeField]
	private GameObject flagPrefab;

	/// <summary>
	/// Шаблон преследователя
	/// </summary>
	[SerializeField]
	private GameObject hunterPrefab;

	/// <summary>
	/// Шаблон препятствий
	/// </summary>
	[SerializeField]
	private GameObject obstaclesPrefab;

	/// <summary>
	/// Шаблон компаньонов
	/// </summary>
	[SerializeField]
	private GameObject companionPrefab;

	#region Закрытые внутренние поля

	/// <summary>
	/// Путь следования
	/// </summary>
	private LinkedList<GameObject> _way = new LinkedList<GameObject>();

	/// <summary>
	/// Преследователь
	/// </summary>
	private GameObject _hunter;

	/// <summary>
	/// Препятствия
	/// </summary>
	private GameObject _obstacles;

	/// <summary>
	/// Компаньоны
	/// </summary>
	private GameObject _companions;

	/// <summary>
	/// Компонент перемещения объекта
	/// </summary>
	private Move _objectMove;

	/// <summary>
	/// Компонент перемещения охотника
	/// </summary>
	private Move _hunterMove;

	/// <summary>
	/// Компонент перемещения компаньонов
	/// </summary>
	private CompanionFollowing _companionFollowing;

	/// <summary>
	/// Действующая цель патрулирования
	/// </summary>
	private LinkedListNode<GameObject> currentPatrolTarget;

	#endregion



	/// <summary>
	/// Возвращает объект движения
	/// </summary>
	public GameObject getTarget => target;

	#region Установка параметров через GUI

	/// <summary>
	/// Устанавливает значение дистанции прибытия к точке пути
	/// </summary>
	public string SetDistancePart { set => distancePart = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает тип движения для охотника
	/// </summary>
	public int SetHunterMovementType { set => _hunterMove.SetMovementType = value == 0 ? 5 : 6; }

	/// <summary>
	/// Устанавливает максимальное значение скорости охотника
	/// </summary>
	public string SetHunterMaxSpeed { set => _hunterMove.SetMaxSpeed = value; }

	/// <summary>
	/// Устанавливает максимальное значение силы движения охотника
	/// </summary>
	public string SetHunterMaxForce{ set => _hunterMove.SetMaxForce = value; }

	/// <summary>
	/// Устанавливает максимальное значение расстояние реагирования охотника
	/// </summary>
	public string SetHunterDistance { set => _hunterMove.SetDistance = value; }

	/// <summary>
	/// Устанавливает дальность предсказания объекта
	/// </summary>
	public string SetHunterProphetic { set => _hunterMove.SetProphetic = value; }

	/// <summary>
	/// Устанавливает массу охотника
	/// </summary>
	public string SetHunterMass { set => _hunterMove.SetMass = value; }

	/// <summary>
	/// Устанавливает максимальное значение скорости компаньонов
	/// </summary>
	public string SetCompanionMaxSpeed { set => _companionFollowing.SetMaxSpeed = value; }

	/// <summary>
	/// Устанавливает максимальное значение силы движения компаньонов
	/// </summary>
	public string SetCompanionMaxForce { set => _companionFollowing.SetMaxForce = value; }

	/// <summary>
	/// Устанавливает максимальное значение расстояние реагирования компаньонов
	/// </summary>
	public string SetCompanionDistance { set => _companionFollowing.SetDistance = value; }

	/// <summary>
	/// Устанавливает массу компаньонов
	/// </summary>
	public string SetCompanionMass { set => _companionFollowing.SetMass = value; }

	/// <summary>
	/// Устанавливает дальность следования компаньонов
	/// </summary>
	public string SetCompanionBehinding { set => _companionFollowing.SetBehinding = value; }

	/// <summary>
	/// Устанавливает рудиус расталкивания компаньонов
	/// </summary>
	public string SetCompanionMaxRadius { set => _companionFollowing.SetMaxRadius = value; }

	/// <summary>
	/// Устанавливает максимальную дальность расталкивания компаньонов
	/// </summary>
	public string SetCompanionMaxSeparation { set => _companionFollowing.SetMaxSeparation = value; }

	/// <summary>
	/// Устанавливает максимальную дальность видения компаньонов
	/// </summary>
	public string SetCompanionMaxVision { set => _companionFollowing.SetMaxVision = value; }

	#endregion

	/// <summary>
	/// Использователь следование по пути
	/// </summary>
	public bool useWay { get; set; }

	/// <summary>
	/// Использовать патрулирование по указанному пути
	/// </summary>
	public bool usePatrol { get; set; }

	/// <summary>
	/// Дистанция прибытия к точке пути
	/// </summary>
	public float distancePart = 2f;

	/// <summary>
	/// Нужен-ли охотник
	/// </summary>
	public bool createHunter { get; set; }

	/// <summary>
	/// Нужны-ли препятствия
	/// </summary>
	public bool createObstacles { get; set; }

	/// <summary>
	/// Нужны-ли компаньоны, следующие за объектом
	/// </summary>
	public bool createCompanions { get; set; }



	/// <summary>
	/// Создает новую целевую точку
	/// </summary>
	/// <param name="point">Точка сбора</param>
	public void CreateFlag(Vector3 point)
	{
		//Не создавать флаги в режиме паузы
		if (Time.timeScale == 0) return;

		//Генерация точки
		var flag = Instantiate(flagPrefab);
		flag.transform.position = point;

		//Если : генерируется путь
		if (useWay)
			_way.AddLast(flag);
		//Цель всегда одна
		else
		{
			//Удаляем флаги, поставленные ранее
			foreach (var fl in _way) Destroy(fl);
			_way.Clear();

			_way.AddFirst(flag);
		}

		//Оповещение объекта о смене флага
		_objectMove.target = _way.First.Value.transform.position;
	}



	void Start()
    {
		_objectMove = target.GetComponent<Move>();
    }

	void Update()
	{
		#region Создание дополнительных элементов

		//Если : охотник требуется
		if (createHunter && _hunter == null) CreateHunter();
		//Если : охотник не требуется
		if (!createHunter && _hunter != null)
		{
			Destroy(_hunter);
			_hunter = null;
		}

		//Если : требуется препятствия
		if (createObstacles && _obstacles == null) _obstacles = Instantiate(obstaclesPrefab);
		//Если : препятствия не требуются
		if (!createObstacles && _obstacles != null)
		{
			Destroy(_obstacles);
			_obstacles = null;
		}

		//Если : требуются компаньоны
		if (createCompanions && _companions == null) CreateCompanion();
		//Если : не требуются компаньоны
		if (!createCompanions && _companions != null)
		{
			Destroy(_companions);
			_companions = null;
		}

		#endregion

		//Патрулирование отключено
		if (!usePatrol)
		{
			//Используется путь
			if (_way.Count > 1)
			{
				var dot = _way.First.Value;

				//Если : расстояние до точки меньше расстояния прибытия, а точек в пути больше одной
				//Надо удалить крайнюю точку и перенаправить объект к следующей
				if (Vector3.Distance(target.transform.position, dot.transform.position) <= distancePart)
				{
					Destroy(dot);
					_way.RemoveFirst();
					_objectMove.target = _way.First.Value.transform.position;
				}
			}
		}
		//Патрулирование включено
		else
		{
			//Если : патрулирование только включено - цель первая точка
			if (currentPatrolTarget == null || currentPatrolTarget.Value == null) currentPatrolTarget = _way.First;

			//Если : объект достиг цели патрулирования
			if (Vector3.Distance(target.transform.position, currentPatrolTarget.Value.transform.position) <= distancePart)
			{
				//Если : следующая точка не последняя в пути, то все ок, иначе - переходим на начало
				currentPatrolTarget = currentPatrolTarget != _way.Last ? currentPatrolTarget.Next : _way.First;
			}

			//Если цель сменилась - устанавливаем новую
			if (_objectMove.target != currentPatrolTarget.Value.transform.position) _objectMove.target = currentPatrolTarget.Value.transform.position;
		}
	}

	/// <summary>
	/// Создает охотника за сферой
	/// </summary>
	private void CreateHunter()
	{
		_hunter = Instantiate(hunterPrefab);
		//Располагаем относительно цели чуть в стороне
		var position = target.transform.position;
		position.x += 3;
		_hunter.transform.position = position;
		//Устанавливаем целью сферу
		_hunterMove = _hunter.GetComponent<Move>();
		_hunterMove.Prey = target;
		_hunterMove.SetMovementType = 5;
	}

	/// <summary>
	/// Создает команду сферы
	/// </summary>
	private void CreateCompanion()
	{
		_companions = Instantiate(companionPrefab);
		_companions.GetComponent<Team>().TakeLeader(target);

		_companionFollowing = _companions.GetComponent<CompanionFollowing>();
	}
}

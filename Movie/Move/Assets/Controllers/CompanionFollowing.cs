using Assets.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CompanionFollowing : MonoBehaviour
{
	/// <summary>
	/// Команда сопровождения
	/// </summary>
	private List<GameObject> _team;

	/// <summary>
	/// Физ тело лидера
	/// </summary>
	private Rigidbody _leaderBody;

	/// <summary>
	/// Физ тело компаньона
	/// </summary>
	private Rigidbody _rigidBody;



	#region Свойства установки параметров GUI

	/// <summary>
	/// Устанавливает максимальное значение скорости компаньонов
	/// </summary>
	public string SetMaxSpeed { set => _maxSpeed = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальное значение силы движения компаньонов
	/// </summary>
	public string SetMaxForce { set => _maxForce = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальное значение расстояние реагирования компаньонов
	/// </summary>
	public string SetDistance { set => _distance = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает массу компаньонов
	/// </summary>
	public string SetMass { set => _rigidBody.mass = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает дальность следования компаньонов
	/// </summary>
	public string SetBehinding { set => _behinding = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает рудиус расталкивания компаньонов
	/// </summary>
	public string SetMaxRadius { set => _maxRadius = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальную дальность расталкивания компаньонов
	/// </summary>
	public string SetMaxSeparation { set => _maxSeparation = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальную дальность видения компаньонов
	/// </summary>
	public string SetMaxVision { set => _maxVision = Converter.ToFloat(value); }

	#endregion

	/// <summary>
	/// Лидер
	/// </summary>
	public GameObject leader { get; set; }

	/// <summary>
	/// Максимальная скорость объекта
	/// </summary>
	private float _maxSpeed = 8.5f;

	/// <summary>
	/// Максимальная ширина качания
	/// </summary>
	private float _maxForce = 8.5f;

	/// <summary>
	/// Расстояние изменения характера движения
	/// </summary>
	private float _distance = 5f;

	/// <summary>
	/// Отступ за спину лидера
	/// </summary>
	private float _behinding = 5f;

	/// <summary>
	/// Расстояние между компаньонами
	/// </summary>
	private float _maxRadius = 3f;

	/// <summary>
	/// Максимальное расстояние между компаньонами
	/// </summary>
	private float _maxSeparation = 3f;

	/// <summary>
	/// Расстояние видения компаньонов
	/// </summary>
	private float _maxVision = 5f;


	void Start()
	{
		_team = GetComponentInParent<Team>().team;
		_leaderBody = leader.GetComponent<Rigidbody>();
		_rigidBody = gameObject.GetComponent<Rigidbody>();
	}

	void Update()
	{
		var behind = -1 * _leaderBody.velocity.normalized * _behinding + leader.transform.position;
		behind.y = 0;

		var vector = Arrival(behind) + Separation();

		//Данный метод прелдлагает автор статьи, однако выглядит это очень жутко - шарики подлетают, ударяют лидера и отлетают
		//Для ухода с траектории использую свой метод
		/*var ahead = _leaderBody.velocity.normalized * _behinding + leader.transform.position;
		if (Vector3.Distance(ahead, transform.position) <= _maxVision
			|| Vector3.Distance(leader.transform.position, transform.position)  <= _maxVision)
			vector += Evading(leader.transform.position);*/

		_rigidBody.velocity = vector;
	}

	private Vector3 Arrival(Vector3 target)
	{
		var desVelocity = target - transform.position;
		var targetDistance = desVelocity.magnitude;

		//Если : объект в области замедления
		if (targetDistance < _distance)
			desVelocity = desVelocity.normalized * _maxForce * (targetDistance / _distance);
		//Если : объект вне области замедления
		else
			desVelocity = desVelocity.normalized * _maxForce;

		var steering = (desVelocity - _rigidBody.velocity) / _rigidBody.mass;

		//Усечение скоординированного вектора движения
		steering = Vector3.ClampMagnitude(_rigidBody.velocity + steering, _maxForce);

		return Vector3.ClampMagnitude(steering, _maxSpeed);
	}

	private Vector3 Separation()
	{
		var force = Vector3.zero;
		var friend = 0f;

		foreach(var person in _team)
			if (person != gameObject 
				&& Vector3.Distance(person.transform.position, gameObject.transform.position) <= _maxRadius)
			{
				force.x += person.transform.position.x - gameObject.transform.position.x;
				force.z += person.transform.position.z - gameObject.transform.position.z;
				friend++;
			}

		if (friend != 0)
		{
			force.x /= friend;
			force.z /= friend;

			force = -1 * force;
		}

		return force.normalized * _maxSeparation;
	}

	private Vector3 Evading(Vector3 target)
	{
		//Положение цели
		var distance = target - transform.position;

		//Коэффициент предсказания на основе расстояния до цели
		//Чем расстояние больше, тем большее количество итераций предсказывается
		var prophetic = distance.magnitude / 10;

		//Будущее положение
		var propheticPosition = target + _leaderBody.velocity * prophetic;

		return Flee(propheticPosition);
	}

	private Vector3 Flee(Vector3 target)
	{
		//steering = desiredVelocity - velocity
		var steering = (transform.position - target).normalized * _maxForce - _rigidBody.velocity;

		//Учет массы объекта (увеличивает ширину качания)
		steering /= _rigidBody.mass;
		//Усечение скоординированного вектора движения
		steering = Vector3.ClampMagnitude(_rigidBody.velocity + steering, _maxSpeed);

		//Убираем движение по высоте
		steering.y = 0;

		//Расстояние до цели
		var targetDistance = Vector3.Distance(target, transform.position);

		//Если расстояние превышает максимум - приравниваем ему
		if (targetDistance > _distance) targetDistance = _distance;

		//Коэффициент скорости при отдалении от цели
		//На середине дистанции максСкорость равна указаной
		var coef = 2 * (1 - targetDistance / _distance);

		return Vector3.ClampMagnitude(steering, coef * _maxSpeed);
	}
	
	//Уход с пути лидера
	public void GetOut(Vector3 target, Vector3 velocity)
	{
		var up = new Vector3(0, 1, 0);

		var cross = Vector3.Cross(up, -velocity);
		cross.y = 0;

		//Если : угол между силой движения и центром лидера отрицательный
		//==> компаньон уворачивается влево, иначе - вправо
		if (Vector3.SignedAngle(target, -velocity, up) < 0) cross = -1 * cross;
		_rigidBody.velocity += Vector3.ClampMagnitude(cross, _maxSpeed);
	}
}


using Assets.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Move : MonoBehaviour
{
	#region Закрытые поля

	/// <summary>
	/// Тип активного движения
	/// </summary>
	private MovementType _movement;

	/// <summary>
	/// Максимальная скорость объекта
	/// </summary>
	private float maxSpeed = 10f;

	/// <summary>
	/// Максимальная ширина качания
	/// </summary>
	private float maxForce = 10f;

	/// <summary>
	/// Расстояние изменения характера движения
	/// </summary>
	private float distance = 10f;

	/// <summary>
	/// Физ тело
	/// </summary>
	private Rigidbody _rigidBody;

	/// <summary>
	/// Расстояние до центра окружности
	/// </summary>
	private float center = 2;

	/// <summary>
	/// Радиус окружности сил блуждания
	/// </summary>
	private float radius = 8f;

	/// <summary>
	/// Интервал смещения угла блуждания
	/// </summary>
	private float angle = 25f;

	/// <summary>
	/// Угол отклонения
	/// </summary>
	private float wanderRad = 0f;

	/// <summary>
	/// Дальность предсказания
	/// </summary>
	private float propheticIndex = 10f;

	/// <summary>
	/// Физ тело преследуемого
	/// </summary>
	private Rigidbody _enemyBody;

	/// <summary>
	/// Максимальная сила убегания от препятствия
	/// </summary>
	private float maxAvoidForce = 10f;

	#endregion

	/// <summary>
	/// Расстояние расматривания препятствий
	/// </summary>
	public float avoidDistance { get; private set; } = 10f;



	#region Свойства установки параметров GUI

	/// <summary>
	/// Устанавливает тип движения
	/// </summary>
	public int SetMovementType { set => _movement = (MovementType)value; }

	/// <summary>
	/// Устанавливает максимальное значение скорости объекта
	/// </summary>
	public string SetMaxSpeed { set => maxSpeed = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальное значение силы движения объекта
	/// </summary>
	public string SetMaxForce { set => maxForce = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальное значение расстояние реагирования объекта
	/// </summary>
	public string SetDistance { set => distance = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает массу объекта
	/// </summary>
	public string SetMass { set => _rigidBody.mass = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает расстояние до центра корректирующих сил блуждания объекта
	/// </summary>
	public string SetCenter { set => center = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает радиус корректирующих сил блуждания объекта
	/// </summary>
	public string SetRadius { set => radius = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает угол смещения блуждания объекта
	/// </summary>
	public string SetAngle { set => angle = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает дальность предсказания цели
	/// </summary>
	public string SetProphetic { set => propheticIndex = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает максимальную силу убегания объекта
	/// </summary>
	public string SetMaxAvoidForce { set => maxAvoidForce = Converter.ToFloat(value); }

	/// <summary>
	/// Устанавливает дальность реагирования на препятствия
	/// </summary>
	public string SetAvoidDistance { set => avoidDistance = Converter.ToFloat(value); }

	#endregion

	/// <summary>
	/// Позиция преследования
	/// </summary>
	public Vector3 target { get; set; }

	/// <summary>
	/// Цель преследования
	/// </summary>
	public GameObject Prey { get; set; }

	void Start()
	{
		_rigidBody = gameObject.GetComponent<Rigidbody>();
		_enemyBody = Prey.GetComponent<Rigidbody>();
	}

	void Update()
	{
		switch (_movement)
		{
			case MovementType.Seek:
				_rigidBody.velocity = Seek(target);
				break;
			case MovementType.Flee:
				_rigidBody.velocity = Flee(target);
				break;
			case MovementType.Arrival:
				_rigidBody.velocity = Arrival(target);
				break;
			case MovementType.Wander:
				_rigidBody.velocity = Wander();
				break;
			case MovementType.CollisionAvoidance:
				_rigidBody.velocity = CollisionAvoidance(target);
				break;
			case MovementType.Pursuit:
				_rigidBody.velocity = Pursuit(Prey.transform.position);
				break;
			case MovementType.Evading:
				_rigidBody.velocity = Evading(Prey.transform.position);
				break;
		}
	}

	private Vector3 Seek(Vector3 target)
	{
		//steering = desiredVelocity - velocity
		var steering = (target - transform.position).normalized * maxForce - _rigidBody.velocity;
		//inlude mass
		steering = Vector3.ClampMagnitude(steering, maxForce) / _rigidBody.mass;
		//velocity
		return Vector3.ClampMagnitude(_rigidBody.velocity + steering, maxSpeed);
	}

	/// <remarks>distance - расстояние замедления движения объекта</remarks>
	private Vector3 Flee(Vector3 target)
	{
		//steering = desiredVelocity - velocity
		var steering = (transform.position - target).normalized * maxForce - _rigidBody.velocity;

		//Учет массы объекта (увеличивает ширину качания)
		steering /= _rigidBody.mass;
		//Усечение скоординированного вектора движения
		steering = Vector3.ClampMagnitude(_rigidBody.velocity + steering, maxSpeed);

		//Убираем движение по высоте
		steering.y = 0;

		//Расстояние до цели
		var targetDistance = Vector3.Distance(target, transform.position);

		//Если расстояние превышает максимум - приравниваем ему
		if (targetDistance > distance) targetDistance = distance;

		//Коэффициент скорости при отдалении от цели
		//На середине дистанции максСкорость равна указаной
		var coef = 2 * (1 - targetDistance / distance);

		return Vector3.ClampMagnitude(steering, coef * maxSpeed);
	}

	/// <remarks>distance - расстояние замедления движения объекта</remarks>
	private Vector3 Arrival(Vector3 target)
	{
		var desVelocity = target - transform.position;
		var targetDistance = desVelocity.magnitude;

		//Если : объект в области замедления
		if (targetDistance < distance)
			desVelocity = desVelocity.normalized * maxForce * (targetDistance / distance);
		//Если : объект вне области замедления
		else
			desVelocity = desVelocity.normalized * maxForce;

		var steering = (desVelocity - _rigidBody.velocity) / _rigidBody.mass;

		//Усечение скоординированного вектора движения
		steering = Vector3.ClampMagnitude(_rigidBody.velocity + steering, maxForce);

		return Vector3.ClampMagnitude(steering, maxSpeed);
	}

	/// <remarks>distance - расстояние центра окружности корректирующей силы
	/// angleChange - половина угла отклонения корректирующей силы
	/// wanderRad - угол действующего отклонения корректирующей силы
	/// radius - радиус окружности корректирующей силы</remarks>
	private Vector3 Wander()
	{
		//Вектор движения объекта к центру к оружности сил блуждания
		var centerVector = _rigidBody.velocity.normalized * center;

		//Смещение угла корректирующей силы в радианах
		var deltaRad = Mathf.PI / 180 * Random.Range(-angle, angle);

		//Изменение угла корректирующей силы
		wanderRad += deltaRad;

		//Приведение угла к границам от 0 до 180 градусов [0, 2 * Pi]
		if (wanderRad > 2 * Mathf.PI) wanderRad -= 2 * Mathf.PI;
		else if (wanderRad < 0) wanderRad += 2 * Mathf.PI;

		//Корректирующий направление вектор
		var displacement =
			new Vector3(Mathf.Cos(wanderRad), 0, Mathf.Sin(wanderRad)).normalized * radius;

		var steering = Vector3.ClampMagnitude(centerVector + displacement, maxForce);
		steering /= _rigidBody.mass;

		return Vector3.ClampMagnitude(_rigidBody.velocity + steering, maxSpeed);
	}

	/// <remarks>propheticIndex - коэффициент предсказания позиции цели</remarks>
	private Vector3 Pursuit(Vector3 target)
	{
		//Положение цели
		var distance = target - transform.position;

		//Коэффициент предсказания на основе расстояния до цели
		//Чем расстояние больше, тем большее количество итераций предсказывается
		var prophetic = distance.magnitude / propheticIndex;

		//Будущее положение
		var propheticPosition = target + _enemyBody.velocity * prophetic;

		return Seek(propheticPosition);
	}

	/// <remarks>distance - расстояние замедления движения объекта
	/// propheticIndex - коэффициент предсказания позиции цели</remarks>
	private Vector3 Evading(Vector3 target)
	{
		//Положение цели
		var distance = target - transform.position;

		//Коэффициент предсказания на основе расстояния до цели
		//Чем расстояние больше, тем большее количество итераций предсказывается
		var prophetic = distance.magnitude / propheticIndex;

		//Будущее положение
		var propheticPosition = target + _enemyBody.velocity * prophetic;

		return Flee(propheticPosition);
	}

	private Vector3 CollisionAvoidance(Vector3 target)
	{
		//Решил применить "особенность" юнити и пускать вперед не вектора ahead и ahead2,
		//как в туториале, а луч в разрезе с шар.
		//Таким образом при прокатывании с препятствием в "притирку" шар все равно имеет силууклонения

		//Направляется к цели
		var steering = (target - transform.position).normalized * maxForce - _rigidBody.velocity;

		var avoid = Vector3.zero;

		//Луч направленный по вектору движения к целе
		var ray = new Ray(transform.position, _rigidBody.velocity.normalized);

		//Бросок луча с разрезом
		if (Physics.SphereCast(ray, 1f, out RaycastHit hit))
			//Если : цель попадания обладает коллайдером
			if (hit.transform.gameObject.GetComponent<Collider>() != null)
				//Если : расстояние до препятствия меньше дальности реагирования
				if (hit.distance <= avoidDistance)
				{
					var ahead = transform.position + _rigidBody.velocity.normalized * maxAvoidForce;
					avoid = (ahead - hit.transform.position).normalized * maxAvoidForce;
				}

		//inlude mass
		steering = Vector3.ClampMagnitude(steering + avoid, maxForce) / _rigidBody.mass;
		//velocity
		return Vector3.ClampMagnitude(_rigidBody.velocity + steering, maxSpeed);
	}
}

/// <summary>
/// Перечисление типов движений
/// </summary>
public enum MovementType : int
{
	Seek = 0,
	Flee = 1,
	Arrival = 2,
	Wander = 3,
	CollisionAvoidance = 4,
	Pursuit = 5,
	Evading = 6,
}

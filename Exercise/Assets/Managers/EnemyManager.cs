using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{	
	/// <summary>
	/// Скорость подъема лавы
	/// </summary>
	public static float LiftFlameSpeed = 1f;

	/// <summary>
	/// Скорость движения шариков
	/// </summary>
	public static float SphereSpeed = .5f;



	/// <summary>
	/// Время между созданиями шариков
	/// </summary>
	[SerializeField]
	private int IntervalSphere = 2;
	   
	/// <summary>
	/// Шаблон враждебного шарика
	/// </summary>
	[SerializeField]
	private GameObject SpherePrefab;

	/// <summary>
	/// Менеджер лестницы
	/// </summary>
	private StairwayManager _Stairway;

	/// <summary>
	/// Время создания сферы
	/// </summary>
	private float _Timer = 0f;

	void Start()
    {
		_Stairway = GetComponent<StairwayManager>();
    }

    void Update()
    {
		//Если : пришло время создавать сферу
		if (Time.time  - _Timer >= IntervalSphere)
		{
			_Timer = Time.time;
			GetPositionSphere(Instantiate(SpherePrefab));
		}
    }

	/// <summary>
	/// Устанавливает позицию сфере
	/// </summary>
	private void GetPositionSphere(GameObject sphere)
	{
		var position = _Stairway.GetLastStep;

		position.y += 1;

		position.z = Random.Range(-3, 4);

		sphere.transform.position = position;
	}
}

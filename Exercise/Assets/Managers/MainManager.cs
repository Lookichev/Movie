using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyManager))]
[RequireComponent(typeof(StairwayManager))]
[RequireComponent(typeof(UIManager))]
[RequireComponent(typeof(MobileManager))]
public class MainManager : MonoBehaviour
{
	/// <summary>
	/// Включен режим мобильного управления
	/// </summary>
	public static bool isMobileVersion { get; private set; } = true;

	public static bool GameOver { get; set; }

	public static EnemyManager Enemy { get; private set; }

	public static StairwayManager Stairway { get; private set; }

	public static UIManager Interface { get; private set; }

	public static MobileManager Mobile { get; private set; }

	private static MainManager _I;

	void Start()
    {
		Enemy = gameObject.GetComponent<EnemyManager>();
		Stairway = gameObject.GetComponent<StairwayManager>();
		Interface = gameObject.GetComponent<UIManager>();
		Mobile = gameObject.GetComponent<MobileManager>();

		_I = this;
	}

	/// <summary>
	/// Игрок проиграл
	/// </summary>
	public static void PlayerIsDie()
	{
		if (GameOver) return;

		_I.StartCoroutine(_I.SlowGame());
	}

	/// <summary>
	/// Замедление игры
	/// </summary>
	private IEnumerator SlowGame()
	{
		GameOver = true;

		while (true)
		{
			yield return Time.timeScale -= Time.deltaTime;

			if (Time.timeScale < 0.1) break;
		}
		Time.timeScale = 0;

		Interface.OpenGameOverMenu();
	}
}

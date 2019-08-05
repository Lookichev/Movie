using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour
{
	/// <summary>
	/// Пауза симуляции
	/// </summary>
	public bool IsPause { get; private set; }

	#region Настройка меню

	/// <summary>
	/// Устанавливает видимость панели настройки охотника
	/// </summary>
	public bool SetActiveHunterPanel { set => hunterPanel.SetActive(value); }

	/// <summary>
	/// Устанавливает видимость панели настройки компаньона
	/// </summary>
	public bool SetActiveCompanionPanel { set => companionPanel.SetActive(value); }

	/// <summary>
	/// Устанавливает видимость панели настройки блуждания
	/// </summary>
	public int SetActiveWanderPanel { set => wanderPanel.SetActive(value == 3); }

	/// <summary>
	/// Устанавливает видимость панели настройки обхода препятствий
	/// </summary>
	public int SetActiveAwoidancenPanel { set => avoidancePanel.SetActive(value == 4); }

	/// <summary>
	/// Устанавливает видимость панели автофлагинга
	/// </summary>
	public int SetActiveAutoFlagPanel { set => autoFlagPanel.SetActive(value != 2); }

	#endregion

	#region Панели меню

	/// <summary>
	/// Настройка параметров ролика
	/// </summary>
	[SerializeField]
	private GameObject settings;

	/// <summary>
	/// Панель настройки охотника
	/// </summary>
	[SerializeField]
	private GameObject hunterPanel;

	/// <summary>
	/// Панель настройки компаньонов
	/// </summary>
	[SerializeField]
	private GameObject companionPanel;

	/// <summary>
	/// Панель блужданий
	/// </summary>
	[SerializeField]
	private GameObject wanderPanel;

	/// <summary>
	/// Панель обхода препятствий
	/// </summary>
	[SerializeField]
	private GameObject avoidancePanel;

	/// <summary>
	/// Панель автофлагинга
	/// </summary>
	[SerializeField]
	private GameObject autoFlagPanel;

	#endregion

	void Start()
    {
		settings.SetActive(false);
		avoidancePanel.SetActive(false);
		companionPanel.SetActive(false);
		hunterPanel.SetActive(false);
		wanderPanel.SetActive(false);
		autoFlagPanel.SetActive(false);
	}

    void Update()
    {
		//Обработка паузы
		if (Input.GetKeyDown(KeyCode.Q)) Pause();
	}


	/// <summary>
	/// Пауза симуляции
	/// </summary>
	private void Pause()
	{
		if (IsPause)
		{
			settings.SetActive(false);
			IsPause = false;
			Time.timeScale = 1f;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
		else
		{
			settings.SetActive(true);
			IsPause = true;
			Time.timeScale = 0f;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}

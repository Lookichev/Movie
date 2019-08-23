using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	/// <summary>
	/// Номер ступеньки
	/// </summary>
	public int NumStep { get; private set; }

	/// <summary>
	/// Переход на следующую ступеньку
	/// </summary>
	public void NextStep()
	{
		NumStep += 1;
		//Запись рекорда
		RecordText.text = NumStep.ToString();
	}

	/// <summary>
	/// Меню паузы
	/// </summary>
	[SerializeField]
	private RectTransform PauseMenu;

	/// <summary>
	/// Меню конца игры
	/// </summary>
	[SerializeField]
	private RectTransform GameOverMenu;

	/// <summary>
	/// Меню результатов
	/// </summary>
	[SerializeField]
	private RectTransform RecordsMenu;

	/// <summary>
	/// Значение рекорда
	/// </summary>
	[SerializeField]
	private Text RecordText;

	/// <summary>
	/// Имя игрока
	/// </summary>
	[SerializeField]
	private InputField Name;

	/// <summary>
	/// Имена чемпионов
	/// </summary>
	[SerializeField]
	private Text[] Names;

	/// <summary>
	/// Достижения чемпионов
	/// </summary>
	[SerializeField]
	private Text[] Results;

	/// <summary>
	/// Рекордсмены
	/// </summary>
	private List<KeyValuePair<string, int>> _Records;

	void Start()
	{
		PauseMenu.gameObject.SetActive(false);
		GameOverMenu.gameObject.SetActive(false);
		RecordsMenu.gameObject.SetActive(false);
		RecordText.text = "0";

		_Records = RecordTable.Records;
	}

	/// <summary>
	/// Установка паузы
	/// </summary>
	public void SetPause()
	{
		Time.timeScale = 0f;
		PauseMenu.gameObject.SetActive(true);
	}

	/// <summary>
	/// Возобновление игры
	/// </summary>
	public void GameResume()
	{
		Time.timeScale = 1f;
		PauseMenu.gameObject.SetActive(false);
	}

	/// <summary>
	/// Перезапуск игры
	/// </summary>
	public void RestartGame()
	{
		//Сохраняем достижения
		RecordTable.Records = _Records;

		SceneManager.LoadScene(1);
		Time.timeScale = 1f;
		MainManager.GameOver = false;
	}

	/// <summary>
	/// Открываем меню конца игры
	/// </summary>
	public void OpenGameOverMenu()
	{
		GameOverMenu.gameObject.SetActive(true);

		//Поиск пустого текстового поля и заполнения его результатами игрока
		var text = GameOverMenu.gameObject.GetComponentsInChildren<Text>().
			FirstOrDefault(t=> string.IsNullOrEmpty(t.text));

		text.text = RecordText.text;
	}

	/// <summary>
	/// Сохранить результаты игры
	/// </summary>
	public void SaveResult()
	{
		//Если : имя не указано - ничего не делаем
		if (string.IsNullOrEmpty(Name.text)) return;

		GameOverMenu.gameObject.SetActive(false);
		RecordsMenu.gameObject.SetActive(true);

		//Добавление нового рекорда, сортировка и удаление лишнего
		_Records.Add(new KeyValuePair<string, int>(Name.text, Convert.ToInt32(RecordText.text)));
		_Records.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
		_Records.RemoveAt(0);

		//Запись результатов в таблицу
		for(int i = 4, j = 0; i >= 0; i--, j++)
		{
			Names[j].text = _Records[i].Key;
			Results[j].text = _Records[i].Value.ToString();
		}
	}
}


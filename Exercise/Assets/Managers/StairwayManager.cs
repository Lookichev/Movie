using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayManager : MonoBehaviour
{
	/// <summary>
	/// Лестничный объект
	/// </summary>
	[SerializeField]
	private GameObject StairwayCreater;

	/// <summary>
	/// Шаблон лестницы
	/// </summary>
	[SerializeField]
	private GameObject StairwayPrefab;

	/// <summary>
	/// Путь в Высшую Лигу
	/// </summary>
	/// <remarks>Эн таро Тассадар</remarks>
	private LinkedList<GameObject> _Ladders;


	/// <summary>
	/// Нужно-ли генерировать новую ступеньку
	/// </summary>
	private bool _NeedNewLadderBlock;

	/// <summary>
	/// Возвращает координаты верхней ступени
	/// </summary>
	public Vector3 GetLastStep => _Ladders.Last.Value.transform.position;

	/// <summary>
	/// Кол-во пройденных ступенек (для генерации нового блока)
	/// </summary>
	private int _Score = 0;

	/// <summary>
	/// Создает новый блок лестницы
	/// </summary>
	private void CreateNewBlock()
	{
		_Ladders.AddLast(Instantiate(StairwayPrefab, StairwayCreater.transform));
		SetPosition();
	}

	void Start()
	{
		_Ladders = new LinkedList<GameObject>();

		_Ladders.AddLast(Instantiate(StairwayPrefab, StairwayCreater.transform));
		//Строим первые ступени
		while (_Ladders.Count != 5) CreateNewBlock();
	}

	void Update()
	{
		//Если : персонаж прошел две ступеньки - надо добавить новые
		if (MainManager.Interface.NumStep - _Score == 2)
		{
			CreateNewBlock();
			_Score = MainManager.Interface.NumStep;
		}

		//Если : блоков много - удаляем старые
		if (_Ladders.Count == 10)
		{
			Destroy(_Ladders.First.Value);
			_Ladders.RemoveFirst();
		}
	}

	/// <summary>
	/// Установка положения новых ступенек
	/// </summary>
	private void SetPosition()
	{
		//Координаты предпоследнего блока лестницы
		var position = _Ladders.Last.Previous.Value.transform.localPosition;

		//Смещение последнего блока на два вверх и вперед (локальные координаты)
		_Ladders.Last.Value.transform.localPosition = new Vector3(position.x + 2, position.y + 2, 0);
	}
}

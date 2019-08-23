using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileManager : MonoBehaviour
{
	/// <summary>
	/// Слушатель для управления на мобильной платформе
	/// </summary>
	[SerializeField]
	private Movement Movement;

	/// <summary>
	/// Область кнопки паузы
	/// </summary>
	[SerializeField]
	private RectTransform PauseButton;


	/// <summary>
	/// Первая позиция касания
	/// </summary>
	private Vector3 _FirstPos;

	/// <summary>
	/// Последняя позиция касания
	/// </summary>
	private Vector3 _LastPos;

	/// <summary>
	/// Минимальная дистанция для определения свайпа
	/// </summary>
	private float _Distance;

	/// <summary>
	/// Храним все позиции касания в списке
	/// </summary>
	private List<Vector3> _TouchsPosList = new List<Vector3>();



	void Start()
	{
		//20% высоты экрана
		_Distance = Screen.height * 20 / 100;
	}

	void Update()
	{
		//Отслеживание всех касаний
		foreach (Touch touch in Input.touches)
		{
			//Если : касание является свайпом - сохраняем его
			if (touch.phase == TouchPhase.Moved) _TouchsPosList.Add(touch.position);

			//Если : касание завершается
			if (touch.phase == TouchPhase.Ended)
			{
				//Положения первого и последнего касаний
				_FirstPos = _TouchsPosList[0];
				_LastPos = _TouchsPosList[_TouchsPosList.Count - 1];

				//Если : дистанция перемещения больше 20% высоты экрана
				if (Mathf.Abs(_LastPos.x - _FirstPos.x) > _Distance || Mathf.Abs(_LastPos.y - _FirstPos.y) > _Distance)
				{
					//Определение направления перемещения 
					if (Mathf.Abs(_LastPos.x - _FirstPos.x) > Mathf.Abs(_LastPos.y - _FirstPos.y))
					{   
						//Если : горизонтальное движение больше, чем вертикальное
						if (_LastPos.x > _FirstPos.x)
							//Свайп вправо
							Movement.AddMovement(TypeMovement.Right);
						else
							//Свайп влево
							Movement.AddMovement(TypeMovement.Left);
					}
					//Вертикальные движения не обязательно отслеживать
					else
					    //Если : вертикальное движение больше, чем горизонтальное
						if (_LastPos.y > _FirstPos.y)
							//Свайп вверх
							Movement.AddMovement(TypeMovement.Up);
						else{ /*Свайп вниз*/ }
				}
				//Расцениваю короткие перемещения, как простые нажатия
				else
				{
					//Если : нажатие было в области кнопки паузы
					if (PauseButton.rect.Contains(_FirstPos)) MainManager.Interface.SetPause();
					//Нажатие было вне кнопки
					else Movement.AddMovement(TypeMovement.Up);
				}
			}

		}
	}
}

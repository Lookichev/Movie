
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseLook : MonoBehaviour
{
	/// <summary>
	/// Цель просмотра
	/// </summary>
	[SerializeField]
	private Transform target;

	[Space]

	/// <summary>
	/// Скорость вращения в горизонтале
	/// </summary>
	[SerializeField]
	private float sensitivityHor = 9.0f;

	/// <summary>
	/// Скорость вращения в вертикале
	/// </summary>
	[SerializeField]
	public float sensitivityVer = 9.0f;

	/// <summary>
	/// Угол наклона камеры по вертикале
	/// </summary>
	private float _rotationX = 0.0f;


	void Start()
	{
		if (target != null) transform.LookAt(target);
	}

	void Update()
	{
		//Если : пауза, ничего не крутить
		if (Time.timeScale == 0) return;

		_rotationX -= Input.GetAxis("Mouse Y") * sensitivityVer;
		_rotationX = Mathf.Clamp(_rotationX, -40, 90);

		//Приращение угла
		float delta = Input.GetAxis("Mouse X") * sensitivityHor;
		//Изменяет угол на величину приращения
		var rotationY = transform.localEulerAngles.y + delta;

		transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
	}
}

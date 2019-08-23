using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	/// <summary>
	/// Имя сцены
	/// </summary>
	public string SceneToLoad;

	/// <summary>
	/// Содержит текст "Loading..."
	/// </summary>
	public RectTransform LoadingOverlay;

	/// <summary>
	/// Кнопка
	/// </summary>
	public GameObject Button;

	/// <summary>
	/// Фоновая загрузка сцены
	/// </summary>
	AsyncOperation SceneLoadingOperation;

    public void Start()
    {
		//Скрывает заставку 
		LoadingOverlay.gameObject.SetActive(false);

		//Асинхронная загрузка сцены
		SceneLoadingOperation = SceneManager.LoadSceneAsync(SceneToLoad);

		//Но не переключает на эту сцену до готовности
		SceneLoadingOperation.allowSceneActivation = false;
    }

    public void LoadScene()
	{
		Button.SetActive(false);

		//Делает заставку Loading  видимой
		LoadingOverlay.gameObject.SetActive(true);

		//Сообщает о переключении сцен по готовности
		SceneLoadingOperation.allowSceneActivation = true;
	}
}

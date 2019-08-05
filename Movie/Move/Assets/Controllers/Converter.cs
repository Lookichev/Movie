using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Controllers
{
	public static class Converter
	{
		/// <summary>
		/// Преобразует decimalNumber UI-элемента Unity в float
		/// </summary>
		/// <param name="unityInputField">Строка из InputField GUI</param>
		/// <returns>Короткое число с плавающей точкой</returns>
		public static float ToFloat(string unityInputField)
		{
			return Convert.ToSingle(unityInputField.Replace('.', ','));
		}

	}
}

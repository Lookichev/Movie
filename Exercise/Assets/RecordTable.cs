using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RecordTable
{
	/// <summary>
	/// Хранит рекорды между сессиями
	/// </summary>
	public static List<KeyValuePair<string, int>> Records { get; set; }

	static RecordTable()
	{
		Records = new List<KeyValuePair<string, int>>(6);

		for (var i = 0; i < 5; i++)
		{
			Records.Add(new KeyValuePair<string, int>("-", 0));
		}
	}
}

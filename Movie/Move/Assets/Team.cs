using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
	public List<GameObject> team { get; private set; }

	public void TakeLeader(GameObject leader)
	{
		team = new List<GameObject>();

		var per = GetComponentsInChildren<CompanionFollowing>();

		//Добавляем в коллекцию всех членов команды
		foreach (var element in per)
		{
			team.Add(element.gameObject);
			element.leader = leader;
		}
	}
}

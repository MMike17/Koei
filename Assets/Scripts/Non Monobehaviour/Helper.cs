using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
	public static List<T> GetChildrenOfType<T>(this Transform transform) where T : MonoBehaviour
	{
		List<T> objects = new List<T>();

		// adds self as component
		if(transform.GetComponent<T>() != null)
			objects.Add(transform.GetComponent<T>());

		// adds children as components
		foreach (Transform child in transform)
			objects.AddRange(child.GetChildrenOfType<T>());

		return objects;
	}
}
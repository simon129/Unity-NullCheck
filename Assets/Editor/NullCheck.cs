using UnityEngine;
using UnityEditor;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class NullCheck : UnityEditor.AssetModificationProcessor
{
	public static string[] OnWillSaveAssets(string[] paths)
	{
		var behaviours = GameObject.FindObjectsOfType<MonoBehaviour>();
		var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		foreach (var b in behaviours)
		{
			if (b.GetType().GetCustomAttributes(typeof(NullCheckable), true).Length == 0)
				continue;

			var fields = b.GetType().GetFields(bindingFlags);
			foreach (var f in fields)
			{
				bool isArray;
				Type baseType;
				if (IsUnitySerializable(f, out isArray, out baseType))
				{
					object obj = f.GetValue(b);
					if (obj == null || obj.Equals(null))
					{
						Debug.LogError(string.Format("{0}.{1} IS NULL", b.GetType().ToString(), f.Name));
						EditorApplication.Beep();
					}
					else if (isArray)
					{
						int count = 0;
						foreach (var a in (IEnumerable)f.GetValue(b))
						{
							count++;
							if (a == null || a.Equals(null))
							{
								Debug.LogError(string.Format("{0}.{1} ARRAY HAS NULL", b.GetType().ToString(), f.Name));
								EditorApplication.Beep();
								break;
							}
						}
						if (count == 0)
						{
							Debug.LogWarning(string.Format("{0}.{1} IS EMPTY ARRAY/LIST", b.GetType().ToString(), f.Name));
						}
					}
				}
			}
		}
		return paths;
	}

	private static bool IsUnitySerializable(FieldInfo f, out bool isArray, out Type baseType)
	{
		var isPublicOrSerializeField = (f.IsPublic || f.GetCustomAttributes(typeof(SerializeField), true).Length > 0) &&
												(f.GetCustomAttributes(typeof(NonSerializedAttribute), true).Length == 0);
		if (!isPublicOrSerializeField)
		{
			isArray = false;
			baseType = null;
			return false;
		}

		return IsSerializable(f, out isArray, out baseType);
	}

	private static bool IsSerializable(FieldInfo f, out bool isArray, out Type baseType)
	{
		var fieldType = f.FieldType;
		if (fieldType.IsArray)
		{
			baseType = fieldType.GetElementType();
			isArray = true;
		}
		else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
		{
			baseType = fieldType.GetGenericArguments()[0];
			isArray = true;
		}
		else
		{
			baseType = fieldType;
			isArray = false;
		}

		return typeof(UnityEngine.Object).IsAssignableFrom(baseType) || baseType.GetCustomAttributes(typeof(SerializableAttribute), true).Length > 0;
	}
}
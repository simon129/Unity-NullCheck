using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[NullCheckable]
public class TestClass : MonoBehaviour
{
	public GameObject Go;
	public GameObject[] ArrayHaveNull;
	public List<GameObject> ListHaveNull;

	[SerializeField]
	private GameObject GoPrivate;
	[SerializeField]
	private GameObject[] GoArrayPrivate;
	[SerializeField]
	private List<GameObject> GoListPrivate;

	[NonSerialized]
	public GameObject GoHide;
	[NonSerialized]
	public GameObject[] GoArrayHide;
	[NonSerialized]
	public List<GameObject> GoListHide;
}
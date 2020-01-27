using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sticker", menuName = "Sticker", order = 1)]
public class Sticker : ScriptableObject
{
	public Texture2D thumbnail;
	public string category;
	public bool isNew = false;
}

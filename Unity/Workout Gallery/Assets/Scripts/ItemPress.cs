using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

	private GameObject itemPressed;
	public bool isPressed;

	public void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
		itemPressed = this.gameObject;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
	}

}

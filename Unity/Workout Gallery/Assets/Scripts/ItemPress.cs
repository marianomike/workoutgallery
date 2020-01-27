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
		//itemPressed.GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
        /*
		itemPressed.GetComponent<Animator>().ResetTrigger("press_show");
		itemPressed.GetComponent<Animator>().ResetTrigger("press_hide");
		itemPressed.GetComponent<Animator>().SetTrigger("press_show");
        */

	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isPressed = false;
		//itemPressed.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        /*
		itemPressed.GetComponent<Animator>().ResetTrigger("press_show");
		itemPressed.GetComponent<Animator>().ResetTrigger("press_hide");
		itemPressed.GetComponent<Animator>().SetTrigger("press_hide");
        */
	}

}

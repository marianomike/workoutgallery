using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemListPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

	private GameObject stickerPressed;
	private GameObject GameEngine;
	private GameObject StickerScrollRect;
	public string stickerName;
	public Sticker stickerToDrag;
	private Sticker[] stickersDataHolder;
	public bool isPressed;

	public void OnPointerDown(PointerEventData eventData)
	{
		isPressed = true;
		stickerPressed = this.gameObject;
		stickerName = stickerPressed.name;
		//itemPressed.GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1.25f);
		stickersDataHolder = GameEngine.GetComponent<Setup>().stickersDataHolder;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		StickerScrollRect.GetComponent<ScrollRect>().enabled = false;
		isPressed = false;
		//itemPressed.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

		for (int i = 0; i < stickersDataHolder.Length; i++)
		{
			if (stickersDataHolder[i].name == stickerName)
			{
				stickerToDrag = stickersDataHolder[i];
			}
		}

		GameEngine.GetComponent<Setup>().CreateDragItem(stickerToDrag, stickerPressed, eventData);
	}

	void Start()
	{
		GameEngine = GameObject.Find("Engine");
		StickerScrollRect = GameObject.Find("StickersScrollRect");
	}

	void Update()
	{
	}
}

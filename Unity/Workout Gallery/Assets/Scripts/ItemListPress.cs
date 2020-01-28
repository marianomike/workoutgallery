using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemListPress : MonoBehaviour
{

	private GameObject stickerPressed;
	private GameObject GameEngine;
	private GameObject StickerScrollRect;
	public string stickerName;
	public Sticker stickerToDrag;
	private Sticker[] stickersDataHolder;
	public bool isPressed;
	public Button listButton;

    private void createSticker()
    {
		stickerPressed = this.gameObject;
		stickerName = stickerPressed.name;
		//StickerScrollRect.GetComponent<ScrollRect>().enabled = false;
		for (int i = 0; i < stickersDataHolder.Length; i++)
		{
			if (stickersDataHolder[i].name == stickerName)
			{
				stickerToDrag = stickersDataHolder[i];
			}
		}

		GameEngine.GetComponent<Setup>().CreateDragItem(stickerToDrag, stickerPressed);
	}

    /*
	public void OnPointerDown(PointerEventData eventData)
	{
		//StartCoroutine(startDragTimer(eventData));

		isPressed = true;
		stickerPressed = this.gameObject;
		stickerName = stickerPressed.name;
		StickerScrollRect.GetComponent<ScrollRect>().enabled = false;

	}

	public void OnPointerUp(PointerEventData eventData)
	{
		// Debug.Log("On Pointer Up");
		
		isPressed = false;
		//StopAllCoroutines();

		for (int i = 0; i < stickersDataHolder.Length; i++)
		{
			if (stickersDataHolder[i].name == stickerName)
			{
				stickerToDrag = stickersDataHolder[i];
			}
		}

		GameEngine.GetComponent<Setup>().CreateDragItem(stickerToDrag, stickerPressed, eventData);
		
		StickerScrollRect.GetComponent<ScrollRect>().enabled = true;
	}
    */

	public IEnumerator startDragTimer(PointerEventData eventData)
	{
		Debug.Log ("coroutine called");
		yield return new WaitForSeconds(0.1f);

		isPressed = true;
		stickerPressed = this.gameObject;
		stickerName = stickerPressed.name;

		for (int i = 0; i < stickersDataHolder.Length; i++)
		{
			if (stickersDataHolder[i].name == stickerName)
			{
				stickerToDrag = stickersDataHolder[i];
			}
		}

		//GameEngine.GetComponent<Setup>().CreateDragItem(stickerToDrag, stickerPressed, eventData);
		StickerScrollRect.GetComponent<ScrollRect>().enabled = false;
	}


	void Start()
	{
		GameEngine = GameObject.Find("Engine");
		StickerScrollRect = GameObject.Find("StickersScrollRect");
		stickersDataHolder = GameEngine.GetComponent<Setup>().stickersDataHolder;
		listButton.onClick.AddListener(createSticker);
	}

	void Update()
	{
	}
}

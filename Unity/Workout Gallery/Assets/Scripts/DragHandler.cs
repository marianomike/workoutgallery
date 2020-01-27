using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{

	public GameObject itemBeingDragged;
	public GameObject itemBeingCollided;

	private GameObject StickerScrollRect;
	private GameObject GameEngine;
	private GameObject StickerPlaceArea;

	public PointerEventData pointerData;
	public Vector3 screenPoint;

	public bool isDragging = false;
	public bool isColliding = false;
	public bool isSideColumn = false;

	void Start()
	{
		GameEngine = GameObject.Find("Engine");
		StickerPlaceArea = GameObject.Find("PhotoHolder");
		StickerScrollRect = GameObject.Find("StickersScrollRect");
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		pointerData = eventData;
		isDragging = true;
	}

	public void OnPointerUp(PointerEventData data)
	{
		isDragging = false;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		pointerData = eventData;
		isDragging = true;
		itemBeingDragged = gameObject;
	}

	public void OnDrag(PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		transform.SetAsLastSibling();
		pointerData = eventData;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (itemBeingCollided != null)
		{
			if (isColliding == true)
			{
				Debug.Log("isSideColumn: " + isSideColumn);

				if (isSideColumn == true)
				{
					Destroy(itemBeingDragged);
				}
			}
		}

		isDragging = false;
        isSideColumn = false;
	}

	void OnCollisionStay2D(Collision2D collision2d)
	{
		itemBeingCollided = collision2d.gameObject;
		isColliding = true;
	}

	void OnCollisionEnter2D(Collision2D collision2d)
	{
		itemBeingDragged = gameObject;
		isColliding = true;
		itemBeingCollided = collision2d.gameObject;

		if (itemBeingCollided.name == "StickerScrollCollider")
		{
			isSideColumn = true;
		}
		else
		{
			isSideColumn = false;
		}
	}

	void OnCollisionExit2D(Collision2D collision2d)
	{
		isColliding = false;
		itemBeingCollided = collision2d.gameObject;
	}

	void Update()
	{
		if (isDragging == true)
		{
			screenPoint = Input.mousePosition;
			screenPoint.z = 10.0f; //distance of the plane from the camera
			itemBeingDragged.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);

			StickerScrollRect.GetComponent<ScrollRect>().enabled = false;
			StickerScrollRect.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
		}
		else
		{
			isDragging = false;
			StickerScrollRect.GetComponent<ScrollRect>().enabled = true;
		}
	}
}
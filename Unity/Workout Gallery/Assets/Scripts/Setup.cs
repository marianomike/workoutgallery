﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Setup : MonoBehaviour, IPointerDownHandler
{
	public GameObject StickerScrollRectBg;
	public GameObject StickerThumbnailHolder;
	private GameObject GameEngine;
	public GameObject DragSticker = null;
	public GameObject StickerPlaceArea = null;

	private float NewWidth;
	private float NewHeight;

	private const string PATH_RESOURCES = "Assets/Resources/";
	private const string PATH_STICKERS = "StickerData/";
	private const string PATH_TEMPLATES = "Prefabs/";
	private const string PATH_STICKER_TEXTURES = "Textures/Stickers/";

	private const string TEMPLATE_DRAG = "template_sticker_drag";
	private const string TEMPLATE_LIST = "template_sticker_list";

	private const string PNG_EXTENSION = ".png";

	public Sticker[] stickersDataHolder;
	private List<Sticker> scrollItems;
	public List<GameObject> dragObjects;
	private Sprite[] spriteTexturesHolder;
	private Object[] stickerDataLoader;
	private Object[] spriteLDataLoader;
	public bool isDragging = false;
	public PointerEventData pointerData;

	void SetScrollBgColliderSize()
	{
		NewWidth = StickerScrollRectBg.GetComponent<RectTransform>().rect.width;
		NewHeight = StickerScrollRectBg.GetComponent<RectTransform>().rect.height;

		StickerScrollRectBg.GetComponent<BoxCollider2D>().size = new Vector2(NewWidth, NewHeight);
	}

    // load the sprite textures
	private void LoadStickerSprites()
	{
		spriteLDataLoader = Resources.LoadAll(PATH_STICKER_TEXTURES, typeof(Sprite));
		spriteTexturesHolder = new Sprite[spriteLDataLoader.Length];

		for (int i = 0; i < spriteLDataLoader.Length; i++)
		{
			spriteTexturesHolder[i] = (Sprite)spriteLDataLoader[i];
		}
	}

    /*
	private void FindSprite(string texture, GameObject imageObject)
	{
		for (int i = 0; i < spriteTexturesHolder.Length; i++)
		{
			string newName = Path.GetFileName(spriteTexturesHolder[i].name);
			if (newName == texture)
			{
				imageObject.GetComponent<Image>().sprite = spriteTexturesHolder[i];
			}
		}
	}
    */

	private void LoadStickerData()
	{
		stickerDataLoader = Resources.LoadAll(PATH_STICKERS, typeof(Sticker));
		stickersDataHolder = new Sticker[stickerDataLoader.Length];

		for (int i = 0; i < stickerDataLoader.Length; i++)
		{
			stickersDataHolder[i] = (Sticker)stickerDataLoader[i];
		}
	}

	public GameObject CreateDragItem(Sticker sticker, GameObject pressSticker, PointerEventData eventData)
	{

		GameObject newDragSticker = Instantiate(Resources.Load(PATH_TEMPLATES + TEMPLATE_DRAG)) as GameObject;
		newDragSticker.name = sticker.name;
		string spriteTexture = sticker.thumbnail.name;

#if UNITY_EDITOR

		Sprite newTexture = (Sprite)AssetDatabase.LoadAssetAtPath(PATH_RESOURCES + PATH_STICKER_TEXTURES + spriteTexture + PNG_EXTENSION, typeof(Sprite));
		newDragSticker.GetComponent<TemplateStickerDrag>().thumbImage.sprite = newTexture;

		// if making a prototype build load from the Resources folder
#else
		for (int i = 0; i < spriteTextures.Length; i++){
			string newName = Path.GetFileName(spriteTextures[i].name);
			if (newName == spriteTexture){
				newItem.GetComponent<TemplateDragItem>().thumb.GetComponent<Image>().sprite = spriteTextures[i];
			}
		}
#endif

		newDragSticker.transform.SetParent(StickerPlaceArea.transform, true);
		newDragSticker.GetComponent<DragHandler>().itemBeingDragged = newDragSticker;

		if (pressSticker != null)
		{
			newDragSticker.GetComponent<DragHandler>().isDragging = true;
			isDragging = true;
			Vector3 screenPoint = eventData.position;
			screenPoint.z = 10.0f; //distance of the plane from the camera
			newDragSticker.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
			newDragSticker.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			DragSticker = newDragSticker;
		}

		dragObjects.Add(newDragSticker);
		return newDragSticker;

	}

	private void CreateSidebar()
	{
		for (int i = 0; i < stickersDataHolder.Length; i++)
		{
			AddToScrollRect(stickersDataHolder[i]);
		}
	}

	public void AddToScrollRect(Sticker sticker)
	{
		GameObject newSticker = Instantiate(Resources.Load(PATH_TEMPLATES + TEMPLATE_LIST)) as GameObject;
		newSticker.name = sticker.name;

		string spriteTexture = sticker.thumbnail.name;

#if UNITY_EDITOR

		Sprite newTexture = (Sprite)AssetDatabase.LoadAssetAtPath(PATH_RESOURCES + PATH_STICKER_TEXTURES + spriteTexture + PNG_EXTENSION, typeof(Sprite));
		newSticker.GetComponent<TemplateStickerList>().thumbImage.sprite = newTexture;

		// if making a prototype build load from the Resources folder
#else
		for (int i = 0; i < spriteTexturesHolder.Length; i++){
			string newName = Path.GetFileName(spriteTexturesHolder[i].name);
			if (newName == spriteTexture){
				newSticker.GetComponent<TemplateStickerList>().thumbImage.GetComponent<Image>().sprite = spriteTexturesHolder[i];
			}
		}
#endif

		newSticker.transform.SetParent(StickerThumbnailHolder.transform, true);
		newSticker.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		newSticker.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		scrollItems.Add(sticker);
	}

	// Start is called before the first frame update
	void Start()
    {
        scrollItems = new List<Sticker>();
		LoadStickerData();
		LoadStickerSprites();
		CreateSidebar();
		SetScrollBgColliderSize();
    }

    // Update is called once per frame
    void Update()
    {
		bool isPressed = Input.GetMouseButton(0);
		if (DragSticker != null)
		{
			bool hasDrag = DragSticker.GetComponent<DragHandler>().isDragging;

			if (isPressed == false && hasDrag == true)
			{
				//Debug.Log ("Mouse let go");
				DragSticker.GetComponent<DragHandler>().isDragging = false;
				DragSticker.GetComponent<DragHandler>().OnEndDrag(pointerData);
				//DragItem.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
			}
		}
	}

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}

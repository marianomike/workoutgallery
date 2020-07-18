using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
using TMPro;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Setup : MonoBehaviour, IPointerDownHandler
{
	// variables used for progression
	private bool photoIsLoaded = false;
	private bool distanceIsEntered = false;
	private float result = 0.0f;

    // store stats here
	private float distance = 0;
	private int hours = 0;
	private int minutes = 0;
	private int seconds = 0;
	private string pace;

    // combines hours + minutes + seconds entered into total seconds
	private float TotalSeconds;

	public GameObject StickerScrollRectBg;
	public GameObject StickerThumbnailHolder;
	public GameObject DragSticker = null;
	public GameObject StickerPlaceArea = null;
	public GameObject ImageHolder;
	public GameObject ButtonsHolder;
	public Splash SplashScreen;

	public Button PickImageButton;
	public Button TakeImageButton;
	public RawImage SelectedImage;
	public Button DeleteImageButton;
	public Button CustomizeButton;

	public TMP_InputField DistanceInput;
	public TMP_Dropdown DistanceDropdown;
	public TMP_InputField HoursInput;
	public TMP_InputField MinutesInput;
	public TMP_InputField SecondsInput;
	public TMP_Text PaceText;

    // Stats Layout 1
    public TMP_Text DistanceDisplay1;
	public TMP_Text TimeDisplay1;
	public TMP_Text PaceDisplay1;

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

	private void ChooseImage(int maxSize)
	{
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				ImageHolder.SetActive(true);
				SelectedImage.texture = texture;
				ButtonsHolder.SetActive(false);
				
				Utilities.SizeToParent(SelectedImage);

				photoIsLoaded = true;
			}
		}, "Select a PNG image", "image/png");

		Debug.Log("Permission result: " + permission);
	}

	private void TakePicture(int maxSize)
	{
		NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create Texture from selected image
				Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				ImageHolder.SetActive(true);
				SelectedImage.texture = texture;
				ButtonsHolder.SetActive(false);

				Utilities.SizeToParent(SelectedImage);

				photoIsLoaded = true;
			}
		}, maxSize);

		Debug.Log("Permission result: " + permission);
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

	public GameObject CreateDragItem(Sticker sticker, GameObject pressSticker)
	{

		GameObject newDragSticker = Instantiate(Resources.Load(PATH_TEMPLATES + TEMPLATE_DRAG)) as GameObject;
		newDragSticker.name = sticker.name;
		string spriteTexture = sticker.thumbnail.name;

#if UNITY_EDITOR

		Sprite newTexture = (Sprite)AssetDatabase.LoadAssetAtPath(PATH_RESOURCES + PATH_STICKER_TEXTURES + spriteTexture + PNG_EXTENSION, typeof(Sprite));
		newDragSticker.GetComponent<TemplateStickerDrag>().thumbImage.sprite = newTexture;

		// if making a prototype build load from the Resources folder
#else
		for (int i = 0; i < spriteTexturesHolder.Length; i++){
			string newName = Path.GetFileName(spriteTexturesHolder[i].name);
			if (newName == spriteTexture){
				newDragSticker.GetComponent<TemplateStickerDrag>().thumbImage.sprite = spriteTexturesHolder[i];
			}
		}
#endif

		newDragSticker.transform.SetParent(StickerPlaceArea.transform, false);
		newDragSticker.GetComponent<DragHandler>().itemBeingDragged = newDragSticker;

		if (pressSticker != null)
		{
			/*
			newDragSticker.GetComponent<DragHandler>().isDragging = true;
			isDragging = true;
			Vector3 screenPoint = eventData.position;
			screenPoint.z = 10.0f; //distance of the plane from the camera
			newDragSticker.transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
			newDragSticker.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 2f);
			DragSticker = newDragSticker;
            */
			newDragSticker.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 2f);

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
				newSticker.GetComponent<TemplateStickerList>().thumbImage.sprite = spriteTexturesHolder[i];
			}
		}
#endif

		newSticker.transform.SetParent(StickerThumbnailHolder.transform, true);
		newSticker.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
		newSticker.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
		scrollItems.Add(sticker);
	}

    private void DeleteImage()
    {
		ImageHolder.SetActive(false);
		SelectedImage.texture = null;
		ButtonsHolder.SetActive(true);
	}

    private void SetDistance(TMP_InputField input)
    {
		distance = float.Parse(input.text);
		CalculateTotalSeconds();
	}

    private void SetHours(TMP_InputField input)
    {
		hours = System.Convert.ToInt32(input.text);
		CalculateTotalSeconds();
	}

	private void SetMinutes(TMP_InputField input)
	{
		minutes = System.Convert.ToInt32(input.text);
		CalculateTotalSeconds();
	}

	private void SetSeconds(TMP_InputField input)
	{
		seconds = System.Convert.ToInt32(input.text);
		CalculateTotalSeconds();
	}

    private void CalculateTotalSeconds()
    {
		TotalSeconds = (hours * 3600) + (minutes * 60) + seconds;

        if(distance != 0)
        {
			float CalculatedPace = TotalSeconds / distance;

			pace = Utilities.FormatTime(CalculatedPace);
			PaceText.text = pace;
		}
	}

	private void CovertDistance(TMP_Dropdown dropdown)
    {
        if(dropdown.value == 0)
        {
			distance = (float)Utilities.ConvertToMiles(distance);
        }
        else
        {
			distance = (float)Utilities.ConvertToKilometers(distance);
		}

        Debug.Log(distance);

        if(distance.ToString().Length > 1)
        {
			DistanceInput.text = distance.ToString(("F2"));
        }
        else
        {
			DistanceInput.text = distance.ToString();
		}
		CalculateTotalSeconds();
	}

	// Start is called before the first frame update
	void Start()
    {
		SplashScreen.gameObject.SetActive(true);
		scrollItems = new List<Sticker>();
		ImageHolder.SetActive(false);
		LoadStickerData();
		LoadStickerSprites();
		CreateSidebar();
		SetScrollBgColliderSize();
		PickImageButton.onClick.AddListener(() => ChooseImage(1024));
		TakeImageButton.onClick.AddListener(() => TakePicture(1024));

		DistanceDropdown.onValueChanged.AddListener(delegate { CovertDistance(DistanceDropdown); });
		DistanceInput.onValueChanged.AddListener(delegate { SetDistance(DistanceInput); });
		HoursInput.onValueChanged.AddListener(delegate { SetHours(HoursInput); });
		MinutesInput.onValueChanged.AddListener(delegate { SetMinutes(MinutesInput); });
		SecondsInput.onValueChanged.AddListener(delegate { SetSeconds(SecondsInput); });

		DeleteImageButton.onClick.AddListener(DeleteImage);
		SplashScreen.SplashIn();
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


		CustomizeButton.interactable = (photoIsLoaded);

		if (float.TryParse(DistanceInput.text, out result))
		{
			DistanceDisplay1.text = DistanceInput.text;
		}
		else
		{
			DistanceDisplay1.text = "Distance";
		}

		/*
        if(float.TryParse(DistanceInput.text, out result) > 0)
        {
			DistanceDisplay1.text = DistanceInput.text;
        }
        else
        {
			DistanceDisplay1.text = "Distance";
		}
        */



		//Debug.Log(DistanceInput.text);

		//CalculateTotalSeconds();
	}

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}

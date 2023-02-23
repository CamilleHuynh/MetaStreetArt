using UnityEngine;
using UnityEngine.UI;

public class StickerMenu : MonoBehaviour
{
    [SerializeField] private Transform Panel;
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject PrefabUISticker;
    [SerializeField] private Stickers soStickers;


    private void Start()
    {
        Panel.gameObject.SetActive(false);
        foreach (var sticker in soStickers.stickerList)
        {
            var ui = Instantiate(PrefabUISticker, Content.transform).transform.GetChild(0);
            ui.GetComponent<Image>().overrideSprite =
                Sprite.Create((Texture2D)sticker.Image, new Rect(0, 0, sticker.Image.width, sticker.Image.height),
                    new Vector2(0.5f, 0.5f));
            ui.GetComponent<Button>().onClick.AddListener(() => { SelectSticker(sticker.id); });
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // RightClick
            SetMenuActive(!Panel.gameObject.activeSelf);
    }

    private void SetMenuActive(bool active)
    {
        Panel.gameObject.SetActive(!Panel.gameObject.activeSelf);
        ((GameSceneController)GameSceneController.instance).SetMenuActive(Panel.gameObject.activeSelf);
    }

    private void SelectSticker(int id)
    {
        Debug.Log($"Select {id}");
        ((GameSceneController)GameSceneController.instance).localDecalController.SetStickerID(id);
        SetMenuActive(!Panel.gameObject.activeSelf);
    }
}
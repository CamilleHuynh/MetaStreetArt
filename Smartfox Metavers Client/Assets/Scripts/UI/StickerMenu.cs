using System;
using UnityEngine;
using UnityEngine.UI;

public class StickerMenu : MonoBehaviour
{
    [SerializeField] private Transform Panel;
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject PrefabUISticker;
    [SerializeField] private Stickers soStickers;

    
    void Start()
    {
        Panel.gameObject.SetActive(false);

        foreach (var sticker in soStickers.stickerList)
        {
            var ui = Instantiate(PrefabUISticker, Content.transform).transform.GetChild(0);
            ui.GetComponent<UnityEngine.UI.Image>().overrideSprite = 
                Sprite.Create((Texture2D)sticker.Image, new Rect(0, 0, sticker.Image.width, sticker.Image.height), 
                new Vector2(0.5f, 0.5f));
            ui.GetComponent<Button>().onClick.AddListener(() => { SelectSticker(sticker.id); });
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) // RightClick
        {
            Panel.gameObject.SetActive(!Panel.gameObject.activeSelf);
        }
    }

    void SelectSticker(int id)
    {
        Debug.Log($"Select {id}");
        Panel.gameObject.SetActive(false);
    }
}

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.Events;
#endif

public class StickerMenu : MonoBehaviour
{
    [SerializeField] private Transform Panel;
    [SerializeField] private Transform Content;
    [SerializeField] private GameObject PrefabUISticker;

    private void Start()
    {
        Panel.gameObject.SetActive(false);
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


#if UNITY_EDITOR
    [SerializeField] private Stickers soStickers;

    [ContextMenu("Update stickers UI")]
    public void UpdateUISticker()
    {
        foreach (var sticker in soStickers.stickerList)
        {
            var ui = Instantiate(PrefabUISticker, Content.transform).transform.GetChild(0);
            ui.GetComponent<Image>().sprite = sticker.Image;

            var btn = ui.GetComponent<Button>();
            UnityAction<int> action = SelectSticker;
            UnityEventTools.AddIntPersistentListener(btn.onClick, action, sticker.id);
        }

        EditorUtility.SetDirty(this);
    }
#endif
}
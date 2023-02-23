using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "StickerDecals", menuName = "ScriptableObjects/Decals/StickerDecalList")]
public class Stickers : ScriptableObject
{
    public Material baseMaterial;
    public DecalProjector decalProjectorPrefab;
    public List<Sticker> stickerList;

#if UNITY_EDITOR
    [ContextMenu("Populate images")]
    public void PopulatesImagesStickers()
    {
        stickerList.Clear();
        var guids = AssetDatabase.FindAssets("t:Texture", new[] { "Assets/Images/Stickers" });

        var i = 0;
        foreach (var guid in guids)
        {
            // Search images
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var t = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            var item = new Sticker();
            item.Image = t;
            item.name = path.Remove(0, "Assets/Images/Stickers/".Length).Replace('\\', '_').Replace(' ', '_')
                .Replace('/', '_').Replace(".png", "");

            //Add list
            stickerList.Add(item);

            // Id
            item.id = i;

            // Material asset
            var material = new Material(Shader.Find("HDRP/Decal"));
            AssetDatabase.CreateAsset(material, "Assets/Materials/DecalMat/sticker_mat/" + item.name + ".mat");
            item.mat = material;
            item.mat.SetTexture("_BaseColorMap", item.Image.texture);
            EditorUtility.SetDirty(material);

            i++;
        }

        EditorUtility.SetDirty(this);
    }
#endif

    [Serializable]
    public class Sticker
    {
        [HideInInspector] public string name; // For visualize
        public Sprite Image;
        public int id;
        public Material mat;
    }
}
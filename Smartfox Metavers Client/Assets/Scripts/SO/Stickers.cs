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
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var t = AssetDatabase.LoadAssetAtPath<Texture>(path);
            var item = new Sticker();
            item.Image = t;
            item.name = path.Remove(0, "Assets/Images/Stickers/".Length).Replace('\\', '_').Replace(' ', '_')
                .Replace('/', '_').Replace(".png", "");
            stickerList.Add(item);
        }

        EditorUtility.SetDirty(this);

        foreach (var s in stickerList)
            if (s.mat == null)
            {
                var material = new Material(Shader.Find("HDRP/Decal"));
                AssetDatabase.CreateAsset(material, "Assets/Materials/DecalMat/sticker_mat/" + s.name + ".mat");
                s.mat = material;
                EditorUtility.SetDirty(material);
            }
    }
#endif

    [Serializable]
    public class Sticker
    {
        [HideInInspector] public string name; // For visualize
        public Texture Image;
        public int id;
        public Material mat;
    }
}
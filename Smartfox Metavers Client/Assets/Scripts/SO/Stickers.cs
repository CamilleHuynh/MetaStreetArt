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

            item.mat.EnableKeyword("_BaseColorMap");
            item.mat.SetTexture("_BaseColorMap", item.Image.texture);

            item.mat.EnableKeyword("_DecalBlend");
            item.mat.SetFloat("_DecalBlend", 1.0f);

            item.mat.EnableKeyword("_Smoothness");
            item.mat.SetFloat("_Smoothness", 0f);

            // EditorUtility.SetDirty(material);

            // Preview Material asset
            var previewMaterial = new Material(Shader.Find("HDRP/Decal"));
            // AssetDatabase.CreateAsset(previewMaterial, "Assets/Materials/DecalMat/sticker_preview_mat/" + item.name + "_preview.mat");
            // item.previewMat = previewMaterial;

            // item.previewMat.EnableKeyword("_BaseColorMap");
            // item.previewMat.SetTexture("_BaseColorMap", item.Image.texture);

            // item.previewMat.EnableKeyword("_DecalBlend");
            // item.previewMat.SetFloat("_DecalBlend", 0.5f);

            // item.previewMat.EnableKeyword("_Smoothness");
            // item.previewMat.SetFloat("_Smoothness", 0f);

            previewMaterial.EnableKeyword("_BaseColorMap");
            previewMaterial.SetTexture("_BaseColorMap", item.Image.texture);

            previewMaterial.EnableKeyword("_DecalBlend");
            previewMaterial.SetFloat("_DecalBlend", 0.5f);

            previewMaterial.EnableKeyword("_Smoothness");
            previewMaterial.SetFloat("_Smoothness", 0f);

            AssetDatabase.CreateAsset(previewMaterial, "Assets/Materials/DecalMat/sticker_preview_mat/" + item.name + "_preview.mat");
            item.previewMat = previewMaterial;

            Debug.Log("Changed mat: " + i + ", _Smoothness: " + item.previewMat.GetFloat("_Smoothness"));

            EditorUtility.SetDirty(previewMaterial);

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
        public Material previewMat;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "StickerDecals", menuName = "ScriptableObjects/Decals/StickerDecalList")]
public class Stickers : ScriptableObject
{
    [Serializable]
    public class Sticker {
        public Texture Image;
        public int id;
        public Material mat;
    }

    public Material baseMaterial;
    public DecalProjector decalProjectorPrefab;
    public List<Sticker> stickerList;
    
    #if UNITY_EDITOR
    
    [ContextMenu("Spawn materials")]
    public void SpawnMaterialsInFolder()
    {
        foreach(Sticker s in stickerList)
        {
            if (s.mat == null)
            {
                Material material = new Material (Shader.Find("HDRP/Decal"));
                AssetDatabase.CreateAsset(material, "Assets/Materials/DecalMat/sticker_mat/"+s.Image.name+".mat");
                s.mat = material;
                EditorUtility.SetDirty(this);
            }
        }
    }
    #endif
}

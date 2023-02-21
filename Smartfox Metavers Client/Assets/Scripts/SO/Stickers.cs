using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

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
}

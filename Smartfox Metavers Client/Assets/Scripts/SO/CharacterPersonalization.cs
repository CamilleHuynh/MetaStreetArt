using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterPersonalization", menuName = "ScriptableObjects/CharacterPersonalization")]
public class CharacterPersonalization : ScriptableObject
{
    [SerializeField] public List<Material> characterMaterial;
    [SerializeField] public List<GameObject> characterItems;
}

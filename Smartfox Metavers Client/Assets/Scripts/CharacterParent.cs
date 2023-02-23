using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterParent : MonoBehaviour
{
    [SerializeField] private Renderer capsuleRenderer;

    public void SetMaterial(Material mat)
    {
        capsuleRenderer.material = mat;
    }
}

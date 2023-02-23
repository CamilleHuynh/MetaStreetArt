using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAppearance : MonoBehaviour
{
    [SerializeField] private CharacterPersonalization characterSO;
    [SerializeField] private CharacterParent characterPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            // SpawnCharacter(new Vector3(0, 0, 0), Quaternion.identity, 1, 0);
        }
    }

    public GameObject SpawnCharacter(int materialID, int itemID, GameObject parent)
    {
        CharacterParent currentPlayer = Instantiate(characterPrefab, parent.transform, false);
        currentPlayer.SetMaterial(characterSO.characterMaterial[materialID]);

        Instantiate(characterSO.characterItems[itemID], currentPlayer.transform, false);

        return currentPlayer.gameObject;
    }

    public GameObject SpawnWorldCharacter(int materialID, int itemID)
    {
        CharacterParent currentPlayer = Instantiate(characterPrefab);
        currentPlayer.SetMaterial(characterSO.characterMaterial[materialID]);

        Instantiate(characterSO.characterItems[itemID], currentPlayer.transform, false);

        return currentPlayer.gameObject;
    }
}

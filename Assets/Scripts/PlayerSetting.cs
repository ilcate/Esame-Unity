using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;

public class PlayerSetting : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    //[SerializeField] private NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField]
    Texture2D rougeTexture;
    [SerializeField]
    Texture2D mageTexture;
    [SerializeField]
    Texture2D barbarianTexture;
    [SerializeField]
    Texture2D knightTexture;
    




    [SerializeField]
    GameObject hat;

    public override void OnNetworkSpawn()
    {
        //Debug.Log("On network spawn");
        //Debug.Log(OwnerClientId);
        //Debug.Log(IsOwner);

        //if (IsOwner)
        //{
        //    networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        //}

        //playerName.text = networkPlayerName.Value.ToString();


        Renderer hatRenderer = hat.GetComponent<Renderer>();

        switch (OwnerClientId)
        {
            case 0:
                hatRenderer.material.mainTexture = mageTexture;
                break;
            case 1:
                hatRenderer.material.mainTexture = rougeTexture;
                break;
            case 2:
                hatRenderer.material.mainTexture = barbarianTexture;
                break;
            case 3:
                hatRenderer.material.mainTexture = knightTexture;
                break;
            default:
                hatRenderer.material.mainTexture = mageTexture; 
                break;
        }
    }

    
}

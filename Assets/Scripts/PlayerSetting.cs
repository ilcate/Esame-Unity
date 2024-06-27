using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using System.Data;

public class PlayerSetting : NetworkBehaviour
{
    public TextMeshProUGUI playerName;
    public NetworkVariable<FixedString128Bytes> networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Texture2D rougeTexture;
    public Texture2D mageTexture;
    public Texture2D barbarianTexture;
    public Texture2D knightTexture;
    public GameObject hat;

    public override void OnNetworkSpawn()
    {
        networkPlayerName.Value = "Player: " + (OwnerClientId + 1);
        playerName.text = networkPlayerName.Value.ToString();

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

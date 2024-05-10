using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using TMPro;

public class PlayerInfo : NetworkBehaviour
{
    private static string playerNamePass;
    public string PName;

    public static PlayerInfo Instance { get; private set; }

    static public void AssignName(string name)
    {
        playerNamePass = name;
    }


    private void Update()
    {
        if (playerNamePass != PName)
        {
            PName = playerNamePass;
        }
        
    }
}

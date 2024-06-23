using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkVariable<bool> inGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone);

    public int randomValue;

    private void Awake()
    {

        randomValue = Random.Range(-10, 10);
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
    }

    public void StartGame()
    {
        inGame.Value = true;
        Debug.Log("Game started!");   
    }
}

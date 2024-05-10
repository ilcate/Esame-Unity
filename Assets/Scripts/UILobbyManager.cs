using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField UsernameInput;

    [SerializeField]
    private Button LogIn;


    private void Awake()
    {
        Cursor.visible = true;
    }

    void Start()
    {
        LogIn?.onClick.AddListener(() =>
        {
            if (!string.IsNullOrEmpty(UsernameInput.text))
            {
                Debug.Log(UsernameInput.text);
                LobbyManager.Instance.Authenticate(EditPlayerName.Instance.GetPlayerName());
            }
            else
            {
                Debug.Log("NULLA");
            }
        });
    }
}


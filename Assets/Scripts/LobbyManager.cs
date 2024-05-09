/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;

public class LobbyManager : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private string playerName;


    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = "player" + UnityEngine.Random.Range(10, 99);

    }

    private void update()
    {
        HandleLobbyHeartbeat();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;

            if(heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }


    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "Test Nome";
            int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = true,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, "mappaUno" ) }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions); //l'utimo parametro è l'otp per entrare
            //Lobby è la classe che ha anche i nomi dei player e dal dictionary gli puoi includere diverse cose(come la skin)

            hostLobby = lobby;
            joinedLobby = hostLobby; //36:16 Minutaggio

            PrintPlayers(hostLobby);

            Debug.Log("Created lobby" + lobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }


        

    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter> //qui sotto crei tutti i filtri di ricerca e se li voglio personalizzati vanno messi qui sort  
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0" , QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log(queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.Name);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyByCode(string lobbyCode) //usala per unirti con il codice
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;

        }catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private async void QuickJoinLobby() //usala per unirti senza codice alla prima lobby libera(credo)
    {
        try
        {
            await LobbyService.Instance.QuickJoinLobbyAsync();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }


    private void PrintPlayer()
    {
        PrintPlayers(joinedLobby);
    }

    private void PrintPlayers (Lobby lobby)
    {
        foreach(Player player in lobby.Players)
        {
            Debug.Log(player.Data["PlayerName"].Value);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                     {
                         {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
                     }
        };
    }

    private async void UpdateLobbyInformation(string map)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject>
                {
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, map) }
                }
            });

            joinedLobby = hostLobby;
        }catch
        {

        }
    }


}
*/
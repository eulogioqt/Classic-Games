using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PARCHIS;

public class TCPClient : MonoBehaviour {
    public GameObject parchisMenuGameObject;

    public Button leaveButton;

    private TcpClient client;
    private IPEndPoint server;
    private User user;

    private NetworkStream stream;

    public TableroColor redColor;
    public TableroColor greenColor;
    public TableroColor yellowColor;
    public TableroColor blueColor;

    public Dictionary<TeamColor, TableroColor> tableroColor;

    private Dictionary<string, Player> users;
    private Player player = null;
    // no bool de isReal si no un script que sea playercontroller y se le asigne este player
    private static TCPClient instance;

    private void Start() {
        tableroColor = new Dictionary<TeamColor, TableroColor>() {
            { TeamColor.RED, redColor },
            { TeamColor.GREEN, greenColor },
            { TeamColor.YELLOW, yellowColor },
            { TeamColor.BLUE, blueColor },
        };

        parchisMenuGameObject.SetActive(false);

        leaveButton.onClick.AddListener(onDisconnect);
    }

    private void Awake() {
        if(instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    public void onConnect(TcpClient client, IPEndPoint server, User user) {
        this.client = client;
        this.server = server;
        this.user = user;

        this.stream = client.GetStream();

        parchisMenuGameObject.SetActive(true);

        StartCoroutine(getResponse());
    }

    public void onDisconnect() {
        client.Close();

        parchisMenuGameObject.SetActive(false);

        ConnectionController.getInstance().tryConnectingLobby(server.Address, server.Port - 1, user);
    }

    private IEnumerator getResponse() {
        while (client.Client != null) {
            try {
                if (client.Available > 0) {
                    byte[] data = new byte[256];
                    string responseData = Encoding.ASCII.GetString(data, 0, stream.Read(data, 0, data.Length));

                    COMMAND cmd = new COMMAND(new string(responseData.Substring(0, responseData.Length - 2)));
                    processCommand(cmd);
                }
            } catch (Exception e) {
                Debug.Log(e.Message);
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }

    public void processCommand(COMMAND cmd) {
        Debug.Log(cmd.getCommand());
        if (cmd.getType() == CommandType.CHAT) {
            CHAT msg = CHAT.process(cmd.getCommand());

            ParchisChat.getInstance().addMessage(users[msg.getKey()] + ": " + msg.getMessage());
        } else if (cmd.getType() == CommandType.INFO) {
            INFO msg = INFO.process(cmd.getCommand());

            player = new GameObject(user.getData().getName(), typeof(Player)).GetComponent<Player>();
            player.initPlayer(user.getData().getName(), msg.getColor());

            users = msg.getUsers();
            IPEndPoint p = (IPEndPoint) client.Client.LocalEndPoint;
            users.Add("/" + p.Address + ":" + p.Port, player);

            foreach (Player pl in users.Values)
                tableroColor[pl.getColor()].setPlayer(pl);
        } else if (cmd.getType() == CommandType.ON) {
            ON msg = ON.process(cmd.getCommand());

            Player newPlayer = new GameObject(user.getData().getName(), typeof(Player)).GetComponent<Player>();
            newPlayer.initPlayer(user.getData().getName(), msg.getColor());
            
            tableroColor[newPlayer.getColor()].setPlayer(newPlayer);
            
            users.Add(msg.getKey(), newPlayer);
        } else if (cmd.getType() == CommandType.OFF) {
            OFF msg = OFF.process(cmd.getCommand());

            Destroy(users[msg.getKey()].gameObject);
            users.Remove(msg.getKey());
        } else if (cmd.getType() == CommandType.UNKNOWN) {
            Debug.Log("Algo salio mal: " + cmd.getCommand());
        }
    }

    private void send(string message) {
        byte[] data = Encoding.ASCII.GetBytes(user.getData().getName() + ": " + message + "\n");
        stream.Write(data, 0, data.Length);
        stream.Flush();
    }

    public void sendCHAT(string message) {
        send(CHAT.getMessage(message));
    }

    private void OnApplicationQuit() {
        if (client != null && client.Client != null && client.Client.Connected)
            client.Close();
    }

    public static TCPClient getInstance() {
        return instance;
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;

public class UDPTest : MonoBehaviour {

    // Started by @eulogioqt on 13/08/2023

    // TO-DO
    // Receive on other thread:
    // https://stackoverflow.com/questions/53731293/sending-udp-calls-in-unity-on-android

    public Text chatText;
    public Button sendButton;
    public InputField sendText;

    public GameObject chatMenuGameObject;
    public Button closeChatButton;
    public Button openChatButton;

    public Text onlineText;
    private int onlineUsers = 0;

    private Dictionary<string, Player> users;
    private Player player;

    public Button exitButton;
    // PORQUE SI MANDO EN BROADCST NO ME PUEDEN RESPONDER WTF ESO NO LO ENTIENDE NI DON GABRIEL LUQUE
    private UdpClient client;
    private IPEndPoint server;
    //on application quit and why cant answer to broadcast udp
    // numero de gente online

    private static UDPTest instance;

    private void Awake() {
        if(instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    void Start() {
        chatMenuGameObject.SetActive(false);

        users = new Dictionary<string, Player>();

        client = new UdpClient();
        client.EnableBroadcast = true;

        server = new IPEndPoint(IPAddress.Parse("192.168.1.19"), 11000);

        player = new GameObject("localhost", typeof(Player)).GetComponent<Player>();
        player.initPlayer();

        try {
            client.Connect(server);

            byte[] sendBytes = Encoding.ASCII.GetBytes("HOLA");
            client.Send(sendBytes, sendBytes.Length);
        } catch (Exception e) {
            chatText.text += "<color=red>" + e.Message + "</color>\n";
        }

        StartCoroutine(getResponse());

        sendButton.onClick.AddListener(delegate { onSend(sendText.text); sendText.text = ""; });
        openChatButton.onClick.AddListener(delegate { chatMenuGameObject.SetActive(true); });
        closeChatButton.onClick.AddListener(delegate { chatMenuGameObject.SetActive(false); });
        exitButton.onClick.AddListener(delegate { Application.Quit(); });
    }

    private void FixedUpdate() {
        Vector3 position = player.transform.localPosition;
        if (Input.GetKey(KeyCode.W)) {
            position.y += 5;
        }
        if (Input.GetKey(KeyCode.D)) {
            position.x += 5;
        }
        if (Input.GetKey(KeyCode.S)) {
            position.y -= 5;
        }
        if (Input.GetKey(KeyCode.A)) {
            position.x -= 5;
        }
        if (position != player.transform.localPosition) {
            player.transform.localPosition = position;
            onMove(position.x + ";" + position.y);
        }
    }

    private void onSend(string text) {
        chatText.text += text + "\n";

        byte[] sendBytes = Encoding.ASCII.GetBytes("CHAT" + text);

        client.Send(sendBytes, sendBytes.Length);
    }

    public void onMove(string position) {
        byte[] sendBytes = Encoding.ASCII.GetBytes("GAME" + position);

        client.Send(sendBytes, sendBytes.Length);
    }

    private IEnumerator getResponse() {
        while (true) {
            if(client.Available > 0) {
                byte[] receiveBytes = client.Receive(ref server);
                string text = Encoding.ASCII.GetString(receiveBytes);

                if (text.StartsWith("CHAT")) {
                    string[] data = text.Substring(4).Split('@');
                    chatText.text += data[1] + " => " + data[0];
                } else if(text.StartsWith("GAME")) { // FORMAT: GAMEx;y@key
                    string[] data = text.Substring(4).Split('@');
                    string[] pos = data[0].Split(';');
                    users[data[1]].updatePosition(new Vector2(int.Parse(pos[0]), int.Parse(pos[1])));
                } else if (text.StartsWith("ON")) {
                    string info = text.Substring(2);
                    onlineUsers++;
                    
                    Player newPlayer = new GameObject(info, typeof(Player)).GetComponent<Player>();
                    newPlayer.initPlayer();
                    users.Add(info, newPlayer);

                    updateInfo();
                } else if (text.StartsWith("OFF")) {
                    string key = text.Substring(3);
                    onlineUsers--;

                    Destroy(users[key].gameObject);
                    users.Remove(key);

                    updateInfo();
                } else if (text.StartsWith("INFO")) {
                    string info = text.Substring(4);
                    string[] uss = info.Split('@');

                    foreach(string user in uss) {
                        string[] data = user.Split(';');

                        Player newPlayer = new GameObject(info, typeof(Player)).GetComponent<Player>();
                        newPlayer.initPlayer(new Vector2(int.Parse(data[1]), int.Parse(data[2])));
                        users.Add(data[0], newPlayer);
                    }

                    onlineUsers = uss.Length + 1;

                    updateInfo();
                }
            }
            yield return new WaitForSeconds(0);
        }
    }

    private void updateInfo() {
        onlineText.text = "Online: " + onlineUsers;
    }

    private void OnApplicationQuit() {
        byte[] sendBytes = Encoding.ASCII.GetBytes("ADIOS");
        client.Send(sendBytes, sendBytes.Length);

        client.Dispose();
    }

    public static UDPTest getInstance() {
        return instance;
    }
}

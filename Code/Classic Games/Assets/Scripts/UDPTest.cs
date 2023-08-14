using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System;

public class UDPTest : MonoBehaviour {

    // Started by @eulogioqt on 13/08/2023

    // TO-DO
    // Receive on other thread:
    // https://stackoverflow.com/questions/53731293/sending-udp-calls-in-unity-on-android

    public Text chatText;
    public Button sendButton;
    public InputField sendText;

    public int serverPort = 11000;

    private UdpClient client;
    private IPEndPoint server;

    void Start() {
        client = new UdpClient();
        client.EnableBroadcast = true;

        server = new IPEndPoint(IPAddress.Parse("192.168.1.19"), serverPort);

        client.Connect(server);

        try {
            byte[] sendBytes = Encoding.ASCII.GetBytes("HOLA");
            client.Send(sendBytes, sendBytes.Length);
        } catch (Exception e) {
            chatText.text += "<color=red>" + e.Message + "</color>\n";
        }

        StartCoroutine(getResponse());

        sendButton.onClick.AddListener(delegate { onSend(sendText.text); sendText.text = ""; });
    }

    private void onSend(string text) {
        chatText.text += text + "\n";

        byte[] sendBytes = Encoding.ASCII.GetBytes(text);

        client.Send(sendBytes, sendBytes.Length);
    }

    private IEnumerator getResponse() {
        while (true) {
            if(client.Available > 0) {
                byte[] receiveBytes = client.Receive(ref server);
                chatText.text += Encoding.ASCII.GetString(receiveBytes) + "\n";
            }
            yield return new WaitForSeconds(0);
        }
    }

    void OnApplicationQuit() {
        byte[] sendBytes = Encoding.ASCII.GetBytes("ADIOS");
        client.Send(sendBytes, sendBytes.Length);

        client.Dispose();
    }
}

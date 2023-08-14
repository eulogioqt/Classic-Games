using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System;
using System.Threading.Tasks;

public class UDPTest : MonoBehaviour {

    public Text chatText;
    public Button sendButton;
    public InputField sendText;

    public int serverPort = 11000;

    private UdpClient client;
    private IPEndPoint server;

    void Start() {
        client = new UdpClient();
        server = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);

        client.Connect(server);

        _ = getResponse();

        sendButton.onClick.AddListener(delegate { onSend(sendText.text); });
    }

    private void onSend(string text) {
        chatText.text += text + "\n";

        byte[] sendBytes = Encoding.ASCII.GetBytes(text);

        client.Send(sendBytes, sendBytes.Length);
    }

    private async Task getResponse() {
        while (true) {
            byte[] receiveBytes = client.Receive(ref server);
            chatText.text += Encoding.ASCII.GetString(receiveBytes) + "\n";
        }
    }
}

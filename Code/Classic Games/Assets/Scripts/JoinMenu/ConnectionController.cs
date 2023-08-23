using System.Net;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net.Sockets;

public class ConnectionController : MonoBehaviour {
    public GameObject connectingMenuGameObject;
    public Text notificationText;
    public Button closeButton;

    private static ConnectionController instance;

    private void Awake() {
        if(instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        connectingMenuGameObject.SetActive(false);
    }

    public void setDisconnected(string message, bool refreshOnClose) {
        closeButton.GetComponent<RectTransform>().localPosition = new Vector3(0, -100, 0);
        closeButton.gameObject.SetActive(true);

        notificationText.GetComponent<RectTransform>().localPosition = new Vector3(0, 100, 0);
        notificationText.text = message;

        closeButton.onClick.AddListener(delegate {
            JoinMenuController.getInstance().setMenuActive(true);
            if (refreshOnClose) JoinMenuController.getInstance().refreshList();
            connectingMenuGameObject.SetActive(false);
        });

        connectingMenuGameObject.SetActive(true);
    }

    public void tryConnecting(IPAddress IP, int port, User user) {
        closeButton.gameObject.SetActive(false);

        notificationText.GetComponent<RectTransform>().localPosition = Vector3.zero;
        notificationText.text = "Conectando con el servidor...";

        PlayerPrefs.SetString("Name", user.getData().getName());
        StartCoroutine(tryConnection(IP, port, user));

        connectingMenuGameObject.SetActive(true);
    }

    private IEnumerator tryConnection(IPAddress IP, int port, User user) {
        IPEndPoint server = new IPEndPoint(IP, port);
        UdpClient client = new UdpClient();

        string state = "";
        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        try {
            client.Connect(server);

            byte[] sendBytes = Encoding.ASCII.GetBytes(HOLA.getMessage(user));
            client.Send(sendBytes, sendBytes.Length);
        } catch (Exception) {
            state = "Host desconocido. Revisa que la direccion IP sea la correcta.";
        }

        while (state.Length <= 0 && DateTimeOffset.UtcNow.ToUnixTimeSeconds() - time < 5) {
            try {
                if (client.Available > 0) {
                    COMMAND cmd = new COMMAND(client.Receive(ref server));
                    if (cmd.getType() == CommandType.INFO) {
                        notificationText.text = "Entrando al servidor..";
                        state = "OK";

                        UDPClient.getInstance().onConnect(cmd, server, client, user);

                        connectingMenuGameObject.SetActive(false);
                        JoinMenuController.getInstance().setMenuActive(false);
                    }
                }
            } catch (Exception) { }

            yield return new WaitForSeconds(1);
        }

        if (state.Length <= 0)
            state = "No se pudo conectar con el servidor";

        if (!state.Equals("OK")) {
            setDisconnected(state, false);
            client.Close();
        }
    }

    public static ConnectionController getInstance() {
        return instance;
    }
}

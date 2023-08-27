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

        JoinMenuController.getInstance().setMenuActive(true);
        connectingMenuGameObject.SetActive(true);
    }

    private void setConnecting(string message) {
        closeButton.gameObject.SetActive(false);

        notificationText.GetComponent<RectTransform>().localPosition = Vector3.zero;
        notificationText.text = message;

        JoinMenuController.getInstance().setMenuActive(true);
        connectingMenuGameObject.SetActive(true);
    }

    public void tryConnectingLobby(IPAddress IP, int port, User user) {
        if (user.getData().getName().Length > 0 && !user.getData().getName().Contains(";")) {
            setConnecting("Conectando con el servidor...");

            PlayerPrefs.SetString("Name", user.getData().getName());
            StartCoroutine(tryConnectionLobby(IP, port, user));
        } else
            setDisconnected("Nombre de usuario no permitido", false);
    }

    private IEnumerator tryConnectionLobby(IPAddress IP, int port, User user) {
        IPEndPoint server = new IPEndPoint(IP, port);
        UdpClient client = new UdpClient();

        string state = "";
        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        try {
            client.Connect(server);

            byte[] sendBytes = Encoding.ASCII.GetBytes(PING.getMessage());
            client.Send(sendBytes, sendBytes.Length);
        } catch (Exception) {
            state = "Host desconocido. Revisa que la direccion IP sea la correcta.";
        }

        while (state.Length <= 0 && DateTimeOffset.UtcNow.ToUnixTimeSeconds() - time < 5) {
            try {
                if (client.Available > 0) {
                    COMMAND cmd = new COMMAND(Encoding.ASCII.GetString(client.Receive(ref server)));
                    if (cmd.getType() == CommandType.PING) {
                        PING msg = PING.process(cmd.getCommand());

                        if (msg.getVersion().Equals(Application.version)) {
                            byte[] sendBytes = Encoding.ASCII.GetBytes(HOLA.getMessage(user));
                            client.Send(sendBytes, sendBytes.Length);

                            notificationText.text = "Entrando al servidor...";
                        } else
                            state = "Tu version del juego (" + Application.version + ") no coincide" +
                                "con la version del servidor (" + msg.getVersion() + ")";
                    } else if (cmd.getType() == CommandType.INFO) {
                        state = "OK";

                        UDPClient.getInstance().onConnect(server, client, user);
                        UDPClient.getInstance().processCommand(cmd);

                        connectingMenuGameObject.SetActive(false);
                        JoinMenuController.getInstance().setMenuActive(false);
                    }
                }
            } catch (Exception e) { Debug.Log(e.Message); time = 0; }

            yield return new WaitForSeconds(1);
        }

        if (state.Length <= 0)
            state = "No se pudo conectar con el servidor";

        if (!state.Equals("OK")) {
            setDisconnected(state, false);
            client.Close();
        }
    }

    public void tryConnectingParchis(IPAddress IP, int port, User user) {
        LobbyChat.getInstance().addMessage("&5&l>> &dConectando con el servidor de parchis...");

        setConnecting("Conectando con el servidor...");

        StartCoroutine(tryConnectionParchis(IP, port, user));
    }

    private IEnumerator tryConnectionParchis(IPAddress IP, int port, User user) {
        IPEndPoint server = new IPEndPoint(IP, port);
        TcpClient client = new TcpClient();
        NetworkStream stream = null;

        string state = "";
        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        try {
            client.Connect(server);
            stream = client.GetStream();
        } catch (Exception) {
            state = "No se reconoce el host asignado al servidor de parchis";
        }

        while (state.Length <= 0 && DateTimeOffset.UtcNow.ToUnixTimeSeconds() - time < 5) {
            try {
                if(client.Available > 0) {
                    byte[] data = new byte[256];
                    string responseData = Encoding.ASCII.GetString(data, 0, stream.Read(data, 0, data.Length));
                    COMMAND cmd = new COMMAND(new string(responseData.Substring(0, responseData.Length - 2)));

                    if (cmd.getType() == CommandType.INFO) {
                        state = "OK";

                        UDPClient.getInstance().onDisconnect();
                        TCPClient.getInstance().onConnect(client, server, user);
                        TCPClient.getInstance().processCommand(cmd);

                        connectingMenuGameObject.SetActive(false);
                        JoinMenuController.getInstance().setMenuActive(false);
                    } else if (cmd.getType() == CommandType.DISCONNECT) {
                        DISCONNECT msg = DISCONNECT.process(cmd.getCommand());

                        state = msg.getDisconnectMessage();
                    }
                }
            } catch (Exception e) { Debug.Log(e.Message); time = 0; }
            yield return new WaitForSeconds(1);
        }

        if (state.Length <= 0)
            state = "No se pudo establecer conexion con el servidor de parchis";

        if (state != "OK") {
            LobbyChat.getInstance().addMessage("&4&l>> &c" + state);

            UDPClient.getInstance().lobbyMenuGameObject.SetActive(true);
            connectingMenuGameObject.SetActive(false);
            JoinMenuController.getInstance().setMenuActive(false);
        }
    }

    public static ConnectionController getInstance() {
        return instance;
    }
}

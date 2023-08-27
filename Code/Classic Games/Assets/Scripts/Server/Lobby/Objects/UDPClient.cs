using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;
using LOBBY;

public class UDPClient : MonoBehaviour {
    // Started by @eulogioqt on 13/08/2023

    // TO-DO
    // Receive on other thread:
    // https://stackoverflow.com/questions/53731293/sending-udp-calls-in-unity-on-android

    private Dictionary<string, Player> users;
    private Player player = null;

    public GameObject lobbyMenuGameObject;
    public Text positionText;

    public Text onlineText;
    private int onlineUsers;
    private string myName;

    public Button disconnectButton;
    public Button parchisButton;

    public SmartButton upButton;
    public SmartButton rightButton;
    public SmartButton downButton;
    public SmartButton leftButton;

    private IPEndPoint server;
    private UdpClient client;
    private User user;
    //on application quit and why cant answer to broadcast udp
    // numero de gente online
    // PORQUE SI MANDO EN BROADCST NO ME PUEDEN RESPONDER WTF ESO NO LO ENTIENDE NI DON GABRIEL LUQUE

    public static int FPU = 10; // Frames Per Update
    private long timeout = long.MaxValue;

    private static UDPClient instance;
    // ESTRUCTURAR BIEN EL CODIGO Y HACER QUE PUEDAS PERSONALIZAR
    // TU CUADRADO CON COLORES SKINS Y DEMAS
    // Y TENER TU PROPIO NOMBRE Y QUE ESTO SE QUEDE COMO EL LOBBY
    // PARA LOS OTROS JUEGOS

    // CONECTARSE Y DESCONECTARSE PREGUNTANDO SI SIGUE AHI CON EL SERVIDOR

    // join and left messages mejorado con distintos tipos d mensaje

    /*
     * en vez de enviar cada movimiento q haces, cada 0.05s o cosa asi, despues 
     * cuando el servidor envia a todos la actualizacion de movimiento, que haga
     * un thread para cada uno y no un for enviando de uno en uno, despues que 
     * si 5 jugadores envian al server su nueva posicion, que el server en vez 
     * de enviar a cada uno de los otros 5 datagramas con la posicion de cada 
     * uno, envie uno solo con las 5 actualizaciones
     * 
    */

    /*
     * lo de version que tengo en whatsapp
    un comando ping para el servidor que te de info nombre del server
    los usuarios conectados y calcule el tiempo de respuesta

    VERSION
    MOTD
    CHAT EN EL SCRIPT CHAT

    servidor configurable
    el MOTD en un archivo y cosas en los archivos tmb la foto del server y el chat
    y el mundo del server que se cree en el server tmb
    y que la camara y el mapa sea mas grande

    pantalla dei nicio donde configurar el jugador 
    pantalla de inicio donde acceder a la lista de servers

    hacer sistema de cuentas
    hacer el /login
    entonces ya te puedes personalizar el personaje dentro del server*/

    // hacer funciones para cada tipo de mensaje
    // refactorizar - version 8
    // decorar un poco todo - version 8

    // version 8 - que todo se ejecute en un thread en el sv y que el main pueda escribir y poner comandos
    // y que se cierre el servefr con un comando que desconecte a los demas y to eso y se guasrde el chat

    // añadir seguridad controlar excepciones si se desconecta alguien y manda un mensaje se crashea el server

    // controlar duplicados, perdida de paquetes, el chat se duplica, si el timeout se pierde estamos jodidos
    // el hola si no llega tambien es jodienda

    // version 9000 cuentas

    // interfacear los protocol commands

    // AÑADIR SEGURIDAD PARA CONECTARSE Y DESCONECTARSE ASEGURARSE DE QUE LLEGUE EL MENSAJE
    // RESPONDIENDO Y DEMAS

    private void Awake() {
        if (instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        lobbyMenuGameObject.SetActive(false);

        loadButtons();
    }

    private void loadButtons() {
        disconnectButton.onClick.AddListener(onLeave);
        parchisButton.onClick.AddListener(joinParchis);
    }

    public void onConnect(IPEndPoint server, UdpClient client, User user) {
        this.server = server;
        this.client = client;
        this.user = user;

        lobbyMenuGameObject.SetActive(true);

        timeout = long.MaxValue;
        LobbyChat.getInstance().resetChat();

        player = new GameObject(user.getData().getName()).AddComponent<Player>();
        player.initPlayer(user.getData().getName(), true);

        StartCoroutine(getResponse());
    }

    public void onDisconnect() {
        try {
            byte[] sendBytes = Encoding.ASCII.GetBytes(ADIOS.getMessage());
            client.Send(sendBytes, sendBytes.Length);

            client.Close();
            client = new UdpClient();
        } catch (Exception) { }

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        for (int i = 0; i < field.transform.childCount; i++)
            Destroy(field.transform.GetChild(i).gameObject);

        lobbyMenuGameObject.SetActive(false);
    }

    private void onLeave() {
        onDisconnect();
        JoinMenuController.getInstance().setMenuActive(true);
        JoinMenuController.getInstance().refreshList();
    }

    private void joinParchis() {
        ConnectionController.getInstance().tryConnectingParchis(server.Address, server.Port + 1, user);
    }

    private void disconnectUser(string message) {
        onDisconnect();
        ConnectionController.getInstance().setDisconnected(message, true);
    }

    private IEnumerator timeoutOnSeconds(int n) {
        yield return new WaitForSeconds(n);

        if (timeout == long.MaxValue)
            disconnectUser("El servidor no responde");
    }


    private void Update() {
        if (Input.GetKeyDown(KeyCode.F11))
            toggleFullScreen();

        if (player != null && DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timeout >= 30) {
            timeout = long.MaxValue;
            StartCoroutine(timeoutOnSeconds(10));

            send(STATUS.getMessage());
        }
    }

    private int waitFrames;
    private Vector3 lastSentPosition;
    private void FixedUpdate() {
        waitFrames++;

        if (player != null && !LobbyChat.getInstance().isChatOpen()) {
            Vector3 position = player.transform.localPosition;
            Vector3 size = player.GetComponent<RectTransform>().sizeDelta;
            positionText.text = position.x + ", " + position.y;

            int move = 5 * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1);

            int x = 0;
            int y = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || upButton.isPressed) {
                upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.y + move <= 1080 / 2 - size.y) y += move;
            } else upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || rightButton.isPressed) {
                rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x + move <= 1920 / 2 - size.x) x += move;
            } else rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || downButton.isPressed) {
                downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.y - move >= -1080 / 2 + size.y) y -= move;
            } else downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || leftButton.isPressed) {
                leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x - move >= -1920 / 2 + size.x) x -= move;
            } else leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (x != 0 || y != 0) {// Actualizar localmente fluidamente
                position.x += x;
                position.y += y;
                player.transform.localPosition = position;
            }

            // Para que no pase que se actualice la posicion localmente, pero no se envie ya que hay que esperar
            // cada FPU frames, entonces lo que hacemos es llevar la ultima posicion enviada.

            // Si ha pasado el tiempo necesario y la ultima posicion enviada no es la actual:
            if (waitFrames >= FPU && lastSentPosition != position) {
                lastSentPosition = position;
                waitFrames = 0;
                send(MOVE.getMessage((int)position.x, (int)position.y));
            }
        }
    }

    public void processCommand(COMMAND cmd) {
        if (cmd.getType() == CommandType.CHAT) {
            CHAT msg = CHAT.process(cmd.getCommand());

            LobbyChat.getInstance().addMessage(msg.getMessage());
        } else if (cmd.getType() == CommandType.MOVE) {
            MOVE msg = MOVE.process(cmd.getCommand());

            users[msg.getKey()].updatePosition(new Vector2(msg.getX(), msg.getY()));
        } else if (cmd.getType() == CommandType.ON) {
            ON msg = ON.process(cmd.getCommand());

            onlineUsers++;

            Player newPlayer = new GameObject(msg.getData().getName(), typeof(Player)).GetComponent<Player>();
            newPlayer.initPlayer(
                new Vector2(msg.getX(),
                msg.getY()),
                msg.getData().getName());
            users.Add(msg.getKey(), newPlayer);

            updateInfo();
        } else if (cmd.getType() == CommandType.OFF) {
            OFF msg = OFF.process(cmd.getCommand());

            onlineUsers--;

            Destroy(users[msg.getKey()].gameObject);
            users.Remove(msg.getKey());

            updateInfo();
        } else if (cmd.getType() == CommandType.INFO) {
            INFO msg = INFO.process(cmd.getCommand());

            users = msg.getUsers();
            IPEndPoint p = (IPEndPoint)client.Client.LocalEndPoint;
            users.Add("/" + p.Address + ":" + p.Port, player);

            onlineUsers = msg.getOnlineUsers();
            foreach (string line in msg.getChat())
                LobbyChat.getInstance().getCGText().addText(line);

            LobbyChat.getInstance().updateChatGUI();
            updateInfo();
        } else if (cmd.getType() == CommandType.STATUS) {
            send(ALIVE.getMessage());
        } else if (cmd.getType() == CommandType.DISCONNECT) {
            DISCONNECT msg = DISCONNECT.process(cmd.getCommand());

            disconnectUser(msg.getDisconnectMessage());
        } else if (cmd.getType() == CommandType.UNKNOWN) {
            Debug.Log("Algo salio mal: " + cmd.getCommand());
        }
    }

    private IEnumerator getResponse() {
        while (client.Client != null) {
            try {
                if (client.Available > 0) {
                    COMMAND cmd = new COMMAND(Encoding.ASCII.GetString(client.Receive(ref server)));
                    timeout = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    processCommand(cmd);
                }
            } catch (Exception e) {
                disconnectUser(player != null ? e.Message : "No se pudo conectar con el servidor: Puerto inalcanzable");
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }

    private void updateInfo() {
        onlineText.text = "Online: " + onlineUsers;
    }

    public void send(string message) {
        byte[] sendBytes = Encoding.ASCII.GetBytes(message);
        client.Send(sendBytes, sendBytes.Length);
    }

    private void OnApplicationQuit() {
        if (client != null && client.Client != null && client.Client.Connected) {
            byte[] sendBytes = Encoding.ASCII.GetBytes(ADIOS.getMessage());
            client.Send(sendBytes, sendBytes.Length);

            client.Close();
        }
    }

    private Vector2Int rez = new Vector2Int(Screen.height, Screen.width) / 2;
    private void toggleFullScreen() {
        FullScreenMode mode;
        if (Screen.fullScreenMode == FullScreenMode.Windowed) {
            rez.y = Screen.height;
            rez.x = Screen.width;

            mode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, mode);
        } else {
            mode = FullScreenMode.Windowed;
            Screen.SetResolution(rez.x, rez.y, mode);
        }
    }

    public static UDPClient getInstance() {
        return instance;
    }
}
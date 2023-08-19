using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;

public class UDPClient : MonoBehaviour {
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

    public Text positionText;

    public Text onlineText;
    private int onlineUsers;
    private string myName;

    private Dictionary<string, Player> users;
    private Player player = null;

    public Button exitButton;

    public SmartButton upButton;
    public SmartButton rightButton;
    public SmartButton downButton;
    public SmartButton leftButton;

    public InputField nameField;
    public Button confirmNameButton;
    public GameObject nameMenuGameObject;

    public GameObject backgroundChatGameObject;

    public GameObject messagesNotReadedGameObject;
    public Text messagesNotReadedText;

    public Text serverErrorText;

    private UdpClient client;
    private IPEndPoint server;
    //on application quit and why cant answer to broadcast udp
    // numero de gente online
    // PORQUE SI MANDO EN BROADCST NO ME PUEDEN RESPONDER WTF ESO NO LO ENTIENDE NI DON GABRIEL LUQUE

    public static int FPU = 10; // Frames Per Update

    private float speedMultiplier = 1;

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

    // MIRAR BUGS Y F11 SUPER BUG

    // hacer funciones para cada tipo de mensaje
    // refactorizar - version 8
    // decorar un poco todo - version 8

    // version 8 - que todo se ejecute en un thread en el sv y que el main pueda escribir y poner comandos
    // y que se cierre el servefr con un comando que desconecte a los demas y to eso y se guasrde el chat

    // añadir seguridad controlar excepciones si se desconecta alguien y manda un mensaje se crashea el server

    // controlar duplicados, perdida de paquetes, el chat se duplica, si el timeout se pierde estamos jodidos
    // el hola si no llega tambien es jodienda

    // version 9000 cuentas

    // AÑADIR SEGURIDAD PARA CONECTARSE Y DESCONECTARSE ASEGURARSE DE QUE LLEGUE EL MENSAJE
    // RESPONDIENDO Y DEMAS

    public InputField ipInputField;
    public InputField portInputField;

    void Start() {
        messagesNotReadedGameObject.SetActive(false);
        chatMenuGameObject.SetActive(false);
        nameMenuGameObject.SetActive(true);

        users = new Dictionary<string, Player>();

        client = new UdpClient();

        sendButton.onClick.AddListener(delegate { sendChat(); });
        openChatButton.onClick.AddListener(delegate { openChat(); });
        closeChatButton.onClick.AddListener(delegate { closeChat(); });
        exitButton.onClick.AddListener(delegate { Application.Quit(); });

        confirmNameButton.onClick.AddListener(delegate { 
            if (nameField.text.Length > 0 && !nameField.text.Contains(";")) {
                myName = nameField.text;
                serverErrorText.text = "";

                player = new GameObject("localhost", typeof(Player)).GetComponent<Player>();
                player.initPlayer(myName, true);

                server = new IPEndPoint(IPAddress.Parse(ipInputField.text), int.Parse(portInputField.text));

                client.Connect(server);

                onlineUsers = 1;

                // hcer funcion para el chat para decorarlo y me polto bonito
                // uno que sea addmessage o algo asi ya q se añaden mensajes de muchos sitios
                try {
                    byte[] sendBytes = Encoding.ASCII.GetBytes("HOLA " + myName + ";0;0");
                    client.Send(sendBytes, sendBytes.Length);
                } catch (Exception e) {
                    chatText.text += "<color=red>" + e.Message + "</color>\n";
                }

                StartCoroutine(getResponse());

                nameMenuGameObject.SetActive(false);
            }
        });
    }

    private Vector2Int rez;
    public void toggleFullScreen() {
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

    private void openChat() {
        clearNotReaded();

        isChatOpen = true;
        chatMenuGameObject.SetActive(true);
    }

    private void closeChat() {
        isChatOpen = false;
        chatMenuGameObject.SetActive(false);
    }

    private void sendChat() {
        if (sendText.text.Length > 0) { 
            onSend(sendText.text.Replace(";", "<pc>")); 
            sendText.text = ""; 
        }
    }

    private void updateNotReaded() {
        if (!isChatOpen) {
            messagesNotReaded++;
            messagesNotReadedText.text = messagesNotReaded.ToString();
        }

        if (messagesNotReaded > 0)
            messagesNotReadedGameObject.SetActive(true);
    }

    private void clearNotReaded() {
        messagesNotReaded = 0;
        messagesNotReadedGameObject.SetActive(false);
    }

    private int messagesNotReaded = -1;
    private void Update() {
        if (player == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift))
            speedMultiplier = 2f;
        else
            speedMultiplier = 1;

        if (chatText.text.StartsWith("\n"))
            chatText.text = chatText.text.Substring(1);

        if (Input.GetKeyDown(KeyCode.F11))
            toggleFullScreen();

        if (Input.GetKeyDown(KeyCode.T) && !isChatOpen)
            openChat();

        if (Input.GetKeyDown(KeyCode.Escape) && isChatOpen)
            closeChat();

        if (Input.GetKeyDown(KeyCode.Return) && isChatOpen)
            sendChat();
    }

    private int waitFrames;
    private Vector3 lastSentPosition;
    private bool isChatOpen = false;
    private void FixedUpdate() {
        waitFrames++;

        if (player != null && !isChatOpen) {
            Vector3 position = player.transform.localPosition;
            Vector3 size = player.GetComponent<RectTransform>().sizeDelta;
            positionText.text = position.x + ", " + position.y;

            float move = 5 * speedMultiplier;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || upButton.isPressed) {
                upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if(position.y + move <= 1080/2 - size.y) position.y += move;
            } else upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || rightButton.isPressed) {
                rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x + move <= 1920/2 - size.x) position.x += move;
            } else rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || downButton.isPressed) {
                downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.y - move >= -1080/2 + size.y) position.y -= move;
            } else downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || leftButton.isPressed) {
                leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x - move >= -1920/2 + size.x) position.x -= move;
            } else leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (position != player.transform.localPosition) // Actualizar localmente fluidamente
                player.transform.localPosition = position;

            // Para que no pase que se actualice la posicion localmente, pero no se envie ya que hay que esperar
            // cada FPU frames, entonces lo que hacemos es llevar la ultima posicion enviada.

            // Si ha pasado el tiempo necesario y la ultima posicion enviada no es la actual:
            if (waitFrames >= FPU && lastSentPosition != position) {
                lastSentPosition = position;
                waitFrames = 0;
                onMove(position.x + ";" + position.y);
            }
        }
    }

    private void onSend(string text) {
        byte[] sendBytes = Encoding.ASCII.GetBytes("CHAT " + text);

        client.Send(sendBytes, sendBytes.Length);
    }

    public void onMove(string position) {
        byte[] sendBytes = Encoding.ASCII.GetBytes("MOVE " + position);

        client.Send(sendBytes, sendBytes.Length);
    }

    private Dictionary<string, string> GameColor = new Dictionary<string, string>() {
        { "&0", "#000000" },
        { "&1", "#0000AA" },
        { "&2", "#00AA00" },
        { "&3", "#00AAAA" },
        { "&4", "#AA0000" },
        { "&5", "#AA00AA" },
        { "&6", "#FFAA00" },
        { "&7", "#AAAAAA" },
        { "&8", "#555555" },
        { "&9", "#5555FF" },
        { "&a", "#55FF55" },
        { "&b", "#55FFFF" },
        { "&c", "#FF5555" },
        { "&d", "#FF55FF" },
        { "&e", "#FFFF55" },
        { "&f", "#FFFFFF" }
    };
    public string transformToGameColors(string message) {
        StringBuilder transformedMessage = new StringBuilder();
        char[] msg = message.ToCharArray();

        bool isFirst = true;
        for (int i = 0; i < message.Length; i++) {
            if (msg[i] == '&') {
                if (i + 1 < msg.Length) {
                    char myChar = msg[i + 1];
                    int listLength = typeof(ChatColor).GetFields().Length;

                    int j = 0;
                    while (j < listLength && ((string) typeof(ChatColor).GetFields()[j].GetValue(null))[1] != myChar)
                        j++;

                    if (j < listLength) {
                        string color = GameColor[(string)typeof(ChatColor).GetFields()[j].GetValue(null)];

                        transformedMessage.Append((isFirst ? "" : "</color>") + "<color=" + color + ">");
                        isFirst = false;

                        i++;
                    } else transformedMessage.Append(msg[i]);
                }
            } else
                transformedMessage.Append(msg[i]);
        }
        if (!isFirst)
            transformedMessage.Append("</color>");
        return transformedMessage.ToString();
    }

    public void addMessage(string message) {
        chatText.text += "\n" + transformToGameColors(message);

        chatText.GetComponent<RectTransform>().sizeDelta = new Vector2(1275 * 4, chatText.preferredHeight + 10);

        backgroundChatGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1300, chatText.GetComponent<RectTransform>().sizeDelta.y / 4);
        float y = -380 + backgroundChatGameObject.GetComponent<RectTransform>().sizeDelta.y;
       
        closeChatButton.transform.localPosition = new Vector2(340, y > 380 ? 380 : y);
    }

    private IEnumerator getResponse() {
        while (true) {
            if(client.Available > 0) {
                COMMAND cmd = null;
                try {
                    cmd = new COMMAND(client.Receive(ref server)); Debug.Log(cmd.getCommand());
                } catch (Exception e) {
                    serverErrorText.text = e.Message;
                    nameMenuGameObject.SetActive(true);
                }

                if (cmd.getType() == CommandType.CHAT) {
                    CHAT msg = CHAT.process(cmd.getCommand());

                    updateNotReaded();

                    addMessage(msg.getMessage());
                } else if(cmd.getType() == CommandType.MOVE) {
                    MOVE msg = MOVE.process(cmd.getCommand());

                    users[msg.getKey()].updatePosition(new Vector2(msg.getX(), msg.getY()));
                } else if (cmd.getType() == CommandType.ON) {
                    ON msg = ON.process(cmd.getCommand());

                    onlineUsers++;
                    
                    Player newPlayer = new GameObject(msg.getData().getName(), typeof(Player)).GetComponent<Player>();
                    newPlayer.initPlayer(
                        new Vector2(msg.getPlayerData().getX(), 
                        msg.getPlayerData().getY()), 
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
                    onlineUsers = msg.getOnlineUsers();
                    foreach (string line in msg.getChat())
                        addMessage(line);

                    updateInfo();
                } else if (cmd.getType() == CommandType.TIMEOUT) {
                    byte[] sendBytes = Encoding.ASCII.GetBytes(ALIVE.getMessage());

                    client.Send(sendBytes, sendBytes.Length);
                } else {
                    Debug.Log("Algo salio mal: " + cmd.getCommand());
                }
            }

            yield return new WaitForSeconds(0);
        }
    }

    private void updateInfo() {
        onlineText.text = "Online: " + onlineUsers;
    }

    private void OnApplicationQuit() {
        byte[] sendBytes = Encoding.ASCII.GetBytes(ADIOS.getMessage());
        client.Send(sendBytes, sendBytes.Length);

        client.Close();
    }
}

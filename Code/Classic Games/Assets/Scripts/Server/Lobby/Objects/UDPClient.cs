using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class UDPClient : MonoBehaviour {
    // Started by @eulogioqt on 13/08/2023

    // TO-DO
    // Receive on other thread:
    // https://stackoverflow.com/questions/53731293/sending-udp-calls-in-unity-on-android
    
    public TMP_Text chatText;
    public TMP_Text shadowText;
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

    public Button disconnectButton;

    public SmartButton upButton;
    public SmartButton rightButton;
    public SmartButton downButton;
    public SmartButton leftButton;
    /*
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     *     COMANDO VERSION DEL WHATSAPP
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     */
    public InputField nameField;

    public GameObject backgroundChatGameObject;

    public GameObject messagesNotReadedGameObject;
    public Text messagesNotReadedText;

    public Button exitButton;

    private IPEndPoint server;
    private UdpClient client;
    private User user;
    //on application quit and why cant answer to broadcast udp
    // numero de gente online
    // PORQUE SI MANDO EN BROADCST NO ME PUEDEN RESPONDER WTF ESO NO LO ENTIENDE NI DON GABRIEL LUQUE

    public static int FPU = 10; // Frames Per Update

    private int speedMultiplier = 1;

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

    private long timeout;

    private static UDPClient instance;
    public static UDPClient getInstance() {
        return instance;
    }

    private void Awake() {
        if(instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        loadButtons();
    }

    private void loadButtons() {
        sendButton.onClick.AddListener(delegate { sendChat(); });
        openChatButton.onClick.AddListener(delegate { openChat(); });
        closeChatButton.onClick.AddListener(delegate { closeChat(); });
        disconnectButton.onClick.AddListener(delegate { onDisconnect("LEAVE"); });
        exitButton.onClick.AddListener(delegate { Application.Quit(); });
    }

    public void onConnect(COMMAND cmd, IPEndPoint server, UdpClient client, User user) {
        this.server = server;
        this.client = client;
        this.user = user;

        timeout = long.MaxValue;
        messagesNotReaded = -1;
        isChatOpen = false;

        messagesNotReadedGameObject.SetActive(false);
        chatMenuGameObject.SetActive(false);

        player = new GameObject(user.getData().getName()).AddComponent<Player>();
        player.initPlayer(user.getData().getName(), true);

        processCommand(cmd);

        StartCoroutine(getResponse());
    }

    private void onDisconnect(string message) {
        try {
            byte[] sendBytes = Encoding.ASCII.GetBytes(ADIOS.getMessage());
            client.Send(sendBytes, sendBytes.Length);

            client.Close();
            client = new UdpClient();
        } catch (Exception) { }

        if (message.Equals("LEAVE")) {
            JoinMenuController.getInstance().setMenuActive(true);
            JoinMenuController.getInstance().refreshList();
        } else ConnectionController.getInstance().setDisconnected(message, true);

        GameObject field = GameObject.FindGameObjectWithTag("Field");
        for (int i = 0; i < field.transform.childCount; i++) {
            Destroy(field.transform.GetChild(i).gameObject);
        }
    }

    // CHAT // CHAT // CHAT // CHAT // CHAT // CHAT // CHAT
    private void sendChat() {
        if (sendText.text.Length > 0) {
            onSend(sendText.text.Replace(";", "<pc>"));
            sendText.text = "";
        }
    }

    private void openChat() {
        clearNotReaded();

        sendText.Select();
        sendText.ActivateInputField();

        isChatOpen = true;
        chatMenuGameObject.SetActive(true);
    }

    private void closeChat() {
        isChatOpen = false;
        chatMenuGameObject.SetActive(false);
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
    private bool isChatOpen = false;

    private Dictionary<string, string> GameColor = new Dictionary<string, string>() {
        { "&0", "<color=#000000>" },
        { "&1", "<color=#0000AA>" },
        { "&2", "<color=#00AA00>" },
        { "&3", "<color=#00AAAA>" },
        { "&4", "<color=#AA0000>" },
        { "&5", "<color=#AA00AA>" },
        { "&6", "<color=#FFAA00>" },
        { "&7", "<color=#AAAAAA>" },
        { "&8", "<color=#555555>" },
        { "&9", "<color=#5555FF>" },
        { "&a", "<color=#55FF55>" },
        { "&b", "<color=#55FFFF>" },
        { "&c", "<color=#FF5555>" },
        { "&d", "<color=#FF55FF>" },
        { "&e", "<color=#FFFF55>" },
        { "&f", "<color=#FFFFFF>" },
        { "&l", "<b>" },
        { "&o", "<i>" },
        { "&n", "<s>" }
    };

    private Dictionary<string, string> GameShadowColor = new Dictionary<string, string>() {
        { "&0", "<color=#000000>" },
        { "&1", "<color=#00002B>" },
        { "&2", "<color=#002B00>" },
        { "&3", "<color=#002B2B>" },
        { "&4", "<color=#2B0000>" },
        { "&5", "<color=#2B002B>" },
        { "&6", "<color=#402B00>" },
        { "&7", "<color=#2C2C2C>" },
        { "&8", "<color=#161616>" },
        { "&9", "<color=#161640>" },
        { "&a", "<color=#164016>" },
        { "&b", "<color=#164040>" },
        { "&c", "<color=#401616>" },
        { "&d", "<color=#401640>" },
        { "&e", "<color=#404016>" },
        { "&f", "<color=#404040>" },
        { "&l", "<b>" },
        { "&o", "<i>" },
        { "&n", "<s>" }
    };

    public string transformToGameColors(string message, bool shadow) {
        StringBuilder transformedMessage = new StringBuilder();
        char[] msg = message.ToCharArray();

        bool isBold = false; int b = 0;
        bool isItalic = false; int it = 0;
        bool isUnderline = false; int u = 0;
        for (int i = 0; i < message.Length; i++) {
            if (msg[i] == '&') {
                if (i + 1 < msg.Length) {
                    string myChar = msg[i + 1].ToString().ToLower();
                    int listLength = typeof(ChatColor).GetFields().Length;

                    int j = 0;
                    while (j < listLength && !((string)typeof(ChatColor).GetFields()[j].GetValue(null))[1].ToString().ToLower().Equals(myChar))
                        j++;

                    if (j < listLength) {
                        string addText = (isBold ? "</b>" : "") + (isItalic ? "</i>" : "") + (isUnderline ? "</s>" : "");

                        bool isColor = false;
                        if (myChar == ChatColor.BOLD[1].ToString().ToLower()) {
                            isBold = true;
                            b++;
                        } else if (myChar == ChatColor.ITALIC[1].ToString().ToLower()) {
                            isItalic = true;
                            it++;
                        } else if (myChar == ChatColor.UNDERLINE[1].ToString().ToLower()) {
                            isUnderline = true;
                            u++;
                        } else {
                            if (isBold) b--;
                            if (isItalic) it--;
                            if (isUnderline) u--;

                            isBold = false;
                            isItalic = false;
                            isUnderline = false;
                            isColor = true;
                        }

                        string color = (isColor ? addText : "") +
                            (shadow ? GameShadowColor[(string)typeof(ChatColor).GetFields()[j].GetValue(null)] :
                            GameColor[(string)typeof(ChatColor).GetFields()[j].GetValue(null)]);
                        transformedMessage.Append(color);

                        i++;
                    } else transformedMessage.Append(msg[i]);
                }
            } else
                transformedMessage.Append(msg[i]);
        }

        for (int i = 0; i < b; i++)
            transformedMessage.Append("</b>");

        for (int i = 0; i < it; i++)
            transformedMessage.Append("</i>");

        for (int i = 0; i < u; i++)
            transformedMessage.Append("</s>");

        return (shadow ? GameShadowColor[ChatColor.WHITE] : GameColor[ChatColor.WHITE]) + transformedMessage.ToString();
    }

    private void updateChatGUI() {
        float p = chatText.preferredHeight;
        chatText.GetComponent<RectTransform>().sizeDelta = new Vector2(1275, p + 5);
        shadowText.GetComponent<RectTransform>().sizeDelta = new Vector2(1275, p + 5);

        backgroundChatGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1300, chatText.GetComponent<RectTransform>().sizeDelta.y);
        float y = -380 + backgroundChatGameObject.GetComponent<RectTransform>().sizeDelta.y;

        closeChatButton.transform.localPosition = new Vector2(340, y > 380 ? 380 : y);
    }

    public void addMessage(string message) {
        chatText.text += "\n" + transformToGameColors(message, false);
        shadowText.text += "\n" + transformToGameColors(message, true);
    }

    // CHAT // CHAT // CHAT // CHAT // CHAT // CHAT // CHAT

    private void sendSTATUS() {
        byte[] sendBytes = Encoding.ASCII.GetBytes(STATUS.getMessage());
        
        client.Send(sendBytes, sendBytes.Length);
    }

    private IEnumerator timeoutOnSeconds(int n) {
        yield return new WaitForSeconds(n);

        if (timeout == long.MaxValue)
            onDisconnect("El servidor no responde");
    }

    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F11))
            toggleFullScreen();

        if (player == null)
            return;



        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - timeout >= 30) {
            timeout = long.MaxValue;
            StartCoroutine(timeoutOnSeconds(10));
            
            sendSTATUS();
        }

        if (Input.GetKey(KeyCode.LeftShift))
            speedMultiplier = 2;
        else
            speedMultiplier = 1;

        if (chatText.text.StartsWith("\n")) {
            shadowText.text = shadowText.text.Substring(1);
            chatText.text = chatText.text.Substring(1);
            updateChatGUI();
        }

        if (Input.GetKeyDown(KeyCode.T) && !isChatOpen)
            openChat();

        if (Input.GetKeyDown(KeyCode.Escape) && isChatOpen)
            closeChat();

        if (Input.GetKeyDown(KeyCode.Return) && isChatOpen)
            sendChat();
    }

    private int waitFrames;
    private Vector3 lastSentPosition;
    private void FixedUpdate() {
        waitFrames++;

        if (player != null && !isChatOpen) {
            Vector3 position = player.transform.localPosition;
            Vector3 size = player.GetComponent<RectTransform>().sizeDelta;
            positionText.text = position.x + ", " + position.y;

            int move = 5 * speedMultiplier;

            int x = 0;
            int y = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || upButton.isPressed) {
                upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if(position.y + move <= 1080/2 - size.y) y += move;
            } else upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || rightButton.isPressed) {
                rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x + move <= 1920/2 - size.x) x += move;
            } else rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || downButton.isPressed) {
                downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.y - move >= -1080/2 + size.y) y -= move;
            } else downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || leftButton.isPressed) {
                leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x - move >= -1920/2 + size.x) x -= move;
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


    

    public void send(string message) {
        byte[] sendBytes = Encoding.ASCII.GetBytes(ALIVE.getMessage());

        client.Send(sendBytes, sendBytes.Length);
    }

    public void processCommand(COMMAND cmd) {
        if (cmd.getType() == CommandType.CHAT) {
            CHAT msg = CHAT.process(cmd.getCommand());

            updateNotReaded();

            addMessage(msg.getMessage());
            updateChatGUI();
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
                addMessage(line);

            updateInfo();
            updateChatGUI();
        } else if (cmd.getType() == CommandType.STATUS) {
            send(ALIVE.getMessage());
        } else if (cmd.getType() == CommandType.DISCONNECT) {
            DISCONNECT msg = DISCONNECT.process(cmd.getCommand());

            onDisconnect(msg.getDisconnectMessage());
        } else if (cmd.getType() == CommandType.UNKNOWN) {
            Debug.Log("Algo salio mal: " + cmd.getCommand());
        }
    }

    private IEnumerator getResponse() {
        while (client.Client != null) {
            try {
                if (client.Available > 0) {
                    COMMAND cmd = new COMMAND(client.Receive(ref server));
                    timeout = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    processCommand(cmd);
                }
            } catch (Exception e) {
                onDisconnect(player != null ? e.Message : "No se pudo conectar con el servidor: Puerto inalcanzable");
                break;
            }
            yield return new WaitForSeconds(0);
        }
    }

    private void updateInfo() {
        onlineText.text = "Online: " + onlineUsers;
    }

    private void OnApplicationQuit() {
        if(client != null && client.Client != null && client.Client.Connected) {
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
}

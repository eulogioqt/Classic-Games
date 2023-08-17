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

    private UdpClient client;
    private IPEndPoint server;
    //on application quit and why cant answer to broadcast udp
    // numero de gente online
    // PORQUE SI MANDO EN BROADCST NO ME PUEDEN RESPONDER WTF ESO NO LO ENTIENDE NI DON GABRIEL LUQUE
    private static UDPTest instance;

    public static int FPU = 10; // Frames Per Update

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
    // refactorizar - version 7
    // decorar un poco todo - version 7

    // version 8 - que todo se ejecute en un thread en el sv y que el main pueda escribir y poner comandos
    // y que se cierre el servefr con un comando que desconecte a los demas y to eso y se guasrde el chat

    // añadir seguridad controlar excepciones si se desconecta alguien y manda un mensaje se crashea el server

    // version 9000 cuentas

    // AÑADIR SEGURIDAD PARA CONECTARSE Y DESCONECTARSE ASEGURARSE DE QUE LLEGUE EL MENSAJE
    // RESPONDIENDO Y DEMAS

    private void Awake() {
        if(instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    void Start() {
        chatMenuGameObject.SetActive(false);
        nameMenuGameObject.SetActive(true);

        users = new Dictionary<string, Player>();

        client = new UdpClient();
        client.EnableBroadcast = true;

        server = new IPEndPoint(IPAddress.Parse("192.168.100.2"), 11000);

        client.Connect(server);

        sendButton.onClick.AddListener(delegate { if (sendText.text.Length > 0) 
            { onSend(sendText.text.Replace(";", "<pc>")); sendText.text = ""; } });
        openChatButton.onClick.AddListener(delegate { chatMenuGameObject.SetActive(true); });
        closeChatButton.onClick.AddListener(delegate { chatMenuGameObject.SetActive(false); });
        exitButton.onClick.AddListener(delegate { Application.Quit(); });

        confirmNameButton.onClick.AddListener(delegate { 
            if (nameField.text.Length > 0 && !nameField.text.Contains(";")) {
                myName = nameField.text;

                player = new GameObject("localhost", typeof(Player)).GetComponent<Player>();
                player.initPlayer(myName, true);

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

    private int waitFrames;
    private Vector3 lastSentPosition;
    private void FixedUpdate() {
        waitFrames++;

        if (Input.GetKeyDown(KeyCode.F11)) 
            toggleFullScreen();

        if (player != null) {
            Vector3 position = player.transform.localPosition;
            Vector3 size = player.GetComponent<RectTransform>().sizeDelta;
            positionText.text = position.x + ", " + position.y;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || upButton.isPressed) {
                upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if(position.y + 5 <= 1080/2 - size.y) position.y += 5;
            } else upButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || rightButton.isPressed) {
                rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x + 5 <= 1920/2 - size.x) position.x += 5;
            } else rightButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || downButton.isPressed) {
                downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.y - 5 >= -1080/2 + size.y) position.y -= 5;
            } else downButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || leftButton.isPressed) {
                leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.pressedColor;
                if (position.x - 5 >= -1920/2 + size.x) position.x -= 5;
            } else leftButton.GetComponent<Image>().color = upButton.GetComponent<Button>().colors.normalColor;

            if (position != player.transform.localPosition) // Actualizar localmente fluidamente
                player.transform.localPosition = position;

            // Para que no pase que se actualice la posicion localmente, pero no se envie ya que hay que esperar
            // cada FPU frames, entonces lo que hacemos es llevar la ultima posicion enviada.

            // Si ha pasado el tiempo necesario y la ultima posicion enviada no es la actual:
            if (waitFrames >= FPU && lastSentPosition != position) {
                lastSentPosition = position;
                Debug.Log("ultima posicion enviada: " + lastSentPosition);
                waitFrames = 0;
                onMove(position.x + ";" + position.y);
            }
        }
    }

    private void onSend(string text) {
        chatText.text += "<color=blue>" + player.getName() + "</color> - " + text.Replace("<pc>", ";") + "\n";

        byte[] sendBytes = Encoding.ASCII.GetBytes("CHAT " + text);

        client.Send(sendBytes, sendBytes.Length);
    }

    public void onMove(string position) {
        byte[] sendBytes = Encoding.ASCII.GetBytes("MOVE " + position);

        client.Send(sendBytes, sendBytes.Length);
    }

    private IEnumerator getResponse() {
        while (true) {
            if(client.Available > 0) {
                byte[] receiveBytes = client.Receive(ref server);
                string text = Encoding.ASCII.GetString(receiveBytes);

                if (text.StartsWith("CHAT")) {
                    string key = text.Substring(5).Split(' ')[0];
                    string message = text.Substring(5 + key.Length + 1); // "CHAT "=5 + KEY + "=1

                    chatText.text += "<color=green>" + users[key].getName() + "</color> - " + message.Replace("<pc>", ";") + "\n";
                } else if(text.StartsWith("MOVE")) {
                    string key = text.Substring(5).Split(' ')[0];
                    string[] data = text.Substring(5 + key.Length + 1).Split(';');

                    int x = int.Parse(data[0]);
                    int y = int.Parse(data[1]);

                    users[key].updatePosition(new Vector2(x, y));
                } else if (text.StartsWith("ON")) {
                    string key = text.Substring(3).Split(' ')[0];
                    string[] data = text.Substring(3 + key.Length + 1).Split(';'); // "ON "=3 + KEY + " "=1

                    string playerName = data[0];
                    int x = int.Parse(data[1]);
                    int y = int.Parse(data[2]);

                    onlineUsers++;
                    
                    Player newPlayer = new GameObject(playerName, typeof(Player)).GetComponent<Player>();
                    newPlayer.initPlayer(new Vector2(x, y), playerName);
                    users.Add(key, newPlayer);

                    chatText.text += "<color=green>" + playerName + "</color> - " + "joined the server" + "\n";
                    updateInfo();
                } else if (text.StartsWith("OFF")) {
                    string key = text.Substring(4);
                    onlineUsers--;

                    Destroy(users[key].gameObject);
                    chatText.text += "<color=green>" + users[key].getName() + "</color> - " + "left the server" + "\n";
                    users.Remove(key);

                    updateInfo();
                } else if (text.StartsWith("INFO")) {
                    string[] lines = text.Substring(5).Split('\n');

                    bool chatPart = false;
                    foreach (string line in lines) {
                        if (!chatPart) { // Parte de usuarios
                            if (!line.Equals(".")) { // Si no es un punto
                                string key = line.Split(' ')[0];
                                string[] data = line.Substring(key.Length + 1).Split(';');
                                string name = data[0];
                                int x = int.Parse(data[1]);
                                int y = int.Parse(data[2]);

                                Player newPlayer = new GameObject(name, typeof(Player)).GetComponent<Player>();
                                newPlayer.initPlayer(new Vector2(x, y), name);
                                users.Add(key, newPlayer);

                                onlineUsers++;
                            } else chatPart = true; // Si es un punto cambia de parte
                        } else if (!line.Equals(".")) { // Parte de chat, si no es el final, sigue
                            string[] data = line.Split(';');
                            string name = data[0];
                            string message = data[1];

                            chatText.text += "<color=green>" + name + "</color> - " + message.Replace("<pc>", ";") + "\n";
                        }
                    }

                    chatText.text += "<color=blue>" + player.getName() + "</color> - " + "joined the server" + "\n";
                    updateInfo();
                } else if (text.StartsWith("TIMEOUT")) {
                    byte[] sendBytes = Encoding.ASCII.GetBytes("ALIVE");

                    client.Send(sendBytes, sendBytes.Length);
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

        client.Close();
    }

    public static UDPTest getInstance() {
        return instance;
    }

    public static string compressData(string data) {
        return data.Replace(" ", "<sp>");
    }

    public static string decompressData(string data) {
        return data.Replace(" ", "<sp>");
    }
}

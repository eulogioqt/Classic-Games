using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ServerObject : MonoBehaviour {

    private int index;

    private string serverName;
    private IPAddress IP;
    private int port;

    private Image frameImage;
    private Image serverImage;
    private TextMeshProUGUI serverNameText;
    private TextMeshProUGUI serverStatusText;
    private TextMeshProUGUI serverOnlinePlayersText;

    private UdpClient client = null;
    private Coroutine lastCoroutine = null;

    public void pingServer() {
        serverOnlinePlayersText.text = "";
        setStatus("");

        if (client != null)
            client.Close();
        client = new UdpClient();
        
        if (lastCoroutine != null)
            StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(ping());
    }

    private IEnumerator ping() {
        IPEndPoint server = new IPEndPoint(IP, port);
        yield return new WaitForSeconds(0.5f + UnityEngine.Random.Range(0f, 1f + 0.1f * index));

        long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        bool answered = false;

        try {
            client.Connect(server);
            setStatus("Pinging...");

            byte[] sendBytes = Encoding.ASCII.GetBytes(PING.getMessage());
            client.Send(sendBytes, sendBytes.Length);
        } catch (Exception) {
            answered = true;
            setStatus("&4Host invalido");
            serverOnlinePlayersText.text = "<color=red>X</color>";
        }

        while (!answered && DateTimeOffset.UtcNow.ToUnixTimeSeconds() - time < 10) {
            try {
                if(client.Available > 0) {
                    COMMAND cmd = new COMMAND(Encoding.ASCII.GetString(client.Receive(ref server)));
                    if (cmd.getType() == CommandType.PING) {
                        PING msg = PING.process(cmd.getCommand());
                        answered = true;

                        serverOnlinePlayersText.text = msg.getVersion().Equals(Application.version) ?
                            "Online: " + msg.getOnlinePlayers() :
                            "<color=red>X " + msg.getVersion() + "</color>";
                        setStatus(msg.getMOTD());
                    }
                }
            } catch (Exception) {
                time = 0;
            }

            if(!answered) {
                byte[] sendBytes = Encoding.ASCII.GetBytes(PING.getMessage());
                client.Send(sendBytes, sendBytes.Length);
            }

            yield return new WaitForSeconds(1);
        }

        if (!answered) {
            setStatus("&4No se puede conectar con el servidor");
            serverOnlinePlayersText.text = "<color=red>X</color>";
        }

        client.Close();
    }

    private void setStatus(string status) {
        serverStatusText.text = CGText.transformToGameColors("&7" + status, false);
    }

    private void doubleClick() {
        joinServer(JoinMenuController.getInstance().getLocalUser());
    }

    private void oneClick() {
        JoinMenuController.getInstance().selectServer(index);
    }

    public void joinServer(User user) {
        ConnectionController.getInstance().tryConnectingLobby(IP, port, user);
    }

    public void initServer(string serverName, IPAddress IP, int port) {
        this.serverName = serverName;
        this.IP = IP;
        this.port = port;

        gameObject.AddComponent<Image>();
        gameObject.AddComponent<Button>().onClick.AddListener(clickServer);

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(1150, 150, 0);
        gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 100);
        gameObject.name = serverName;

        frameImage = new GameObject("frameImage").AddComponent<Image>();
        frameImage.transform.SetParent(gameObject.transform);
        frameImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        frameImage.GetComponent<RectTransform>().sizeDelta = new Vector3(1150, 150, 0);
        frameImage.GetComponent<Image>().color = new Color32(255, 255, 255, 125);
        frameImage.raycastTarget = false;
        frameImage.gameObject.SetActive(false);

        serverImage = new GameObject("serverImage", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        serverImage.transform.SetParent(gameObject.transform);
        serverImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("multiplayerImage");
        serverImage.GetComponent<RectTransform>().sizeDelta = new Vector3(100, 100, 0);
        serverImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(-487.5f, 0, 0);
        serverImage.raycastTarget = false;


        serverNameText = new GameObject("serverNameText").AddComponent<TextMeshProUGUI>();
        serverNameText.transform.SetParent(gameObject.transform);
        serverNameText.GetComponent<RectTransform>().sizeDelta = new Vector3(760, 55, 0);
        serverNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(-20, 30, 0);
        serverNameText.text = serverName;
        serverNameText.fontSize = 40;
        serverNameText.overflowMode = TextOverflowModes.Truncate;
        serverNameText.color = Color.black;
        serverNameText.font = Resources.Load<TMP_FontAsset>("Fonts/Minecraftia-Regular SDF");
        serverNameText.alignment = TextAlignmentOptions.MidlineLeft;
        serverNameText.raycastTarget = false;


        serverStatusText = new GameObject("serverStatusText").AddComponent<TextMeshProUGUI>();
        serverStatusText.transform.SetParent(gameObject.transform);
        serverStatusText.GetComponent<RectTransform>().sizeDelta = new Vector3(952, 55, 0);
        serverStatusText.GetComponent<RectTransform>().anchoredPosition = new Vector3(76, -30, 0);
        serverStatusText.text = "";
        serverStatusText.fontSize = 40;
        serverStatusText.font = Resources.Load<TMP_FontAsset>("Fonts/Minecraftia-Regular SDF");
        serverStatusText.alignment = TextAlignmentOptions.MidlineLeft;
        serverStatusText.raycastTarget = false;


        serverOnlinePlayersText = new GameObject("serverOnlinePlayersText").AddComponent<TextMeshProUGUI>();
        serverOnlinePlayersText.transform.SetParent(gameObject.transform);
        serverOnlinePlayersText.GetComponent<RectTransform>().sizeDelta = new Vector3(952, 55, 0);
        serverOnlinePlayersText.GetComponent<RectTransform>().anchoredPosition = new Vector3(76, 30, 0);
        serverOnlinePlayersText.text = "";
        serverOnlinePlayersText.fontStyle = FontStyles.Bold;

        serverOnlinePlayersText.fontSize = 30;
        serverOnlinePlayersText.color = Color.black;
        serverOnlinePlayersText.font = Resources.Load<TMP_FontAsset>("Fonts/Minecraftia-Regular SDF");
        serverOnlinePlayersText.alignment = TextAlignmentOptions.TopRight;
        serverOnlinePlayersText.raycastTarget = false;
    }

    public string getName() {
        return serverName;
    }

    public IPAddress getIP() {
        return IP;
    }

    public int getPort() {
        return port;
    }

    public void setIndex(int index) {
        this.index = index;
    }

    public void selectFrame(bool b) {
        frameImage.gameObject.SetActive(b);
    }

    private long time = 0;
    private void clickServer() {
        if (DateTimeOffset.Now.ToUnixTimeMilliseconds() - time < 500 && JoinMenuController.getInstance().getSelectedServer() == index)
            doubleClick();
        else
            oneClick();

        time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}

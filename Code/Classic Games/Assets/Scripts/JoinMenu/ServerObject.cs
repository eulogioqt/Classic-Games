using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ServerObject : MonoBehaviour {

    private JoinMenuController controller;
    private int index;

    private string serverName;
    private string IP;
    private int port;

    private Image frameImage;
    private Image serverImage;
    private Text serverNameText;
    private Text serverStatusText;
    private Text serverOnlinePlayersText;

    public void pingServer() {

    }

    public void joinServer() {

    }

    public void initServer(int index, JoinMenuController controller, string serverName) {
        this.serverName = serverName;
        this.controller = controller;
        this.index = index;

        gameObject.AddComponent<Image>();
        gameObject.AddComponent<Button>().onClick.AddListener(delegate {
            this.controller.selectServer(this.index);
        });

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector3(1150, 150, 0);
        gameObject.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
        gameObject.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
        gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
        gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 175);
        gameObject.name = serverName;

        frameImage = new GameObject("frameImage").AddComponent<Image>();
        frameImage.transform.SetParent(gameObject.transform);
        frameImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        frameImage.GetComponent<RectTransform>().sizeDelta = new Vector3(1150, 150, 0);
        frameImage.GetComponent<Image>().color = new Color32(255, 255, 255, 200);
        frameImage.raycastTarget = false;
        frameImage.gameObject.SetActive(false);

        serverImage = new GameObject("serverImage", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        serverImage.transform.SetParent(gameObject.transform);
        serverImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("multiplayerImage");
        serverImage.GetComponent<RectTransform>().sizeDelta = new Vector3(100, 100, 0);
        serverImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(-487.5f, 0, 0);
        serverImage.raycastTarget = false;


        serverNameText = new GameObject("serverNameText").AddComponent<Text>();
        serverNameText.transform.SetParent(gameObject.transform);
        serverNameText.GetComponent<RectTransform>().sizeDelta = new Vector3(760, 55, 0);
        serverNameText.GetComponent<RectTransform>().anchoredPosition = new Vector3(-20, 30, 0);
        serverNameText.text = serverName;
        serverNameText.fontSize = 40;
        serverNameText.color = Color.black;
        serverNameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        serverNameText.alignment = TextAnchor.MiddleLeft;
        serverNameText.raycastTarget = false;


        serverStatusText = new GameObject("serverStatusText").AddComponent<Text>();
        serverStatusText.transform.SetParent(gameObject.transform);
        serverStatusText.GetComponent<RectTransform>().sizeDelta = new Vector3(952, 55, 0);
        serverStatusText.GetComponent<RectTransform>().anchoredPosition = new Vector3(76, -30, 0);
        serverStatusText.text = "Conectando con el servidor...";
        serverStatusText.fontSize = 40;
        serverStatusText.color = new Color32(142, 142, 142, 255);
        serverStatusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        serverStatusText.alignment = TextAnchor.MiddleLeft;
        serverStatusText.raycastTarget = false;


        serverOnlinePlayersText = new GameObject("serverOnlinePlayersText").AddComponent<Text>();
        serverOnlinePlayersText.transform.SetParent(gameObject.transform);
        serverOnlinePlayersText.GetComponent<RectTransform>().sizeDelta = new Vector3(952, 55, 0);
        serverOnlinePlayersText.GetComponent<RectTransform>().anchoredPosition = new Vector3(76, 30, 0);
        serverOnlinePlayersText.text = "Online: ?";
        serverOnlinePlayersText.fontSize = 30;
        serverOnlinePlayersText.color = Color.black;
        serverOnlinePlayersText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        serverOnlinePlayersText.alignment = TextAnchor.UpperRight;
        serverOnlinePlayersText.raycastTarget = false;
    }

    public void selectFrame(bool b) {
        frameImage.gameObject.SetActive(b);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour {

    public GameObject chatMenuGameObject;
    public GameObject backgroundChatGameObject;

    public Button sendButton;
    public Button openChatButton;
    public Button closeChatButton;

    public InputField sendText;
    public CGText chatText;

    public GameObject messagesNotReadedGameObject;
    public Text messagesNotReadedText;
    private int messagesNotReaded = -1;

    private static Chat instance;

    private void Awake() {
        if(instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        sendButton.onClick.AddListener(delegate { sendMessage(sendText); });
        openChatButton.onClick.AddListener(delegate { openChat(); });
        closeChatButton.onClick.AddListener(delegate { closeChat(); });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.T) && !isChatOpen())
            openChat();

        if (Input.GetKeyDown(KeyCode.Escape) && isChatOpen())
            closeChat();

        if (Input.GetKeyDown(KeyCode.Return) && isChatOpen())
            sendMessage(sendText);
    }

    private void openChat() {
        clearNotReaded();

        sendText.Select();
        sendText.ActivateInputField();

        chatMenuGameObject.SetActive(true);
    }

    private void closeChat() {
        chatMenuGameObject.SetActive(false);
    }

    public void resetChat() {
        firstMessage = true;
        messagesNotReaded = -1;
        messagesNotReadedGameObject.SetActive(false);
        chatText.resetText();
        closeChat();
    }

    public void updateNotReaded() {
        if (!isChatOpen()) {
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

    public void updateChatGUI() {
        float p = chatText.getPreferredHeight();
        chatText.setSizeDelta(new Vector3(1275, p + 5, 0));
        backgroundChatGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1300, chatText.getSizeDelta().y);
        
        float y = -380 + backgroundChatGameObject.GetComponent<RectTransform>().sizeDelta.y;
        closeChatButton.transform.localPosition = new Vector2(340, y > 380 ? 380 : y);
    }

    public bool isChatOpen() {
        return chatMenuGameObject.activeSelf;
    }

    private void sendMessage(InputField sendText) {
        if (sendText.text.Length > 0) {
            UDPClient.getInstance().send(CHAT.getMessage(sendText.text.Replace(";", "<pc>")));
            sendText.text = "";
        }
    }

    bool firstMessage = true;
    public void addMessage(string message) {
        chatText.addText((firstMessage ? "" : "\n") + message);

        if (firstMessage)
            firstMessage = false;
    }

    public static Chat getInstance() {
        return instance;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ParchisChat : MonoBehaviour {
    public GameObject chatMenuGameObject;

    public InputField sendInput;
    public Button sendButton;
    public Text chatText;

    public Button openChatButton;
    public Button closeChatButton;

    private static ParchisChat instance;

    private void Awake() {
        if(instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        closeChatMenu();

        openChatButton.onClick.AddListener(openChatMenu);
        closeChatButton.onClick.AddListener(closeChatMenu);

        sendButton.onClick.AddListener(sendMessage);
    }

    private void Update() {
        if (chatMenuGameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
            sendMessage();
    }

    public void sendMessage() {
        if(sendInput.text.Length > 0) {
            TCPClient.getInstance().sendCHAT(sendInput.text);
            sendInput.text = "";
        }
    }

    public void addMessage(string message) {
        chatText.text += "\n" + message;
    }

    private void closeChatMenu() {
        chatMenuGameObject.SetActive(false);
    }

    private void openChatMenu() {
        chatMenuGameObject.SetActive(true);
    }

    public static ParchisChat getInstance() {
        return instance;
    }
}

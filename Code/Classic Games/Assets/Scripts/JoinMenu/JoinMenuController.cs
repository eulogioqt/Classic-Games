using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class JoinMenuController : MonoBehaviour {
    public GameObject joinMenuGameObject;
    public GameObject serverListContent;

    private List<ServerObject> serverList;
    private int selectedServer = -1;

    public InputField nameInput;

    public Button refreshButton;
    public Button addButton;
    public Button deleteButton;
    public Button joinButton;
    public Button directConexionButton;
    public Button exitButton;


    public GameObject addServerMenuGameObject;
    public InputField serverNameInput;
    public InputField serverIPInput;
    public InputField serverPortInput;

    public Button addConfirmButton;
    public Button cancelAddServerButton;


    public GameObject directConnectMenuGameObject;
    public InputField directIPInput;
    public InputField directPortInput;

    public Button directConexionConfirmButton;
    public Button cancelDirectConexionButton;

    private static JoinMenuController instance;

    private void Awake() {
        if (instance != null) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        joinMenuGameObject.SetActive(true);
        closeAddServerMenu();
        closeDirectConnectionMenu();

        if (PlayerPrefs.GetString("Name", "").Length > 0)
            nameInput.text = PlayerPrefs.GetString("Name");

        loadButtons();
        loadServers();
    }

    private void loadButtons() {
        refreshButton.onClick.AddListener(refreshList);
        addButton.onClick.AddListener(openAddServerMenu);
        deleteButton.onClick.AddListener(deleteServer);
        joinButton.onClick.AddListener(joinServer);
        directConexionButton.onClick.AddListener(openDirectConnectionMenu);
        exitButton.onClick.AddListener(Application.Quit);

        addConfirmButton.onClick.AddListener(addServer);
        cancelAddServerButton.onClick.AddListener(closeAddServerMenu);

        directConexionConfirmButton.onClick.AddListener(directConnection);
        cancelDirectConexionButton.onClick.AddListener(closeDirectConnectionMenu);
    }

    public void refreshList() {
        for(int i = 0; i < serverList.Count; i++)
            serverList[i].pingServer();

        goUpOnList();
        selectServer(-1);
        updateButtons();
    }

    public void directConnection() {
        try {
            int port = int.Parse(directPortInput.text);
            if (port >= 0 && port <= 65535) {
                ConnectionController.getInstance().tryConnecting(
                    IPAddress.Parse(directIPInput.text), port, getLocalUser());
            }
        } catch (Exception) { }
    }

    public void addServer(string serverName, IPAddress serverIP, int serverPort) {
        ServerObject newServer = createServer(serverName, serverIP, serverPort);
        serverList.Add(newServer);
        newServer.pingServer();
        updateList();
    }

    private void addServer() {
        try {
            int port = int.Parse(serverPortInput.text);
            if (port >= 0 && port <= 65535 && serverNameInput.text.Length > 0 && !serverNameInput.text.Contains(';') && !serverNameInput.text.Contains('@')) {
                string name = serverNameInput.text;
                IPAddress ip = IPAddress.Parse(serverIPInput.text);

                addServer(name, ip, port);
                PlayerPrefs.SetString("Servers", PlayerPrefs.GetString("Servers", "") + getSaveString(name, ip, port));
                closeAddServerMenu();
            }
        } catch (Exception) { }
    }

    public void deleteServer() {
        Destroy(serverList[selectedServer].gameObject);
        serverList.Remove(serverList[selectedServer]);

        string serverSave = "";
        for(int i = 1; i < serverList.Count; i++)
            serverSave += getSaveString(serverList[i]);
        PlayerPrefs.SetString("Servers", serverSave);

        updateList();
    }

    public void joinServer() {
        serverList[selectedServer].joinServer(getLocalUser());
    }

    public void selectServer(int index) {
        selectedServer = index;
        for (int i = 0; i < serverList.Count; i++)
            serverList[i].selectFrame(i == index);

        updateButtons();
    }

    public void setMenuActive(bool b) {
        joinMenuGameObject.SetActive(b);
    }

    private void openDirectConnectionMenu() {
        directIPInput.text = "";
        directPortInput.text = "";

        directConnectMenuGameObject.SetActive(true);
    }

    private void closeDirectConnectionMenu() {
        directConnectMenuGameObject.SetActive(false);
    }

    private void openAddServerMenu() {
        serverNameInput.text = "";
        serverIPInput.text = "";
        serverPortInput.text = "";

        addServerMenuGameObject.SetActive(true);
    }

    private void closeAddServerMenu() {
        addServerMenuGameObject.SetActive(false);
    }

    private void loadServers() {
        serverList = new List<ServerObject>();
        
        string defaultServer = getSaveString("default", IPAddress.Parse("192.168.100.2"), 11000);
        string serverLoad = defaultServer + PlayerPrefs.GetString("Servers", "");

        string[] lines = serverLoad.Substring(1).Split("@");
        foreach (string line in lines) {
            string[] data = line.Split(";");

            string serverName = data[0];
            IPAddress IP = IPAddress.Parse(data[1]);
            int port = int.Parse(data[2]);
            
            addServer(serverName, IP, port);
        }

        updateList();
    }

    private void updateButtons() {
        joinButton.interactable = selectedServer > -1;
        deleteButton.interactable = selectedServer > 0;
    }

    private void updateList() {
        selectServer(-1);

        int i;
        for (i = 0; i < serverList.Count; i++) {
            serverList[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -25 + -175 * i, 0);
            serverList[i].setIndex(i);
        }
        serverListContent.GetComponent<RectTransform>().sizeDelta = new Vector3(1200, 25 + 175 * i, 0);

        updateButtons();
    }

    private ServerObject createServer(string serverName, IPAddress IP, int port) {
        ServerObject sv = new GameObject().AddComponent<ServerObject>();
        sv.initServer(serverName, IP, port);
        sv.transform.SetParent(serverListContent.transform);
        return sv;
    }

    private string getSaveString(string name, IPAddress IP, int port) {
        return "@" + name + ";" + IP + ";" + port;
    }

    private string getSaveString(ServerObject sv) {
        return getSaveString(sv.getName(), sv.getIP(), sv.getPort());
    }

    public int getSelectedServer() {
        return selectedServer;
    }

    public User getLocalUser() {
        return new User(new UserData(nameInput.text), 0, 0);
    }

    private void goUpOnList() {
        serverListContent.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }

    public static JoinMenuController getInstance() {
        return instance;
    }
}

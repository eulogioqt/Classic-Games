using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinMenuController : MonoBehaviour
{
    public GameObject serverListContent;

    private List<ServerObject> serverList;
    private int selectedServer = -1;

    public Button refreshButton;
    public Button addButton;
    public Button deleteButton;
    public Button joinButton;

    private void Start() {
        serverList = new List<ServerObject>();

        refreshButton.onClick.AddListener(refreshList);
        addButton.onClick.AddListener(addServer);
        deleteButton.onClick.AddListener(deleteServer);
        joinButton.onClick.AddListener(joinServer);

        int i;
        for (i = 0; i < 2; i++)
            serverList.Add(createServer(i, "Server " + i));

        updateList();
    }

    public void refreshList() {
        updateList();
    }

    public void addServer() {
        serverList.Add(createServer(serverList.Count, "Server " + serverList.Count));

        updateList();
    }

    public void deleteServer() {
        serverList.Remove(serverList[selectedServer]);
        
        updateList();
    }

    public void joinServer() {
        serverList[selectedServer].joinServer();
    }

    public void selectServer(int index) {
        selectedServer = index;
        for (int i = 0; i < serverList.Count; i++)
            serverList[i].selectFrame(i == index);

        updateButtons();
    }

    private ServerObject createServer(int i, string serverName) {
        ServerObject sv = new GameObject().AddComponent<ServerObject>();
        sv.initServer(i, this, serverName);
        sv.transform.SetParent(serverListContent.transform);
        return sv;
    }

    private void updateButtons() {
        joinButton.interactable = selectedServer != -1;
        deleteButton.interactable = selectedServer != -1;
    }

    private void updateList() {
        selectServer(-1);

        int i;
        for (i = 0; i < serverList.Count; i++)
            serverList[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -175 * i, 0);
        serverListContent.GetComponent<RectTransform>().sizeDelta = new Vector3(1200, 175 * i, 0);

        updateButtons();
    }
}

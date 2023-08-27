using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using PARCHIS;

public class TableroColor : MonoBehaviour {
    private Player player = null;

    public GameObject nameGameObject;
    public Text nameText;

    public GameObject userImageGameObject;

    void Start() {
        nameGameObject.SetActive(false);
        userImageGameObject.SetActive(false);
    }

    void Update() {
        
    }

    public void setPlayer(Player player) {
        this.player = player;

        nameText.text = player.getPlayerName();
        nameGameObject.SetActive(true);
        userImageGameObject.SetActive(true);
    }

    public void unsetPlayer() {
        this.player = null;

        nameText.text = "";
        nameGameObject.SetActive(false);
        userImageGameObject.SetActive(false);
    }
}

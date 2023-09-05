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

    private void Start() {
        if (player == null)
            gameObject.SetActive(false);
    }

    public void setPlayer(Player player) {
        this.player = player;

        nameText.text = player.getPlayerName();
        gameObject.SetActive(true);
    }

    public void unsetPlayer() {
        this.player = null;

        nameText.text = "";
        gameObject.SetActive(false);
    }
}

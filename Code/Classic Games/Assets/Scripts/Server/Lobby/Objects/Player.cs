using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public bool isRealPlayer = false;
    private string playerName;

    private Vector3 newPosition = Vector3.zero;
    private Vector3 difference = Vector3.zero;

    private int totalFrames = 0;

    private void FixedUpdate() {
        if (!isRealPlayer) {
            if (newPosition != gameObject.transform.localPosition) {
                totalFrames++;

                gameObject.transform.localPosition += difference / UDPClient.FPU;
                if (totalFrames == UDPClient.FPU) {
                    totalFrames = 0;

                    gameObject.transform.localPosition = newPosition;
                    difference = Vector3.zero;
                }
            }
        } else if (newPosition != Vector3.zero) {
            gameObject.transform.localPosition = newPosition;
            newPosition = Vector3.zero;
        }
    }

    public void updatePosition(Vector2 position) {
        totalFrames = 0;
        newPosition = position;
        difference = newPosition - gameObject.transform.localPosition;
    }

    public string getName() {
        return playerName;
    }

    public void initPlayer(Vector2 position, string playerName) {
        initPlayer(playerName);

        newPosition = position;
        gameObject.transform.localPosition = position;
    }

    public void initPlayer(string playerName, bool isRealPlayer) {
        this.isRealPlayer = isRealPlayer;

        initPlayer(playerName);
    }

    public void initPlayer(string playerName) {
        this.playerName = playerName;

        gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<Image>();

        Image image2 = new GameObject("backgroundImage", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        image2.GetComponent<RectTransform>().sizeDelta = new Vector2(85, 85);
        image2.transform.SetParent(gameObject.transform);

        Image nameBackground = new GameObject("backgroundTextImage", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        nameBackground.transform.localPosition = new Vector2(0, 80);
        nameBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 40);
        nameBackground.transform.SetParent(gameObject.transform);

        Text nameText = new GameObject("nameText", typeof(RectTransform), typeof(Text)).GetComponent<Text>();
        nameText.text = playerName;
        nameText.transform.SetParent(nameBackground.transform);
        nameText.transform.localPosition = new Vector2(0, 0);
        nameText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(190, 30);
        nameText.resizeTextForBestFit = true;
        nameText.resizeTextMaxSize = 100;
        nameText.resizeTextMinSize = 14;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        nameText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Field").transform);
        gameObject.transform.localPosition = new Vector2(0, 0);
        gameObject.transform.localScale = Vector3.one;
        gameObject.GetComponent<Image>().color = Color.gray;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
    }
}

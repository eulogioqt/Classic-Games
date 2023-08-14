using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public void initPlayer(Vector2 position) {
        initPlayer();
        updatePosition(position);
    }

    public void initPlayer() {
        gameObject.AddComponent<RectTransform>();
        gameObject.AddComponent<Image>();
        gameObject.transform.parent = GameObject.FindGameObjectWithTag("Field").transform;
        gameObject.transform.localPosition = new Vector2(0, 0);
        gameObject.transform.localScale = Vector3.one;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
    }

    public void updatePosition(Vector2 position) {
        gameObject.transform.localPosition = position;
    }
}

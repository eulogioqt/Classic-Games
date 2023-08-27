using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class CGText : MonoBehaviour {
    public TMP_Text chatText;
    public TMP_Text shadowText;

    private static Dictionary<string, string> GameColor = new Dictionary<string, string>() {
        { "&0", "<color=#000000>" },
        { "&1", "<color=#0000AA>" },
        { "&2", "<color=#00AA00>" },
        { "&3", "<color=#00AAAA>" },
        { "&4", "<color=#AA0000>" },
        { "&5", "<color=#AA00AA>" },
        { "&6", "<color=#FFAA00>" },
        { "&7", "<color=#AAAAAA>" },
        { "&8", "<color=#555555>" },
        { "&9", "<color=#5555FF>" },
        { "&a", "<color=#55FF55>" },
        { "&b", "<color=#55FFFF>" },
        { "&c", "<color=#FF5555>" },
        { "&d", "<color=#FF55FF>" },
        { "&e", "<color=#FFFF55>" },
        { "&f", "<color=#FFFFFF>" },
        { "&l", "<b>" },
        { "&o", "<i>" },
        { "&n", "<s>" }
    };

    private static Dictionary<string, string> GameShadowColor = new Dictionary<string, string>() {
        { "&0", "<color=#000000>" },
        { "&1", "<color=#00002B>" },
        { "&2", "<color=#002B00>" },
        { "&3", "<color=#002B2B>" },
        { "&4", "<color=#2B0000>" },
        { "&5", "<color=#2B002B>" },
        { "&6", "<color=#402B00>" },
        { "&7", "<color=#2C2C2C>" },
        { "&8", "<color=#161616>" },
        { "&9", "<color=#161640>" },
        { "&a", "<color=#164016>" },
        { "&b", "<color=#164040>" },
        { "&c", "<color=#401616>" },
        { "&d", "<color=#401640>" },
        { "&e", "<color=#404016>" },
        { "&f", "<color=#404040>" },
        { "&l", "<b>" },
        { "&o", "<i>" },
        { "&n", "<s>" }
    };

    public static string transformToGameColors(string message, bool shadow) {
        StringBuilder transformedMessage = new StringBuilder();
        char[] msg = message.ToCharArray();

        bool isBold = false; int b = 0;
        bool isItalic = false; int it = 0;
        bool isUnderline = false; int u = 0;
        for (int i = 0; i < message.Length; i++) {
            if (msg[i] == '&') {
                if (i + 1 < msg.Length) {
                    string myChar = msg[i + 1].ToString().ToLower();
                    int listLength = typeof(ChatColor).GetFields().Length;

                    int j = 0;
                    while (j < listLength && !((string)typeof(ChatColor).GetFields()[j].GetValue(null))[1].ToString().ToLower().Equals(myChar))
                        j++;

                    if (j < listLength) {
                        string addText = (isBold ? "</b>" : "") + (isItalic ? "</i>" : "") + (isUnderline ? "</s>" : "");

                        bool isColor = false;
                        if (myChar == ChatColor.BOLD[1].ToString().ToLower()) {
                            isBold = true;
                            b++;
                        } else if (myChar == ChatColor.ITALIC[1].ToString().ToLower()) {
                            isItalic = true;
                            it++;
                        } else if (myChar == ChatColor.UNDERLINE[1].ToString().ToLower()) {
                            isUnderline = true;
                            u++;
                        } else {
                            if (isBold) b--;
                            if (isItalic) it--;
                            if (isUnderline) u--;

                            isBold = false;
                            isItalic = false;
                            isUnderline = false;
                            isColor = true;
                        }

                        string color = (isColor ? addText : "") +
                            (shadow ? GameShadowColor[(string)typeof(ChatColor).GetFields()[j].GetValue(null)] :
                            GameColor[(string)typeof(ChatColor).GetFields()[j].GetValue(null)]);
                        transformedMessage.Append(color);

                        i++;
                    } else transformedMessage.Append(msg[i]);
                }
            } else
                transformedMessage.Append(msg[i]);
        }

        for (int i = 0; i < b; i++)
            transformedMessage.Append("</b>");

        for (int i = 0; i < it; i++)
            transformedMessage.Append("</i>");

        for (int i = 0; i < u; i++)
            transformedMessage.Append("</s>");

        return (shadow ? GameShadowColor[ChatColor.WHITE] : GameColor[ChatColor.WHITE]) + transformedMessage.ToString();
    }

    public float getPreferredHeight() {
        return chatText.preferredHeight;
    }

    public void setText(string message) {
        chatText.text = transformToGameColors(message, false);
        shadowText.text = transformToGameColors(message, true);
    }

    public string getText() {
        return chatText.text;
    }

    bool firstMessage = true;
    public void addText(string message) {
        chatText.text += (firstMessage ? "" : "\n") + transformToGameColors(message, false);
        shadowText.text += (firstMessage ? "" : "\n") + transformToGameColors(message, true);

        if (firstMessage)
            firstMessage = false;
    }

    public void resetText() {
        firstMessage = true;

        chatText.text = "";
        shadowText.text = "";
    }

    public void setSizeDelta(Vector3 vector) {
        chatText.GetComponent<RectTransform>().sizeDelta = vector;
        shadowText.GetComponent<RectTransform>().sizeDelta = vector;
    }

    public Vector3 getSizeDelta() {
        return chatText.GetComponent<RectTransform>().sizeDelta;
    }
}

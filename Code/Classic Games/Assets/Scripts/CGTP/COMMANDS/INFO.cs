using System.Collections.Generic;
using UnityEngine;

public class INFO {
    private Dictionary<string, Player> users;
    private List<string> chat;

    private INFO(Dictionary<string, Player> users, List<string> chat) {
        this.users = users;
        this.chat = chat;
    }

    public Dictionary<string, Player> getUsers() {
        return users;
    }

    public List<string> getChat() {
        return chat;
    }

    public int getOnlineUsers() {
        return users.Count;
    }

    public static INFO process(string message) {
        string[] lines = message.Substring(5).Split('\n');

        Dictionary<string, Player> users = new Dictionary<string, Player>();
        List<string> chat = new List<string>();

        bool chatPart = false;
        foreach (string line in lines) {
            if (!chatPart) { // Parte de usuarios
                if (!line.Equals(".")) { // Si no es un punto
                    string key = line.Split(' ')[0];
                    string[] data = line.Substring(key.Length + 1).Split(';');
                    string name = data[0];
                    int x = int.Parse(data[1]);
                    int y = int.Parse(data[2]);

                    Player newPlayer = new GameObject(name, typeof(Player)).GetComponent<Player>();
                    newPlayer.initPlayer(new Vector2(x, y), name);

                    users.Add(key, newPlayer);
                } else chatPart = true; // Si es un punto cambia de parte
            } else if (!line.Equals(".")) { // Parte de chat, si no es el final, sigue
                chat.Add(line);
            }
        }

        return new INFO(users, chat);
    }
}

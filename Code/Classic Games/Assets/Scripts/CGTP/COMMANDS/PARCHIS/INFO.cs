
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PARCHIS {
    public class INFO {
        private Dictionary<string, Player> users;
        private TeamColor color;

        private INFO(Dictionary<string, Player> users, TeamColor color) {
            this.users = users;
            this.color = color;
        }

        public Dictionary<string, Player> getUsers() {
            return users;
        }

        public TeamColor getColor() {
            return color;
        }

        public static INFO process(string message) {
            string color = message.Substring(5).Split('\n')[0];
            string[] lines = message.Substring(5 + color.Length + 1).Split('\n');

            TeamColor myColor;
            Enum.TryParse(color, out myColor);

            Dictionary<string, Player> users = new Dictionary<string, Player>();

            foreach (string line in lines) {
                if (!line.Equals(".")) { // Si no es un punto
                    string key = line.Split(' ')[0];
                    string[] data = line.Substring(key.Length + 1).Split(';');
                    string name = data[0];
                    string c = data[1];

                    TeamColor playerColor;
                    Enum.TryParse(c, out playerColor);

                    Player newPlayer = new Player();
                    newPlayer.initPlayer(name, playerColor);

                    users.Add(key, newPlayer);
                }
            }

            return new INFO(users, myColor);
        }
    }
}

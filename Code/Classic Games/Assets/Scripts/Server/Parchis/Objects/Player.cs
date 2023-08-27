using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PARCHIS {
    public class Player : MonoBehaviour {
        private string playerName;
        private TeamColor color;

        public void initPlayer(string playerName, TeamColor color) {
            this.playerName = playerName;
            this.color = color;
        }

        public TeamColor getColor() {
            return color;
        }

        public string getPlayerName() {
            return playerName;
        }
    }
}

using System;

namespace PARCHIS {
    public class ON {
        private string key;
        private UserData data;

        private TeamColor color;

        private ON(string key, UserData data, TeamColor color) {
            this.key = key;
            this.data = data;

            this.color = color;
        }

        public string getKey() {
            return key;
        }

        public UserData getData() {
            return data;
        }

        public TeamColor getColor() {
            return color;
        }

        public static ON process(string message) {
            string key = message.Substring(3).Split(" ")[0];
            string[] data = message.Substring(3 + key.Length + 1).Split(";");

            TeamColor playerColor;
            Enum.TryParse(data[1], out playerColor);

            return new ON(key, new UserData(data[0]), playerColor);
        }
    }
}
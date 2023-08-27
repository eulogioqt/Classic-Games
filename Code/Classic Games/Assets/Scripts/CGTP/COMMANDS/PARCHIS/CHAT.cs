namespace PARCHIS {
    public class CHAT {
        private string key;
        private string message;

        private CHAT(string key, string message) {
            this.message = message;
        }

        public string getKey() {
            return key;
        }

        public string getMessage() {
            return message;
        }

        public static CHAT process(string message) {
            string key = message.Substring(5).Split(" ")[0];
            string msg = message.Substring(5 + key.Length + 1);

            return new CHAT(key, msg);
        }

        public static string getMessage(string message) {
            return "CHAT " + message;
        }
    }
}
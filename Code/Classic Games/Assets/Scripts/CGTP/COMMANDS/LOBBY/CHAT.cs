namespace LOBBY {
    public class CHAT {
        private string message;

        private CHAT(string message) {
            this.message = message;
        }

        public string getMessage() {
            return message;
        }

        public static CHAT process(string message) {
            return new CHAT(message.Substring(5));
        }

        public static string getMessage(string message) {
            return "CHAT " + message;
        }
    }
}

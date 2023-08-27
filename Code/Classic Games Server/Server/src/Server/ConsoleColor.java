package Server;

public enum ConsoleColor {
	BLACK("\033[30m"), 
	DARK_BLUE("\033[34m"), 
	DARK_GREEN("\033[32m"), 
	DARK_AQUA("\033[36m"), 
	DARK_RED("\033[31m"),
	DARK_PURPLE("\033[35m"), 
	GOLD("\033[33m"), 
	GRAY("\033[37m"), 
	DARK_GRAY("\033[90m"), 
	BLUE("\033[94m"),
	GREEN("\033[92m"), 
	AQUA("\033[96m"), 
	RED("\033[91m"), 
	LIGHT_PURPLE("\033[95m"), 
	YELLOW("\033[93m"),
	WHITE("\033[97m"),
	BOLD("\033[1m"),
	ITALIC("\033[2m"),
	UNDERLINE("\033[4m");

	private final String value;

	private ConsoleColor(String value) {
		this.value = value;
	}

	public static String transformToConsoleColors(String message) { // Replaces ChatColor codes efficiently
		char[] transformedMessage = new char[1024 * 8];
		char[] msg = message.toCharArray();

		int j = 0;
		boolean[] style = new boolean[4];
		for (int i = 0; i < message.length(); i++) {
			if (msg[i] == '&') {
				if (i + 1 < msg.length) {
					char myChar = Character.toLowerCase(msg[i + 1]);
					int listLength = ChatColor.values().length;

					int k = 0;
					while (k < listLength && ChatColor.values()[k].toString().charAt(1) != myChar)
						k++;

					if (k < listLength) {
						String addText = (style[0] ? "\033[22m" : "") + (style[1] ? "\033[23m" : "") + (style[2]  ? "\033[24m" : "");
						
						style[3] = false;
						if(myChar == ChatColor.BOLD.toString().charAt(1))
							style[0] = true;
						else if(myChar == ChatColor.ITALIC.toString().charAt(1))
							style[1] = true;
						else if(myChar == ChatColor.UNDERLINE.toString().charAt(1))
							style[2] = true;
						else 
							style = new boolean[] { false, false, false, true };
						
						char[] color = ((style[3] ? addText : "") + ConsoleColor.valueOf(ChatColor.values()[k].name())).toString().toCharArray();
						int codeLength = color.length;
						for (int h = 0; h < codeLength; h++) {
							transformedMessage[j] = color[h];
							j++;
						}
						
						i++;
					} else
						transformedMessage[j] = msg[i];
				}
			} else
				transformedMessage[j] = msg[i];
			j++;
		}
		
		return new String(transformedMessage) + ConsoleColor.WHITE + "\033[24m";
	}
	
	@Override
	public String toString() {
		return value;
	}
}

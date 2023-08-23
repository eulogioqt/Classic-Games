package Server;

enum ConsoleColor {
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

	@Override
	public String toString() {
		return value;
	}
}

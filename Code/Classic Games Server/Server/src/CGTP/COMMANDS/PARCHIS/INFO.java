package CGTP.COMMANDS.PARCHIS;

import java.util.List;

import Server.Parchis.Objects.ParchisUser;
import Server.Parchis.Objects.TeamColor;

public class INFO {
	public static String getMessage(TeamColor color, List<ParchisUser> users) {
		String INFO = "INFO " + color + "\n";
		for(ParchisUser user : users)
			INFO += user.getKey() + " " + user + "\n";
		INFO += ".";

		return INFO;
	}
}

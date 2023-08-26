package Server;

import java.io.FileWriter;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;

import Server.Lobby.LobbyServer;
import Server.Objects.ChatColor;

public class FileManager {
	public static void writeFile(String path, String fileName, String data) {
		try {
			try(FileWriter fileWriter = new FileWriter(path + "/" + fileName)) {
			    fileWriter.write(data);
			    fileWriter.close();
			}
		}  catch (IOException e) {
			LobbyServer.getServerConsole().sendMessage(ChatColor.RED + "Error al intentar escribir " + path + ": " + e.getMessage());
		}
	}
	
	public static String readFile(String path, String fileName, String IFNull) {
		String data = IFNull;
		
		try {
			data = new String(Files.readAllBytes(Paths.get(path + "/" + fileName)), StandardCharsets.UTF_8);
		} catch (IOException e) {
			if(!Files.exists(Paths.get(path))) {
				try {
					Files.createDirectory(Paths.get(path));
				} catch (IOException e1) {
					LobbyServer.getServerConsole().sendMessage(ChatColor.RED + "No se puede crear " + path + ": " + e.getMessage());
				}
			}
			writeFile(path, fileName, IFNull);
		}
		
		return data;
	}
}

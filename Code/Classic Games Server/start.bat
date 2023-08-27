@echo off
start cmd ´/k java -Xms1024M -Xmx1024M -jar CGParchisServer.jar -o true
java -Xms1024M -Xmx1024M -jar CGLobbyServer.jar -o true
PAUSE
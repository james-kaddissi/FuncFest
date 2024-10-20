# FuncFest

FuncFest is a party minigame style videogame that uses your mobile phone as the controller, and a single device running the host game. When the host launches the game and ideally connects it up to a TV (AirPlay, HDMI, etc.) all the players can launch the app over the same wifi and connect to the game and participate in a variety of fun party minigames.

## How it Works

A simple WebSockets local server is created over the IP of the host clients device. All the app clients are required to be on the same network to connect to the server automatically. Controls are sent via simple string messages between the game client, the server, and the app clients. All devices are given an ID to track who is emiting what, which is appended to every message string in some form. Host<->Server<->Apps. The clients use Unity and the NativeWebSockets library  to send and receive all messages.

## Repository Overview

UnityAppClient - contains the Unity project for the controller app

UnityHostClient - contains the games themselves that are displayed on the main screen

server.py - creates and runs the WebSockets server, and manages broadcasting messages to all clients

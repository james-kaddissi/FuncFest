import asyncio
import websockets
import threading
import math

player_id_counter = 1

async def echo(websocket, path):
    global player_id_counter
    player_id = player_id_counter
    connected_clients[websocket] = player_id
    player_id_counter += 1

    print(f"New player connected: {player_id}")
    await websocket.send(f"ID {player_id} connected to server")

    try:
        async for message in websocket:
            print(f"Received message from Player {player_id}: {message}")

            await websocket.send(f"ID: {player_id} sent: {message}")
            for client in connected_clients:
                if client != websocket:
                    await client.send(f"Player {player_id} action: {message}")
    finally:
        del connected_clients[websocket]
        print(f"Player {player_id} disconnected")

async def start_server():
    server = await websockets.serve(echo, "192.168.1.156", 6789)
    print("WebSocket server started on ws://192.168.1.156:6789")
    await server.wait_closed()

if __name__ == "__main__":
    connected_clients = {}
    server_thread = threading.Thread(target=asyncio.run, args=(start_server(),))
    server_thread.start()
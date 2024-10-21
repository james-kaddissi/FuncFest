import asyncio
import websockets
import threading

player_id_counter = 1
uuid_id_map = {}

async def echo(websocket, path):
    global player_id_counter
    player_id = None

    print("Player attempting to connect...")

    try:
        async for message in websocket:
            if message.startswith("uuid "):
                uuid = message.split(" ")[1]
                if uuid in uuid_id_map:
                    player_id = uuid_id_map[uuid]
                else:
                    player_id = player_id_counter
                    player_id_counter += 1
                    uuid_id_map[uuid] = player_id

                connected_clients[websocket] = player_id
                print(f"Player {player_id} connected")
                print(uuid_id_map)
                await websocket.send(f"Connected to server as Player {player_id}")
            elif message.startswith("host"):
                player_id = 99
                connected_clients[websocket] = player_id
                print("Host connected")
            else:
                if player_id is not None:
                    print(f"Received message from Player {player_id}: {message}")

                    await websocket.send(f"ID: {player_id} sent: {message}")
                    for client in connected_clients:
                        if client != websocket:
                            await client.send(f"Player {player_id} action: {message}")
                else:
                    print("Player not connected yet")
    finally:
        if player_id is not None:
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
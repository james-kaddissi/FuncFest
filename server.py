import asyncio
import websockets
import threading
import socket

player_id_counter = 1
uuid_id_map = {}

last_messages = {}

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
                for client, id in connected_clients.items():
                    if id == 99:
                        await client.send(f"Connected {player_id} {uuid}")

            elif message.startswith("host"):
                player_id = 99
                connected_clients[websocket] = player_id
                print("Host connected")
            else:
                if player_id is not None:

                    last_message = last_messages.get(player_id)

                    if last_message != message:
                        print(f"Received message from Player {player_id}: {message}")

                        last_messages[player_id] = message

                    await websocket.send(f"ID: {player_id} sent: {message}")
                    for client in connected_clients:
                        if client != websocket:
                            await client.send(f"Player {player_id} action: {message}")
                else:
                    pass
    finally:
        if player_id is not None:
            del connected_clients[websocket]
            print(f"Player {player_id} disconnected")

def get_ip_address():
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))
        ip_address = s.getsockname()[0]
    finally:
        s.close()
    return ip_address

async def start_server():
    ip = get_ip_address()
    server = await websockets.serve(echo, '0.0.0.0', 6789)
    print(f"WebSocket server started on ws://{ip}:6789")
    await server.wait_closed()



if __name__ == "__main__":
    connected_clients = {}
    server_thread = threading.Thread(target=asyncio.run, args=(start_server(),))
    server_thread.start()
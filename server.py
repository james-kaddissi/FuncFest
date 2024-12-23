import asyncio
import websockets
import threading
import socket
import time
import ipaddress
import netifaces

player_id_counter = 1
uuid_id_map = {}
last_messages = {}
connected_clients = {}

def calculate_broadcast_ip(ip, subnet_mask):
    network = ipaddress.IPv4Network(f'{ip}/{subnet_mask}', strict=False)
    return str(network.broadcast_address)

def get_default_interface():
    gateways = netifaces.gateways()
    default_gateway = gateways.get('default')
    if default_gateway:
        # Typically, the default gateway for IPv4 is at index 0
        interface = default_gateway[netifaces.AF_INET][1]
        return interface
    else:
        raise Exception("No default gateway found.")

def get_subnet_mask(interface):
    try:
        return netifaces.ifaddresses(interface)[netifaces.AF_INET][0]['netmask']
    except (ValueError, KeyError):
        raise Exception(f"Cannot get subnet mask for interface {interface}")

def get_ip_address(interface):
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        # Doesn't have to be reachable
        s.connect(("8.8.8.8", 80))
        ip_address = s.getsockname()[0]
    finally:
        s.close()
    return ip_address

def broadcast_ip(interface):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)

    server_ip = get_ip_address(interface)
    subnet_mask = get_subnet_mask(interface)
    broadcast_ip_address = calculate_broadcast_ip(server_ip, subnet_mask)
    message = f"Server IP: {server_ip}"

    print(f"Broadcasting on interface: {interface}")
    print(f"Server IP: {server_ip}")
    print(f"Subnet Mask: {subnet_mask}")
    print(f"Broadcast IP: {broadcast_ip_address}")

    while True:
        sock.sendto(message.encode(), (broadcast_ip_address, 6788))
        print(f"Broadcasted server IP: {server_ip} to {broadcast_ip_address}")
        time.sleep(1)

async def echo(websocket, path):
    global player_id_counter
    player_id = None

    print("Player attempting to connect...")

    try:
        async for message in websocket:
            if message.startswith("uuid "):
                parts = message.split(" ")
                if len(parts) < 3:
                    await websocket.send("Invalid UUID message format.")
                    continue
                uuid = parts[1]
                name = parts[2]
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
                        await client.send(f"Connected {player_id} {uuid} {name}")

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
                    await websocket.send("You are not registered. Please send a UUID.")
    finally:
        if player_id is not None:
            del connected_clients[websocket]
            print(f"Player {player_id} disconnected")

async def start_server():
    interface = get_default_interface()
    ip = get_ip_address(interface)
    server = await websockets.serve(echo, '0.0.0.0', 6789)
    print(f"WebSocket server started on ws://{ip}:6789")
    await server.wait_closed()

if __name__ == "__main__":
    try:
        interface = get_default_interface()
    except Exception as e:
        print(f"Error determining default interface: {e}")
        exit(1)

    server_thread = threading.Thread(target=asyncio.run, args=(start_server(),), daemon=True)
    server_thread.start()

    try:
        broadcast_thread = threading.Thread(target=broadcast_ip, args=(interface,), daemon=True)
        broadcast_thread.start()
    except Exception as e:
        print(f"Error starting broadcast thread: {e}")
        exit(1)

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Shutting down server...")
        exit(0)
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

def get_ip_address(interface):

    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        s.connect(("8.8.8.8", 80))
        ip_address = s.getsockname()[0]
    except Exception as e:
        raise Exception(f"Cannot get IP address for interface {interface}: {e}")
    finally:
        s.close()
    return ip_address

def get_active_interfaces():
    interfaces = netifaces.interfaces()
    active_interfaces = []
    for interface in interfaces:
        addrs = netifaces.ifaddresses(interface)
        if netifaces.AF_INET in addrs:
            inet_info = addrs[netifaces.AF_INET]
            for addr in inet_info:
                if 'addr' in addr and addr['addr'] != '127.0.0.1':
                    active_interfaces.append({
                        'interface': interface,
                        'ip': addr['addr'],
                        'netmask': addr['netmask'],
                        'broadcast': addr.get('broadcast')
                    })
    return active_interfaces

def get_broadcast_addresses(interfaces):
    broadcast_addresses = []
    for iface in interfaces:
        if iface['broadcast']:
            broadcast_addresses.append((iface['broadcast'], iface['interface']))
    return broadcast_addresses

def broadcast_ip(broadcast_addresses):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)

    print("Starting UDP broadcast on the following addresses:")
    for addr, iface in broadcast_addresses:
        print(f" - {addr}:6788 on interface {iface}")

    while True:
        for addr, iface in broadcast_addresses:
            try:
                server_ip = get_ip_address(iface)
                message = f"Server IP: {server_ip}"
                sock.sendto(message.encode(), (addr, 6788))
                print(f"Broadcasted message to {addr}:6788 -> '{message}'")
            except Exception as e:
                print(f"Error broadcasting to {addr}:6788 -> {e}")
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
    server = await websockets.serve(echo, '0.0.0.0', 6789)
    print(f"WebSocket server started on ws://0.0.0.0:6789")
    await server.wait_closed()

if __name__ == "__main__":
    active_interfaces = get_active_interfaces()
    if not active_interfaces:
        print("No active network interfaces found.")
        exit(1)

    broadcast_addresses = get_broadcast_addresses(active_interfaces)
    if not broadcast_addresses:
        print("No broadcast addresses found on active interfaces.")
        exit(1)

    server_thread = threading.Thread(target=lambda: asyncio.run(start_server()), daemon=True)
    server_thread.start()

    broadcast_thread = threading.Thread(target=broadcast_ip, args=(broadcast_addresses,), daemon=True)
    broadcast_thread.start()

    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        print("Shutting down server...")
        exit(0)

# -*- coding: utf-8 -*-
"""
Created on Fri Jan 10 21:08:19 2020

@author: Anthony Melin
"""


__all__ = [
    "ServerGui",
    "main",
    ]


import os
import socket
import socketserver
import http.server
import threading
import websockets
import asyncio

try:
    from ServerGuiSaver import ServerGuiSaver, getParametersFilename
except ModuleNotFoundError:
    from PyRemoteCam.ServerGuiSaver import ServerGuiSaver, getParametersFilename


###############################################################################
class ServerGui(ServerGuiSaver):
    
    ###########################################################################    
    def __init__(self):
        
        ServerGuiSaver.__init__(self, getParametersFilename())
        
        self.protocol("WM_DELETE_WINDOW", self.close)
        self.serveButton["command"] = self.serve
        self.closeButton["command"] = self.close
        
        self.isOnline = False
            
    
    ###########################################################################
    async def WSConnection(self, sock, path):
        
        intAdress = ("127.0.0.1", int(self.parameters["intUdpPort"]))
        extAdress = ("127.0.0.1", int(self.parameters["extUdpPort"]))
    
        udpConnection = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        udpConnection.bind(intAdress)
        udpConnection.settimeout(1)
        
        self.status["text"] = "Status: client connected"
    
        while True:
            
            try:
                await sock.send("send")
                data = await sock.recv()
            except websockets.exceptions.ConnectionClosedError:
                break
            else:
                try:
                    udpConnection.recv(4096)
                except socket.timeout:
                    pass
                else:
                    try:
                        udpConnection.sendto(data.encode(), extAdress)
                    except OSError:
                        pass
        
        udpConnection.close()
        self.status["text"] = "Status: online"
    
    
    ###########################################################################
    def serveHttp(self):
    
        host = self.parameters["host"]
        port = int(self.parameters["httpPort"])
        handler = http.server.SimpleHTTPRequestHandler
        
        server = socketserver.TCPServer((host, port), handler)
        threading.Thread(target=server.serve_forever, args=(), daemon=True).start()
    
    
    ###########################################################################
    def serveWebsocket(self):
    
        websocket = websockets.serve(self.WSConnection, self.parameters["host"], self.parameters["websocketPort"])
        asyncio.get_event_loop().run_until_complete(websocket)
        threading.Thread(target=asyncio.get_event_loop().run_forever, args=(), daemon=False).start()
    
    
    ###########################################################################
    def serve(self):
    
        self.saveEntries()
        
        if not self.isOnline:
            
            self.serveHttp()
            self.serveWebsocket()
    
            self.status["text"] = "Status: online"
            self.isOnline = True
            
            
    ###########################################################################
    def close(self):
    
        asyncio.get_event_loop().stop()
        self.destroy()
        os._exit(0)
    
    

###############################################################################
def getPackagePath():
    
    package_path, package_name = os.path.split(__file__)
    return package_path


###############################################################################
def main():
    
    path = getPackagePath()
    if path != '': os.chdir(path)
    
    app = ServerGui()
    app.mainloop()


###############################################################################
if __name__ == "__main__":
    main()


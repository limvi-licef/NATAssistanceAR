# -*- coding: utf-8 -*-

"""
@author: Anthony Melin
@date: 5/4/2020
"""


import os
import socket
import websockets
import asyncio


    
###############################################################################
class WebsocketServer:
    
    
    ###########################################################################
    def __init__(self):
            
        self.websocket = None
        self._frame = None
        
    
    ###########################################################################
    def serve(self, host, ext_port, int_port):
        
        self.udp = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.int_port = int_port
        
        self.websocket = websockets.serve(self._connection, host, ext_port)
        
        try:
            asyncio.get_event_loop().run_until_complete(self.websocket)
            asyncio.get_event_loop().run_forever()
        except RuntimeError:
            pass
        
        
    
    ###########################################################################
    def close(self):
        
        self.websocket.ws_server.close()
        

    ###########################################################################
    async def _connection(self, sock, path):
        
        print("connect", sock.remote_address)
        
        while True:
            
            try:
                await self._handler(sock, path)
                
            except websockets.exceptions.ConnectionClosedError:
                print("close by client")
                self.close()
                os._exit(0)
                break
            
            except websockets.exceptions.ConnectionClosedOK:
                print("close by server")
                break


    ###########################################################################
    async def _handler(self, sock, path):
        
        await sock.send("send")
        frame = await sock.recv()

        size = 65000
        for n, start in enumerate(range(0, len(frame), size)):
            chunk = frame[start:start+size]
            self.udp.sendto(chunk.encode(), ("127.0.0.1", self.int_port+n))
        

###############################################################################
def main(ip, ext_port, int_port):

    server = WebsocketServer()
    server.serve(ip, ext_port, int_port)


###############################################################################
if __name__ == "__main__":
    
    import sys
    
    if len(sys.argv) == 4:
        
        ip = sys.argv[1]
        ext_port = int(sys.argv[2])
        int_port = int(sys.argv[3])
        
        main(ip, ext_port, int_port)
        
    
    
# -*- coding: utf-8 -*-
"""
Created on Fri Jan 10 20:17:01 2020

@author: Anthony Melin
"""


import tkinter as tk


###############################################################################
class ServerGuiCore(tk.Tk):
    
    ###########################################################################    
    def __init__(self):
        
        tk.Tk.__init__(self)
        self.title("PyRemoteCam")
    
        tk.Label(self, text="Host").grid(row=0, column=0)
        self.host = tk.Entry(self, width=20)
        self.host.grid(row=0, column=1)
    
        tk.Label(self, text="Http port").grid(row=1, column=0)
        self.httpPort = tk.Entry(self, width=20)
        self.httpPort.grid(row=1, column=1)
    
        tk.Label(self, text="Websocket port").grid(row=2, column=0)
        self.websocketPort = tk.Entry(self, width=20)
        self.websocketPort.grid(row=2, column=1)
    
        tk.Label(self, text="External udp port").grid(row=3, column=0)
        self.extUdpPort = tk.Entry(self, width=20)
        self.extUdpPort.grid(row=3, column=1)
    
        tk.Label(self, text="Internal udp port").grid(row=4, column=0)
        self.intUdpPort = tk.Entry(self, width=20)
        self.intUdpPort.grid(row=4, column=1)
    
        self.serveButton = tk.Button(self, text="Serve", width=10)
        self.serveButton.grid(row=5, column=0)
    
        self.closeButton = tk.Button(self, text="Exit", width=10)
        self.closeButton.grid(row=5, column=1)
    
        self.status = tk.Label(self, text="Status: offline")
        self.status.grid(row=6, column=0, columnspan=2)

        
        
###############################################################################
if __name__ == "__main__":
    app = ServerGuiCore()
    app.mainloop()
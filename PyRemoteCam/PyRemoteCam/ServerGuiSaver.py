# -*- coding: utf-8 -*-
"""
Created on Fri Jan 10 20:37:18 2020

@author: Anthony Melin
"""


import tkinter as tk
import json
import os

try:
    from ServerGuiCore import ServerGuiCore
except ModuleNotFoundError:
    from PyRemoteCam.ServerGuiCore import ServerGuiCore


###############################################################################
def getParametersFilename():
    
    module_path, module_name = os.path.split(__file__)
    return os.path.join(module_path, "parameters.json")


###############################################################################
class ServerGuiSaver(ServerGuiCore):
    
    ###########################################################################
    def __init__(self, file):
        
        ServerGuiCore.__init__(self)
        
        self.file = file
        self.loadEntries()
        
        self.host.insert(tk.END, self.parameters["host"])
        self.httpPort.insert(tk.END, self.parameters["httpPort"])
        self.websocketPort.insert(tk.END, self.parameters["websocketPort"])
        self.extUdpPort.insert(tk.END, self.parameters["extUdpPort"])
        self.intUdpPort.insert(tk.END, self.parameters["intUdpPort"])
        
        
    ###########################################################################
    def loadEntries(self):
        
        try:
            with open(self.file) as file:
                data = file.read()
                self.parameters = json.loads(data)
                
        except FileNotFoundError:
            self.parameters = {}
            self.parameters["host"] = "192.168.1.1"
            self.parameters["httpPort"] = 9996
            self.parameters["websocketPort"] = 9999
            self.parameters["extUdpPort"] = 9998
            self.parameters["intUdpPort"] = 9997
        
    
    ###########################################################################
    def saveEntries(self):
        
        self.parameters = {}
        self.parameters["host"] = self.host.get()
        self.parameters["httpPort"] =self. httpPort.get()
        self.parameters["websocketPort"] = self.websocketPort.get()
        self.parameters["extUdpPort"] = self.extUdpPort.get()
        self.parameters["intUdpPort"] = self.intUdpPort.get()
    
        with open(self.file, mode="w") as file:
            data = json.dumps(self.parameters)
            file.write(data)
            


###############################################################################
if __name__ == "__main__":

    app = ServerGuiSaver(getParametersFilename())
    app.serveButton["command"] = app.saveEntries
    app.mainloop()

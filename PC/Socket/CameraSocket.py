"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package CameraSocket
# Module defining a specific socket for receive frames

# coding: utf-8

try:
    # for importation from outside this directory
    from Socket.UdpConnection import UdpConnection
except:
    # for importation from here
    from UdpConnection import UdpConnection
    

import numpy as np
import cv2
import matplotlib.pyplot as plt


"""
###############################################################
#                      UdpDataSocket                          #
#                                                             #
# AskData(self)                                               #
# RecvHeader(self)                                            #
# FormatHeader(self, header)                                  #
# RecvPackets(self)                                           #
#                                                             #
###############################################################
"""
## Inherited from UdpConnection.
# Udp server for asking data in byte format to connected client.
class UdpDataSocket(UdpConnection):
    
    data = b""
    data_readable = False
    
    
    """###########################################################"""
    ## Request data to the client.
    # UdpDataSocket.data_readable indicate if received data are valid. If so, it's stored in UdpDataSocket.data
    def AskData(self):
        
        if not self.IsConnected():
            self.echo("Request not possible: client not connected")
            
        else:
            
            self.echo("Send request for data to client")
            self.sendto(b"\xff", self.client)
            
            self.header = self.RecvHeader()
            if self.header:
                self.data_readable, self.data = self.RecvPackets()
                

    """###########################################################"""
    ## Wait for a reader that indicate information such as numbre of packet that the client will send
    # Return the result of FormatHeader if received the header otherwise return false
    def RecvHeader(self):
        
        size, header = self.WaitMsg(32, 1, "header") # wait the header
        
        # if received
        if size > 0:
            self.echo("Received header")
            return self.FormatHeader(header)
        
        else:
            return False
    
    
    """###########################################################"""
    ## Method for specify how to read the header. Return a dictionnary containing informations
    # must be overriden
    # @param header a byte array 
    def FormatHeader(self, header):
        
        return {"packet":0}
        
    
    """###########################################################"""
    ## Method that receive all packets from client en gather it to restore data
    # return a tuple like (data valid as boolean, data as byte array)
    def RecvPackets(self):
        
        data = b""
        
        # loop as many as packet to receive
        for n in range(self.header["packet"]):
            
            self.sendto(b"\xfe", self.client) # send message for next packet
            received, packet = self.WaitMsg(65000, 1) # wait the packet
            
            if not received: return False, self.data # in case of timeout or error
            else: data += packet # concat the packet to data
        
        return True, data



"""
###############################################################
#                     UdpCameraSocket                         #
#                                                             #
#  FormatHeader(self, header)                                 #
#  GetFrame(self)                                             #
#                                                             #
###############################################################
"""
## Inherited from UdpDataSocket.
# Receive frames as data and make sure it's readable
class CameraSocket(UdpDataSocket):
    
    
    """###########################################################"""
    ## Overrided method. Extract the number of packet to receive from the header.
    # 
    def FormatHeader(self, header):
        
        packet = header[0] # packet is the 1st byte received. It's implicitly converted to int
        self.echo("packet: {}".format(packet))
            
        return {"packet":packet}
        

    """###########################################################"""
    ## Ask and return a frame. It works only with jpeg as decoder is integrated inside.
    # Frame is returned as numpy array. All values are set to 0 if image is not valid. 
    def GetFrame(self):

        self.AskData()

        # try to decode frame
        try:
            frame = cv2.imdecode(np.frombuffer(self.data, np.uint8), -1) # decode the frame
            
            # in case of error
            if frame is None:
                self.ClearReception()
                frame = np.zeros((720,1280,3)) # empty array
        except:
            self.ClearReception()
            frame = np.zeros((720,1280,3))

        return frame
    
"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package RayCollisionSocket
# Module defining a specific socket for ray hit test

# coding: utf-8

try:
    # for importation from outside this directory
    from Socket.UdpConnection import UdpConnection
except:
    # for importation from here
    from UdpConnection import UdpConnection
    

import numpy as np

"""
####################################################################
#                      UdpRayCollisionSocket                       #
#                                                                  #
#  AskPositions(self, coords)                                      #
#  Format(self, coords)                                            #
#  Unzip(self, packet)                                             #
#                                                                  #
####################################################################
"""
## Inherited from UdpConnection. Socket for receiving 3D coordinates coresponding to 2D coordinates sended previously
class RayCollisionSocket(UdpConnection):
    
    
    """####################################################################"""
    ## Request to the client the associated coordinates in space of the 2D coordinates.
    # @param coords a list of 2D coordinates type (x, y) with normalised values coresponding to the client camera view.
    # return a list of coordinates type (x, y, z) with values exprimed in meters or an empty list if no reply is received.
    def AskPositions(self, coords):
        
        if not self.IsConnected():
            self.echo("Request not possible: client not connected")
            
        # if coords list is empty
        elif len(coords) == 0:
            return []
            
        else:
            message = self.ToBytes(coords) # convert list to message
            
            # send the 2D coordinates
            self.sendto(message, self.client)
            self.echo("Send request for positions to client")
            
            received, packet = self.WaitMsg(65000, 1) # await coordinates
            # convert packet (string) to list if received
            if received > 0:
                return self.ToArray(packet)
            else:
                return []
            
    
    """####################################################################"""
    ## Convert the 2D coordinates list to a string for being sendable
    # @param coords list of 2D coordinates like [(x1,y1), (x2,y2), (x3,y3), ... ]
    # return a string as byte array like x1,y1;x2,y2;x3,y3;... 
    def ToBytes(self, coords):
        
        message = ""
        
        # for each coordinates, concat to the message x and y value
        for x, y in coords:
            message += "{},{};".format(x,y)
            
        return message[:-1].encode() #remove the last character (a useless ;) and convert to bytes
    
    
    """####################################################################"""
    ## Convert the received message to 3D coordinate list
    # @param packet bytes received from client, format : x1,y1,z1;x2,y2,z2;x3,y3,z3;
    # return an array like [ (x1,y1,z1), (x2,y2,z2), (x3,y3,z3), ... ] 
    def ToArray(self, packet):
        
        packet = packet.decode()[:-1] # convert bytes to string and remove last character (a useless ;)
        
        vectors = packet.split(";")
        
        # foreach vector as single string, split into 3 values (x, y, z)
        for n, vec in enumerate(vectors):
            vectors[n] = vec.split(",")
            
        return np.asarray(vectors).astype(float)

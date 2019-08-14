"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package AnnotationSocket
# Module defining a specific socket for sending annotation command

# coding: utf-8

try:
    # for importation from outside this directory
    from Socket.UdpConnection import UdpConnection
except:
    # for importation from here
    from UdpConnection import UdpConnection

"""
###############################################################
#                     AnnotationSocket                        #
#                                                             #
#  Draw(self, cmd, vector, *args)                             #
#  Clear(self)                                                #
#                                                             #
###############################################################
"""
## Inherited from UdpConnection.
# Specific socket for sending display command
class AnnotationSocket(UdpConnection):
    
    
    """###########################################################"""
    ## Send the command and a vector to client
    # @param cmd string, command for displaying
    # @param vector tuple representing the 3D coordinates of what the command call to display, values (x, y, z) may be float
    # @param *args optional args, must be a type that could be converted to float
    def Draw(self, cmd, *args):
        
        if not self.IsConnected():
            self.echo("Request not possible: client not connected")
            
        else:
            # add command to message
            message = cmd+";"

            # add each optional argument
            for arg in args:
                message += str(arg)+";"

            # send command and wait a message that indicate the command is applied
            self.sendto(message.encode(), self.client)
            self.WaitMsg(32, 1)

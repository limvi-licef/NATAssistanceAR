# -*- coding: utf-8 -*-
"""
Created on Sat Jan 11 19:27:30 2020

@author: Anthony Melin
"""


__all__ = [
    "VideoReceiver",
    "main",
    ]


import cv2
import numpy as np
import base64
import socket


###################################################################
def decodeJpeg(js_data):
    
    base64_sequence = js_data[23:]
    jpg_sequence = base64.b64decode(base64_sequence)
    str_sequence = np.fromstring(jpg_sequence, np.uint8)
    
    return cv2.imdecode(str_sequence, 1)


###################################################################
class VideoReceiver(socket.socket):

    
    ###############################################################
    def __init__(self, intPort, extPort, timeout=1):

        self.host = "127.0.0.1"
        self.intPort = intPort
        self.extPort = extPort

        socket.socket.__init__(self, socket.AF_INET, socket.SOCK_DGRAM)
        self.bind((self.host, self.extPort))
        
        self.settimeout(timeout)
        
        self.novideo = np.zeros((480,640))
        cv2.putText(self.novideo,'No video', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (255,255,255), 2)
        

    ###############################################################
    def getFrame(self):
        
        try:
            self.sendto(b"ok", (self.host,self.intPort))
            data = self.recv(65000).decode()
        except ConnectionResetError:
            return self.novideo
        except socket.timeout:
            return self.novideo
        else:
            return decodeJpeg(data)


def main():

    IntUDP = 9997
    ExtUDP = 9998
    cam = VideoReceiver(IntUDP,ExtUDP)

    while True:
        frame = cam.getFrame()
        
        cv2.imshow('image', frame)
        if cv2.waitKey(1) == ord("a"): break
        
    cv2.destroyAllWindows()


if __name__ == "__main__":

    main()

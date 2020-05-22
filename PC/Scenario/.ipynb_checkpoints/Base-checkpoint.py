"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package Base
# Module defining a base class for managing object detection


# -*- coding: utf-8 -*-


import numpy as np
import glm


"""################################################################"""
## Return the length between two points in 3D space
# @param pos1 and pos2 are glm.vec3 (x, y, z)
def length(pos1, pos2):
    
    v1 = glm.vec3(pos1)
    v2 = glm.vec3(pos2)
    
    return glm.length(v2-v1)


"""####################################################################"""
## Base class for managing detected objects and configure application actions. 
# obj attribute is a dictionnary that associate an object name as key to its position as glm.vec3.
# first_detection and excluded_detection are respectively used to indicate in child class object that an object is detected for the first time and that its position must not be updated.
# sensibility is the length (meter in real world) for wich object position is not updated if its last move length is less.
class BaseScenario:
    

    """################################################################"""
    ## Constructor that init the obj and excluded_detection
    def __init__(self):
        
        self.obj = {}
        self.excluded_detection = []
        self.sensibility = 0.1
        

    """################################################################"""
    ## Method to call in the mainloop to execute scenario.
    # @param detection dictionnary provided by the frame detection that include 3D position
    # @param socket the annotation socket to use for sending display command
    def Update(self, detection, socket, frame=None):
        
        # set these as attribute in order to let it accessible in child class
        self.socket = socket
        self.frame = frame
        
        # list of object detected for the first time
        self.first_detection = []
        
        # enumerate names of detected objects
        for n, obj in enumerate(detection["classes_names"]):
            
            x, y, z = detection["positions"][n] # position of the object
            
            # if the object is excluded from the detection
            if obj in self.excluded_detection:
                pass
            
            # if it's the first detection, send a draw command and add the object to the list
            elif obj not in self.obj:
                self.obj[obj] = glm.vec3(detection["positions"][n])
                self.first_detection.append(obj)
                
            # for known objects, update the position if it moves more than 7cm
            elif length((x,y,z), self.obj[obj]) > self.sensibility:
                self.obj[obj] = glm.vec3(detection["positions"][n])
                
        # method where decision are made
        self.Action()
                
            
    """################################################################"""
    ## Method to override in child class. The scenario must be defined inside
    def Action(self):
        
        pass
    

    """################################################################"""
    ## Return the length between position of 2 objects
    def Length(self, key1, key2):

        return length(self.obj[key1], self.obj[key2])
    

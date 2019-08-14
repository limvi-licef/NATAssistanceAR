"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package ObjectDetector
# Define ObjectDetector class for automatisation of somes processes for object detection

# -*- coding: utf-8 -*-

try:
    # when imported from outside
    from Tensorflow.ObjectDetection import *
except:
    # when imported from its directory
    from ObjectDetection import *

import cv2
import numpy as np

"""
##################################################################################
#                              ObjectDetector                                    #
#                                                                                #
#  __init__(self, session, image_tensor, tensors_list, classes_names)            #
#  SetThreshold(self, threshold)                                                 #
#  ApplyThresold(self, boxes, scores, classes)                                   #
#  BoundingboxCenter(self, box_coords)                                           #
#  IncludeCenters(self, detected)                                                #
#  DrawCenters(self, frame, detected)                                            #
#  Detect(self, frame)                                                           #
#                                                                                #
##################################################################################
"""
## Detector of object on a frame using a trained model
class ObjectDetector:
    
    
    """##############################################################################"""
    ## Constructor that set as attributes the model input and output tensors
    # @param session tensorflow session loaded with the model
    # @param image_tensor tensor loaded with the model that will receive frames
    # @param tensors_list list of outputs tensors loaded with the model
    # @param classes_names object names that the object is able to detect
    def __init__(self, session, image_tensor, tensors_list, classes_names):
        
        self.threshold = 0.5
        self.draw = False
    
        self.session = session
        self.tensorsList = tensors_list
        self.imageTensor = image_tensor
        self.classesNames = classes_names
        
    
    """##############################################################################"""
    ## Set the threshold used for exclude low confident detections
    # @param threshold int, a percentage range 0 to 100
    def SetThreshold(self, threshold):
        
        assert 0<threshold<101, "Threshold value not valid (expect percentage)"
        self.threshold = threshold / 100
        
    
    """##############################################################################"""
    ## Apply the threshold attribute to the last detection results
    # @param boxes, array of bounding box
    # @param scores, array of float
    # @param classes, array of index
    # return a dictionnary with contains arrays of bounding boxes (key = boxes), scores (key = scores), object indexes (key = classes) and object names (key = classes_names)
    def ApplyThresold(self, boxes, scores, classes):
        
        selector = (scores > self.threshold) # selector is a list of index where values match the condition (see numpy)
        
        # update the outputs tensor according to se selection above  
        b = boxes[selector]
        s = scores[selector]
        c = classes[selector]
        
        # make the array with object names instead of index
        cn = []
        for n, index in enumerate(c):
            cn.append(self.classesNames[int(index)]["name"])
        
        return { "boxes": b, "scores": s, "classes": c, "classes_names": cn }
        
    
    """##############################################################################"""
    ## Calculate the center of a bounding box
    # @param bbox bounding box from a detection
    # return a 2D coordinate (x, y)
    def BoundingboxCenter(self, bbox):
        
        ymin = bbox[0]
        xmin = bbox[1]

        ymax = bbox[2]
        xmax = bbox[3]

        return (xmin+xmax)/2, (ymin+ymax)/2
    
    
    """##############################################################################"""
    ## Build a the list of the objects centers
    # @param detected dictionnary of detection returned by ApplyThresold
    def IncludeCenters(self, detected):
    
        detected["centers"] = []
        
        # for each bounding box, add its center
        for box in detected["boxes"]:
            center = self.BoundingboxCenter(box)
            detected["centers"].append(center)
    
    
    """##############################################################################"""
    ## Represent the calculated centers on the frame
    # @param frame numpy array representing a frame
    # @param detected dictionnary of data provided by the detection
    def DrawCenters(self, frame, detected):
        
        # get frame size
        height, width, color = frame.shape
        
        # for each center, get the bounding box color and draw a small circle
        for n, (x, y) in enumerate(detected["centers"]):
            
            # get the color in bounding box corner
            x_min, y_min = int(detected["boxes"][n][1] * width), int(detected["boxes"][n][0] * height)
            color = frame[y_min][x_min].astype(float)
            
            # draw the circle
            cv2.circle(frame, (int(x*width), int(y*height)), 8, color, -1)
    
    
    """##############################################################################"""
    ## Main method to call for execute a detection on frame
    # @param frame numpy array representing a frame
    # return a dictionnary including tensorflow detection and calculated parameters as centers
    def Detect(self, frame):
        
        # tensorflow detection
        frame_expanded = np.expand_dims(frame, axis=0)
        (boxes, scores, classes, num) = self.session.run(self.tensorsList, feed_dict={self.imageTensor: frame_expanded})
        
        # exclude unsignificant detections and calculate centers
        detected = self.ApplyThresold(boxes, scores, classes)
        self.IncludeCenters(detected)
            
        # draw bounding box and centers if enabled
        if self.draw:
            draw_bounding_boxes(frame, boxes, classes, scores, self.classesNames, self.threshold, 4)
            self.DrawCenters(frame, detected)
            
        return detected
    
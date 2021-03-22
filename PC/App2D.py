#!/usr/bin/env python
# coding: utf-8

# -------------------------
# ### Author: Anthony Melin
# ### Date: 2020 January 11
# -------------------------

# # Import

# In[1]:


from RemoteCam.RemoteCam import RemoteCam
from Tensorflow.ObjectDetection import *
from Tensorflow.ObjectDetector import ObjectDetector

from Scenario.Base2D import BaseScenario2D

import cv2
import sys
import numpy as np
import os


# # Global variables
# ## Model files and classes
# MODEL_DIRECTORY = "MODEL_DIRECTORY"
MODEL_DIRECTORY = MODEL_DIRECTORY = os.path.join(os.path.abspath(''),"../ModelsForNAT/model_3")

GRAPH = MODEL_DIRECTORY + "/frozen_inference_graph.pb"
LABELS = MODEL_DIRECTORY + "/labelmap.pbtxt"
NUM_CLASSES = 7


# ## Scenario
scenario = BaseScenario2D()


# # Object detection configuration

# ### Load model
category_index = load_categories(LABELS, NUM_CLASSES)
sess, inputs, outputs = load_model(GRAPH)


# ### Set frame detector
detector = ObjectDetector(sess, inputs, outputs, category_index)
detector.SetThreshold(60)
detector.draw = True


# # Loop
camera = RemoteCam(10000, 3)

while True:

    # frame
    frame = camera.getFrame()
    
    if frame.shape == (720, 1280, 3):
        
        # detection
        detected = detector.Detect(frame)
        scenario.Update(detected, frame)
    
    # display
    cv2.imshow("frame", frame)
    if cv2.waitKey(1) == ord('q'): break

cv2.destroyAllWindows()
camera.close()



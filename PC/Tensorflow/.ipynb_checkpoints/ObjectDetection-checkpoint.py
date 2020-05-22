"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package ObjectDetection
# provide an easy way to integrate tensorflow extra libraries

# -*- coding: utf-8 -*-


import os
import sys


"""###############################################################################################"""
## function that return the tensorflow path on the computer
# must be completed before using tensorflow extra libraries
def tensorflow_path():
    
#     return "WRITE THE PATH OF TENSORFLOW ON THE COMPUTER HERE"
    return "C:/MesProgrammes/TensorFlow"


"""###############################################################################################"""
## function that return the object detection api path on the computer
# Don't forget to complete the tensorflow_path function
def object_detection_path():
    
    return os.path.join(tensorflow_path(), "research", "object_detection")


"""###############################################################################################"""
## Add tensorflow extras libraries such as object detection to the sys path in order to make them reachable everywhere
# Called when its module is imported
# Don't forget to complete the tensorflow_path() function
def import_object_detection():
    
    path = tensorflow_path()
    default_path = "WRITE THE PATH OF TENSORFLOW ON THE COMPUTER HERE"
    
    assert path != default_path, "tensorflow path is not set in tensorflow_path() function from Tensorflow.lib"
    
    # path required for using object detection api
    paths = [
        path,
        os.path.join(path, "research"),
        os.path.join(path, "research", "object_detection"),
        os.path.join(path, "research", "slim")
    ]
    
    for p in paths:
        if p not in sys.path:
            sys.path.append(p)


"""###############################################################################################"""
## load model categories from labelmap file
# @param path string, path to the .pbtxt file
# @param num int, number of category to use, range : 1 to total of catergies
# return a dictionnary with an id associated to a name like { 1:obj1, 2:obj2, 3:obj3 }
def load_categories(path, num):
    
    label_map = label_map_util.load_labelmap(path)
    categories = label_map_util.convert_label_map_to_categories(label_map, max_num_classes=num, use_display_name=True)
    category_index = label_map_util.create_category_index(categories)

    return category_index


"""###############################################################################################"""
## load an object detection model in memory
# @param path string, path to the .pb file (frozen_graph) that define the trained model
# return the tensorflow session of the detector, the input and outputs tensors
def load_model(path):
    
    # tensorflow code that load the graph
    detection_graph = tf.Graph()
    with detection_graph.as_default():
        od_graph_def = tf.GraphDef()
        with tf.gfile.GFile(path, 'rb') as fid:
            serialized_graph = fid.read()
            od_graph_def.ParseFromString(serialized_graph)
            tf.import_graph_def(od_graph_def, name='')

        # the session to use
        sess = tf.Session(graph=detection_graph)

    #input tensor = where the frame must be sended
    image_tensor = detection_graph.get_tensor_by_name('image_tensor:0')
    
    # output tensors
    detection_boxes = detection_graph.get_tensor_by_name('detection_boxes:0')
    detection_scores = detection_graph.get_tensor_by_name('detection_scores:0')
    detection_classes = detection_graph.get_tensor_by_name('detection_classes:0')
    num_detections = detection_graph.get_tensor_by_name('num_detections:0')

    # session, input and outputs (all in an array for easy manipulations)
    return sess, image_tensor, [detection_boxes, detection_scores, detection_classes, num_detections]


"""###############################################################################################"""
## A simplified call of object detection fonction that draw bounding box on frame
# @param frame numpy array representing the frame
# @param boxes array of bounding box coordinates (ymin, xmin, ymax, xmax)
# @param classes int array representing the category index of the object
# @param scores float, the probability (0.0 to 1.0) representing the confident of the model prediction
# @param category_index dictionnary loaded from the labelmap file
# @param thresold float, indicate the detections to exclude using prediction scores
# @param thickness int, line thickness in pixel of the drawed bounding boxes
def draw_bounding_boxes(frame, boxes, classes, scores, category_index, thresold=0.0, thickness=4):
    
    visualization_utils.visualize_boxes_and_labels_on_image_array(
            frame,
            np.squeeze(boxes),
            np.squeeze(classes).astype(np.int32),
            np.squeeze(scores),
            category_index,
            use_normalized_coordinates=True,
            line_thickness=thickness,
            min_score_thresh=thresold)



import_object_detection()

import tensorflow as tf
import numpy as np

from utils import label_map_util
from utils import visualization_utils
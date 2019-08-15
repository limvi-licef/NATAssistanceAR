"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

# -*- coding: utf-8 -*-


try:
    from Base import *
except:
    from Scenario.Base import *
    

import numpy as np
import glm
import time

    
####################################################################
####################################################################
####################################################################
class SimpleNATDebug(BaseScenario):
    
    left_area_set = False
    right_area_set = False
    area_range = 0.2
    
    distractors = ("Fork")
    left_objects = ("Lunchbox", "Bottle")
    right_objects = ("Notebook", "Pen")
    
    
    ################################################################
    def Action(self):
        
        self.NewDetections()
        
        if not self.left_area_set or not self.right_area_set:
            self.SetAreas()
        else:
            self.SortObjects()

            
    ################################################################
    def NewDetections(self):
        
        for obj in self.first_detection:
            pos = self.obj[obj]
            self.socket.Draw("new_text", pos.x, pos.y, pos.z, obj)
        
    
    ################################################################
    def SetAreas(self):
        
        for obj, pos in self.obj.items():
            
            if obj == "Left" and not self.left_area_set:
                self.left_area_set = True
                self.excluded_detection.append("Left")
                self.socket.Draw("new_area", pos[0], pos[1], pos[2], "left", self.area_range, "gray")
            
            elif obj == "Right" and not self.right_area_set:
                self.right_area_set = True
                self.excluded_detection.append("Right")
                self.socket.Draw("new_area", pos[0], pos[1], pos[2], "right", self.area_range, "gray")
                
    
    ################################################################
    def SortObjects(self):
            
        # update texts color
        for obj, pos in self.obj.items():
            
            # object to put in left area
            if obj in self.left_objects:
                self.LeftAreaObject(obj, pos)

            # object to put in right area
            elif obj in self.right_objects:
                self.RightAreaObject(obj, pos)

            # distractor
            elif obj in self.distractors:
                self.DistractorObject(obj, pos)

            # for other objects not in lists, use transparent color
            else:
                self.OtherObjects(obj, pos)
    
    
    ################################################################
    def LeftAreaObject(self, obj, pos):
        
        if self.Length(obj, "Left") < self.area_range:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "green")
        
        elif self.Length(obj, "Right") < self.area_range:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "red")
            
        else:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "white")
    
    
    ################################################################
    def RightAreaObject(self, obj, pos):
        
        if self.Length(obj, "Right") < self.area_range:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "green")
        
        elif self.Length(obj, "Left") < self.area_range:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "red")
            
        else:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "white")
            
    
    ################################################################
    def DistractorObject(self, obj, pos):
        
        if self.Length(obj, "Right") < self.area_range:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "red")
        
        elif self.Length(obj, "Left") < self.area_range:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "red")
            
        else:
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "white")
            
            
    ################################################################
    def OtherObjects(self, obj, pos):
        
        self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "alpha")
            
    
    

####################################################################
####################################################################
####################################################################
class SimpleNATRelease(SimpleNATDebug):
    
    
    left_color = "gray"
    right_color = "gray"
    
    
    ################################################################
    def NewDetections(self):
        
        pass
    
    
    ################################################################
    def SortObjects(self):
            
        self.left_error = False
        self.right_error = False
    
        SimpleNATDebug.SortObjects(self)
        
        self.AreaUpdate()
        
    
    ################################################################
    def AreaUpdate(self):
        
        # left area
        if self.left_error and self.left_color == "gray":
            self.left_color = "red"
            self.socket.Draw("update_area", "left", "red")
        elif not self.left_error and self.left_color == "red":
            self.left_color = "gray"
            self.socket.Draw("update_area", "left", "gray")
            
        # right area
        if self.right_error and self.right_color == "gray":
            self.right_color = "red"
            self.socket.Draw("update_area", "right", "red")
        elif not self.right_error and self.right_color == "red":
            self.right_color = "gray"
            self.socket.Draw("update_area", "right", "gray")
            
    
    ################################################################
    def LeftAreaObject(self, obj, pos):
        
        if self.Length(obj, "Right") < self.area_range:
            self.right_error = True
    
    
    ################################################################
    def RightAreaObject(self, obj, pos):
        
        if self.Length(obj, "Left") < self.area_range:
            self.left_error = True
            
    
    ################################################################
    def DistractorObject(self, obj, pos):
        
        self.LeftAreaObject(obj, pos)
        self.RightAreaObject(obj, pos)
            
            
    ################################################################
    def OtherObjects(self, obj, pos):
        
        pass
    
        
        
        
####################################################################
####################################################################
####################################################################
class SimpleNATReleaseTimer(SimpleNATRelease):
    
    timer_left = time.time()
    timer_right = time.time()
    delay = 5
        
    ################################################################
    def SortObjects(self):
            
        self.left_error = False
        self.right_error = False
    
        SimpleNATDebug.SortObjects(self)
        
        self.CheckErrorDelay()
        
        self.AreaUpdate()
        
    
    ################################################################
    def CheckErrorDelay(self):
        
        # calc delay in second
        left_delay = time.time() - self.timer_left
        right_delay = time.time() - self.timer_right
        
        # timer is updated if there is not error detected
        if not self.left_error:
            self.timer_left = time.time()
        # cancel the error as it occurs for less than 5 seconds
        elif left_delay < 5:
            self.left_error = False
        # else the the error will still be treated in AreaUpdate()
        
        # right area
        if not self.right_error:
            self.timer_right = time.time()
        elif self.right_error and right_delay < 5:
            self.right_error = False
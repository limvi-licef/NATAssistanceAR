"""
    Authors: Thomas Leonardon, Pierre-Baptiste Cougnenc, Dylan Mielot, Anthony Melin
    Date: 2020 May 15
"""

# -*- coding: utf-8 -*-


import cv2


try:
    # when imported from here
    from Base import BaseScenario
except:
    # when imported from outside
    from Scenario.Base import BaseScenario


"""##########################################################################"""
class PenNearNotebook(BaseScenario):


    """######################################################################"""
    def Action(self):
        
        for obj in self.first_detection:
            pos = self.obj[obj]
            print("                             ", obj, pos)
            self.socket.Draw("new_text", pos.x, pos.y, pos.z, obj)
            
        for obj in self.obj:
            pos = self.obj[obj] * 0.8
            print(obj, pos)
            self.socket.Draw("update_text", pos.x, pos.y, pos.z, obj, "white")
        
#         if ("Pen" in self.obj and "Notebook" in self.obj):
            
#             if self.Length("Pen", "Notebook") < 0.2:

#                 cv2.putText(self.frame,'Interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,255,0), 2)
#                 self.socket.Draw("update_text", self.obj["Pen"][0], self.obj["Pen"][1], self.obj["Pen"][2], "Pen", "white")
#                 self.socket.Draw("update_text", self.obj["Notebook"][0], self.obj["Notebook"][1], self.obj["Notebook"][2], "Notebook", "white")

#             else:

#                 cv2.putText(self.frame,'No interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,0,255), 2)
#                 self.socket.Draw("update_text", self.obj["Pen"][0], self.obj["Pen"][1], self.obj["Pen"][2], "Pen", "alpha")
#                 self.socket.Draw("update_text", self.obj["Notebook"][0], self.obj["Notebook"][1], self.obj["Notebook"][2], "Notebook", "alpha")
            
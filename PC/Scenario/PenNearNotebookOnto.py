"""
    Authors: Thomas Leonardon, Pierre-Baptiste Cougnenc, Dylan Mielot, Anthony Melin
    Date: 2020 May 15
"""


# -*- coding: utf-8 -*-


import cv2
import owlready2 as owl


try:
    # when imported from here
    from Base import BaseScenario
except:
    # when imported from outside
    from Scenario.Base import BaseScenario


"""##########################################################################"""
class PenNearNotebookOnto2D(BaseScenario):


    """######################################################################"""
    def LoadOnto(self, file):
        
        self.onto = owl.get_ontology(file)
        self.onto.load()
    
    

    """######################################################################"""
    def Action(self):

        # display object at first detection
        for obj in self.first_detection:
            self.socket.Draw("new", pos.x, pos.y, pos.z, obj)

        
        # iterates on detected objects 
        for obj, pos in self.obj.items():
            
            # if object is in ontology
            ontoObj = self.onto[obj]
            if ontoObj:
                
                # iterates on possible nextTo interaction
                for nextObj in ontoObj.nextTo:
                    
                    # if possible next object is detected
                    if nextObj.name in self.obj:
                        
                        # test if objects are near each other
                        if self.Length(obj, nextObj.name) < 0.2:

                            cv2.putText(self.frame,'Interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,255,0), 2)
                            self.socket.Draw("update_text", pos.x, pos.y, pos.z, "Pen", "white")
                            self.socket.Draw("update_text", pos.x, pos.y, pos.z, "Notebook", "white")

                        else:

                            cv2.putText(self.frame,'No interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,0,255), 2)
                            self.socket.Draw("update_text", pos.x, pos.y, pos.z, "Pen", "alpha")
                            self.socket.Draw("update_text", pos.x, pos.y, pos.z, "Notebook", "alpha")

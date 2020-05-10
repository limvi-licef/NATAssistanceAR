"""
    Authors: Thomas Leonardon, Pierre-Baptiste Cougnenc, Dylan Mielot, Anthony Melin
    Date: 2020 January 11
"""


# -*- coding: utf-8 -*-


import cv2
import owlready2 as owl


try:
    # when imported from here
    from Base2D import BaseScenario2D
except:
    # when imported from outside
    from Scenario.Base2D import BaseScenario2D


"""##########################################################################"""
class PenNearNotebookOnto2D(BaseScenario2D):


    """######################################################################"""
    def LoadOnto(self, file):
        
        self.onto = owl.get_ontology(file)
        self.onto.load()
    
    

    """######################################################################"""
    def Action(self):
        
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

                        else:

                            cv2.putText(self.frame,'No interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,0,255), 2)
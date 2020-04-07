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
class PenNotebookBottleInteractionOnto2D(BaseScenario2D):


    """######################################################################"""
    def LoadOnto(self, file):
        
        self.onto = owl.get_ontology(file)
        self.onto.load()
    
    

    """######################################################################"""
    def Action(self):
        
        y = 30
        
        for n1, (obj1, pos1) in enumerate(self.obj.items()):
            if self.onto[obj1]:
                for n2, (obj2, pos2) in [(n, obj_pos) for (n, obj_pos) in enumerate(self.obj.items()) if n > n1]:
                    if self.onto[obj2]:
                        
                        if self.Length(obj1, obj2) < 0.25:
                            txt = "{} is next to {}".format(obj1, obj2)
                            color = (0,255,0) if self.onto[obj1].is_a[0] == self.onto[obj2].is_a[0] else (0,0,255)
                            cv2.putText(self.frame, txt, (10, y), cv2.FONT_HERSHEY_SIMPLEX, 1, color, 2)
                            y += 30

        
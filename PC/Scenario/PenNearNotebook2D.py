"""
    Authors: Thomas Leonardon, Pierre-Baptiste Cougnenc, Dylan Mielot, Anthony Melin
    Date: 2020 January 11
"""

# -*- coding: utf-8 -*-


import cv2


try:
    # when imported from here
    from Base2D import BaseScenario2D
except:
    # when imported from outside
    from Scenario.Base2D import BaseScenario2D


"""##########################################################################"""
class PenNearNotebook2D(BaseScenario2D):


    """######################################################################"""
    def Action(self):
        
        if ("Pen" in self.obj and "Notebook" in self.obj):
            
            if self.Length("Pen", "Notebook") < 0.2:

                cv2.putText(self.frame,'Interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,255,0), 2)

            else:

                cv2.putText(self.frame,'No interaction', (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (0,0,255), 2)
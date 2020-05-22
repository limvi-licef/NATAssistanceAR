"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

"""
    Author: Anthony Melin
    Date: 2019 August 14
"""

## @package Base
# Module defining a basic scenario

# -*- coding: utf-8 -*-


try:
    # when imported from here
    from Base import *
except:
    # when imported from outside
    from Scenario.Base import *
    

import numpy as np
import glm
    

    
"""####################################################################"""
class SimpleObjectDetection(BaseScenario):
        
    
    """################################################################"""
    def Action(self):
        
        for obj, pos in self.obj.items():
            
            # create label for new object
            if obj in self.first_detection:
                self.socket.Draw("new", pos.x, pos.y, pos.z, obj)
            
            # update the object position
            else:
                self.socket.Draw("update", pos.x, pos.y, pos.z, obj)
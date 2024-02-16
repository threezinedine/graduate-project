# -*- coding: utf-8 -*-
"""
Created on Tue Oct 24 09:36:33 2023

@author: Acer
"""

from resistor_value import resistorValues
from copy import deepcopy


r1Options = deepcopy(resistorValues)
r2Options = deepcopy(resistorValues)

def expression(r1, r2):
    # return r2/r1
    return 0.6 * (1 + r1/r2)


RATIO = 4.2

minRatioEsp = 99999

final_r1 = None
final_r2 = None


for r1 in r1Options:
    for r2 in r2Options:
        result = expression(r1, r2)
        if abs(result - RATIO) < minRatioEsp:
            minRatioEsp = abs(result - RATIO)
            final_r1 = r1
            final_r2 = r2
            
print(final_r1, final_r2, final_r2/final_r1)

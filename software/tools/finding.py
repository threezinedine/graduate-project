# -*- coding: utf-8 -*-
"""
Created on Thu Oct 19 08:53:20 2023

@author: Acer
"""

#%% Getting value
from copy import deepcopy
from resistor_value import resistorValues


r_ntc_max = 26
r_ntc_min = 4
uncertain = 0.5

# baseValues = [
#     0.1, 0.11, 0.12, 0.13, 0.15, 0.16, 0.18,
#     0.2, 0.22, 0.24, 0.27,
#     0.3, 0.33, 0.36, 0.39,
#     0.43, 0.47,
#     0.51, 0.56,
#     0.62, 0.68,
#     0.75,
#     0.82,
#     0.91,
# ]

# r1Options = []
# r2Options = []

# for i in range(-1, 3):
#     for val in baseValues:
#         r1Options.append(val * 10 ** i)
#         r2Options.append(val * 10 ** i)

r1Options = deepcopy(resistorValues)
r2Options = deepcopy(resistorValues)

results = []

#%% List of condition

def r_parallel(r2, r_ntc):
    return r2 * r_ntc / (r2 + r_ntc)

def find_ratio(r1, r2, r_ntc):
    return r_parallel(r2, r_ntc) / (r1 + r_parallel(r2, r_ntc))


conditions = [
    lambda r1, r2: find_ratio(r1, r2, i/10) < 0.45
    for i in range (r_ntc_max * 10, r_ntc_max * 20)
] + [
      lambda r1, r2: find_ratio(r1, r2, i/10) > 0.8
      for i in range (1, r_ntc_min * 10)  
] \


#%% finding

for r1 in r1Options:
    for r2 in r2Options:
        valid = True
        for condition in conditions:
            if not condition(r1, r2):
                valid = False
                
        if valid:
            results.append((r1, r2, find_ratio(r1, r2, r_ntc_min), find_ratio(r1, r2, r_ntc_max)))
            
results

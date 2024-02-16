# -*- coding: utf-8 -*-
"""
Created on Tue Oct 24 09:36:58 2023

@author: Acer
"""

baseValues = [
    0.1, 0.11, 0.12, 0.13, 0.15, 0.16, 0.18,
    0.2, 0.22, 0.24, 0.27,
    0.3, 0.33, 0.36, 0.39,
    0.43, 0.47,
    0.51, 0.56,
    0.62, 0.68,
    0.75,
    0.82,
    0.91,
]

resistorValues = []

for i in range(-1, 3):
    for val in baseValues:
        resistorValues.append(val * 10 ** i)
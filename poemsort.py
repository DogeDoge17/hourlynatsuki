import os
import sys

tier1 = []
tier2 = []

if len(sys.argv) < 2:
    print("please supply the file path for the uhhhh the uhhh the yknow the poemwords from ddlc")
    exit(-1)

with open(sys.argv[1], 'r') as file:    
    for line in file:                
        line = line.strip()
        if "#" in line or line == "":
            continue

        args = line.split(',')

        if args[2] == "3":
            tier1.append(args[0])
        elif args[2] == "2":
            tier2.append(args[0])

with open("poemwords.txt",'w') as outFile:
    for word in tier1:
        outFile.write(word + "\n")
    outFile.write("\n")
    for word in tier2:
        outFile.write(word + "\n")

print("succesfully sorted")
import os
import sys
import shutil

if(len(sys.argv) < 1):
	print("please include path of where the images should be sampled from")

filesrc = sys.argv[1]

files = os.listdir(filesrc)

print("making dirs")

parhs = {"eyes", "body", "brows", "head", "mouth", "nose", "extra"}


for patr in parhs:
	if not os.path.isdir(os.getcwd() + "\\"+patr+"\\ff"):	
		os.makedirs(os.getcwd() + "\\"+patr+"\\ff") 	
	if not os.path.isdir(os.getcwd() + "\\"+patr+"\\fs"):		
		os.makedirs(os.getcwd() + "\\"+patr+"\\fs") 	

#if not os.path.isdir(os.getcwd() + "\\eyes"):
#	os.makedirs(os.getcwd() + "\\ff\\eyes") 
#	os.makedirs(os.getcwd() + "\\fs\\eyes") 
#if not os.path.isdir(os.getcwd() + "\\body"):
#	os.makedirs(os.getcwd() + "\\body") 
#if not os.path.isdir(os.getcwd() + "\\brows"):
#	os.makedirs(os.getcwd() + "\\brows") 
#if not os.path.isdir(os.getcwd() + "\\head"):
#	os.makedirs(os.getcwd() + "\\head") 
#if not os.path.isdir(os.getcwd() + "\\mouth"):
#	os.makedirs(os.getcwd() + "\\mouth")
#if not os.path.isdir(os.getcwd() + "\\nose"):
#	os.makedirs(os.getcwd() + "\\nose") 
#if not os.path.isdir(os.getcwd() + "\\extra"):
#	os.makedirs(os.getcwd() + "\\extra") 

print("sorting")
for file in files:
	if not file.endswith(".png"): 
		continue

	sort = "ff"
    
	if "fs" in file or "sad" in file:
		sort = "fs"

	if "eyes_" in file :       
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\eyes\\" + sort +"\\"+ file)
		print("put " + file + " into eyes")	
	elif "eyebrows_" in file:
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\brows\\" + sort +"\\"+ file)
		print("put " + file + " into brows")
	elif "face" in file and "scream" not in file:
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\head\\" + sort +"\\"+ file)
		print("put " + file + " into face")
	elif "mouth_" in file:
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\mouth\\" + sort +"\\"+ file)
		print("put " + file + " into mouth")
	elif "nose_" in file:
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\nose\\" + sort +"\\"+ file)
		print("put " + file + " into nose")
	elif ("crossed" in file or  "turned_" in file or "shy_" in file or "forward_" in file or "lean_" in file) and "scream" not in file:
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\body\\" + sort +"\\"+ file)
		print("put " + file + " into body")
	else:
		shutil.copy(filesrc + "\\" + file, os.getcwd() + "\\extra\\" + sort +"\\" + file)
		print("put " + file + " into extra")

print("finished successfully")


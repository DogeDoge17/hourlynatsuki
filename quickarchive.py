import os
import sys

if len(sys.argv) < 2:
    print("not enough args")
    exit(1);

if len(sys.argv[1]) < 40:
    print("invalid url")
    exit(2)

url = sys.argv[1].strip().strip("\",")[28:43]


print("opening to image ", url)

os.system("start \"\" https://web.archive.org/web/https://pbs.twimg.com/media/" + url + ".jpg")

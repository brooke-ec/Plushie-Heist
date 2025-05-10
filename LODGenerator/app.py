import os
import subprocess
import time
from tkinter import filedialog

MAX_PROCESSES = 1

workbench = os.path.join(os.path.dirname(__file__), "workbench.blend")
script = os.path.join(os.path.dirname(__file__), "script.py")
files = filedialog.askopenfilenames(filetypes=(("FBX", ".fbx"),))

processes: list[subprocess.Popen] = []
for file in files:
    if file.lower().endswith(".fbx"):
        processes.append(subprocess.Popen(["blender", "-b", workbench, "-P", script, "--", os.path.abspath(file)]))

    while len(processes) >= MAX_PROCESSES:
        processes = [p for p in processes if p.poll is None]
        time.sleep(0.1)

print("All files processed!")
input()

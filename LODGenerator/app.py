import os
import subprocess
from tkinter import filedialog

workbench = os.path.join(os.path.dirname(__file__), "workbench.blend")
script = os.path.join(os.path.dirname(__file__), "script.py")
directory = filedialog.askdirectory()

processes = []
for file in os.listdir(directory):
    if file.lower().endswith(".fbx"):
        path = os.path.join(directory, file)
        processes.append(subprocess.Popen(["blender", "-b", workbench, "-P", script, "--", os.path.abspath(path)]))

[p.wait() for p in processes]
print("All files processed!")
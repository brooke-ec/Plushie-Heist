import sys

import bpy

FILEPATH = sys.argv[-1]
MINIMUM = 0.3
LEVELS = 4

STEP = (1 - MINIMUM) / LEVELS

state = bpy.ops.import_scene.fbx(filepath=FILEPATH)

if "FINISHED" not in state:
    raise Exception("Import failed")

original = bpy.data.objects[0]

for i in range(1, LEVELS + 1):
    # Duplicate the original object
    obj = original.copy()
    obj.name = f"{original.name}_LOD{i}"
    bpy.context.collection.objects.link(obj)

    # Reduce polygon count
    obj.modifiers.new(name="Decimate", type="DECIMATE")
    obj.modifiers["Decimate"].ratio = 1 - STEP * i

    # Disable smooth shading
    if i > LEVELS / 2:
        obj.modifiers.new(name="Geometry Nodes", type="NODES")
        obj.modifiers["Geometry Nodes"].node_group = bpy.data.node_groups["No Smooth"]

original.name = f"{original.name}_LOD0"

bpy.ops.export_scene.fbx(filepath=FILEPATH)

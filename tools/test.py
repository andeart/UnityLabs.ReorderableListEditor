from pathlib import Path

mdb_file_pattern = "ReorderableListEditor.Demo\Assets\Libraries\Andeart.ReorderableListEditor\Andeart.ReorderableListEditor.dll*"
mdb_file_pattern_as_str = str(mdb_file_pattern)

direc = Path.cwd()

print (f"direc: {direc}")
print (f"mdb_file_pattern_as_str: {mdb_file_pattern_as_str}")

mdb_file_paths = direc.glob(str(mdb_file_pattern))
for fp in mdb_file_paths:
    print(f"fp: {fp}")
import argparse
from pathlib import Path
from shutil import copyfile

from shyprint import Logger, LogLevel


class NetCopy:

    def __init__(self, silent = False):
        print("NetCopy.")        

        # Initialize shyprint
        self.__logger = Logger(self)
        self.__logger.silent = silent

        # Parse CLI args
        parser = argparse.ArgumentParser(description="Copy built assembly to target directories. Best used as a post-build event on the VS project.")
        parser.add_argument("--asname", "-a", type=str, metavar="AssemblyName", default=None, help="The name of the built assembly (without file-extension).\nNetCopy copies the assembly of this name to the target directories.\nThis name may also additionally be used to create subdirectories if needed.")
        parser.add_argument("--asdir", "-d", type=str, metavar="AssemblyLocation", default=None, help="The location of the built assembly.")        
        parser.add_argument("--astypes", "-e", type=str, metavar="AssemblyExtensions", default="dll", help="A semicolon-separated list of extensions to apply on the AssemblyName and copy from the AssemblyLocation to the target directories. Ex: \"dll;pdb;xml\". Applies \"dll\" by default")
        parser.add_argument("--targetdirs", "-t", type=str, metavar="TargetDirectories", default=None, help="A semicolon-separated list of target directory paths to copy the assembly to.\nOptionally, you can also use this to provide the path to a text file containing the target paths, separated by line-breaks.\nDirectory is created if it does not exist.")
        parser.add_argument("--createsubdir", "-c", action="store_true", default=True, help="Specify if a sub-directory with the assembly name should be created in the target directories. Use this as a flag, i.e. simply add -c or --createsubdir without additional args")

        args = parser.parse_args()
        self.__asname = args.asname
        self.__asdir_str = args.asdir
        self.__astypes = args.astypes.split(";")
        self.__targetdirs_str = args.targetdirs
        self.__createsubdir = args.createsubdir

        self.__logger.log(f"Assembly name: {self.__asname}"
                        + f"\nAssembly directory: {self.__asdir_str}"
                        + f"\nAssembly extensions: {self.__astypes}"
                        + f"\nTarget directories (unparsed): {self.__targetdirs_str}"
                        + f"\nShould create sub-directory?: {self.__createsubdir}",
                        LogLevel.WARNING)

        # self.__logger.log(str(self.__targetdirs_str)[1:-1])

        self.__dir = Path.cwd()
        
        if (self.__asname == None): self.__exit_with_error(1, "Assembly name was not provided.", parser.format_help())
        if (self.__asdir_str == None): self.__exit_with_error(1, "Assembly directory was not provided.", parser.format_help())
        aspath = self.__full_path(self.__asdir_str, self.__asname, self.__astypes[0])
        self.__logger.log(f"Parsed assembly path: {aspath}")
        if not self.__is_file(aspath): self.__exit_with_error(1, "Built assembly does not exist.", parser.format_help())

        # If targetdirs is a file, parse the targetdirs list from its contents
        if self.__is_file(self.__targetdirs_str):
            targetdirs_list = []
            with open(self.__targetdirs_str) as targetdirs_file:
                targetdirs_list = targetdirs_file.readlines()
            targetdirs_list = [line.strip() for line in targetdirs_list] 
            self.__targetdirs_str = targetdirs_list
        # else split the string by semicolon for the list
        else:
            self.__targetdirs_str = self.__targetdirs_str.split(";")

        self.__logger.log(f"Parsed target directories: {str(self.__targetdirs_str)[1:-1]}")


    def copy_files(self):
        for targetdir in self.__targetdirs_str:
            self.__logger.log_linebreaks(1)
            if (self.__createsubdir):
                targetdir = Path(targetdir).joinpath(self.__asname)
            self.__logger.log(f"Covering target directory: {targetdir}...")

            if not self.__is_dir(targetdir):
                self.__logger.log("Directory does not exist. Creating new directory.")
                targetdir.mkdir(parents=True)

            # Remove any existing files in the target directory.
            for astype in self.__astypes:
                self.__logger.log_linebreaks(1)
                # Delete existing files. Add * to the extension glob to delete helper files (ex: .meta files)
                file_pattern = self.__full_path(targetdir, self.__asname, astype + "*")
                file_paths = Path.cwd().glob(str(file_pattern))
                for file_path in sorted(file_paths):
                    if (self.__is_file(file_path)):
                        self.__logger.log (f"Deleting {file_path}", LogLevel.WARNING)
                        file_path.unlink()

                # Copy new files
                source_file_path = self.__full_path(self.__asdir_str, self.__asname, astype)
                if not self.__is_file(source_file_path):
                    self.__exit_with_error(1, f"Could not find expected source file at: {source_file_path}")
                target_file_path = self.__full_path(targetdir, self.__asname, astype)                
                self.__logger.log(f"Copying {source_file_path} to {target_file_path}...")
                copyfile(source_file_path, target_file_path)


    def __exit_with_error(self, error_code, error_msg, usage_info = None):
        self.__logger.log("ERROR! Exiting..."
                        + f"\nError code: {str(error_code)}"
                        + f"\nError message: {error_msg}",
                        LogLevel.ERROR)
        if usage_info != None: self.__logger.log(usage_info, LogLevel.WARNING)
        exit(error_code)


    def __full_path(self, directory, name, extension):
        full_name = f"{name}.{extension}"
        # self.__logger.log(f"name is: {str(full_name)}")
        full_file_path = Path(directory).joinpath(full_name)
        # self.__logger.log(f"full_file_path is: {str(full_file_path)}")
        return full_file_path


    def __is_dir(self, location):
        return Path(location).is_dir()


    def __is_file(self, location):
        return Path(location).is_file()

         

if __name__ == "__main__":
    copy = NetCopy(False)
    copy.copy_files()
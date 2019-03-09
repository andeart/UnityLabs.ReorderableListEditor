from argparse import ArgumentParser
from pathlib import Path

from shyprint import Logger, LogLevel


class PlainExist:


    def __init__(self, silent = False):
        print("PlainExist.")

        # Initialize shyprint
        self.__logger = Logger(self)
        self.__logger.silent = silent
        
        # Parse CLI args
        parser = ArgumentParser(description="A simple check to see if files or directories exist.")
        parser.add_argument("--files", "-f", type=str, metavar="FilePaths", default=None, help="A semicolon-separated list of paths to the files to check. Use at least one of this or -d for directories.")
        parser.add_argument("--dirs", "-d", type=str, metavar="DirectoryPaths", default=None, help="A semicolon-separated list of paths to the directories to check. Use at least one of this or -f for files.")
        args = parser.parse_args()

        self.__logger.log(f"Unparsed file paths: {self.__filepaths}"
                            + f"\nUnparsed directory paths: {self.__dirpaths}",
                            LogLevel.WARNING)
                
        if (self.__filepaths == None and self.__dirpaths == None): self.__exit_with_error(1, "Neither file nor directory paths were provided for check.", parser.format_help())
            
        self.__filepaths = self.__filepaths.split(";")
        self.__dirpaths = self.__dirpaths.split(";")
        self.__logger.log(f"Parsed file paths: {self.__filepaths}"
                            + f"\nParsed directory paths: {self.__dirpaths}",
                            LogLevel.WARNING)


    def check(self):
        for filepath in self.__filepaths:
            if self.__is_file(filepath):
                self.__logger.log(f"File was successfully found at: {filepath}")
            else:
                self.__exit_with_error(1, f"File was not successfully found at: {filepath}")

        for dirpath in self.__dirpaths:
            if self.__is_file(dirpath):
                self.__logger.log(f"Directory was successfully found at: {dirpath}")
            else:
                self.__exit_with_error(1, f"Directory was not successfully found at: {dirpath}")

        self.__logger.log("All files and directories were successfully verified.", LogLevel.SUCCESS)


    def __exit_with_error(self, error_code, error_msg, usage_info = None):
        self.__logger.log("ERROR! Exiting..."
                        + f"\nError code: {str(error_code)}"
                        + f"\nError message: {error_msg}",
                        LogLevel.ERROR)
        if usage_info != None: self.__logger.log(usage_info, LogLevel.WARNING)
        exit(error_code)


    def __is_dir(self, location):
        return Path(location).is_dir()


    def __is_file(self, location):
        return Path(location).is_file()



if __name__ == "__main__":
    check = PlainExist(False)
    check.check()
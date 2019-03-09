import argparse
import glob
import pathlib
import xml.etree.ElementTree as ET

from processrun import ProcessRunner
from shyprint import Logger, LogLevel


class FodyCleaner:


    def __init__(self, silent = False):
        print("FodyCleaner.")

        # Initialize shyprint
        self.__logger = Logger(self)
        self.__logger.silent = silent
        
        # Parse CLI args
        parser = argparse.ArgumentParser(description="Clean Fody references from all projects in VS solution directory.")
        parser.add_argument("--slnpath", "-s", type=str, metavar="SolutionPath", default=None, help="The path to the solution whose directory, or the path to the directory itself, contains all the targeted projects.")
        args = parser.parse_args()
        sln_path = args.slnpath

        if (sln_path == None): self.__exit_with_error(1, "Solution path is not provided for build.", parser.format_help())

        sln_strong_path = pathlib.Path(sln_path)
        if sln_strong_path.is_file():
            self.__dir_path = str(sln_strong_path.parent)
        else:
            self.__dir_path = str(sln_strong_path)


    def clean(self):
        # Initialize processrun
        self.__process_run = ProcessRunner(self.__logger.silent)

        exit_code = self.__clean_refs(self.__dir_path)
        if (exit_code != 0): self.__exit_with_error(exit_code, "Fody references in projects were not removed successfully.")

    
    def __clean_refs(self, dir_path):
        self.__logger.log_linebreaks(2)
        self.__logger.log(f"Cleaning Fody references...", LogLevel.WARNING)

        packages_path_format = dir_path + "*/*/packages.config"
        file_paths = glob.glob(packages_path_format)
        package_keys = [".//package[@id=\"Costura.Fody\"]", ".//package[@id=\"Fody\"]"]
        for file_path in file_paths:            
            self.__logger.log(f"Searching for Fody references in {file_path}.")
            file = ET.parse(file_path)
            root = file.getroot()
            was_ref_found = False
            for package in package_keys:
                elem = root.find(package)
                if elem is not None:
                    root.remove(elem)
                    was_ref_found = True
            if (was_ref_found == True):
                file.write(file_path, encoding="utf-8", xml_declaration=True)
                self.__logger.log(f"Cleaned Fody refs in {file_path}.", LogLevel.WARNING)
        return 0

    def __exit_with_error(self, error_code, error_msg, usage_info = None):
        self.__logger.log("ERROR! Exiting..."
                        + f"\nError code: {str(error_code)}"
                        + f"\nError message: {error_msg}",
                        LogLevel.ERROR)
        if usage_info != None: self.__logger.log(usage_info, LogLevel.WARNING)
        exit(error_code)



if __name__ == "__main__":
    cleaner = FodyCleaner(False)
    cleaner.clean()
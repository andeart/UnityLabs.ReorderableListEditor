import argparse
import pathlib

from processrun import ProcessRunner
from shyprint import Logger, LogLevel

class NetRestore:


    def __init__(self, silent = False):
        print("NetRestore.")

        # Initialize shyprint
        self.__logger = Logger(self)
        self.__logger.silent = silent

        # Parse CLI args
        parser = argparse.ArgumentParser(description="Restore dependencies in VS solution.")
        parser.add_argument("--slnpath", "-s", type=str, metavar="SolutionPath", default=None, help="The path to the solution to restore.")
        args = parser.parse_args()
        self.__sln_path = args.slnpath
        
        self.__logger.log(f"Solution path: {self.__sln_path}", LogLevel.WARNING)
        
        if (self.__sln_path == None): self.__exit_with_error(1, "Solution path was not provided for build.", parser.format_help())
        
        if not self.__is_file(self.__sln_path): self.__exit_with_error(1, "Solution path is not a valid file.", parser.format_help())

    
    def restore(self):        
        # Initialize processrun
        self.__process_run = ProcessRunner(self.__logger.silent)

        result = self.__run_nuget_restore(self.__sln_path)
        if (result.status != 0): self.__exit_with_error(result.status, "NuGet packages for solution were not restored correctly.")


    def __run_nuget_restore(self, sln_path):
        self.__logger.log_linebreaks(2)
        self.__logger.log("Running nuget restore...", LogLevel.WARNING)
        return self.__process_run.run_line(f"nuget restore {sln_path}")


    def __exit_with_error(self, error_code, error_msg, usage_info = None):
        self.__logger.log("ERROR! Exiting..."
                        + f"\nError code: {str(error_code)}"
                        + f"\nError message: {error_msg}",
                        LogLevel.ERROR)
        if usage_info != None: self.__logger.log(usage_info, LogLevel.WARNING)
        exit(error_code)

    def __is_file(self, location):
        path = pathlib.Path(location)
        return path.is_file()



if __name__ == "__main__":
    restore = NetRestore(False)
    restore.restore()
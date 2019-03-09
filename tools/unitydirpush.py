from shyprint import Logger, LogLevel

class UnityDirectoryPush:

    def __init__(self, silent = False):
        print("UnityDirectoryPush.")        

        # Initialize shyprint
        self.__logger = Logger(self)
        self.__logger.silent = silent

        # Parse CLI args
        parser = argparse.ArgumentParser(description='Push built assembly to Unity project directories.')
        parser.add_argument("--slndir", "-s", type=str, metavar="SolutionDirectory", default=None, help="The Solution directory. UnityDirectoryPush will look in its parent for provided ")
        parser.add_argument("--config", "-c", choices=["Release", "Debug"], type=str, metavar="ConfigurationName", default="Debug", help="The ConfigurationName to be used with MSBuild.")
        args = parser.parse_args()
        self.__sln_path = args.slnpath
        self.__config_name = args.config
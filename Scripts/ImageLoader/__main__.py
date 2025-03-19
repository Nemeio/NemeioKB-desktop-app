#!/usr/bin/env python3

import click
import os
import subprocess
import uuid
import time
import re
from sys import platform
from zipfile import ZipFile
from os.path import basename

if platform == "win32":
    CLI_EXECUTABLE_PATH = "Nemeio.Cli.Windows"
elif platform == "linux" or platform == "linux2":
    CLI_EXECUTABLE_PATH = "Nemeio.Cli.Linux"
else:
    raise Exception("Unsupported platform")

IMAGE_PACKAGE_BUILDER_EXECUTABLE_PATH = "Nemeio.Tools.Image.ImagePackageBuilder"
BACKGROUND_BLACK = 0
BACKGROUND_WHITE = 1

class ImageBuilder:

    def build(self, inputDirPath, outputDirPath):
        if not os.path.exists(outputDirPath):
            os.mkdir(outputDirPath)

        for currentFile in os.listdir(inputDirPath):
            self._buildPackage(inputDirPath, outputDirPath, currentFile)

    def _buildPackage(self, inputDirPath, outputDirPath, currentFile):        
        filePath = os.path.join(inputDirPath, currentFile)

        fileName = os.path.basename(filePath);
        background = BACKGROUND_WHITE
        
        if re.search("black", fileName):
            background = BACKGROUND_BLACK

        packagePath = os.path.join(outputDirPath, f"{fileName}.zip")
        
        subprocess.run([
            IMAGE_PACKAGE_BUILDER_EXECUTABLE_PATH,
            "-image",
            filePath,
            "-output",
            packagePath,
            "-compression",
            "GZip",
            "-json",
            f"{{\"BackgroundColor\":{background}}}"
        ])

class Sender:

    def publish(self, inputDirPath, isFactory):

        print(f"Performing factory reset")
        self._factoryReset()
        time.sleep(10)

        for currentFile in os.listdir(inputDirPath):
            extension = os.path.splitext(currentFile)[1]
            background = BACKGROUND_WHITE
            
            if extension == ".zip":        
                print(f"Sending {currentFile}")
                self._sendConfiguration(currentFile, inputDirPath, isFactory)

    def _factoryReset(self):
    
        subprocess.run([
            CLI_EXECUTABLE_PATH, 
            "factoryReset"
        ])

    def _sendConfiguration(self, currentFile, inputDirPath, isFactory):
        args = [
            CLI_EXECUTABLE_PATH, 
            "add",
            "-path",
            os.path.join(inputDirPath, currentFile)
        ]

        if isFactory:
            args.append("-factory")

        subprocess.run(args)
        
class KeyboardParametersSetter:

    def setDemoMode(self, demoMode):
        value = "true" if demoMode else "false"
        subprocess.run([
            CLI_EXECUTABLE_PATH, 
            "setParameters",
            "-json",
            f"{{\"demoMode\":{value}}}"
        ])

@click.group()
def cli():
    # No options
    pass

@cli.command(short_help='Create the images from the input folder')
@click.option('--input', required=True, type=click.STRING, help='Input folder path')
@click.option('--output', required=True, type=click.STRING, help='Output folder path')
def create_images(input, output):
    ImageBuilder().build(input, output)

@cli.command(short_help='Send layouts from folder')
@click.option('--input', required=True, type=click.STRING, help='Input folder path')
@click.option('--factory', is_flag =True, default=False, type=click.BOOL, help='Send as factory layouts')
def send_layouts(input, factory):
    Sender().publish(input, factory)
    
@cli.command(short_help='Enable the demo mode')
def enable_demo_mode():
    KeyboardParametersSetter().setDemoMode(True)

@cli.command(short_help='Disable the demo mode')
def disable_demo_mode():
    KeyboardParametersSetter().setDemoMode(False)

if __name__ == '__main__':
    cli()

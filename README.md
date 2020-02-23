# NATAssistanceAR

## Purpose
This project aims at implementing an augmented reality (using the Hololens first generation) assistance for the third test of the Naturalistic Action Test (https://www.tandfonline.com/doi/abs/10.1080/09602010244000084).

## Implemented functionalities
Currently, the implemented functionalities are:
- Streaming of the Hololens' front webcam to a distant PC
- On the distant PC, each frame of the video stream is analyzed using TensorFlow to identify the objects used in the third test of the NAT. Currently, the following objects are detected:
  - Pen
  - Bottle
  - Notebook
- The assistance provided to the user is binary: it informs him if the object is located on the correct place (schoolbag or lunchbox). Those two places are symbolized by two virtual area, one being the schoolbag and the other being the lunchbox. Those two virtual areas are identified when the software is launched, using a pattern.

The project is ongoing and new functionalities are being implemented.

## Compilation
To install and compile this project, first clone this repository.
The two following section describe how to compile the PC and the Hololens applications.

### PC Application
- Clone the TensorFlow/Models repository: https://github.com/tensorflow/models
- Install Anaconda for Python 3.xx : https://www.anaconda.com/distribution/#download-section
- In the Anaconda console, execute the following commands:
  - pip install tensorflow opencv-python PyGLM utils
  - conda install protobuf
  - Go to the TensorFlow/Models local directory > research.
  - Execute the second command from the section 2f of https://github.com/EdjeElectronics/TensorFlow-Object-Detection-API-Tutorial-Train-Multiple-Objects-Windows-10 (starting by "protoc ").
- Execute the fo
- In the file PC > Tensorflow > ObjectDetection.py, update the paths in the function "tensorflow_path" where "WRITE THE PATH OF TENSORFLOW ON THE COMPUTER HERE" is written to the TensorFlow/Models local directory.
- In Jupyter, open PC > App.ipynb:
  - In the "Global variables > Model files and classes" section: set the variable "NUM_CLASSES" to 7.
  - In the "Global variables > Host and port" section: set the "HOST" variable to the local IP.
  - In the "Global variables > Scenario" section: set the "scenario" variable to the function related to the NAT scenario you would like to use. To use the aforementioned scenario, use the function "SimpleNATRelease()". The available scenario are listed in PC > Scenario.

### Hololens application
- Open Hololens > HoloObjectDetection.sln with Microsoft visual studio 2017.
- If the project "Projects > 1 - Simple NAT" is not loaded, then right click on the project and click "Download update"
- Compile and deploy the "Projects > 1 - Simple NAT" project in x86 / release to the Device (i.e. the Hololens)

## Run
- Run the App.ipynb in Jupyter.
- 

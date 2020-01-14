# -*- coding: utf-8 -*-
"""
Created on Fri Jan 10 16:32:54 2020

@author: Anthony Melin
"""


from setuptools import setup, find_packages

import PyRemoteCam


setup(
      name='PyRemoteCam',
      version=PyRemoteCam.__version__,
      packages=find_packages(),
      
      author="Anthony Melin",
      author_email="addresse.pypi@gmail.com",
      url='http://monsite',
      
      description="Connect a remote camera to python",
      long_description=open('README.md').read(),
      
      install_requires= ["websockets", "opencv-python"],
      include_package_data=True,
      
      classifiers=[
          "Programming Language :: Python",
          "Natural Language :: French",
          "Operating System :: OS Independent",
          "Programming Language :: Python :: 3",
          ],
      
      entry_points = {
          'console_scripts': [
              'pyremotecam-server = PyRemoteCam.ServerGui:main',
              'pyremotecam-videoframe = PyRemoteCam.VideoReceiver:main',
              ],
          },
      
      license="WTFPL",
)
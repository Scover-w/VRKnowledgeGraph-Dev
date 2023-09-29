# AIDEN: Artificial Intelligence for Data Exploration and Navigation

## Overview

AIDEN is a Unity VR application designed to visualize knowledge graphs in a 3D virtual environment. Developed at LS2N (http://www.ls2n.fr/), the app aims to provide a more intuitive way of interacting with complex data structures, leveraging the capabilities of the VIVE Focus 3 for an immersive experience.

## Prerequisites

- Unity Editor version 2021.3.22f1
- VIVE Focus 3 VR headset

### Dependencies

Install the following packages via Nuget:
- Azure.AI.openAi 1.0.0-beta.8
- OggVorbisEncoder 1.2.2
- QuikGraph 2.5
- dotNetRdf.Core 3.1.0

## Installation

1. Clone the repository.
2. Open the project in Unity Editor.
3. Install the required Nuget packages with https://github.com/GlitchEnzo/NuGetForUnity.

## Configuration

1. Provide your OpenAi key in the `OpenAIKey` class.

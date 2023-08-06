<img src="Design/meadow-projectlab-samples.jpg" style="margin-bottom:10px" />

# Meadow.ProjectLab.Samples

This repo contains code samples for the Wilderness Labs Meadow [Project Lab](https://github.com/WildernessLabs/Meadow.Project.Lab) board. Project Lab is a hardware development and prototyping board designed to enable rapid prototyping and IoT software development with [Meadow](http://developer.wildernesslabs.co/Meadow/) and [Meadow.Foundation](http://developer.wildernesslabs.co/Meadow/Meadow.Foundation/).


## Contents
* [Project Samples](#project-samples)
* [Hardware Specifications](#hardware-specifications)
* [Pinout Diagram](#pinout-diagram)
  * [Project Lab v3.e](#project-lab-v3e)
  * [Project Lab v2.e](#project-lab-v2e)
  * [Project Lab v1.e](#project-lab-v1e)
* [License](#license)
* [Support](#support)

## Project Samples

<table>
    <tr>
        <td>
            <a href="https://github.com/WildernessLabs/Meadow.ProjectLab/tree/main/Source/ProjectLab_Demo"><img src="Design/GettingStarted.png"/></a><br/>
            Getting started with Project Lab running a diagnostics app.</br>
            <a href="https://www.hackster.io/wilderness-labs/getting-started-with-meadow-s-project-lab-eeb569">Hackster</a> | <a href="https://github.com/WildernessLabs/Meadow.ProjectLab/tree/main/Source/ProjectLab_Demo">Source Code</a>
        </td>
        <td>
            <a href="Source/MeadowAzureIoTHub/"><img src="Design/MeadowAzureIoTHub.png"/></a><br/>
            Send anvironmental data from a BME688 to Azure IoT Hub<br/>
            <a href="https://www.hackster.io/wildernesslabs/send-environmental-data-from-projectlab-to-azure-w-iot-hub-7d3d07">Hackster</a> | 
            <a href="Source/MeadowAzureIoTHub/">Source Code</a>
        </td>
        <td>
            <a href="Source/MagicEightMeadow/"><img src="Design/MagicEightMeadow.png"/></a><br/>
            Make a Magic Eight ball with MicroGraphics and accelerometer sensor.</br>
            <a href="https://www.hackster.io/wilderness-labs/build-your-own-magic-eight-ball-with-a-projectlab-28044f">Hackster</a> | <a href="Source/MagicEightMeadow/">Source Code</a>
        </td>
        <!-- <td>
            <a href="Source/MeadowAzureServo/"><img src="Design/MeadowAzureServo.png"/></a><br/>
            Control a Grove Servo with Azure IoT Hub messages<br/>
            <a href="Source/MeadowAzureServo/">Source Code</a>
        </td> -->
    </tr>
        <td>
            <a href="Source/Connectivity/"><img src="Design/maple.png"/></a><br/>
            Control a Project Lab Board over Wi-Fi with a MAUI app</br>
            <a href="Source/Connectivity/">Source Code</a>
        </td>
        <td>
            <a href="Source/Connectivity/"><img src="Design/bluetooth.png"/></a><br/>
            Control a Project Lab Board over Bluetooth with a companion app<br/>
            <a href="Source/Connectivity/">Source Code</a>
        </td>
        <td>
            <a href="Source/MoistureMeter/"><img src="Design/MoistureMeter.png"/></a><br/>
            Use a Grove Soil Moisture sensor and graph its value on the display<br/>
            <a href="Source/MoistureMeter/">Source Code</a>
        </td>
    </tr>
    <tr>
        <td>
            <a href="Source/GalleryViewer/"><img src="Design/GalleryViewer.png"/></a><br/>
            Make an Image Gallery with a ST7789 Display and Meadow.<br/>
            <a href="Source/GalleryViewer/">Source Code</a>
        </td>
        <td>
            <a href="Source/WifiWeather/"><img src="Design/WifiWeather.png"/></a><br/>
            Weather Station using public web service on a Project Lab Board<br/>
            <a href="Source/WifiWeather/">Source Code</a>
        </td>
        <td>
            <a href="Source/AnalogClockFace/"><img src="Design/MeadowClockGraphics.png"/></a><br/>
            Draw an analog clock watch face on using MicroGraphics<br/>
            <a href="Source/AnalogClockFace/">Source Code</a>
        </td>
    </tr>
    <tr>
        <td>
            <a href="Source/AmbientRoomMonitor/"><img src="Design/TemperatureMonitor.png"/></a><br/>
            Room ambient monitor with a BME688 on a Project Lab Board<br/>
            <a href="Source/AmbientRoomMonitor/">Source Code</a>
        </td>
        <td>
            <a href="Source/Simon/"><img src="Design/Simon.png"/></a><br/>
            Run a Simon Game on a display and push button d-pad</br>
            <a href="Source/Simon/">Source Code</a>
        </td>
        <td>
            <a href="Source/MorseCodeTrainer/"><img src="Design/MorseCodeTrainer.png"/></a><br/>
            Train your Morse Code spelling skills with Meadow<br/>
            <a href="Source/MorseCodeTrainer/">Source Code</a>
        </td>
    </tr>
    <tr>
        <td>
            <p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</p>
        </td>
        <td>
            <p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</p>
        </td>
        <td>
            <p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</p>
        </td>
    </tr>
</table>

## Hardware Specifications

<img src="Design/project-lab-specs.jpg" alt="project-lab, specs, iot, dotnet" style="margin-top:10px;margin-bottom:10px" />

<table>
    <tr>
        <th>Onboard Peripherals</th>
        <th>Connectivity</th>
    </tr>
    <tr>
        <td><strong>ILI9341</strong> - SPI 320x240 color display</li></td>
        <td><strong>MikroBUS</strong> - Two sets of MikroBUS pin headers</td>
    </tr>
    <tr>
        <td><strong>BMI270</strong> - I2C motion and acceleration sensor</td>
        <td><strong>Qwiic</strong> - Stemma QT I2C connector</td>
    </tr>
    <tr>
        <td><strong>BH1750</strong> - I2C light sensor</td>
        <td><strong>Grove</strong> - Analog header</td>
    </tr>
    <tr>
        <td><strong>BME688</strong> - I2C atmospheric sensor</td>
        <td><strong>Grove</strong> - GPIO/serial header</td>
    </tr>
    <tr>
        <td><strong>Push Button</strong> - 4 momentary buttons</td>
        <td><strong>RS-485</strong> - Serial</td>
    </tr>
    <tr>
        <td><strong>Magnetic Audio Transducer</strong> - High quality piezo speaker</td>
        <td><strong>Ports</strong> - 3.3V, 5V, ground, one analog and two GPIO ports</td>
    </tr>
</table>

## Pinout Diagram

Check the diagrams below to see what pins on the Meadow are connected to every peripheral on board and its connectors:
&nbsp;

### Project Lab v3.e

<img src="Design/projectlab-pinout-v3.jpg" alt="project-lab-v3, pinout, iot, dotnet" style="margin-top:10px;margin-bottom:10px" />

### Project Lab v2.e

<img src="Design/projectlab-pinout-v2.jpg" alt="project-lab-v2, specs, iot, dotnet" style="margin-top:10px;margin-bottom:10px" />

### Project Lab v1.e

<img src="Design/projectlab-pinout-v1.jpg" alt="project-lab-v1, specs, iot, dotnet" style="margin-top:10px;margin-bottom:10px" />

## License
Copyright 2023, Wilderness Labs Inc.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

## Support

Having trouble building/running these projects? 
* File an [issue](https://github.com/WildernessLabs/Meadow.Desktop.Samples/issues) with a repro case to investigate, and/or
* Join our [public Slack](http://slackinvite.wildernesslabs.co/), where we have an awesome community helping, sharing and building amazing things using Meadow.

<img src="Design/banner.jpg" style="margin-bottom:10px" />

# Meadow.ProjectLab.Samples

This repo contains code samples for the Wilderness Labs Meadow [Project Lab](https://github.com/WildernessLabs/Meadow.Project.Lab) board. Project Lab is a hardware development and prototyping board designed to enable rapid prototyping and IoT software development with [Meadow](http://developer.wildernesslabs.co/Meadow/) and [Meadow.Foundation](http://developer.wildernesslabs.co/Meadow/Meadow.Foundation/).

## Project Samples

<table>
    <tr>
        <td>
            <a href="https://github.com/WildernessLabs/Meadow.ProjectLab/tree/main/Source/ProjectLab_Demo"><img src="Design/GettingStarted.png"/></a><br/>
            Make a Magic Eight ball with a Project Lab</br>
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
            Make a Magic Eight ball with a Project Lab</br>
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

## Onboard On/Off switch

Project Lab boards come with an Enable/Disable switch onboard so you can reset the board by flipping the switch off and on. Make sure the switch is set to ENABLE when intending to work on your project, otherwise it will remain off and it wont be detected on your computer.

<p align="center">
    <img src="Design/EnableDisable.png" width="50%" />
</p>

## Hardware

<table>
    <tr>
        <th>Onboard Peripherals</th>
        <th>Connectivity</th>
    </tr>
    <tr>
        <td><strong>ST7789</strong> - SPI 240x240 color display</li></td>
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

### Project Lab v2.e

<img src="Design/PinoutV2.jpg" style="margin-top:10px;margin-bottom:10px" />

### Project Lab v1.e

<img src="Design/PinoutV1.jpg" style="margin-top:10px;margin-bottom:10px" />

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

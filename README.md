
# <img src="https://github.com/bertwagner/Coffee-Roaster/blob/master/Roaster-Client/Web/Content/Images/apple-icon-114x114.png?raw=true" width="40" title="Coffee Roaster"> Coffee Roaster 
A Raspberry Pi 3 coffee roaster running on Windows IoT.

[![BCH compliancy](https://bettercodehub.com/edge/badge/bertwagner/Coffee-Roaster)](https://bettercodehub.com)

---

<img src="https://github.com/bertwagner/Coffee-Roaster/blob/master/Roaster-Client/Web/Content/Images/CoffeeRoaster.jpg?raw=true" width="600" title="Coffee Roaster">
### Purpose
Coffee roasting at home allows complete control over the final flavor of a cup of coffee.  

Although many manual methods can be used for roasting coffee beans at home, I wanted a precise and mostly automated way of roasting small batches of beans.

<img src="https://github.com/bertwagner/Coffee-Roaster/blob/master/Roaster-Client/Web/Content/Images/WebApp.png?raw=true" width="400" title="Coffee Roaster">

### Features
 * Operated via web app on a mobile device
 * Ability to control fan and heating elements separately 
 * Ability to hold a steady temperature
 * Chart to display roasting profile
 * Automatic safety cool-down if temperature is too high
 * REST API control of hardware for easy integration across platforms

<img src="https://github.com/bertwagner/Coffee-Roaster/blob/master/Roaster-Client/Web/Content/Images/Schematic.jpg?raw=true" width="600" title="Coffee Roaster">

### Hardware
 * Raspberry Pi 3
 * MAX31855 Thermocouple
 * 1040 watt heating element from Nostalgia hot air popper
 * 18 volt fan from Nostalgia hot air popper
 * 25A 24-380VAC relay for controlling heating element
 * 25A 5-200VDC relay for controlling fan
 * Glass hurricane lamp chimney
 * PC power supply for easy power management

### Software
 * [Windows IoT Core](https://developer.microsoft.com/en-us/windows/iot)
 * [jQuery Mobile](http://jquerymobile.com/)
 * [Restup](https://github.com/tomkuijsten/restup) REST api for Windows UWP


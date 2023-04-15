# UMCS - Unity Multi Controller Support
Currently developed on Unity 2022.2.2.f1

It's my first open source library for Unity, i hope that you will enjoy it :)
You can use it freely in your projects, but don't forget to credit me!

Currently supported gamepads:
- Nintendo Switch Joycons
- Xbox One

## How to install
Go into the releases section of GitHub and download the latest unitypackage. Open it with Unity inside your project, everything will be imported inside "/Assets/Plugins/Fabate/UMCS".
It will throw some errors like that:

![image](https://user-images.githubusercontent.com/24304905/232238054-e406221a-249d-4d67-abdc-2dd2517c7447.png)

You have to install all required dependencies (view Dependencies section and install the labeled ones)

## How to use it
In your scene create an empty gameobject and attach the Gamepad Manager component.

![image](https://user-images.githubusercontent.com/24304905/232223140-c55e4a8e-e4d4-4e13-91ee-569df3cded65.png)

## Dependencies
- [Looking-Glass/JoyconLib](https://github.com/Looking-Glass/JoyconLib) version 0.6
- [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4/manual/index.html) version 1.4.4 - **to install manually**

# PHAROS VR Observatory

## Quick Deployment to Oculus Quest VR Device

Install [SideQuest](https://sidequestvr.com/#/download) and fetch the PHAROS VR APK in the releases section of this repository. Connect your Oculus Quest device via USB 3.0 and use SideQuest to load the APK file into Quest, it should appear in your library. To do so, you will need to set your Oculus Quest device in Developer Mode, refer to a later section in this README (Connecting and Setting Up Oculus Quest for Development).

## Development Instructions

If you want to develop the code here you can find the requirements, how to clone the repo, and how to set up the whole project and deploy it in the device. Pull requests are welcome.

### Requirements

* Git.
* Unity 2019.2.14 (f12).
* Unity Android Build Support and Android SDK & NDK tools (for Oculus Quest deployment).
* Oculus Quest for deployment.

### How to Download and Develop

Fetch this repository by cloning it:

```
git clone https://github.com/csic-ice-magnesia/pharos_vr.git
```

Open UnityHub after installing Unity, in the window that pops up hit "Add" and browse for the folder where you cloned the repository. Everything should be automatically imported and ready to develop.

### How to Deploy for VR with Oculus Quest with Unity

Most of the settings for developing and deploying in VR for the Oculus Quest headset are already included in the project so you could skip to the "Add an Oculus App ID" subsection down this section to proceed to the actual deployment. If you are curious you can read the first sections to know how this project was set up for VR development and deployment.

#### Project Settings

Some project settings must be modified to make it work in VR (specifically on VR for Android since we are developing for Oculus Quest). To do so, naviagate to the “Edit” menu in the toolbar and select “Project Settings”. In the window that pops up, select “Player” in the left drop-down menu.

In the right side, select the “Android” tab and scroll down to find the “XR Settings” section. In such section, tick the “Virtual Reality Supported” option. In the same section, use the “+” sign under the “Virtual Reality SDKs” option to add a new SDK and select “Oculus”.

Now scroll down to find “Other Settings” and navigate to “Graphics APIs”. If “Vulkan” is present, remove it using the “-” sign.

Also in this section, it is important to remark that the Oculus Quest need a minimum Android API level 19 so we are going to specify it. Scroll down to find “Minimum API Level” and set it to “Android 4.4 KitKat (API Level 19)”.

#### Add an Oculus App ID

In order to later deploy the app to a developer-mode enabled Quest device, the project needs to be linked to an Oculus App developer ID.

If the plugin was properly installed, you should see a new “Oculus” menu in the toolbar. Go for it and navigate to “Platform” and later to “Edit Settings”.

The window that pops up will complain about a “Valid Oculus Rift App ID” and “Valid User Credentials”.

If you don’t have one yet, click on the button that says “Create/Find your app on Dashboard”. That will redirect you to the Oculus dashboard homepage, login with your account (the same you are using for the Oculus Quest phone app) and hit “Create new app” choose Oculus Quest and give it a name. The next screen will show you the app ID that you need to put on the Unity screen that you saw before and untick the “Use standalone platform” checkbox.

#### Connecting and Setting Up Oculus Quest for Development

Before connecting the headset to the computer, they must be set in Developer Mode for special access and for deploying apps to the device.

To do so, open the Oculus app in your phone and connect to your Oculus Quest device. It is important to use the same account in the app as the one used to generate the app key. If so, go to the “Settings” tab, tap your headset, once it is connected navigate to “More Settings” and then “Developer Mode”, toogle it on.

Now you can connect the Quest to your computer using a USB-C cable.
The first time you do this you’ll get a dialog in the headset asking permission to allow the connected computer to access the Quest. You’ll need to put on the headset and use the controller to allow permission.

#### Deploying Unity app to Quest Device

Now it is time to generate an Android app file (APK) with the current Unity scene and deploy it in the connected device.

First, go to the toolbar menu “File” and then to “Build Settings”. In that dialog, select the “Android” platform and change texture compression to “ATSC”. After that, press the “Switch Platform” button, it will take some time. If it is already in "Android" there is no need to do anything.

Open the desired Unity scene (if have just created the project you probably have the only one you have already opened) and go back to the “Build Settings” dialog and hit “Add Open Scene” which will add the current scene for the deployment.

In the “Run Device” dropdown menu down there in the dialog you should be able to select your Oculus Quest device, do it.

Hit “Build and Run” with the headset on and connected, provide a path in your computer to generate the APK file and grab a coffee, it will take some time.

## License

This project is licensed under the MIT License, permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## Credits

* Texture: "MilkyWay galaxy sphere map 8k" by darth-biomech (https://www.deviantart.com/darth-biomech). Licensed under Creative Commons Attribution-Share Alike 3.0 License.
* Music: "Ambient Motivational" by Alexander Nakarada (www.serpentsoundstudios.com) Licensed under Creative Commons BY Attribution 4.0 License.

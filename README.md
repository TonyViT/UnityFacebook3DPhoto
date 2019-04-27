# UnityFacebook3DPhoto
A plugin to let you shoot easily 3D Photos for Facebook directly from Unity.

With this solution you will be able to just press a key in your Unity project and generate instantly the color and depth images required to publish a 3D Photo post of Facebook. The photos will be taken by a Camera that is in the Unity scene and saved on the disk. This way you will be able to promote your Unity project on Facebook in a more innovative way.

The solution is experimental, so feel free to experiment with it and improve its features.

## Getting Started
To see what the UnityFacebook3DPhoto plugin is able to do, my advice is to start by just giving it a try. So, clone this repository, open the project with Unity and open the SampleScene that is located in \Assets\3DPhotos\Sample Scene. Run the application in the editor and press P on your keyboard. Now go to the main folder of the project (the parent folder of Assets): you should find two images, with a name like "UnityApp_Photo3D_2019_4_27_17_1_7.png" and "UnityApp_Photo3D_2019_4_27_17_1_7_depth.png". If you try to drag contemporarily them on Facebook, Facebook will create a 3D post out of them.

## Using it in other projects
After you have tested the sample, you would like for sure to use this plugin in a project of yours. To do this, you have just to:
* Copy the \Assets\3DPhotos folder inside the Assets folder of your Unity project;
* Drag into your Unity scene the 3DPhotoCamera Prefab that is located in the \Assets\3DPhotos\Prefabs folder
* Tweak the parameters of the Photo3DShooter behaviour attached to the prefab, if needed
* Modify the code of the Photo3DShooter script, if needed
* Run your application and trigger the photo, using the P key

The resulting Photo files will be located in:
* The main project folder, if you are in the Unity editor
* The executable folder, if you are launching an .EXE on windows
* The \DCIM\\&lt;ProgramTag> folder, on Android (included AR and VR devices that run Android). "ProgramTag" is one of the parameters of the script, that is set by default as "UnityApp"

### Script parameters
The parameters of the Photo3DShooter behaviour define how the 3D photo will be shot. They are heavily commented in the code and also through tooltips, so it is easy for the developer modifying them.

They are:
* Postprocess Material: you had better not changing it. It is a material used to create the depth map
* Depth Multiplier: it is a constant for which the depth values of the scene gets multiplied for. Unity computes the depth of the scene as a value from 0 to 1. With this multiplier, you can amplify the depth value. Since the resulting color is computed by 1 - multiplied_depth, the more this value is big, the more the depth map will appear black and background objects will disappear from the photo. The more this value is small, the more all objects in the scene will be visualized. NOTE: the default value is 5, but to shoot the photo for my mixed reality game Hit Motion, I had to use a value of around 400, because all objects were close to the camera and I wanted more precision in detecting their different depths. So, experiment with various values: if all objects are white, make this value bigger.
* Photo Width, Photo Height: used to determine the resolution of the 3D Photo files
* Program Tag: name of your application. It is used to set the filename of the photo. On Android, it also determines the folder inside DCIM into which store the photo
* Reference Camera: if this value is null, the photo will be shot by the current Camera present into the prefab, from its position and orientation and with its parameters. If this value is not null, the photo will be shot from the position and rotation of the Reference Camera and using the render parameters of the Reference Camera

### Required code modifications
You may want to modify the code to let it suit your needs. 

This may be useful for instance to:
* Save the photos in a different folder or with a different name;
* Implement saving mechanics for other OS (currently only Windows and Android are supported)
* Implement a new mechanism to trigger the photo: e.g. if you use a VR headset, you would probably want to trigger the photo when the user presses the Trigger button on his controller and not when the S key is pressed. This can be easily modified in the Update method of the script 3DPhotoCamera

## Understanding the code
The code is very short and highly commented.

In short, the 3DPhotoCamera behaviour, when the P key is pressed, renders an image from the point of view of the camera that is attached to its gameobject. This is the color image. Then it renders a second image that gets modified by the shader DepthPostProcessing so that every pixel in this new image gets a color that corresponds to its depth in the scene, and so you also have the depth map. These images gets later saved to disk.

## Prerequisites
If you want to use the project, you must have Unity editor installed. It has been developed and tested with Unity 2018.3.6f1.
  
## Known issues
This is an experimental project in its first version and so has various problems:
* It is not compatible with all platforms (but you can easily expand it in this sense)
* It is not multithread, so when the photo gets shot, the program halts for an instant
* It may be not the most efficient implementation to shoot a 3D photo

People more skilled than me are welcome to contribute to this project... to improve and expand it.

## Authors

* **Antony Vitillo (Skarredghost)** - [Blog](http://skarredghost.com)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

This project uses in part some code from [Ronja Tutorials](https://github.com/ronja-tutorials/ShaderTutorials) that is released under the CC0 1.0 license

## Acknowledgments

I'm releasing this for free, to be helpful for the community. I would really appreciate whatever kind of support if you use this plugin in your project: a hug, a thank you, a subscription to the newsletter of [my blog](https://skarredghost.com), a mention in the credits of your project, a collaboration proposal for your XR project, a donation of 1 million Euros, the phone number of Scarlett Johansson, etc... 

You can contact me [here](https://skarredghost.com/contact/) if you wish.

Have fun :)

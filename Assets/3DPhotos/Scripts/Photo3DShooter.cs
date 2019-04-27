//MIT License

//Copyright(c) 2019 Antony Vitillo(a.k.a. "Skarredghost")

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

//The code is a inspired by a tutorial by Ronja, that is released until a CC-0 license
//You can find his tutorial at this link https://www.ronja-tutorials.com/2018/07/01/postprocessing-depth.html
//And the related GitHub repository at this link https://github.com/ronja-tutorials/ShaderTutorials

using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Photo3D
{
    /// <summary>
    /// Shoots a 3D photo
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class Photo3DShooter : MonoBehaviour
    {
        /// <summary>
        /// Material used to compute the depth distance
        /// </summary>
        [Tooltip("Material used to compute the depth map... do not touch :)")]
        public Material PostprocessMaterial;

        /// <summary>
        /// Multiplier of depth value
        /// </summary>
        [Tooltip("Constant for which the depth map computed by Unity has to be multiplied. The more it is small, the more the depth map becomes white; the more it is big, the more the depth map becomes black and background objects disappear in the 3D Photo")]
        public float DepthMultiplier = 5f;

        /// <summary>
        /// Width of the screenshot to take
        /// </summary>
        [Tooltip("Width of the 3D Photo")]
        public int PhotoWidth = 1920;

        /// <summary>
        /// Height of the screenshot to take
        /// </summary>
        [Tooltip("Height of the 3D Photo")]
        public int PhotoHeight = 1080;

        /// <summary>
        /// The name of the app, used to change the filename and the folder used to save the photo
        /// </summary>
        [Tooltip("The name of your application")]
        public string ProgramTag = "UnityApp";

        /// <summary>
        /// Reference camera to be used to take the photos. If this value is null, the photo will be taken with the camera attached to this object, as is.
        /// If the value is not null, the camera that takes the 3D photo will copy all parameters of the reference camera and will also match the position and rotation of the reference camera when shooting the photo.
        /// </summary>
        [Tooltip("Reference camera to shoot the photos. If this value is null, the photo will be taken with the camera attached to this object, as is. If the value is not null, the camera that takes the 3D photo will copy all parameters of the reference camera and will also match the position and rotation of the reference camera when shooting the photo.")]
        public Camera ReferenceCamera = null;

        /// <summary>
        /// Render Texture used to shoot the photos from this camera
        /// </summary>
        private RenderTexture m_renderTexture;

        /// <summary>        
        /// Texture used to copy the values of the render texture
        /// </summary>
        private Texture2D m_texture;

        /// <summary>
        /// Reference to the camera attached to this object
        /// </summary>
        private Camera m_camera;

        /// <summary>
        /// False if we are shooting the color photo, true if we are shooting the depth map photo
        /// </summary>
        private bool m_depthImage;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            //get the camera and initialize it
            m_camera = GetComponent<Camera>();

            if (ReferenceCamera != null)
                m_camera.CopyFrom(ReferenceCamera); //copy parameters of reference camera

            m_camera.depthTextureMode = m_camera.depthTextureMode | DepthTextureMode.Depth; //needed to compute the depth map
            m_camera.enabled = false;

            //init textures
            m_renderTexture = new RenderTexture(PhotoWidth, PhotoHeight, 24, RenderTextureFormat.ARGB32);
            m_renderTexture.Create();
            m_texture = new Texture2D(PhotoWidth, PhotoHeight, TextureFormat.ARGB32, false);

            //set depth multiplier, as required by the user
            PostprocessMaterial.SetFloat("_DepthMultiplier", DepthMultiplier);
        }

        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            //TODO: Change this to match your requirements. Most probably on Android or in AR/VR experiences you will have to set your own mechanics to trigger the Capture function
            if (Input.GetKeyDown(KeyCode.P))
                Capture();
        }

        /// <summary>
        /// Method which is automatically called by unity after the camera is done rendering
        /// </summary>
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            //draws the pixels from the source texture to the destination texture. If we are computing the depth image, apply the special shader to compute it, otherwise just copy the image as-is
            if (m_depthImage)
                Graphics.Blit(source, destination, PostprocessMaterial);
            else
                Graphics.Blit(source, destination);
        }

        /// <summary>
        /// Capture the 3D photo
        /// </summary>
        private void Capture()
        {
            //copy pose of reference camera, if needed
            if (ReferenceCamera != null)
                m_camera.transform.SetPositionAndRotation(ReferenceCamera.transform.position, ReferenceCamera.transform.rotation);        

            //enable camera and render texture
            m_camera.enabled = true;
            m_camera.targetTexture = m_renderTexture;
            RenderTexture.active = m_renderTexture;

            //compute date for the file name. We do it here, otherwise the name of the file may change between depth and color computations
            DateTime nowTime = DateTime.Now;

            //shoot Color photo
            RenderAndSaveImage(false, nowTime);

            //shoot Depth photo
            RenderAndSaveImage(true, nowTime);           

            //disable camera and render texture
            RenderTexture.active = null;
            m_camera.targetTexture = null;
            m_camera.enabled = false;
        }

        /// <summary>
        /// Renders an image from current camera and saves it to a file on disk
        /// </summary>
        /// <param name="isDepthImage">True if we must compute depth map, false for color image</param>
        /// <param name="nowTime">Present time used to save the images</param>
        private void RenderAndSaveImage(bool isDepthImage, DateTime nowTime)
        {
            //set depth parameter: it will be used by the OnRenderImage method called internally by the Render() method of the camera to decide if applying a depth-compute shader or not
            m_depthImage = isDepthImage;
            
            //render the scene and save it to texture
            m_camera.Render();            
            m_texture.ReadPixels(new Rect(0, 0, PhotoWidth, PhotoHeight), 0, 0); 

            //convert to PNG
            byte[] imgPng = m_texture.EncodeToPNG();

            //save image to disk
            //TODO: put it into another thread to not make the saving block the application
            StartCoroutine(SaveImage(imgPng, isDepthImage, nowTime));            
        }

        /// <summary>
        /// Saves the image to disk
        /// </summary>
        /// <param name="imageToSave">PNG image to save</param>
        /// <param name="isDepthImage">True if it is a depth image, false otherwise</param>
        /// <param name="nowTime">Present time used to compute the filename</param>
        /// <returns></returns>
        private IEnumerator SaveImage(byte[] imageToSave, bool isDepthImage, DateTime nowTime)
        {
            //create a file name. Notice that the depth map must end with "_depth"
            string fileName = String.Format("{0}_Photo3D_{1}_{2}_{3}_{4}_{5}_{6}.png", ProgramTag, nowTime.Year, nowTime.Month, nowTime.Day, nowTime.Hour, nowTime.Minute, nowTime.Second + (isDepthImage ? "_depth" : ""));

#if UNITY_STANDALONE || UNITY_EDITOR
            //on PC, let's save the image on the directory of the executable
            //If we are in the Unity editor, it will be saved in the directory of the project
            File.WriteAllBytes(Application.dataPath + @"\..\" + fileName, imageToSave);
            yield break;

#elif UNITY_ANDROID
            //on Android devices (so also VR standalones), let's save the image in "/DCIM/ProgramTag/

            string imageGalleryPath = "";
            string temporaryImageName = "image";

            //set the directory into which save the image so that when you connect the Focus to the PC it appears in /DCIM/FocusSelfies
            try
            {
                using (var environment = new AndroidJavaClass("android.os.Environment"))
                {

                    var dirFile = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory");
                    imageGalleryPath = dirFile.Call<string>("getAbsolutePath");
                }

            }
            catch (Exception e)
            {
                yield break;
            }

            imageGalleryPath += "/DCIM/" + ProgramTag;

            //save the image to a temporary file into application internal path. Wait until the file gets created
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + temporaryImageName + ".png", imageToSave);

            while (!System.IO.File.Exists(Application.persistentDataPath + "/" + temporaryImageName + ".png"))
                yield return null;            

            //copy the temporary file to the final destination
            if (!System.IO.Directory.Exists(imageGalleryPath))
                System.IO.Directory.CreateDirectory(imageGalleryPath);
            System.IO.File.Copy(Application.persistentDataPath + "/" + temporaryImageName + ".png", imageGalleryPath + "/" + fileName);

            //delete the temporary file
            System.IO.File.Delete(Application.persistentDataPath + "/" + temporaryImageName + ".png");
#endif
        }
    }

}
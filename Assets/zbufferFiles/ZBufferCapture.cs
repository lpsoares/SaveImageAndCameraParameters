using System.Collections;
using System.Collections.Generic;
// using System.Numerics;
using UnityEngine;
using System.IO;

public class ZBufferCapture : MonoBehaviour
{
    public int subdivisions = 3;
    public float radius = 1000f;

    private int fileCounter = 0;

    private Mesh sphereMesh;

    public KeyCode screenshotKey = KeyCode.Space;

    public IcosphereGenerator icosphereGenerator;

    public Camera _camera;

    private string basePath;

    void Start()
    {
        basePath=  Application.dataPath + "/zbufferFiles/trainTest";
        //delete all files under basepath
        System.IO.DirectoryInfo di = new DirectoryInfo(basePath);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        //create icosphere
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        // getting icosphere generator script
        icosphereGenerator = GetComponent<IcosphereGenerator>();
        mesh.Clear();
        sphereMesh = icosphereGenerator.Create(subdivisions, radius);

    }


 IEnumerator CaptureImages() {
        
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Wait function
       
        
        foreach (Vector3 vertex in sphereMesh.vertices){

                Vector3 inward = -vertex.normalized;
                //debug the main camera forward vector
                _camera.transform.position = vertex;
                _camera.transform.LookAt(vertex + inward);
                GetTextureFromCamera(_camera);
                yield return wait; //Pause the loop for some second. 
                fileCounter++;

            }
        //end
        UnityEditor.EditorApplication.isPlaying = false;

    }



    private void LateUpdate() {
        if (Input.GetKeyDown(screenshotKey)) {
            StartCoroutine(CaptureImages());
        }
    }
 
    private void GetTextureFromCamera(Camera mCamera)
    {
        Rect rect = new Rect(0, 0, mCamera.pixelWidth, mCamera.pixelHeight);
        RenderTexture renderTexture = new RenderTexture(mCamera.pixelWidth, mCamera.pixelHeight, 24);
        Texture2D screenShot = new Texture2D(mCamera.pixelWidth, mCamera.pixelHeight, TextureFormat.RGBA32, false);
    
        mCamera.targetTexture = renderTexture;
        mCamera.Render();
    
        RenderTexture.active = renderTexture;
    
        screenShot.ReadPixels(rect, 0, 0);
        screenShot.Apply();
    
    
        mCamera.targetTexture = null;
        RenderTexture.active = null;
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(basePath+"/i_"+fileCounter + ".png", bytes);
        Destroy(screenShot);

    }



}

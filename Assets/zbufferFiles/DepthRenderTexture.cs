using System.Collections;
using UnityEngine;
using System.IO;
    
[RequireComponent(typeof(Camera))]
public class DepthRenderTexture : MonoBehaviour
{
    public RenderTexture RTDepth;
    public RenderTexture RTResult;

    public Camera _camera;

    private string basePath;

    public int subdivisions = 3;
    public float radius = 10f;

    private Mesh sphereMesh;

    public IcosphereGenerator icosphereGenerator;

    private int fileCounter = 0;

    public string folderPath;

    void Start()
    {
        basePath=  Application.dataPath + "/" + folderPath;
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
        Debug.Log(icosphereGenerator);
        mesh.Clear();
        sphereMesh = icosphereGenerator.Create(subdivisions, radius);

        _camera = Camera.main;
        StartCoroutine(CaptureImages());
    }

    IEnumerator CaptureImages() {
        
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Wait function
       
        
        foreach (Vector3 vertex in sphereMesh.vertices){

                Vector3 inward = -vertex.normalized;
                //debug the main camera forward vector
                _camera.transform.position = vertex;
                _camera.transform.LookAt(vertex + inward);
                Debug.Log(_camera.transform.position);
                string filePath = basePath+"/i_"+fileCounter + ".png";
                GetDepth(filePath);
                yield return wait; //Pause the loop for some second. 
                fileCounter++;

            }
        //end
        UnityEditor.EditorApplication.isPlaying = false;

    }

    void GetDepth(string filePath){
        if (RTResult)
        RenderTexture.ReleaseTemporary(RTResult);

        RTDepth = RenderTexture.GetTemporary(_camera.pixelWidth, _camera.pixelHeight, 24, RenderTextureFormat.Depth); // only a depth buffer
        RTResult = RenderTexture.GetTemporary(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.RFloat); // depth buffer copy


        _camera.targetTexture = RTDepth;

        _camera.Render();
        _camera.targetTexture = null;

        Texture2D depthTexture2D = new Texture2D(RTResult.width, RTResult.height, TextureFormat.RFloat, false);
        RenderTexture.active = RTResult;
        Graphics.Blit(RTDepth, RTResult);

        
        depthTexture2D.ReadPixels(new Rect(0, 0, RTResult.width, RTResult.height), 0, 0);
        depthTexture2D.Apply();

        // Encode depth texture to PNG bytes
        byte[] bytes = depthTexture2D.EncodeToPNG();
        Destroy(depthTexture2D);

        // Save PNG bytes to file
        
        System.IO.File.WriteAllBytes(filePath, bytes);

        RenderTexture.ReleaseTemporary(RTDepth);
    }
    
}

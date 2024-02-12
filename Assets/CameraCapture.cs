using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
 
public class CameraCapture : MonoBehaviour
{
    public int fileCounter;  // count each image generated 
    
    public KeyCode screenshotKey = KeyCode.Space;  // defines key to start capture process
    
    public Vector3 origin;  // origin for rotation

    public float azimuth_start_angle; // horizontal
    //public float zenith_start_angle; // vertical

    public float azimuth_step_divisions; // horizontal
    public float zenith_step_divisions; // vertical


    private Camera Camera {
        get {
            if (!_camera) {
                _camera = Camera.main;
            }
            Debug.Log("in camera");
            Debug.Log(_camera);
            return _camera;
        }
    }

    private Camera _camera;

    private void Start() {
        System.IO.DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/train");
        // empty train directory
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete(); 
        }
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        transform.RotateAround(origin, transform.right, 90); // start on the top
        //transform.RotateAround(origin, transform.right, 90-zenith_start_angle);
        transform.RotateAround(origin, Vector3.up, azimuth_start_angle);
        Debug.Log("in main");
        Debug.Log(_camera);
    }
 
    IEnumerator SomeCoroutine() {
        
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Wait function
 
        string path = "Assets/transforms_train.json";
        string ImagesTxtPath = "Assets/images_train.txt";
        //fieldOfView = 0.6911112070083618
 
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        StreamWriter ImageTxtWriter = new StreamWriter(ImagesTxtPath,true);

        Debug.Log(zenith_step_divisions);
        for(int i = 0; i < zenith_step_divisions; i++) {
            Debug.Log(i);
            
            for(int j = 0; j < azimuth_step_divisions; j++) {  // horizontal
                string name  = "./train/i_" + fileCounter; 
                var matrix = transform.localToWorldMatrix;
                var worldRotation = transform.rotation;
                var position = new Vector3(matrix[0,3], matrix[1,3], matrix[2,3]);
                var ID = i*10 + j;
                var cameraID = 1;
                //Debug.Log("Transform position from matrix is: " + matrix);

                WriteImagesTxt(ID, name, worldRotation, position, cameraID, ImageTxtWriter);
                WriteTransformJson(i, matrix, writer, fileCounter, zenith_step_divisions,name);
                Capture();

                fileCounter++;
                yield return wait; //Pause the loop for some second. 

                transform.RotateAround(origin, Vector3.up, (360.0f / azimuth_step_divisions) );
            }

            transform.RotateAround(origin, transform.right, -(180.0f / zenith_step_divisions));

        }

        writer.WriteLine("\t]");
        writer.WriteLine("}");
        writer.Close();
        ImageTxtWriter.Close();
        //end
        UnityEditor.EditorApplication.isPlaying = false;

    }


    private void WriteTransformJson(int i, Matrix4x4 matrix, StreamWriter writer, int fileCounter, float zenith_step_divisions, string name) {
                if (i == 0){
                    writer.WriteLine("{");
                    writer.WriteLine("\t\"camera_angle_x\": " + (Math.PI / 180) * Camera.fieldOfView + ",");
                    writer.WriteLine("\t\"frames\": [");
                }
                
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\t\"file_path\": \""+name + "\",");
                writer.WriteLine("\t\t\t\"rotation\": 0.012566370614359171,");
                writer.WriteLine("\t\t\t\"transform_matrix\": [");
                
                writer.WriteLine("\t\t\t[");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[0,0]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[0,1]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[0,2]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[0,3]));
                writer.WriteLine("\t\t\t],");

                writer.WriteLine("\t\t\t[");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[1,0]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[1,1]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[1,2]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[1,3]));
                writer.WriteLine("\t\t\t],");

                writer.WriteLine("\t\t\t[");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[2,0]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[2,1]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[2,2]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[2,3]));
                writer.WriteLine("\t\t\t],");

                writer.WriteLine("\t\t\t[");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[3,0]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[3,1]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[3,2]) + ",");
                writer.WriteLine("\t\t\t\t" + System.String.Format("{0:0.################}", matrix[3,3]));
                writer.WriteLine("\t\t\t]");

                writer.WriteLine("\t\t]");
                writer.WriteLine("\t\t}" + (i != zenith_step_divisions-1 ? "," : "") );
    }

    private void WriteImagesTxt(int ID, string NAME, Quaternion Q, Vector3 T, int CAMERA_ID, StreamWriter writer){ //assuming second line is optional we don't need , NAME, POINTS2D[] as (X, Y, POINT3D_ID)
        float QW = Q.w;
        float QX = Q.x;
        float QY = Q.y;
        float QZ = Q.z;
        
        float TX = T.x;
        float TY = T.y;
        float TZ = T.z;

        writer.WriteLine($"{ID} {QW} {QX} {QY} {QZ} {TX} {TY} {TZ} {CAMERA_ID} {NAME}");
        // writer.WriteLine("2362.39 248.498 58396 1784.7 268.254 59027 1784.7 268.254 -1");
    }

    private void WriteCamerasTxt(){
        
    }
    
 
    private void LateUpdate() {
        if (Input.GetKeyDown(screenshotKey)) {
            StartCoroutine(SomeCoroutine());
        }
    }
 
    public void Capture() {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;
 
        Camera.Render();
 
        Texture2D image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;
 
        byte[] bytes = image.EncodeToPNG();
        Destroy(image);
 
        File.WriteAllBytes(Application.dataPath + "/train/i_" + fileCounter + ".png", bytes);
        
    }

}



// {
//     "camera_angle_x": 0.6911112070083618,
//     "frames": [
//         {
//             "file_path": "./train/r_0",
//             "rotation": 0.012566370614359171,
//             "transform_matrix": [
//                 [
//                     -0.9999021887779236,
//                     0.004192245192825794,
//                     -0.013345719315111637,
//                     -0.05379832163453102
//                 ],
//                 [
//                     -0.013988681137561798,
//                     -0.2996590733528137,
//                     0.95394366979599,
//                     3.845470428466797
//                 ],
//                 [
//                     -4.656612873077393e-10,
//                     0.9540371894836426,
//                     0.29968830943107605,
//                     1.2080823183059692
//                 ],
//                 [
//                     0.0,
//                     0.0,
//                     0.0,
//                     1.0
//                 ]
//             ]
//         },
//     ]
// }


// # Camera list with one line of data per camera:
// #   CAMERA_ID, MODEL, WIDTH, HEIGHT, PARAMS[]
// # Number of cameras: 3
// 1 SIMPLE_PINHOLE 3072 2304 2559.81 1536 1152
// 2 PINHOLE 3072 2304 2560.56 2560.56 1536 1152
// 3 SIMPLE_RADIAL 3072 2304 2559.69 1536 1152 -0.0218531



// # Image list with two lines of data per image:
// #   IMAGE_ID, QW, QX, QY, QZ, TX, TY, TZ, CAMERA_ID, NAME
// #   POINTS2D[] as (X, Y, POINT3D_ID)
// # Number of images: 2, mean observations per image: 2
// 1 0.851773 0.0165051 0.503764 -0.142941 -0.737434 1.02973 3.74354 1 P1180141.JPG
// 2362.39 248.498 58396 1784.7 268.254 59027 1784.7 268.254 -1
// 2 0.851773 0.0165051 0.503764 -0.142941 -0.737434 1.02973 3.74354 1 P1180142.JPG
// 1190.83 663.957 23056 1258.77 640.354 59070
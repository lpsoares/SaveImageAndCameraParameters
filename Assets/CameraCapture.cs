using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using System.IO;
 
public class CameraCapture : MonoBehaviour
{
    public int fileCounter;  // count each image generated 
    
    public KeyCode screenshotKey;  // defines key to start capture process
    
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
            return _camera;
        }
    }

    private Camera _camera;

    private void Start() {
        System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

        transform.RotateAround(origin, transform.right, 90); // start on the top
        //transform.RotateAround(origin, transform.right, 90-zenith_start_angle);
        transform.RotateAround(origin, Vector3.up, azimuth_start_angle);
    }
 
    IEnumerator SomeCoroutine() {
        
        WaitForSeconds wait = new WaitForSeconds(0.2f); // Wait function
 
        string path = "Assets/transforms_train.json";
 
        //fieldOfView = 0.6911112070083618
 
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("{");
        writer.WriteLine("\t\"camera_angle_x\": " + (Math.PI / 180) * Camera.fieldOfView + ",");
        writer.WriteLine("\t\"frames\": [");
        

        for(int i = 0; i < zenith_step_divisions; i++) {
            
            for(int j = 0; j < azimuth_step_divisions; j++) {  // horizontal

                var matrix = transform.localToWorldMatrix;
                //var position = new Vector3(matrix[0,3], matrix[1,3], matrix[2,3]);
                //Debug.Log("Transform position from matrix is: " + matrix);

                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\t\"file_path\": \"./train/i_" + fileCounter + "\",");
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
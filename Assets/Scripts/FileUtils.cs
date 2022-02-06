using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Globalization;

using SFB;

public static class FileUtils
{
    // Start is called before the first frame update
    public static void ExportMap(in Mesh mesh, int nx, int ny) {
        //string path = EditorUtility.SaveFilePanel("Save to ...", "", "unity_export.obj","obj");
        string path = StandaloneFileBrowser.SaveFilePanel("Save to ...", "", "unity_export.obj","obj");

        if (path.Length != 0) {
            StringBuilder file = new StringBuilder();
            file.Append("# Map export from Unity\n");
            file.Append("# by Yannick Kwansa\n");
            file.Append("\n");

            CultureInfo c = CultureInfo.InvariantCulture;
            foreach(Vector3 v in mesh.vertices) { // Les vertices
                file.Append("v " + v.x.ToString("G", c) + " " + v.y.ToString("G", c) + " " + v.z.ToString("G", c) + "\n");
            }

            file.Append("\n");

            foreach(Vector3 v in mesh.normals) { // Les normales
                file.Append("vn " + v.x.ToString("G", c) + " " + v.y.ToString("G", c) + " " + v.z.ToString("G", c) + "\n");
            }
            
            file.Append("\n");

            for(int i = 0; i < nx * ny * 6; i += 3) { // Les faces
                file.Append("f " + (mesh.triangles[i] + 1) + " " + (mesh.triangles[i + 1] + 1) + " " + (mesh.triangles[i + 2] + 1) + "\n");
            }            

            StreamWriter sw = new StreamWriter(path); // Sauvegarde du fichier
            sw.Write(file);
            sw.Close();
        }
    }

    public static void ExportPNG(in Mesh mesh, int nx, int ny) {
        /*string path = EditorUtility.SaveFilePanel("Save to ...", "", "unity_export.png","png");

        if(path.Length > 0) {
            Texture2D export = new Texture2D(nx, ny);
            export.SetPixels32(mesh.colors32,0);
            export.Apply();

            byte[] bytes = export.EncodeToPNG();

            File.WriteAllBytes(path, bytes); // Sauvegarde du fichier
        }*/
    }

    public static Texture2D LoadPNG() {
        //string path = EditorUtility.OpenFilePanel("Open from ...", "", "png");
        string path = StandaloneFileBrowser.OpenFilePanel("Open from ...", "", "png", false)[0];
        
        byte[] bytes = File.ReadAllBytes(path);

        
        Texture2D res = new Texture2D(1,1);
        res.LoadImage(bytes);

        return res;
    }
}
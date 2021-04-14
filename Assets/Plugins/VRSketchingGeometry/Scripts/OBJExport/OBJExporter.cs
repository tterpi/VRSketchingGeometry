using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

/*=============================================================================
 |	    Project:  Unity3D Scene OBJ Exporter
 |
 |		  Notes: Only works with meshes + meshRenderers. No terrain yet
 |
 |       Author:  aaro4130
 |       Adapted for non-editor use: tterpi
 |
 |     DO NOT USE PARTS OF THIS CODE, OR THIS CODE AS A WHOLE AND CLAIM IT
 |     AS YOUR OWN WORK. USE OF CODE IS ALLOWED IF I (aaro4130) AM CREDITED
 |     FOR THE USED PARTS OF THE CODE.
 |
 *===========================================================================*/

namespace VRSketchingGeometry.Export
{
    /// <summary>
    /// Export game objects as OBJ files at runtime.
    /// </summary>
    public class OBJExporter
    {
        public bool applyPosition = true;
        public bool applyRotation = true;
        public bool applyScale = true;
        public bool generateMaterials = true;
        public bool exportTextures = true;
        public bool splitObjects = true;
        public bool objNameAddIdNum = false;

        private string versionString = "v2.0";
        private string lastExportFolder;

        Vector3 RotateAroundPoint(Vector3 point, Vector3 pivot, Quaternion angle)
        {
            return angle * (point - pivot) + pivot;
        }

        Vector3 MultiplyVec3s(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        /// <summary>
        /// Get a file path for an exported sketch inside UnityEngine.Application.dataPath.
        /// The default folder is created if it doesn't exist.
        /// A file name that doesn't exist is chosen.
        /// </summary>
        /// <returns>file path</returns>
        public static string GetDefaultExportPath()
        {
            string defaultDirectoryName = "ExportedSketches";
            string defaultFileName = "SketchExport_";
            string fileExtension = ".obj";
            int fileNr = 0;
            string fullPath;

            string fullDirectoryPath = System.IO.Path.Combine(Application.dataPath, defaultDirectoryName);

            if (!System.IO.Directory.Exists(fullDirectoryPath))
            {
                System.IO.Directory.CreateDirectory(fullDirectoryPath);
            }

            do
            {
                fullPath = System.IO.Path.Combine(Application.dataPath, defaultDirectoryName, defaultFileName + fileNr.ToString());
                fullPath = System.IO.Path.ChangeExtension(fullPath, fileExtension);
                fileNr++;
            }
            while (System.IO.File.Exists(fullPath));

            return fullPath;
        }

        /// <summary>
        /// Export all meshes that are part of this gameObject or it's children to a .obj file.
        /// If enabled, a material file will also be created.
        /// </summary>
        /// <param name="gameObject">The game object to be exported.</param>
        /// <param name="exportPath">Full path that contains file name and extension.</param>
        public void ExportGameObject(GameObject gameObject, string exportPath)
        {
            System.Globalization.CultureInfo originalCulture = System.Globalization.CultureInfo.CurrentCulture;
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            //init stuff
            var exportFileInfo = new System.IO.FileInfo(exportPath);
            lastExportFolder = exportFileInfo.Directory.FullName;
            string baseFileName = System.IO.Path.GetFileNameWithoutExtension(exportPath);

            //get list of required export things
            MeshFilter[] sceneMeshes = gameObject.GetComponentsInChildren<MeshFilter>();
            //sceneMeshes = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];

            if (Application.isPlaying)
            {
                foreach (MeshFilter mf in sceneMeshes)
                {
                    MeshRenderer mr = mf.gameObject.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        if (mr.isPartOfStaticBatch)
                        {
                            Debug.LogError("Static batched object detected. Static batching is not compatible with this exporter. Please disable it before starting the player.");
                            return;
                        }
                    }
                }
            }

            //work on export
            GetObjFileContent(baseFileName, sceneMeshes, out StringBuilder sb, out StringBuilder sbMaterials);

            //write to disk
            System.IO.File.WriteAllText(exportPath, sb.ToString());
            if (generateMaterials)
            {
                System.IO.File.WriteAllText(exportFileInfo.Directory.FullName + "\\" + baseFileName + ".mtl", sbMaterials.ToString());
            }

            System.Globalization.CultureInfo.CurrentCulture = originalCulture;
        }

        /// <summary>
        /// Get the contents of the obj and mtl file as StringBuilders.
        /// </summary>
        /// <param name="baseFileName"></param>
        /// <param name="sceneMeshes"></param>
        /// <param name="meshStringBuilder"></param>
        /// <param name="materialStringBuilder"></param>
        public void GetObjFileContent(string baseFileName, MeshFilter[] sceneMeshes, out StringBuilder meshStringBuilder, out StringBuilder materialStringBuilder)
        {
            //work on export
            Dictionary<string, bool> materialCache = new Dictionary<string, bool>();
            meshStringBuilder = new StringBuilder();
            materialStringBuilder = new StringBuilder();
            meshStringBuilder.AppendLine("#Based on Aaro4130 OBJ Exporter " + versionString);
            if (generateMaterials)
            {
                meshStringBuilder.AppendLine("mtllib " + baseFileName + ".mtl");
            }
            float maxExportProgress = (float)(sceneMeshes.Length + 1);
            int lastIndex = 0;
            for (int i = 0; i < sceneMeshes.Length; i++)
            {
                string meshName = sceneMeshes[i].gameObject.name;
                float progress = (float)(i + 1) / maxExportProgress;
                MeshFilter mf = sceneMeshes[i];
                MeshRenderer mr = sceneMeshes[i].gameObject.GetComponent<MeshRenderer>();

                if (splitObjects)
                {
                    string exportName = meshName;
                    if (objNameAddIdNum)
                    {
                        exportName += "_" + i;
                    }
                    meshStringBuilder.AppendLine("g " + exportName);
                }
                if (mr != null && generateMaterials)
                {
                    Material[] mats = mr.sharedMaterials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        Material m = mats[j];
                        if (!materialCache.ContainsKey(m.name))
                        {
                            materialCache[m.name] = true;
                            materialStringBuilder.Append(MaterialToString(m));
                            materialStringBuilder.AppendLine();
                        }
                    }
                }

                //export the meshhh :3
                Mesh msh = mf.sharedMesh;
                int faceOrder = (int)Mathf.Clamp((mf.gameObject.transform.lossyScale.x * mf.gameObject.transform.lossyScale.z), -1, 1);

                //export vector data (FUN :D)!
                foreach (Vector3 vx in msh.vertices)
                {
                    Vector3 v = vx;
                    if (applyScale)
                    {
                        v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale);
                    }

                    if (applyRotation)
                    {

                        v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                    }

                    if (applyPosition)
                    {
                        v += mf.gameObject.transform.position;
                    }
                    v.x *= -1;
                    meshStringBuilder.AppendLine("v " + v.x + " " + v.y + " " + v.z);
                }
                foreach (Vector3 vx in msh.normals)
                {
                    Vector3 v = vx;

                    if (applyScale)
                    {
                        v = MultiplyVec3s(v, mf.gameObject.transform.lossyScale.normalized);
                    }
                    if (applyRotation)
                    {
                        v = RotateAroundPoint(v, Vector3.zero, mf.gameObject.transform.rotation);
                    }
                    v.x *= -1;
                    meshStringBuilder.AppendLine("vn " + v.x + " " + v.y + " " + v.z);

                }
                foreach (Vector2 v in msh.uv)
                {
                    meshStringBuilder.AppendLine("vt " + v.x + " " + v.y);
                }

                for (int j = 0; j < msh.subMeshCount; j++)
                {
                    if (mr != null && j < mr.sharedMaterials.Length)
                    {
                        string matName = mr.sharedMaterials[j].name;
                        meshStringBuilder.AppendLine("usemtl " + matName);
                    }
                    else
                    {
                        meshStringBuilder.AppendLine("usemtl " + meshName + "_sm" + j);
                    }

                    int[] tris = msh.GetTriangles(j);
                    for (int t = 0; t < tris.Length; t += 3)
                    {
                        int idx2 = tris[t] + 1 + lastIndex;
                        int idx1 = tris[t + 1] + 1 + lastIndex;
                        int idx0 = tris[t + 2] + 1 + lastIndex;
                        if (faceOrder < 0)
                        {
                            meshStringBuilder.AppendLine("f " + ConstructOBJString(idx2) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx0));
                        }
                        else
                        {
                            meshStringBuilder.AppendLine("f " + ConstructOBJString(idx0) + " " + ConstructOBJString(idx1) + " " + ConstructOBJString(idx2));
                        }

                    }
                }

                lastIndex += msh.vertices.Length;
            }
        }

        private string TryExportTexture(string propertyName, Material m)
        {
            if (m.HasProperty(propertyName))
            {
                Texture t = m.GetTexture(propertyName);
                if (t != null)
                {
                    return ExportTexture((Texture2D)t);
                }
            }
            return "false";
        }
        private string ExportTexture(Texture2D t)
        {
            try
            {
                string exportName = lastExportFolder + "\\" + t.name + ".png";
                Texture2D exTexture = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false);
                exTexture.SetPixels(t.GetPixels());
                System.IO.File.WriteAllBytes(exportName, exTexture.EncodeToPNG());
                return exportName;
            }
            catch (System.Exception ex)
            {
                Debug.Log("Could not export texture : " + t.name + ". is it readable?\n" + ex.ToString());
                return "null";
            }

        }

        private string ConstructOBJString(int index)
        {
            string idxString = index.ToString();
            return idxString + "/" + idxString + "/" + idxString;
        }

        private string MaterialToString(Material m)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("newmtl " + m.name);


            //add properties
            if (m.HasProperty("_Color"))
            {
                sb.AppendLine("Kd " + m.color.r.ToString() + " " + m.color.g.ToString() + " " + m.color.b.ToString());
                if (m.color.a < 1.0f)
                {
                    //use both implementations of OBJ transparency
                    sb.AppendLine("Tr " + (1f - m.color.a).ToString());
                    sb.AppendLine("d " + m.color.a.ToString());
                }
            }
            if (m.HasProperty("_SpecColor"))
            {
                Color sc = m.GetColor("_SpecColor");
                sb.AppendLine("Ks " + sc.r.ToString() + " " + sc.g.ToString() + " " + sc.b.ToString());
            }
            if (exportTextures)
            {
                //diffuse
                string exResult = TryExportTexture("_MainTex", m);
                if (exResult != "false")
                {
                    sb.AppendLine("map_Kd " + exResult);
                }
                //spec map
                exResult = TryExportTexture("_SpecMap", m);
                if (exResult != "false")
                {
                    sb.AppendLine("map_Ks " + exResult);
                }
                //bump map
                exResult = TryExportTexture("_BumpMap", m);
                if (exResult != "false")
                {
                    sb.AppendLine("map_Bump " + exResult);
                }

            }
            sb.AppendLine("illum 2");
            return sb.ToString();
        }
    }
}
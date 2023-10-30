using UnityEngine;
using System.IO;
using Dummiesman;
using System.Collections.Generic;
//using TriLibCore.Dae.Schema;
//using TriLibCore.Interfaces;
using System.Runtime.CompilerServices;

public class MeshAnimationController : MonoBehaviour
{
    public string m_folderPath; // Path to the folder containing the meshes
    public float m_animationDuration = 0.01f; // Duration of each animation state in seconds
    private int m_current_id = 0;
    
    private List<string> m_filepaths = new List<string>();

    OBJLoader m_objloader = new OBJLoader();
    private GameObject m_currentMesh;
    private GameObject m_mesh;

    private void Start()
    {
        // Load meshes from the folder
        LoadMeshes();
    }

    private void Update()
    {
        if (m_filepaths.Count > 0 && m_current_id < m_filepaths.Count)
        {
            ReleaseMesh(m_objloader);
            
            //On CharWordReader, OBJLoader did not account for number such as 7E-05
            m_currentMesh = m_objloader.Load(m_filepaths[m_current_id]);
            Debug.Log("Showing mesh: " + m_filepaths[m_current_id].ToString());


            if (m_mesh != null)
            {

                m_mesh.SetActive(false);

                //For some reason OBJLoader leaves 2 Textures on the memory leading to crushes due to max memory
                //1 such texture is referenced on the m_mesh and can be easily deleted
                //The other is not referenced anywhere and it is hard to find. 
                Destroy(m_mesh);
                Resources.UnloadUnusedAssets();
                m_current_id++;
            }
            // Vector3 parent_position = transform.parent.position;
            // m_currentMesh.transform.position = parent_position + new Vector3(0f, 1.1f, 0f);
            m_currentMesh.transform.position = new Vector3(0f, 1.1f, 0f);
            m_currentMesh.transform.Rotate(270.0f, 0.0f, 0.0f);
            m_currentMesh.SetActive(true);
            m_mesh = m_currentMesh;
        }
    }

    private void LoadMeshes()
    {
        // Get all mesh files in the folder
        string[] files = Directory.GetFiles(m_folderPath, "*.obj");
        
        // Remove number from filename
        string fileName = Path.GetFileName(files[0]);
        string newFileName = ChangeFilename(fileName);

        
        // Load each mesh file
        for (int i = 0; i < files.Length; i++)
        {
            string filePath = Path.Combine(m_folderPath, newFileName.Insert(0, i.ToString()));
            m_filepaths.Add(filePath);
        }

    }

    private void ReleaseMesh(OBJLoader m_objloader){
        //OBJLoader does not clear the objloader object, and everything is added, so clear everything first
        m_objloader.Vertices.Clear();
        m_objloader.Normals.Clear();
        m_objloader.UVs.Clear();

        //Materials are only read if the Materials==null
        if (m_objloader.Materials != null)
        {
            m_objloader.Materials = null;
        }
        return;
    }

    private string ChangeFilename(string fileName)
    {
        // Find the index where the number ends
        int endIndex = 0;
        while (endIndex < fileName.Length && char.IsDigit(fileName[endIndex]))
        {
            endIndex++;
        }

        // Remove number
        if (endIndex > 0)
        {
            fileName = fileName.Remove(0, endIndex);
        }

        return fileName;
    }



}

using UnityEngine;
using System.Collections;




public class Graph : MonoBehaviour {

    public GameObject FunctionGraph;

    Mesh newMesh;
    public int Xmax = 20;
    public int Xmin = -20;
    public int Zmax = 20;
    public int Zmin = -20;
    public int resolutionScalar = 2;
    public float zScale = .1F;
    int Xwidth;
    int Zwidth;

    // Use this for initialization
    void Start() {
        //these are magic numbers. it's just a test
        newMesh = new Mesh();
        Xwidth = (Xmax - Xmin) * resolutionScalar + 1;
        Zwidth = (Zmax - Zmin) * resolutionScalar + 1;

        //generate data for new mesh, but does not update the mesh of the graph
        generateVertices();
        generateTriangles();
        generateUV();


        //sets the new mesh as the mesh of the Graph
        FunctionGraph.GetComponent<MeshFilter>().mesh = newMesh;
    }

    void generateVertices()
    {
        Vector3[] vertices;
        vertices = new Vector3[Xwidth * Zwidth];
        Debug.Log(vertices.Length);
        //generate the z-values for a grid of values and saves it as the vectors newMesh
        int i = 0;
        for (int xi = Xmin * resolutionScalar; xi <= Xmax * resolutionScalar; xi++)
        {
            for (int zi = Zmin * resolutionScalar; zi <= Zmax * resolutionScalar; zi++)
            {
                vertices[i].x = xi;// / resolutionScalar;
                vertices[i].y = 0;// function(xi / resolutionScalar, zi / resolutionScalar);
                vertices[i].z = zi;// / resolutionScalar;
                i++;
            }
        }
        
        for (int j = 0; j < vertices.Length; j++)
        {
            //don't ask me why I don't do this division in the first place. It just doesn't work when I do that, even though they should be equivalent
            vertices[j].x = vertices[j].x / resolutionScalar;
            vertices[j].z = vertices[j].z / resolutionScalar;
            vertices[j].y = function(vertices[j].x, vertices[j].z);
            //Debug.Log(vertices[j].y);
        }
        
        //adds vertices to mesh
        newMesh.vertices = vertices;
        Debug.Log(vertices.Length);

    }

    void generateTriangles()
    {
        //generate triangles for the mesh, then set them as the traingles of newMesh
        //the number of triangles is the width-1 times the length-1 times 2, then times 2 again so it is visible from both sides
        int trianglesCount = (Xwidth - 1) * (Zwidth - 1) * 2 * 2;
        //the size is the number of triangles times 3
        int[] triangles = new int[3 * trianglesCount];
        //
        for (int xi = 0; xi < (Xwidth - 1); xi++)
        {
            for (int zi = 0; zi < (Zwidth - 1); zi++)
            {
                //first triangle
                //base vertex
                triangles[12 * (zi + (Zwidth - 1) * xi) + 0] = xi * Zwidth + zi;
                //horizontal vertex (up)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 2] = (xi + 1) * Zwidth + zi;
                //diagonal vertex (up and to the right)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 1] = (xi + 1) * Zwidth + zi + 1;

                //second triangle
                //base vertex
                triangles[12 * (zi + (Zwidth - 1) * xi) + 3] = xi * Zwidth + zi;
                //vertical vertex (up)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 4] = xi * Zwidth + zi + 1;
                //diagonal vertex (up and to the Right)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 5] = (xi + 1) * Zwidth + zi + 1;

                //first triangle backside
                triangles[12 * (zi + (Zwidth - 1) * xi) + 6] = xi * Zwidth + zi;
                //horizontal vertex (up)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 7] = (xi + 1) * Zwidth + zi;
                //diagonal vertex (up and to the right)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 8] = (xi + 1) * Zwidth + zi + 1;

                //second triangle backside
                //base vertex
                triangles[12 * (zi + (Zwidth - 1) * xi) + 9] = xi * Zwidth + zi;
                //vertical vertex (up)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 11] = xi * Zwidth + zi + 1;
                //diagonal vertex (up and to the Right)
                triangles[12 * (zi + (Zwidth - 1) * xi) + 10] = (xi + 1) * Zwidth + zi + 1;
            }
        }
        //adds this triangle set to newMesh
        newMesh.triangles = triangles;
    }

    void generateUV()
    {
        //creates uv
        Vector2[] uvs = new Vector2[newMesh.vertices.Length];
        for (int j = 0; j < uvs.Length; j++)
        {
            uvs[j] = new Vector2(newMesh.vertices[j].x, newMesh.vertices[j].z);
        }

        //add uv to mesh
        newMesh.uv = uvs;
    }

    float function(float x, float z)
    {
        /*
        if (x==.5 & z==.5)
        {
            return (1);
        }
        else
        {
            return (0);
        }
        */
        //the function to be graphed
        //not yet sure how to have the user enter this
        return (x*x-z*z)*zScale;
    }

	// Update is called once per frame
	void Update ()
    {
        //The names of the direction vectors do not match the directions in the space. It works, but the code is just a little unintuitive
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            FunctionGraph.transform.Rotate(Vector3.down);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            FunctionGraph.transform.Rotate(Vector3.up);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            FunctionGraph.transform.Rotate(Vector3.right);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            FunctionGraph.transform.Rotate(Vector3.left);
        }
    }
}

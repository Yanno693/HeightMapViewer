using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Display : MonoBehaviour
{
    public Texture2D heigthMap;

    private HeightField heightField;

    private Color32[] lastColor;
    private bool rotate;
    private bool shading;
    private float height_value;

    private float wetness_epsilon;

    void Start()
    {   
        rotate = true;
        //rotate = false;
        shading = true;
        height_value = 25.0f;
        wetness_epsilon = 0.001f;
        Box2D box = new Box2D(new Vec2D(0.0f, 0.0f), new Vec2D(10.0f, 10.0f));
        Grid2D grid = new Grid2D(box, 250, 250); // grille de 250x250 vertices
        ScalarField scalarField = new ScalarField(grid);
        heightField = new HeightField(scalarField);

        loadMesh(); // Initialisation du mesh
    }

    private void updateColor() {
        GetComponent<MeshFilter>().mesh.colors32 = this.lastColor;
    }

    private void loadMesh() {
        heightField.initHeight(heigthMap); // Initialisation de la HF a partir d'une texture
        
        Vector3[] _vertices = new Vector3[heightField.nx * heightField.ny];
        Color32[] _colors = new Color32[heightField.nx * heightField.ny];
        int[] _triangles = new int[heightField.nx * heightField.ny * 6];

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                _vertices[heightField.Index(i, j)] = new Vector3(i - heightField.nx/2, Mathf.Pow(heightField[i, j], 1.0f) * height_value, j - heightField.ny/2);
                _colors[heightField.Index(i, j)] = new Color32(
                    (byte)Mathf.RoundToInt((float)heightField[i, j] * 255.0f),
                    (byte)Mathf.RoundToInt((float)heightField[i, j] * 255.0f),
                    (byte)Mathf.RoundToInt((float)heightField[i, j] * 255.0f),
                    255);

                if(i != heightField.nx - (int)1 && j != heightField.ny - (int)1) {
                    int _id = heightField.Index(i, j);
                    _triangles[_id * 6] = (int)heightField.Index(i, j);
                    _triangles[_id * 6 + 1] = (int)heightField.Index(i + 1, j + 1);
                    _triangles[_id * 6 + 2] = (int)heightField.Index(i + 1, j);
                    _triangles[_id * 6 + 3] = (int)heightField.Index(i, j);
                    _triangles[_id * 6 + 4] = (int)heightField.Index(i, j + 1);
                    _triangles[_id * 6 + 5] = (int)heightField.Index(i + 1, j + 1);
                }
            }
        }

        Mesh m = new Mesh();
        m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.vertices = _vertices;
        m.triangles = _triangles;
        m.colors32 = _colors;
        lastColor = _colors;
        m.RecalculateNormals(); // On recalcule les normales pour l'affichage
        GetComponent<MeshFilter>().mesh = m;
    }

    public void displayHeight() {
        lastColor = Compute.colorHeight(this.heightField);
        updateColor();
    }

    public void changeHeight(float value) {
        height_value = value;
        updateHeight();
    }

    public void changeWetness(float value) {
        wetness_epsilon = value;
    }

    public void doThermalErosion() {
        heightField.ThermalErosion(0.0001f);
        updateHeight();
    }

    public void doHydrolicErosion() {
        heightField.HydrolicErosion(0.1f);
        updateHeight();
    }

    public void doBothErosion() {
        heightField.HydrolicErosion(0.1f);
        heightField.ThermalErosion(0.0001f);
        updateHeight();
    }

    // Changement de la hauteur maximale
    public void updateHeight()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().mesh.vertices;

        for(int i = 0; i < heightField.nx; i++) {
            for(int j = 0; j < heightField.ny; j++) {
                vertices[heightField.Index(i, j)].y = Mathf.Pow(heightField[i, j], 1.0f) * height_value;
            }
        }

        GetComponent<MeshFilter>().mesh.vertices = vertices;
        GetComponent<MeshFilter>().mesh.RecalculateNormals();
    }

    public void displaySlope() {
        lastColor = Compute.colorSlope(this.heightField);
        updateColor();
    }

    public void displayGradiant() {
        lastColor = Compute.colorGradiant(this.heightField);
        updateColor();
    }

    public void displayLaplacian() {
        /*HeightField _h = new HeightField(this.heightField);

        float[,] filter = new float[3,3];

        for(int i = 0; i < filter.GetLength(0); i++)
            for(int j = 0; j < filter.GetLength(1); j++)
                filter[i,j] = 1;

        Texture2D tex = TextureFilter.Convolution(heigthMap, filter);
        
        _h.initHeight(tex);*/
        lastColor = Compute.colorLaplacian(this.heightField);
        updateColor();
    }

    public void displayAverageSlope() {
        lastColor = Compute.colorAverageSlope(this.heightField);
        updateColor();
    }

    public void displayDrainageArea() {
        lastColor = Compute.colorDrainageArea(this.heightField);
        updateColor();
    }

    public void displayStreamPower() {
        lastColor = Compute.colorStreamPower(this.heightField);
        updateColor();
    }

    public void displayWetnessIndex() {
        lastColor = Compute.colorWetnessIndex(this.heightField, this.wetness_epsilon);
        updateColor();
    }

    public void displayWetnessIndexFilter() {
        lastColor = Compute.colorWetnessIndex(this.heightField, this.wetness_epsilon);
        updateColor();
    }

    public void ExportToOBJ() {
        FileUtils.ExportMap(GetComponent<MeshFilter>().mesh, heightField.nx, heightField.ny);
    }

    public void ExportToPNG() {
        FileUtils.ExportPNG(GetComponent<MeshFilter>().mesh, heightField.nx, heightField.ny);
    }

    public void loadPNG() {
        heigthMap =  FileUtils.LoadPNG();
        loadMesh();
    }

    // On decice si on fait tourner le mesh ou pas
    public void changeRotate () {
        this.rotate = !this.rotate;
    }

    // Choix au materiaux Unlit ou Shading
    public void changeShading () {
        shading = !shading;

        Material m;

        if (shading)
            m = Resources.Load<Material>("PBRMaterial");
        else
            m = Resources.Load<Material>("UnlitMaterial");
        GetComponent<MeshRenderer>().material = m;

    }

    // Update is called once per frame
    void Update()
    {
        if(rotate)
            transform.Rotate(Vector3.up * Time.deltaTime * 10.0f);
    }
}

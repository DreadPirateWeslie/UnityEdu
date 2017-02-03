using UnityEngine;
using System.Collections;

public class heightShaderScript : MonoBehaviour {

    public enum interpModeEnum {none, linear};
    public enum divisionModeEnum {absolute, proportional, local};

    public divisionModeEnum divisionMode;
    public float[] layerHeights;
    public Color[] layerColors;
    public interpModeEnum[] layerInterpModes;
    public string fileName;
    private int numberOfLayers;
    private float objectHeight = 1;

    // Generate Shader
    void Start () {

        if (divisionMode == divisionModeEnum.proportional)
        {
            //find objectHeight
        }

        //add base

        //replace placeholders

        for (int i = 0; i < numberOfLayers; i++)
        {
            //add if statement for layer
            
            //replace instances of "<l>"
            
            //replace instances of other placeholders
        }

        //save shader
        //add shader to object
	}
	
	// do not use
	void Update () {
	
	}
}

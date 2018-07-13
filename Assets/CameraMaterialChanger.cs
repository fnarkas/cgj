using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMaterialChanger : MonoBehaviour {

    public Material material; // material you want the camera to change
    public bool Glitch;

    void OnPreRender() {
        if(Glitch)
            material.SetInt("_IsOn",1);
    }

    void OnPostRender() {
            material.SetInt("_IsOn",0);
    }

    private Color _default;
}
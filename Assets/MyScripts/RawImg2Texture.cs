using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RawImg2Texture : MonoBehaviour
{
    public RawImage Test;
    public Texture TheTexture;
    // Start is called before the first frame update
    void Start()
    {
        TheTexture = Test.texture as Texture;
    }

    // Update is called once per frame
    void Update()
    {
        TheTexture = Test.texture;
    }
}

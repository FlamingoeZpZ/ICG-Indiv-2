using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Bloom1 : MonoBehaviour
{
    // Start is called before the first frame update
    const int BoxDownPrefilterPass = 0;
    const int BoxDownPass = 1;
    const int BoxUpPass = 2;
    const int ApplyBloomPass = 3;
    const int DebugBloomPass = 4;


    public Shader bloomShader;
    public bool debug;
    
    [Range(1, 16)]
	public int iterations = 4;

    [Range(0, 10)]
    public float threshold = 1;

    [Range(0, 10)]
    public float intensity = 1;

    RenderTexture[] textures = new RenderTexture[16];
    
    [NonSerialized]
	Material bloom;
	
	void OnRenderImage (RenderTexture source, RenderTexture destination) {
		if (bloom == null) {
			bloom = new Material(bloomShader);
			bloom.hideFlags = HideFlags.HideAndDontSave;
		}

        bloom.SetFloat("_Threshold", threshold);
        bloom.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));

        int width = source.width / 2;
		int height = source.height / 2;
		
        RenderTextureFormat format = source.format;
        RenderTexture currentDestination = textures[0] =
		
        RenderTexture.GetTemporary(width, height, 0, format);
		Graphics.Blit(source, currentDestination, bloom, BoxDownPrefilterPass);
	    
        RenderTexture currentSource = currentDestination;
        int i = 1;
        for (; i < iterations; i++)
        {
            width /= 2;
            height /= 2;
            if (height < 2)
            {
                break;
            }
            currentDestination = textures[i] =
                RenderTexture.GetTemporary(width, height, 0, format);

            Graphics.Blit(currentSource, currentDestination, bloom, BoxDownPass);
            currentSource = currentDestination;
        }
        for (i -= 2; i >= 0; i--)
        {
            currentDestination = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSource, currentDestination, bloom, BoxUpPass);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        if (debug)
        {
            Graphics.Blit(currentSource, destination, bloom, DebugBloomPass);
        }
        else
        {
            //Send the final screen image to the shader code
            bloom.SetTexture("_SourceTex", currentSource);
            //Then blit it onto the screen after the final shader pass
            Graphics.Blit(source, destination, bloom, ApplyBloomPass);
        }
        RenderTexture.ReleaseTemporary(currentSource);
    }

}

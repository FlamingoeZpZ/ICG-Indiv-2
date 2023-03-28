using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, ImageEffectAllowedInSceneView]
public class CodeSnippet : MonoBehaviour
{
    [SerializeField] private int integerRange;
    [SerializeField]  private int iterations;
    
    //This code modifies the games resolution effectively make it pixelate the screen
    void OnRenderImage(RenderTexture source, RenderTexture
        destination){
        
        //Take the width and height of the screen and divide to get the new resolution
        int width = source.width / integerRange;
        int height = source.height / integerRange;
        
        //Format the screen image and prepare it for manipulation
        RenderTextureFormat format = source.format;
        RenderTexture[] textures = new RenderTexture[16];
        //This is pretty much creating a new "screen image" based on the new width / height defined earlier
        RenderTexture currentDestination = textures[0] = RenderTexture.GetTemporary(width, height, 0, format);
        Graphics.Blit(source, currentDestination);
        RenderTexture currentSource = currentDestination;
        Graphics.Blit(currentSource, destination);
        RenderTexture.ReleaseTemporary(currentSource);

        int i = 1;
        
        //Iterate through the rest of the screen images ( The code above creates the first one)

        for (; i < iterations; i++) {
            width /= 2;
            height /= 2;
            currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, format);
            if (height < 2) {
                break;
            }
            currentDestination = RenderTexture.GetTemporary(width, height, 0, format);
            Graphics.Blit(currentSource, currentDestination);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        
        //This for loop does literally nothing??? at this point i is already equal to iterations and this for loop will not run...
        for (; i < iterations; i++) {
            //But I guess what it was trying to do was upload the images back to the screen?
            Graphics.Blit(currentSource, currentDestination);
         //RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        //This for loop now goes back until I is zero.
        for (i -= 2; i >= 0; i--) {
            //Store the current image
            currentDestination = textures[i];
            textures[i] = null; //Empty it.. Honestly not necessary
            
            //Upload the image back onto the screen 
            Graphics.Blit(currentSource, currentDestination);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDestination;
        }
        //Upload the last image onto the screen?
        Graphics.Blit(currentSource, destination);
    }
}

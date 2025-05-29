using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

public class PhotoShotManager : MonoBehaviour
{
    public Camera photoCamera;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public string uploadUrl = "https://mixed-restaurant.bogay.me/api/upload";

    public IEnumerator TakePhotoAndUpload(string uploadUrl, string filename)
    {
        // Create render texture and capture image
        RenderTexture rt = new RenderTexture(resolutionWidth, resolutionHeight, 24);
        photoCamera.targetTexture = rt;
        Texture2D photo = new Texture2D(resolutionWidth, resolutionHeight, TextureFormat.RGB24, false);
        photoCamera.Render();
        RenderTexture.active = rt;
        photo.ReadPixels(new Rect(0, 0, resolutionWidth, resolutionHeight), 0, 0);
        photo.Apply();
        // Encode to PNG
        byte[] imageData = photo.EncodeToPNG();
        // Cleanup
        photoCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        // Send image data to API
        yield return StartCoroutine(UploadImage(imageData, uploadUrl, filename));
    }

    private IEnumerator UploadImage(byte[] imageData, string uploadUrl, string filename)
    {
        // Prepare form data
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", imageData, "photo.png", "image/png");
        form.AddField("filename", filename);
        UnityWebRequest www = UnityWebRequest.Post(uploadUrl, form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image uploaded successfully!");
            Debug.Log("Server response: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Image upload failed: " + www.error);
        }
    }
}

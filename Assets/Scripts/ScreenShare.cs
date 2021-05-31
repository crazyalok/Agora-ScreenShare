using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using agora_gaming_rtc;
using UnityEngine.UI;
using System.Globalization;
using System.Runtime.InteropServices;
using System;

public class ScreenShare : MonoBehaviour
{
    Texture2D mTexture;
    Rect mRect;
    [SerializeField]
    private string appId = "Your_AppID";
    [SerializeField]
    private string channelName = "agora";
    public IRtcEngine mRtcEngine;
    int i = 100;

    void Start()
    {
        mRtcEngine = IRtcEngine.GetEngine(appId);
        mRtcEngine.EnableVideo();
        mRtcEngine.EnableVideoObserver();
        mRtcEngine.SetExternalVideoSource(true, false);
        mRtcEngine.JoinChannel(channelName, null, 0);
        mRect = new Rect(0, 0, Screen.width, Screen.height);
        mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGBA32, false);
    }

    void Update()
    {
        StartCoroutine(shareScreen());
    }

    IEnumerator shareScreen()
    {
        yield return new WaitForEndOfFrame();
        mTexture.ReadPixels(mRect, 0, 0);
        mTexture.Apply();
        byte[] bytes = mTexture.GetRawTextureData();
        int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
        IRtcEngine rtc = IRtcEngine.QueryEngine();
        if (rtc != null)
        {
            // Creates a new external video frame.
            ExternalVideoFrame externalVideoFrame = new ExternalVideoFrame();
            // Sets the buffer type of the video frame.
            externalVideoFrame.type = ExternalVideoFrame.VIDEO_BUFFER_TYPE.VIDEO_BUFFER_RAW_DATA;
            // Sets the format of the video pixel.
            externalVideoFrame.format = ExternalVideoFrame.VIDEO_PIXEL_FORMAT.VIDEO_PIXEL_RGBA;
            // Applies the raw data.
            externalVideoFrame.buffer = bytes;
            // Sets the width (pixel) of the video frame.
            externalVideoFrame.stride = (int)mRect.width;
            // Sets the height (pixel) of the video frame.
            externalVideoFrame.height = (int)mRect.height;
            // Removes pixels from the sides of the frame
            externalVideoFrame.cropLeft = 10;
            externalVideoFrame.cropTop = 10;
            externalVideoFrame.cropRight = 10;
            externalVideoFrame.cropBottom = 10;
            // Rotates the video frame (0, 90, 180, or 270)
            externalVideoFrame.rotation = 180;
            // Increments i with the video timestamp.
            externalVideoFrame.timestamp = i++;
            // Pushes the external video frame with the frame you create.
            int a = rtc.PushVideoFrame(externalVideoFrame);
        }
    }
}
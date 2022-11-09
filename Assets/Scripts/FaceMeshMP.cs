using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mediapipe.Unity
{
    public class HolisticMP : MonoBehaviour
    {
        // Produce serialized fields to be configured in the inspector
        
        [SerializeField] private TextAsset _configAsset;
        [SerializeField] private RawImage _screen;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _fps;
        [SerializeField] private MultiFaceLandmarkListAnnotationController _multiFaceLandmarksAnnotationController;
        
        [SerializeField] private GameObject _model;
        [SerializeField] private GameObject _debug;

        private CalculatorGraph _graph;
        private ResourceManager _resourceManager;

        private WebCamTexture _webCamTexture;
        private Texture2D _inputTexture;
        private Color32[] _pixelData;



        // Start is called before the first frame update
        private IEnumerator Start()
        {
            // check if webcam is available
            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("No webcam found");
            }
            
            // select webcam and play new texture
            var webcamDevice = WebCamTexture.devices[0];
            _webCamTexture = new WebCamTexture(webcamDevice.name, _width, _height, _fps);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.width > 16);

            // resize screen
            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);

            // prepare input texture
            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _pixelData = new Color32[_width * _height];

            _resourceManager = new StreamingAssetsResourceManager();
            yield return _resourceManager.PrepareAssetAsync("face_detection_short_range.bytes");
            yield return _resourceManager.PrepareAssetAsync("face_landmark_with_attention.bytes");

            // begin stopwatch to send timestamped packets to MediaPipe graph
            var stopwatch = new Stopwatch();

            // create graph
            _graph = new CalculatorGraph(_configAsset.text);
            
            // start streams for output streams from the graph we want to use
            var outputVideoStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, "output_video");
            outputVideoStream.StartPolling().AssertOk();
            var faceLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "face_landmarks");
            faceLandmarksStream.StartPolling().AssertOk();
            var poseLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "pose_landmarks");
            poseLandmarksStream.StartPolling().AssertOk();
            
            // start graph
            _graph.StartRun().AssertOk();
            stopwatch.Start();

            var screenRect = _screen.GetComponent<RectTransform>().rect;

            // main graph output processing loop
            while (true)
            {
                
                // check for face landmarks
                if (faceLandmarksStream.TryGetNext(out var faceLandmarks))
                {
                    var landmark = faceLandmarks.Landmark[10];
                    //Debug.Log($"1: {landmark.ToString()}");
                }
                
                // check for pose landmarks
                if (poseLandmarksStream.TryGetNext(out var poseLandmarks))
                {
                    var nose = poseLandmarks.Landmark[0];
                    var left_eye_inner = poseLandmarks.Landmark[1];
                    var left_eye = poseLandmarks.Landmark[2];
                    var left_eye_outer = poseLandmarks.Landmark[3];
                    var right_eye_inner = poseLandmarks.Landmark[4];
                    var right_eye = poseLandmarks.Landmark[5];
                    var right_eye_outer = poseLandmarks.Landmark[6];
                    var left_ear = poseLandmarks.Landmark[7];
                    var right_ear = poseLandmarks.Landmark[8];
                    var mouth_left = poseLandmarks.Landmark[9];
                    var mouth_right = poseLandmarks.Landmark[10];
                    var left_shoulder = poseLandmarks.Landmark[11];
                    var right_shoulder = poseLandmarks.Landmark[12];
                    var left_elbow = poseLandmarks.Landmark[13];
                    var right_elbow = poseLandmarks.Landmark[14];
                    var left_wrist = poseLandmarks.Landmark[15];
                    var right_wrist = poseLandmarks.Landmark[16];
                    var left_pinky = poseLandmarks.Landmark[17];
                    var right_pinky = poseLandmarks.Landmark[18];
                    var left_index = poseLandmarks.Landmark[19];
                    var right_index = poseLandmarks.Landmark[20];
                    var left_thumb = poseLandmarks.Landmark[21];
                    var right_thumb = poseLandmarks.Landmark[22];
                    var left_hip = poseLandmarks.Landmark[23];
                    var right_hip = poseLandmarks.Landmark[24];
                    var left_knee = poseLandmarks.Landmark[25];
                    var right_knee = poseLandmarks.Landmark[26];
                    var left_ankle = poseLandmarks.Landmark[27];
                    var right_ankle = poseLandmarks.Landmark[28];
                    var left_heel = poseLandmarks.Landmark[29];
                    var right_heel = poseLandmarks.Landmark[30];
                    var left_foot_index = poseLandmarks.Landmark[31];
                    var right_foot_index = poseLandmarks.Landmark[32];
                    
                    var upDirection = Vector3.up;
                    var forwardDirection = Vector3.forward;
                    
                    // animate right upperarm
                    var rightUpperArm = _model.transform.Find("root")
                        .transform.Find("pelvis")
                        .transform.Find("spine_01")
                        .transform.Find("spine_02")
                        .transform.Find("spine_03")
                        .transform.Find("clavicle_r")
                        .transform.Find("upperarm_r");
                    rightUpperArm.rotation = Quaternion.LookRotation(new Vector3(right_shoulder.X - right_elbow.X, right_shoulder.Y - right_elbow.Y, right_shoulder.Z - right_elbow.Z), upDirection);
                    
                    // animate the debug
                    _debug.transform.Find("l_shoulder").transform.position = new Vector3(left_shoulder.X, left_shoulder.Y, left_shoulder.Z);
                    _debug.transform.Find("r_shoulder").transform.position = new Vector3(right_shoulder.X, right_shoulder.Y, right_shoulder.Z);
                    _debug.transform.Find("l_elbow").transform.position = new Vector3(left_elbow.X, left_elbow.Y, left_elbow.Z);
                    _debug.transform.Find("r_elbow").transform.position = new Vector3(right_elbow.X, right_elbow.Y, right_elbow.Z);
                    _debug.transform.Find("l_wrist").transform.position = new Vector3(left_wrist.X, left_wrist.Y, left_wrist.Z);
                    _debug.transform.Find("r_wrist").transform.position = new Vector3(right_wrist.X, right_wrist.Y, right_wrist.Z);
                    _debug.transform.Find("l_hip").transform.position = new Vector3(left_hip.X, left_hip.Y, left_hip.Z);
                    _debug.transform.Find("r_hip").transform.position = new Vector3(right_hip.X, right_hip.Y, right_hip.Z);
                    _debug.transform.Find("l_knee").transform.position = new Vector3(left_knee.X, left_knee.Y, left_knee.Z);
                    _debug.transform.Find("r_knee").transform.position = new Vector3(right_knee.X, right_knee.Y, right_knee.Z);
                    _debug.transform.Find("l_ankle").transform.position = new Vector3(left_ankle.X, left_ankle.Y, left_ankle.Z);
                    _debug.transform.Find("r_ankle").transform.position = new Vector3(right_ankle.X, right_ankle.Y, right_ankle.Z);
                    
                    
                }
                
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_pixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
                _graph.AddPacketToInputStream("input_video", new ImageFramePacket(imageFrame, new Timestamp(currentTimestamp))).AssertOk();

                yield return new WaitForEndOfFrame();

                
                var outputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
                var outputPixelData = new Color32[_width * _height];
                _screen.texture = outputTexture;

                // try to get next output video frame
                if (outputVideoStream.TryGetNext(out var outputVideo))
                {
                    if (outputVideo.TryReadPixelData(outputPixelData))
                    {
                        // apply output video frame to screen
                        outputTexture.SetPixels32(outputPixelData);
                        outputTexture.Apply();
                    }
                }
            
               
            }

        }

        private void OnDestroy()
        {
            // clean up
            if (_webCamTexture != null)
            {
                _webCamTexture.Stop();
            }

            if (_graph != null)
            {
                try
                {
                    _graph.CloseInputStream("input_video").AssertOk();
                    _graph.WaitUntilDone().AssertOk();
                }
                finally
                {
                    _graph.Dispose();
                }
            }
        }

    }
}
 
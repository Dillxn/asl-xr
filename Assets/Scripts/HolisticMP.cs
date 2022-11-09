using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mediapipe.Unity.Tutorial
{
    public class HolisticMP : MonoBehaviour
    {
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
            if (WebCamTexture.devices.Length == 0)
            {
                throw new System.Exception("No webcam found");
            }

            var webcamDevice = WebCamTexture.devices[0];
            _webCamTexture = new WebCamTexture(webcamDevice.name, _width, _height, _fps);
            _webCamTexture.Play();

            yield return new WaitUntil(() => _webCamTexture.width > 16);

            _screen.rectTransform.sizeDelta = new Vector2(_width, _height);

            _inputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
            _pixelData = new Color32[_width * _height];

            _resourceManager = new StreamingAssetsResourceManager();
            yield return _resourceManager.PrepareAssetAsync("face_detection_short_range.bytes");
            yield return _resourceManager.PrepareAssetAsync("face_landmark_with_attention.bytes");

            var stopwatch = new Stopwatch();

            _graph = new CalculatorGraph(_configAsset.text);
            var outputVideoStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, "output_video");
            outputVideoStream.StartPolling().AssertOk();
            var faceLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "face_landmarks");
            faceLandmarksStream.StartPolling().AssertOk();
            var poseLandmarksStream = new OutputStream<NormalizedLandmarkListPacket, NormalizedLandmarkList>(_graph, "pose_landmarks");
            poseLandmarksStream.StartPolling().AssertOk();
            
            _graph.StartRun().AssertOk();
            stopwatch.Start();

            var screenRect = _screen.GetComponent<RectTransform>().rect;

            while (true)
            {
                _inputTexture.SetPixels32(_webCamTexture.GetPixels32(_pixelData));
                var imageFrame = new ImageFrame(ImageFormat.Types.Format.Srgba, _width, _height, _width * 4, _inputTexture.GetRawTextureData<byte>());
                var currentTimestamp = stopwatch.ElapsedTicks / (System.TimeSpan.TicksPerMillisecond / 1000);
                _graph.AddPacketToInputStream("input_video", new ImageFramePacket(imageFrame, new Timestamp(currentTimestamp))).AssertOk();

                yield return new WaitForEndOfFrame();
                
                // new
                if (faceLandmarksStream.TryGetNext(out var faceLandmarks))
                {
                  
                }
                
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
                    
                    // flip the y axis
                    nose.Y = -nose.Y;   
                    left_eye_inner.Y = -left_eye_inner.Y;
                    left_eye.Y = -left_eye.Y;
                    left_eye_outer.Y = -left_eye_outer.Y;
                    right_eye_inner.Y = -right_eye_inner.Y;
                    right_eye.Y = -right_eye.Y;
                    right_eye_outer.Y = -right_eye_outer.Y;
                    left_ear.Y = -left_ear.Y;
                    right_ear.Y = -right_ear.Y;
                    mouth_left.Y = -mouth_left.Y;
                    mouth_right.Y = -mouth_right.Y;
                    left_shoulder.Y = -left_shoulder.Y;
                    right_shoulder.Y = -right_shoulder.Y;
                    left_elbow.Y = -left_elbow.Y;
                    right_elbow.Y = -right_elbow.Y;
                    left_wrist.Y = -left_wrist.Y;
                    right_wrist.Y = -right_wrist.Y;
                    left_pinky.Y = -left_pinky.Y;
                    right_pinky.Y = -right_pinky.Y;
                    left_index.Y = -left_index.Y;
                    right_index.Y = -right_index.Y;
                    left_thumb.Y = -left_thumb.Y;
                    right_thumb.Y = -right_thumb.Y;    
                    left_hip.Y = -left_hip.Y;
                    right_hip.Y = -right_hip.Y;
                    left_knee.Y = -left_knee.Y;
                    right_knee.Y = -right_knee.Y;
                    left_ankle.Y = -left_ankle.Y;
                    right_ankle.Y = -right_ankle.Y;
                    left_heel.Y = -left_heel.Y;
                    right_heel.Y = -right_heel.Y;
                    left_foot_index.Y = -left_foot_index.Y;
                    right_foot_index.Y = -right_foot_index.Y;
                    
                        
                    
                    // animate right upperarm
                    var rightUpperArm = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("clavicle_r").transform.Find("upperarm_r");
                    rightUpperArm.rotation = Quaternion.LookRotation(new Vector3(right_shoulder.X - right_elbow.X, right_shoulder.Y - right_elbow.Y, right_shoulder.Z - right_elbow.Z), upDirection);
                    // position right upperarm
                    rightUpperArm.position = new Vector3(right_shoulder.X, right_shoulder.Y, right_shoulder.Z);
                    
                    // animate right lowerarm
                    var rightLowerArm = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("clavicle_r").transform.Find("upperarm_r").transform.Find("lowerarm_r");
                    rightLowerArm.rotation = Quaternion.LookRotation(new Vector3(right_elbow.X - right_wrist.X, right_elbow.Y - right_wrist.Y, right_elbow.Z - right_wrist.Z), upDirection);
                    // position right lowerarm
                    rightLowerArm.position = new Vector3(right_elbow.X, right_elbow.Y, right_elbow.Z);
                    
                    // animate right hand
                    var rightHand = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("clavicle_r").transform.Find("upperarm_r").transform.Find("lowerarm_r").transform.Find("hand_r");
                    rightHand.rotation = Quaternion.LookRotation(new Vector3(right_wrist.X - right_thumb.X, right_wrist.Y - right_thumb.Y, right_wrist.Z - right_thumb.Z), upDirection);
                    // position right hand
                    rightHand.position = new Vector3(right_wrist.X, right_wrist.Y, right_wrist.Z);
                    
                    // animate left upperarm
                    var leftUpperArm = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("clavicle_l").transform.Find("upperarm_l");
                    leftUpperArm.rotation = Quaternion.LookRotation(new Vector3(left_shoulder.X - left_elbow.X, left_shoulder.Y - left_elbow.Y, left_shoulder.Z - left_elbow.Z), upDirection);
                    // position left upperarm
                    leftUpperArm.position = new Vector3(left_shoulder.X, left_shoulder.Y, left_shoulder.Z);
                    
                    // animate left lowerarm
                    var leftLowerArm = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("clavicle_l").transform.Find("upperarm_l").transform.Find("lowerarm_l");
                    leftLowerArm.rotation = Quaternion.LookRotation(new Vector3(left_elbow.X - left_wrist.X, left_elbow.Y - left_wrist.Y, left_elbow.Z - left_wrist.Z), upDirection);
                    // position left lowerarm
                    leftLowerArm.position = new Vector3(left_elbow.X, left_elbow.Y, left_elbow.Z);
                    
                    // animate left hand
                    var leftHand = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("clavicle_l").transform.Find("upperarm_l").transform.Find("lowerarm_l").transform.Find("hand_l");
                    leftHand.rotation = Quaternion.LookRotation(new Vector3(left_wrist.X - left_thumb.X, left_wrist.Y - left_thumb.Y, left_wrist.Z - left_thumb.Z), upDirection);
                    // position left hand
                    leftHand.position = new Vector3(left_wrist.X, left_wrist.Y, left_wrist.Z);
                    
                    // animate right thigh
                    var rightThigh = _model.transform.Find("root").transform.Find("pelvis").transform.Find("thigh_r");
                    rightThigh.rotation = Quaternion.LookRotation(new Vector3(right_hip.X - right_knee.X, right_hip.Y - right_knee.Y, right_hip.Z - right_knee.Z), upDirection);
                    // position right thigh
                    rightThigh.position = new Vector3(right_hip.X, right_hip.Y, right_hip.Z);
                    
                    // animate right shin
                    var rightShin = _model.transform.Find("root").transform.Find("pelvis").transform.Find("thigh_r").transform.Find("calf_r");
                    rightShin.rotation = Quaternion.LookRotation(new Vector3(right_knee.X - right_ankle.X, right_knee.Y - right_ankle.Y, right_knee.Z - right_ankle.Z), upDirection);
                    // position right shin
                    rightShin.position = new Vector3(right_knee.X, right_knee.Y, right_knee.Z);
                    
                    // animate right foot
                    var rightFoot = _model.transform.Find("root").transform.Find("pelvis").transform.Find("thigh_r").transform.Find("calf_r").transform.Find("foot_r");
                    rightFoot.rotation = Quaternion.LookRotation(new Vector3(right_ankle.X - right_heel.X, right_ankle.Y - right_heel.Y, right_ankle.Z - right_heel.Z), upDirection);
                    // position right foot
                    rightFoot.position = new Vector3(right_ankle.X, right_ankle.Y, right_ankle.Z);
                    
                    // animate left thigh
                    var leftThigh = _model.transform.Find("root").transform.Find("pelvis").transform.Find("thigh_l");
                    leftThigh.rotation = Quaternion.LookRotation(new Vector3(left_hip.X - left_knee.X, left_hip.Y - left_knee.Y, left_hip.Z - left_knee.Z), upDirection);
                    // position left thigh
                    leftThigh.position = new Vector3(left_hip.X, left_hip.Y, left_hip.Z);
                    
                    // animate left shin
                    var leftShin = _model.transform.Find("root").transform.Find("pelvis").transform.Find("thigh_l").transform.Find("calf_l");
                    leftShin.rotation = Quaternion.LookRotation(new Vector3(left_knee.X - left_ankle.X, left_knee.Y - left_ankle.Y, left_knee.Z - left_ankle.Z), upDirection);
                    // position left shin
                    leftShin.position = new Vector3(left_knee.X, left_knee.Y, left_knee.Z);
                    
                    // animate left foot
                    var leftFoot = _model.transform.Find("root").transform.Find("pelvis").transform.Find("thigh_l").transform.Find("calf_l").transform.Find("foot_l");
                    leftFoot.rotation = Quaternion.LookRotation(new Vector3(left_ankle.X - left_heel.X, left_ankle.Y - left_heel.Y, left_ankle.Z - left_heel.Z), upDirection);
                    // position left foot
                    leftFoot.position = new Vector3(left_ankle.X, left_ankle.Y, left_ankle.Z);
                    
                    // animate head
                    var head = _model.transform.Find("root").transform.Find("pelvis").transform.Find("spine_01").transform.Find("spine_02").transform.Find("spine_03").transform.Find("neck_01").transform.Find("head");
                    // determine neck landmark
                    var neck = new Vector3((left_shoulder.X + right_shoulder.X) / 2, (left_shoulder.Y + right_shoulder.Y) / 2, (left_shoulder.Z + right_shoulder.Z) / 2);
                    head.rotation = Quaternion.LookRotation(new Vector3(neck.x - head.position.x, neck.y - head.position.y, neck.z - head.position.z), upDirection);
                    
                    
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

                
                var outputTexture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
                var outputPixelData = new Color32[_width * _height];
                _screen.texture = outputTexture;

                if (outputVideoStream.TryGetNext(out var outputVideo))
                {
                    if (outputVideo.TryReadPixelData(outputPixelData))
                    {
                        outputTexture.SetPixels32(outputPixelData);
                        outputTexture.Apply();
                    }
                }
            
               
            }

        }

        private void OnDestroy()
        {
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
 
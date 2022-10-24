using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mediapipe.Unity.Tutorial
{
    public class FaceMeshMP : MonoBehaviour
    {
        [SerializeField] private TextAsset _configAsset;
        [SerializeField] private RawImage _screen;
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _fps;
        [SerializeField] private MultiFaceLandmarkListAnnotationController _multiFaceLandmarksAnnotationController;

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
            var outputVideoStream = new OutputStream<ImageFramePacket, ImageFrame>(_graph, "transformed_input_video");
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
                    var landmark = faceLandmarks.Landmark[10];
                    Debug.Log($"1: {landmark.ToString()}");
                }
                
                if (poseLandmarksStream.TryGetNext(out var poseLandmarks))
                {
                    var landmark = poseLandmarks.Landmark[0];
                    Debug.Log($"2: {landmark.ToString()}");
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

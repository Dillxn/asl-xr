using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mediapipe.Unity.Tutorial { 

    public class HelloWorldMediaPipe : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var configText = @"
                input_stream: ""in""
                output_stream: ""out""
                node {
                    calculator: ""PassThroughCalculator""
                    input_stream: ""in""
                    output_stream: ""out1""
                }
                node {
                    calculator: ""PassThroughCalculator""
                    input_stream: ""out1""
                    output_stream: ""out""
                }
            ";
            var graph = new CalculatorGraph(configText);

            graph.StartRun().AssertOk();

            for (var i = 0; i < 10; i++)
            {
                var input = new StringPacket("Hello World!");
                graph.AddPacketToInputStream("in", input).AssertOk();
            }

            graph.CloseInputStream("in").AssertOk();
            graph.WaitUntilDone().AssertOk();
            graph.Dispose();

            Debug.Log("Done");
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }

}
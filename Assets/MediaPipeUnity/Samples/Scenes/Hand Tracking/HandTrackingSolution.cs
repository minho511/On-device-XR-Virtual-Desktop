// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Barracuda;

namespace Mediapipe.Unity.HandTracking
{
  public class HandTrackingSolution : ImageSourceSolution<HandTrackingGraph>
  {
    [SerializeField] private DetectionListAnnotationController _palmDetectionsAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromPalmDetectionsAnnotationController;
    [SerializeField] private MultiHandLandmarkListAnnotationController _handLandmarksAnnotationController;
    [SerializeField] private NormalizedRectListAnnotationController _handRectsFromLandmarksAnnotationController;
    [SerializeField] private TextMeshProUGUI tmp2;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private HandManager handmanager;
    public NNModel modelSource;
    public Model model;
    //[SerializeField] private GameObject pp;
    public HandTrackingGraph.ModelComplexity modelComplexity
    {
      get => graphRunner.modelComplexity;
      set => graphRunner.modelComplexity = value;
    }

    public int maxNumHands
    {
      get => graphRunner.maxNumHands;
      set => graphRunner.maxNumHands = value;
    }

    public float minDetectionConfidence
    {
      get => graphRunner.minDetectionConfidence;
      set => graphRunner.minDetectionConfidence = value;
    }

    public float minTrackingConfidence
    {
      get => graphRunner.minTrackingConfidence;
      set => graphRunner.minTrackingConfidence = value;
    }

    protected override void OnStartRun()
    {
      model = ModelLoader.Load(modelSource);
      Debug.Log("Model Load complete!");
      if (!runningMode.IsSynchronous())
      {
        graphRunner.OnPalmDetectectionsOutput += OnPalmDetectionsOutput;
        graphRunner.OnHandRectsFromPalmDetectionsOutput += OnHandRectsFromPalmDetectionsOutput;
        graphRunner.OnHandLandmarksOutput += OnHandLandmarksOutput;
        // TODO: render HandWorldLandmarks annotations
        graphRunner.OnHandRectsFromLandmarksOutput += OnHandRectsFromLandmarksOutput;
        graphRunner.OnHandednessOutput += OnHandednessOutput;
      }

      var imageSource = ImageSourceProvider.ImageSource;
      
      SetupAnnotationController(_palmDetectionsAnnotationController, imageSource, true);
      SetupAnnotationController(_handRectsFromPalmDetectionsAnnotationController, imageSource, true);
      SetupAnnotationController(_handLandmarksAnnotationController, imageSource, true);
      SetupAnnotationController(_handRectsFromLandmarksAnnotationController, imageSource, true);
    }

    protected override void AddTextureFrameToInputStream(TextureFrame textureFrame)
    {
      graphRunner.AddTextureFrameToInputStream(textureFrame);
    }

    protected override IEnumerator WaitForNextValue()
    {
      
      List<Detection> palmDetections = null;
      List<NormalizedRect> handRectsFromPalmDetections = null;
      List<NormalizedLandmarkList> handLandmarks = null;
      List<LandmarkList> handWorldLandmarks = null;
      List<NormalizedRect> handRectsFromLandmarks = null;
      List<ClassificationList> handedness = null;

      if (runningMode == RunningMode.Sync)
      {
        var _ = graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness, true);
      }
      else if (runningMode == RunningMode.NonBlockingSync)
      {
        yield return new WaitUntil(() => graphRunner.TryGetNext(out palmDetections, out handRectsFromPalmDetections, out handLandmarks, out handWorldLandmarks, out handRectsFromLandmarks, out handedness, false));
      }
      
      _palmDetectionsAnnotationController.DrawNow(palmDetections);
      _handRectsFromPalmDetectionsAnnotationController.DrawNow(handRectsFromPalmDetections);
      if (handLandmarks != null)
      {
        
        var hand = handLandmarks[0].Landmark;
        tmp.text = hand.ToString();
        var inputTensor = new Tensor(1, 1, 21, 3);
        for (int i = 0; i<21; i++)
        {
          inputTensor[0, 0, i, 0] = (float)hand[i].X;
          inputTensor[0, 0, i, 1] = (float)hand[i].Y;
          inputTensor[0, 0, i, 2] = (float)hand[i].Z;
        }
        handmanager.handpos = new Vector3((float)hand[8].X, (float)hand[8].Y, (float)hand[8].Z);
        handmanager.finger4 = new Vector3((float)hand[4].X, (float)hand[4].Y, (float)hand[4].Z);
        handmanager.finger6 = new Vector3((float)hand[6].X, (float)hand[6].Y, (float)hand[6].Z);
        var worker = WorkerFactory.CreateWorker(model, WorkerFactory.Device.CPU);
        _ = worker.Execute(inputTensor);
        var output = worker.PeekOutput("Y");

        int maxIndex = 0;
        float maxvalue = output[0];
        
        for(int i = 0; i<5; i++)
        {
          if (output[i] > maxvalue)
          {
            maxvalue = output[i];
            maxIndex = i;
          }
        }
        //if (maxvalue < 0.5) maxIndex = -1;
        if (maxIndex == 0) tmp2.text = "Gesture: " + "Expand Size" + maxvalue.ToString();
        else if (maxIndex == 1) tmp2.text = "Gesture: " + "Reduce Size" + maxvalue.ToString();
        else if (maxIndex == 2) tmp2.text = "Gesture: " + "Grab" + maxvalue.ToString();
        else if (maxIndex == 3) tmp2.text = "Gesture: " + "Default" + maxvalue.ToString();
        else tmp2.text = "Gesture: " + "Pointing";
        handmanager.gesture_class = maxIndex;
        print(maxIndex);
        inputTensor.Dispose();
        output.Dispose();
        worker.Dispose();

      }
      else
      {
        tmp.text = "Hands not detected";
        tmp2.text = "Hands not detected";
        handmanager.gesture_class = -1;

      }
      _handLandmarksAnnotationController.DrawNow(handLandmarks, handedness);
      // TODO: render HandWorldLandmarks annotations
      _handRectsFromLandmarksAnnotationController.DrawNow(handRectsFromLandmarks);
    }

    private void OnPalmDetectionsOutput(object stream, OutputEventArgs<List<Detection>> eventArgs)
    {
      _palmDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandRectsFromPalmDetectionsOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      _handRectsFromPalmDetectionsAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandLandmarksOutput(object stream, OutputEventArgs<List<NormalizedLandmarkList>> eventArgs)
    {
      _handLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandRectsFromLandmarksOutput(object stream, OutputEventArgs<List<NormalizedRect>> eventArgs)
    {
      _handRectsFromLandmarksAnnotationController.DrawLater(eventArgs.value);
    }

    private void OnHandednessOutput(object stream, OutputEventArgs<List<ClassificationList>> eventArgs)
    {
      
      _handLandmarksAnnotationController.DrawLater(eventArgs.value);
    }
  }
}

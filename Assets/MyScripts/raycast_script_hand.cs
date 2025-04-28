using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;
using System;
public class raycast_script_hand : MonoBehaviour
{
  // Game Object
  public GameObject monitor_prefab;
  public GameObject clock_prefab;
  GameObject spawned_monitor;
  GameObject spawned_clock;
  bool monitor_spawned;
  bool clock_spawned;
  Vector3 monitor_position;
  Vector3 clock_position;
  Vector3 s_monitor_position;
  Vector3 s_clock_position;
  Vector3 starting_position;
  bool grab_ing_monitor;
  bool grab_ing_clock;

  // Log
  public TextMeshProUGUI tmp;
  public TextMeshProUGUI tmp2;
  String Logger;
  //AR
  public ARRaycastManager arraymanager;
  public ARPlaneManager arplanemanager;
  
  List<ARRaycastHit> hits = new List<ARRaycastHit>();
  bool plane_visible;

  // Interaction
  public HandManager handmanager;
  public float r_moving = 0.02f; // grab moving ratio
  public float scale_factor_size = 0.01f;// 버튼이 눌리는 동안 스케일링 할 배수 up -> 1.2 down -> 0.8
  public float finger_4_6_distance;
  public Camera camera;
  public marker marker_sol;
  bool check_click = false;
  float hand_clock_distance;
  float hand_monitor_distance;
  int waiting_click = 5;
  int check_time = 0;
  int hand_gesture;
  Vector3 hand_position;
  Vector3 scaled_hand_position;

  float DistanceAB(Vector3 A, Vector3 B)
  {
    return (float)Math.Sqrt((double)(pow(Math.Abs(A[0] - B[0])) + pow(Math.Abs(A[1] - B[1]))));
  }

  void Start()
  {
    arraymanager = GetComponent<ARRaycastManager>();
    monitor_spawned = false;
    clock_spawned = false;
    plane_visible = true;
    monitor_position = new Vector3(0f, 0f, 0f);
    clock_position = new Vector3(0f, 0f, 0f);
    s_monitor_position = new Vector3(0f, 0f, 0f);
    s_clock_position = new Vector3(0f, 0f, 0f);
  }

  void ScaleUp(GameObject obj)
  {
    Vector3 tem = obj.transform.localScale;
    tem.x = tem.x * (1f + scale_factor_size);
    tem.y = tem.y * (1f + scale_factor_size);
    obj.transform.localScale = tem;
  }

  void ScaleDown(GameObject obj)
  {
    Vector3 tem = obj.transform.localScale;
    tem.x = tem.x * (1 - scale_factor_size);
    tem.y = tem.y * (1 - scale_factor_size);
    obj.transform.localScale = tem;
  }

  void LookAtMe(GameObject obj)
  {
    obj.transform.LookAt(camera.transform);
  }

  // Update is called once per frame
  void Update()
  {
    Logger = "Distance :";
    hand_position = handmanager.handpos;
    hand_gesture = handmanager.gesture_class;
    //scaled_hand_position = new Vector3((1f - hand_position[0]) * 1024f, (1f - hand_position[1]) * 1024f, hand_position[2]);
    scaled_hand_position = new Vector3((1f - hand_position[0]) * 1536f, (1f - hand_position[1]) * 1430f, hand_position[2]);


    if (monitor_spawned)
    {
      LookAtMe(spawned_monitor);
      var rot = spawned_monitor.transform.eulerAngles;
      rot.y += 180f;
      spawned_monitor.transform.eulerAngles = rot;
      monitor_position = spawned_monitor.transform.position;
      s_monitor_position = camera.WorldToScreenPoint(monitor_position);
      //hand_monitor_distance = (float)Math.Sqrt((double)(pow(Math.Abs(scaled_hand_position[0] - s_monitor_position[0])) +
      //  pow(Math.Abs(scaled_hand_position[1] - s_monitor_position[1]))));
      hand_monitor_distance = DistanceAB(scaled_hand_position, s_monitor_position);
      Logger += "\nfinger8 - monitor : " + hand_monitor_distance.ToString();
    }
    if (clock_spawned)
    {
      LookAtMe(spawned_clock);
      clock_position = spawned_clock.transform.position;
      s_clock_position = camera.WorldToScreenPoint(clock_position);
      //hand_clock_distance = (float)Math.Sqrt((double)(pow(Math.Abs(scaled_hand_position[0] - s_clock_position[0])) + pow(Math.Abs(scaled_hand_position[1] - s_clock_position[1]))));
      hand_clock_distance = DistanceAB(scaled_hand_position, s_clock_position);
      Logger += "\nfinger8 - clock : " + hand_clock_distance.ToString();
    }
    if ((hand_gesture == 4) && !monitor_spawned && clock_spawned)
    {
      var finger_4_position = marker_sol.scaling_hand(handmanager.finger4);
      var finger_6_position = marker_sol.scaling_hand(handmanager.finger6);
      //finger_4_6_distance = (float)Math.Sqrt((double)(pow(Math.Abs(finger_4_position[0] - finger_6_position[0])) +
      //  pow(Math.Abs(finger_4_position[1] - finger_6_position[1]))));
      finger_4_6_distance = DistanceAB(finger_4_position, finger_6_position);
      

      if (finger_4_6_distance > 250)
      {
        if (check_click && check_time > waiting_click)
        {
          //tmp.text = "CLICK !  " + finger_4_6_distance.ToString();
          check_click = false;
          check_time = 0;
          if (!monitor_spawned)
          {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(scaled_hand_position[0], scaled_hand_position[1]));
            if (arraymanager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
            {
              Pose hitPose = hits[0].pose;
              spawned_monitor = Instantiate(monitor_prefab, hitPose.position, monitor_prefab.transform.localRotation);
              //monitor_setAct = true;
              monitor_spawned = true;
              monitor_position = spawned_monitor.transform.position;

              //hide trackable plane
              if (plane_visible){

                foreach(var plane in arplanemanager.trackables)
                {
                  plane.gameObject.SetActive(false);
                }
                plane_visible = false;
                arplanemanager.enabled = false;
              }
            }
          }
        }
        else if(check_click && (check_time < waiting_click))
        {
          check_time = 0;
          check_click = false;
        }
      }else if(finger_4_6_distance < 150)
      {
        if (!check_click)
        {
          check_click = true;
        }
        else
        {
          //tmp.text = "wait ! "+ check_time.ToString() +"\ndistance : " + finger_4_6_distance.ToString();
          check_time++;
        }

      }
    }

    // monitor logic
    if (monitor_spawned && clock_spawned && (hand_clock_distance > hand_monitor_distance)) {
      
      if (hand_monitor_distance < 800)
      {
        if (hand_gesture == -1)
        {
          //tmp.text = "Hand X";
        }
        else
        {
          if (hand_gesture == 2)
          {
            //tmp.text = "monitor grab " + hand_monitor_distance.ToString();
            if (grab_ing_monitor)
            {
              var temp = spawned_monitor.transform.position;
              var X = temp.x;
              var Y = temp.y;
              var Z = temp.z;
              spawned_monitor.transform.position = new Vector3(X - r_moving * (starting_position.x - scaled_hand_position.x) / 1024f,
                                                               Y - r_moving * (starting_position.y - scaled_hand_position.y) / 1024f, Z);
            }
            else
            {
              grab_ing_monitor = true;
              starting_position = scaled_hand_position;
            }
          }
          else if (hand_gesture == 0) // size up
          {
            ScaleUp(spawned_monitor);
            //tmp.text = "monitor size up " + hand_monitor_distance.ToString();
          }
          else if (hand_gesture == 1)
          {
            ScaleDown(spawned_monitor);
            //tmp.text = "monitor size down " + hand_monitor_distance.ToString();
          }
          else
          {
            grab_ing_monitor = false;
            //tmp.text = "monitor " + hand_monitor_distance.ToString();
          }
        }
      }
      else
      {
        //tmp.text = "hand - monitor distance : " + hand_monitor_distance.ToString() + "\n";
      }
    }
    //clock logic
    if ((hand_gesture == 4) && !monitor_spawned && !clock_spawned)
    {
      var finger_4_position = marker_sol.scaling_hand(handmanager.finger4);
      var finger_6_position = marker_sol.scaling_hand(handmanager.finger6);
      //finger_4_6_distance = (float)Math.Sqrt((double)(pow(Math.Abs(finger_4_position[0] - finger_6_position[0])) + pow(Math.Abs(finger_4_position[1] - finger_6_position[1]))));
      finger_4_6_distance = DistanceAB(finger_4_position, finger_6_position);
      //tmp.text = "before making clock : " + finger_4_6_distance.ToString();
      if (finger_4_6_distance > 250)
      {
        if (check_click && check_time > waiting_click)
        {
          //tmp.text = "CLICK !  " + finger_4_6_distance.ToString();
          check_click = false;
          check_time = 0;
          if (!clock_spawned)
          {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(scaled_hand_position[0], scaled_hand_position[1]));
            if (arraymanager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon))
            {
              Pose hitPose = hits[0].pose;
              spawned_clock = Instantiate(clock_prefab, hitPose.position, clock_prefab.transform.localRotation);
              //clock_setAct = true;
              clock_spawned = true;
              clock_position = spawned_clock.transform.position;
            }
          }
        }
        else if (check_click && (check_time < waiting_click))
        {
          check_time = 0;
          check_click = false;
        }
      }
      else if (finger_4_6_distance < 150)
      {
        if (!check_click)
        {
          check_click = true;
        }
        else
        {
          //tmp.text = "Click " + check_time.ToString() + " sec \n 4-6 finger distance : " + finger_4_6_distance.ToString();
          check_time++;
        }

      }
    }
    // Clock hand interaction
    if (monitor_spawned && clock_spawned && (hand_clock_distance < hand_monitor_distance))
    {

      if (hand_clock_distance < 800)
      {
        if (hand_gesture == -1) { }
        else
        {
          if (hand_gesture == 2)
          {
            tmp.text = "clock grab " + hand_clock_distance.ToString();
            if (grab_ing_clock)
            {
              var temp = spawned_clock.transform.position;
              var X = temp.x;
              var Y = temp.y;
              var Z = temp.z;
              spawned_clock.transform.position = new Vector3(X - r_moving * (starting_position.x - scaled_hand_position.x) / 1024f,
                                                               Y - r_moving * (starting_position.y - scaled_hand_position.y) / 1024f, Z);
            }
            else
            {
              grab_ing_clock = true;
              starting_position = scaled_hand_position;
            }
          }
          else if (hand_gesture == 0)
          {
            ScaleUp(spawned_clock);
            //tmp.text = "clock size up " + hand_clock_distance.ToString();
          }
          else if (hand_gesture == 1)
          {
            ScaleDown(spawned_clock);
            //tmp.text = "clock size down " + hand_clock_distance.ToString();
          }
          else
          {
            grab_ing_clock = false;
            //tmp.text = "clock " + hand_clock_distance.ToString();
          }
        }
      }
      else
      {
        //tmp.text += "\n hand - clock distance : " + hand_clock_distance.ToString() + "\n";
      }
    }
    Logger += "\nfinger4 - finger6 : " + finger_4_6_distance.ToString();
    if (Logger == "")
    {
      tmp.text = "No hands detected.";
    }
    else { tmp.text = Logger; }
    
  }
  float pow(float a)
  {
    return a * a;
  }
}
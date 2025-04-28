using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class marker : MonoBehaviour
{
    public GameObject marker_finger8;
    public GameObject marker_finger4;
    public GameObject marker_finger6;
    public GameObject marker_finger8R;
    public GameObject marker_finger4R;
    public GameObject marker_finger6R;
    public HandManager handmanager;
    public Vector3 scaled_hand_position; //8
    public Vector3 hand_position;//8
    public Vector3 finger_6;
    public Vector3 finger_6_pos;
    public Vector3 finger_4;
    public Vector3 finger_4_pos;
    public Vector3 finger_8R;
    public Vector3 finger_8R_pos;
    public Vector3 finger_4R;
    public Vector3 finger_4R_pos;
    public Vector3 finger_6R;
    public Vector3 finger_6R_pos;
    public Vector3 scaling_hand(Vector3 position)
    {
      return new Vector3((1f - position[0]) * 1536f, (1f - position[1]) * 1430f, 800f);
    }

    public Vector3 scaling_handR(Vector3 position)
    {
      return new Vector3(1536f+(1f - position[0]) * 1536f, (1f - position[1]) * 1430f, 800f);
    }
    // Start is called before the first frame update
    void Start()
    {
      scaled_hand_position = new Vector3(0f, 0f, 0f);
      finger_4_pos = new Vector3(0f, 0f, 0f);
      finger_6_pos = new Vector3(0f, 0f, 0f);
    } 

    // Update is called once per frame
    void Update()
    {
      if (handmanager.gesture_class == -1)
      {
        marker_finger4.SetActive(false);
        marker_finger8.SetActive(false);
        marker_finger6.SetActive(false);
        marker_finger4R.SetActive(false);
        marker_finger8R.SetActive(false);
        marker_finger6R.SetActive(false);
      }
      else {
      marker_finger4.SetActive(true);
      marker_finger8.SetActive(true);
      marker_finger6.SetActive(true);
      marker_finger4R.SetActive(true);
      marker_finger8R.SetActive(true);
      marker_finger6R.SetActive(true);
      hand_position = handmanager.handpos;
      scaled_hand_position = scaling_hand(hand_position);
      marker_finger8.transform.position = scaled_hand_position;
      
      finger_4 = handmanager.finger4;
      finger_4_pos = scaling_hand(finger_4);
      marker_finger4.transform.position = finger_4_pos;
      finger_6 = handmanager.finger6;
      finger_6_pos = scaling_hand(finger_6);
      marker_finger6.transform.position = finger_6_pos;

      finger_8R_pos = scaling_handR(hand_position);
      marker_finger8R.transform.position = finger_8R_pos;
      finger_4R_pos = scaling_handR(finger_4);
      marker_finger4R.transform.position = finger_4R_pos;
      finger_6R_pos = scaling_handR(finger_6);
      marker_finger6R.transform.position = finger_6R_pos;
    }
  }
}

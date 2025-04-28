using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
  public static HandManager Instance;
  public Vector3 handpos; //8
  public Vector3 finger4;
  public Vector3 finger6;
  public int gesture_class;

  private void Awake()
  {

  }
}

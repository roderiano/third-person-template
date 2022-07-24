 using UnityEngine;
 using System.Collections;
 
 public class CameraController : MonoBehaviour {
 
    public float sensitivity = 3f;
    public float height = 1f;
    public float distance = 2f;
    public Transform player;
 
    private Vector3 offsetX;
    private Vector3 offsetY;
 
    void Start () 
    {
        offsetX = new Vector3 (0, height, distance);
        offsetY = new Vector3 (0, 0, distance);
    }
      
    void LateUpdate()
    {
        offsetX = Quaternion.AngleAxis (Input.GetAxis("Mouse X") * sensitivity, Vector3.up) * offsetX;
        offsetY = Quaternion.AngleAxis (Input.GetAxis("Mouse Y") * sensitivity, Vector3.right) * offsetY;
        transform.position = player.position + offsetX + offsetY;
        transform.LookAt(player.position);
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;
    public Camera MinimapCam;

    private void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

    public int Zoom = 0;
    public int oSize = 0;

       public void ChangeZoom ()
    {
        
        if (Zoom == 0)
        {
            oSize = 25;
            Zoom = 1;
        }
        else 
        {
            oSize = 15;
            Zoom = 0;
        } 

        MinimapCam.orthographicSize = oSize;
      
    }

}

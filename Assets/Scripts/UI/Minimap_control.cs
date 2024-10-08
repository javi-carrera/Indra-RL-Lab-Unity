using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap_control : MonoBehaviour
{
    public Transform TB_source;
    public Transform TT_source;

    public Image MapFrame;
    public Image TankBody;
    public Image TankTurret;


    private void LateUpdate()
    {
        Quaternion newRotationTB = TB_source.rotation;
        Quaternion newRotationTT = TT_source.rotation;

        newRotationTB.z = -newRotationTB.y;
        newRotationTB.y = 0;

        newRotationTT.z = -newRotationTT.y;
        newRotationTT.y = 0;

        TankBody.transform.localRotation = newRotationTB;
        TankTurret.transform.rotation = newRotationTT;
        
    }

    int C = 0;
    Vector3 FrameSize = new Vector3(0.0f, 0.0f, 0.0f);
    public void ChangeMapSize ()
    {
        
        if (C == 0)
        {
            FrameSize = new Vector3(2.0f, 2.0f, 2.0f);
            C = 1;
        }
        else
        {
            FrameSize = new Vector3(1.0f, 1.0f, 1.0f);
            C = 0;

        }
        MapFrame.transform.localScale = FrameSize;
    }
}

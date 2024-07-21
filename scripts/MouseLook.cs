using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform mainCam;
    // sensitivity of scrolling for zoom in/out
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public float MinimumY = -360F;
    public float MaximumY = 360F;

    private Quaternion m_CameraTargetRot;
    public static bool isLocked = false;

    public float scrollSpeed = 100f;
    // Start is called before the first frame update
    void Start()
    {
        // get first rotation(quaternion of the camera)
        m_CameraTargetRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocked)
        {
            // for controlling rotation when click mouse right-button
            if (Input.GetMouseButton(1))
            {
                float yRot = Input.GetAxis("Mouse X") * XSensitivity;
                float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

                m_CameraTargetRot *= Quaternion.Euler(-xRot, yRot, 0f);
                m_CameraTargetRot = ClampRotationAroundXYAxis(m_CameraTargetRot);

                transform.rotation = m_CameraTargetRot;
            }

            // for controlling zoom in/out when clicking mouse scroll
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                mainCam.localPosition += new Vector3(0f, 0f, scrollInput * scrollSpeed);
            }
        }
    }
    /// <summary>
    /// limits rotation angle between MIN and MAX values
    /// </summary>
    /// <param name="q"></param>
    /// <returns>Quaternion</returns>
    Quaternion ClampRotationAroundXYAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        float angleY = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.y);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        angleY = Mathf.Clamp(angleY, MinimumY, MaximumY);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        q.y = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleY);

        return q;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private float scrollSpeed;
    private PlatformManager manager;
    private bool isSafe;

    public void Initialize(float speed, PlatformManager mgr, bool safe)
    {
        scrollSpeed = speed;
        manager = mgr;
        isSafe = safe;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        if (transform.position.x < Camera.main.transform.position.x - 20f)
        {
            gameObject.SetActive(false);
        }
    }

    public bool IsSafe => isSafe;
}

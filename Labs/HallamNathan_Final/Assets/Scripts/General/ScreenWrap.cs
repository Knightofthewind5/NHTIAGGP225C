using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private Vector3 horizontalOffset;
    private Vector3 verticalOffset;
    private float horizontalEdge;
    private float verticalEdge;

    // Start is called before the first frame update
    void Start()
    {
        horizontalOffset = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0.5f, -Camera.main.transform.position.z)); //Changed from v
        verticalOffset = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 1.0f, -Camera.main.transform.position.z)); //
        horizontalEdge = horizontalOffset.x;
        verticalEdge = verticalOffset.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > horizontalEdge)
		{
            transform.position -= horizontalOffset * 2;
		}
        else if (transform.position.x < -horizontalEdge)
        {
            transform.position += horizontalOffset * 2;
        }
        if (transform.position.y > verticalEdge)
        {
            transform.position -= verticalOffset * 2;
        }
        else if (transform.position.y < -verticalEdge)
        {
            transform.position += verticalOffset * 2;
        }
    }
}

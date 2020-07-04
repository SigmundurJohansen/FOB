using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vert = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float horz = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        
        transform.Translate(vert, horz, 0);
    }
}

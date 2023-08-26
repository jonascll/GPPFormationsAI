using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] float _speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("w")) {
            Camera.main.transform.Translate(Vector3.up * _speed * Time.deltaTime) ;
        }
        if (Input.GetKey("s"))
        {
            Camera.main.transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        if (Input.GetKey("d"))
        {
            Camera.main.transform.Translate(Vector3.right* _speed * Time.deltaTime);
        }
        if (Input.GetKey("a"))
        {
            Camera.main.transform.Translate(Vector3.left * _speed * Time.deltaTime);
        }
    }
   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBC1 : MonoBehaviour
{
    [Range(0,1)]
    public float t;

    public float speed;
    public float step;
    public int count;

    public GameObject sphere1;
    public GameObject sphere2;
    public GameObject sphere3;
    public GameObject sphere4;

    public Vector3 s1RotVector;
    public Vector3 s2RotVector;
    public Vector3 s3RotVector;
    public Vector3 s4RotVector;

    public GameObject point;

    public List<GameObject> spheres;

    GameObject dot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        sphere1.transform.rotation = Quaternion.Euler(s1RotVector);
        sphere2.transform.rotation = Quaternion.Euler(s2RotVector);
        sphere3.transform.rotation = Quaternion.Euler(s3RotVector);
        sphere4.transform.rotation = Quaternion.Euler(s4RotVector);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sphere1.transform.position, 0.7f);
        Gizmos.DrawSphere(sphere2.transform.position, 0.7f);
        Gizmos.DrawSphere(sphere3.transform.position, 0.7f);
        Gizmos.DrawSphere(sphere4.transform.position, 0.7f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(sphere1.transform.GetChild(0).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere2.transform.GetChild(0).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere3.transform.GetChild(0).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere4.transform.GetChild(0).transform.position, 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(sphere1.transform.GetChild(1).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere2.transform.GetChild(1).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere3.transform.GetChild(1).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere4.transform.GetChild(1).transform.position, 0.2f);
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(sphere1.transform.GetChild(2).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere2.transform.GetChild(2).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere3.transform.GetChild(2).transform.position, 0.2f);
        Gizmos.DrawSphere(sphere4.transform.GetChild(2).transform.position, 0.2f);

        Gizmos.color = Color.red;

        float tee=0f;
        for(int i=0; i<count; i++)
        {
            if(tee+step>1f)
                break;

            Vector3 _point = CubicBézier(sphere1.transform.position, 
                sphere2.transform.position, 
                sphere3.transform.position, 
                sphere4.transform.position, tee);

            // Vector3 _point = CustomLerp(sphere1.transform.position, 
            //     sphere2.transform.position, tee);
            
            tee+=step;

            Gizmos.DrawSphere(_point, 0.3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        float rayRange = 20;
        if(Physics.Raycast(ray.origin, ray.direction, out hit, rayRange))
        {
            if(Input.GetKey(KeyCode.Mouse0)){
                hit.transform.position = new Vector3(ray.GetPoint(hit.distance).x,ray.GetPoint(hit.distance).y,
                    hit.transform.position.z);
                dot = hit.transform.gameObject;
            }
        }

        if(dot!=null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.localPosition.z+dot.transform.position.z;
            dot.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }

        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            dot = null;
        }

        Debug.DrawRay(ray.origin, ray.direction*rayRange, Color.green);

        // foreach(GameObject sphere in spheres)


        if(Input.GetKey(KeyCode.Space)&&t<1)
        {
            if(t+Time.deltaTime*speed<1)
            {
                t+=Time.deltaTime*speed;
            }
            else
            {
                t=1;
            }
        }

        point.transform.position = 
        CubicBézier(sphere1.transform.position, 
                    sphere2.transform.position, 
                    sphere3.transform.position,
                    sphere4.transform.position, t);

        Vector3 rot =
        CubicBézier(s1RotVector, 
                    s2RotVector, 
                    s3RotVector,
                    s4RotVector, t);
        point.transform.rotation = Quaternion.Euler(rot);
    }

    Vector3 CustomLerp(Vector3 pos1, Vector3 pos2, float _t)
    {
        return pos1*(1-_t)+pos2*_t;
    }

    Vector3 QuadraticBézier(Vector3 pos1, Vector3 pos2, Vector3 pos3, float _t)
    {
        Vector3 _a = CustomLerp(pos1, pos2, _t);
        Vector3 _b = CustomLerp(pos2, pos3, _t);
        return CustomLerp(_a, _b, _t);
    }

    Vector3 CubicBézier(Vector3 pos1, Vector3 pos2, Vector3 pos3, Vector3 pos4, float _t)
    {
        Vector3 _a = QuadraticBézier(pos1, pos2, pos3, _t);
        Vector3 _b = QuadraticBézier(pos2, pos3, pos4, _t);
        return CustomLerp(_a, _b, _t);
    }

    Vector3 Q2V3(Quaternion q)
    {
        return new Vector3(q.x, q.y, q.z);
    }
}

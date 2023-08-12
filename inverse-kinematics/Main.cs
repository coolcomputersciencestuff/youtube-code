using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class Main : MonoBehaviour
{
    public bool video, drawRays, startShakingPlatform;
    public CinemachineBrain cineBrain;
    public GameObject followTarget, platform;
    [HideInInspector]
    public List<GameObject> articulations1, articulations2, articulations3, articulations4, backLeftLegs, frontLeftLegs, frontRightLegs, backRightLegs;
    public GameObject spiderGO, terrain;
    public bool moveCube;
    public float speed;
    Vector3 dir;
    [HideInInspector]
    public GameObject toe1, toe2, toe3, toe4, toesGO, legsGO;
    public InverseKinematics fl_leg, fr_leg, bl_leg, br_leg;
    public Vector3 averageToePos;
    [Range(0, 1)]
    public float all;
    public LayerMask mask;
    public TextMeshProUGUI statsTMP;

    // Start is called before the first frame update
    void Start()
    {
        toe1 = toesGO.transform.GetChild(0).gameObject;
        toe2 = toesGO.transform.GetChild(1).gameObject;
        toe3 = toesGO.transform.GetChild(2).gameObject;
        toe4 = toesGO.transform.GetChild(3).gameObject;
        // if(moveCube){
        // }
        // else{
            // cube.transform.position=Vector3.zero;
        // }

        articulations1.Add(toe1.gameObject);
        articulations1.Add(toe1.transform.GetChild(0).gameObject);
        articulations1.Add(toe1.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);

        articulations2.Add(toe2.gameObject);
        articulations2.Add(toe2.transform.GetChild(0).gameObject);
        articulations2.Add(toe2.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);

        articulations3.Add(toe3.gameObject);
        articulations3.Add(toe3.transform.GetChild(0).gameObject);
        articulations3.Add(toe3.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);

        articulations4.Add(toe4.gameObject);
        articulations4.Add(toe4.transform.GetChild(0).gameObject);
        articulations4.Add(toe4.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject);

        backRightLegs.Add(legsGO.transform.GetChild(0).gameObject);
        backRightLegs.Add(legsGO.transform.GetChild(1).gameObject);

        frontRightLegs.Add(legsGO.transform.GetChild(2).gameObject);
        frontRightLegs.Add(legsGO.transform.GetChild(3).gameObject);

        frontLeftLegs.Add(legsGO.transform.GetChild(4).gameObject);
        frontLeftLegs.Add(legsGO.transform.GetChild(5).gameObject);

        backLeftLegs.Add(legsGO.transform.GetChild(6).gameObject);
        backLeftLegs.Add(legsGO.transform.GetChild(7).gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        VideoControls();

        float rotationSpeed = 2f;
        float rotationSpeedCam = 2f;

        if(startShakingPlatform)
        {
            // platform.transform.RotateAround(platform.transform.position, Vector3.up, Input.GetAxis("Mouse Y")*rotationSpeed);
            // platform.transform.RotateAround(platform.transform.position, Vector3.right, Input.GetAxis("Mouse X")*rotationSpeed);
            // platform.transform.RotateArround(platform.transform.position, new Vector3(0f, 0f, 1f), -Input.GetAxis("Mouse X")*rotationSpeed);
            platform.transform.Rotate(new Vector3(1f, 0f, 0f), -Input.GetAxis("Mouse Y")*rotationSpeed, Space.Self);
            platform.transform.Rotate(new Vector3(0f, 0f, 1f), -Input.GetAxis("Mouse X")*rotationSpeed, Space.Self);
        }

        // cube.transform.Rotate(
        //     // (Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime)
        //     0,
        //     // (Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime)
        //     (Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime)
        //     , 0, Space.World);

        // float smoothSpeed = 3f;
        // Quaternion a = cube.transform.localRotation;
        // Quaternion b = Quaternion.Euler(
        //     // Input.mousePosition.x / 100
        //     0
        //     ,
        //     // cube.transform.position.x * -2
        //     Input.mousePosition.x
        //     ,
        //     // cube.transform.position.x * -2.5f
        //     0
        //     );
        // float t = Time.deltaTime * smoothSpeed;
        // Quaternion smooth = Quaternion.Slerp(a, b, t);
        // cube.transform.localRotation = smooth;

        if(!startShakingPlatform)
        {
            if(!Input.GetKey(KeyCode.LeftShift))
                spiderGO.transform.RotateAround(spiderGO.transform.position, Vector3.up, Input.GetAxis("Mouse X")*rotationSpeed);
            else
                followTarget.transform.RotateAround(spiderGO.transform.position, Vector3.up, Input.GetAxis("Mouse X")*rotationSpeed);
            float _y = followTarget.transform.localEulerAngles.y;
            float _z = followTarget.transform.localEulerAngles.z;
            // float _y = 0f;
            // float _z = 0f;

            followTarget.transform.RotateAround(followTarget.transform.position, followTarget.transform.right, -Input.GetAxis("Mouse Y")*rotationSpeedCam);
            followTarget.transform.localEulerAngles = new Vector3(
                followTarget.transform.localEulerAngles.x
                // 0f
                ,
                 _y, _z);
        }

        averageToePos = (toe1.transform.position+toe2.transform.position+toe3.transform.position+toe4.transform.position)/4;

        Movement();

        // Vector3 upTarget = new Vector3(targetPoint.position.x, cube.position.y+1f, targetPoint.position.z);
        Ray ray = new Ray(spiderGO.transform.position, -spiderGO.transform.up);
        if(Physics.Raycast(ray, out RaycastHit hit, 3f, mask))
        {
            if(hit.normal==new Vector3(0f, 1f, 0f))
                spiderGO.transform.GetChild(0).transform.rotation = spiderGO.transform.rotation;

            spiderGO.transform.GetChild(0).transform.rotation = Quaternion.Slerp(spiderGO.transform.GetChild(0).transform.rotation, 
                Quaternion.FromToRotation(spiderGO.transform.GetChild(0).transform.up, hit.normal)
                *spiderGO.transform.rotation,
                5f * Time.deltaTime);

            toe1.transform.rotation = Quaternion.Slerp(toe1.transform.rotation, 
                Quaternion.FromToRotation(toe1.transform.up, hit.normal)*toe1.transform.rotation,
                5f * Time.deltaTime);
            toe2.transform.rotation = Quaternion.Slerp(toe2.transform.rotation, 
                Quaternion.FromToRotation(toe2.transform.up, hit.normal)*toe2.transform.rotation,
                5f * Time.deltaTime);
            toe3.transform.rotation = Quaternion.Slerp(toe3.transform.rotation, 
                Quaternion.FromToRotation(toe3.transform.up, hit.normal)*toe3.transform.rotation,
                5f * Time.deltaTime);
            toe4.transform.rotation = Quaternion.Slerp(toe4.transform.rotation, 
                Quaternion.FromToRotation(toe4.transform.up, hit.normal)*toe4.transform.rotation,
                5f * Time.deltaTime);

            // if(!toe.gameObject.name.Contains("front")) // humanoid
                // targetPoint.position = new Vector3(targetPoint.position.x, hit.point.y, targetPoint.position.z);
        }

        // ray = new Ray(spiderGO.transform.position, -spiderGO.transform.up);
        if(drawRays)
            Debug.DrawRay(ray.origin, ray.direction*3f, Color.green);

        for(int i=0; i<backLeftLegs.Count; i++)
            SetLegsPos(articulations1[i].transform.position, articulations1[i+1].transform.position, backLeftLegs[i].transform);

        for(int i=0; i<frontLeftLegs.Count; i++)
            SetLegsPos(articulations2[i].transform.position, articulations2[i+1].transform.position, frontLeftLegs[i].transform);

        for(int i=0; i<frontRightLegs.Count; i++)
            SetLegsPos(articulations3[i].transform.position, articulations3[i+1].transform.position, frontRightLegs[i].transform);

        for(int i=0; i<backRightLegs.Count; i++)
            SetLegsPos(articulations4[i].transform.position, articulations4[i+1].transform.position, backRightLegs[i].transform);
    }

    void VideoControls()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            startShakingPlatform=!startShakingPlatform;
        }

        statsTMP.text = "";

        if(Input.GetKeyDown(KeyCode.C))
        {
            cineBrain.enabled = !cineBrain.enabled;
        }

        statsTMP.text += "(C) camera follow enabled = ";
        statsTMP.text += cineBrain.enabled ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.S))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

        statsTMP.text += "(S) show mouse enabled = ";
        statsTMP.text += Cursor.visible ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.T))
        {
            for(int i=0; i<toesGO.transform.childCount; i++)
            {
                toesGO.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled =
                !toesGO.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled;
            }
        }
        
        bool propBool = toe1.gameObject.GetComponent<MeshRenderer>().enabled;
        statsTMP.text += "(T) toes enabled = ";
        statsTMP.text += propBool ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.A))
        {
            for(int i=0; i<toesGO.transform.childCount; i++)
            {
                toesGO.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = 
                !toesGO.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled;

                toesGO.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled =
                !toesGO.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;

                toesGO.transform.GetChild(i).transform.GetChild(0)
                .transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled =
                !toesGO.transform.GetChild(i).transform.GetChild(0)
                .transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
            }
        }

        propBool = toe1.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
        statsTMP.text += "(A) articulations enabled = ";
        statsTMP.text += propBool ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.L))
        {
            for(int i=0; i<legsGO.transform.childCount; i++)
            {
                legsGO.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled =
                !legsGO.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled;
            }
        }

        propBool = legsGO.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled;
        statsTMP.text += "(L) legs enabled = ";
        statsTMP.text += propBool ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.M))
        {
            for(int i=0; i<4; i++)
            {
                spiderGO.transform.GetChild(0).gameObject.transform.GetChild(i).GetComponent<MeshRenderer>().enabled =
                !spiderGO.transform.GetChild(0).gameObject.transform.GetChild(i).GetComponent<MeshRenderer>().enabled;
            }
        }

        propBool = spiderGO.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled;
        statsTMP.text += "(M) main joints enabled = ";
        statsTMP.text += propBool ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.P))
        {
            for(int i=0; i<4; i++)
            {
                spiderGO.transform.GetChild(0).GetChild(i+4).gameObject.GetComponent<MeshRenderer>().enabled =
                !spiderGO.transform.GetChild(0).GetChild(i+4).gameObject.GetComponent<MeshRenderer>().enabled;
            }
        }

        propBool = spiderGO.transform.GetChild(0).GetChild(4).gameObject.GetComponent<MeshRenderer>().enabled;
        statsTMP.text += "(P) poles enabled = ";
        statsTMP.text += propBool ? "true" : "false";
        statsTMP.text += "\n";

        if(Input.GetKeyDown(KeyCode.Y))
        {
            for(int i=0; i<4; i++)
            {
                spiderGO.transform.GetChild(i+1).gameObject.GetComponent<MeshRenderer>().enabled =
                !spiderGO.transform.GetChild(i+1).gameObject.GetComponent<MeshRenderer>().enabled;
            }
        }

        propBool = spiderGO.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled;
        statsTMP.text += "(Y) toes targets enabled = ";
        statsTMP.text += propBool ? "true" : "false";
        // statsTMP.text += "\n";
    }

    void SetLegsPos(Vector3 start, Vector3 end, Transform _t)
    {
        var dir = end - start;
        var mid = (dir) / 2.0f + start;
        _t.position = mid;
        _t.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        Vector3 scale = _t.localScale;
        scale.y = dir.magnitude * 1.0f;
        scale.x = scale.z = .2f;
        _t.localScale = scale;
    }

    void Movement()
    {
        dir = Vector3.zero;

        if(Input.GetKey(KeyCode.Z)){
            // dir=new Vector3(0f, 0f, -1f);
            dir+=-spiderGO.transform.forward;
            if(dir.x<-1||dir.x>1)
                dir = dir/2;
            if(dir.y<-1||dir.y>1)
                dir = dir/2;
            if(dir.z<-1||dir.z>1)
                dir = dir/2;
        }
        if(Input.GetKey(KeyCode.S)){
            // dir=new Vector3(0f, 0f, 1f);
            dir+=spiderGO.transform.forward;
            if(dir.x<-1||dir.x>1)
                dir = dir/2;
            if(dir.y<-1||dir.y>1)
                dir = dir/2;
            if(dir.z<-1||dir.z>1)
                dir = dir/2;
        }
        if(Input.GetKey(KeyCode.D)){
            // dir=new Vector3(-1f, 0f, 0f);
            dir+=-spiderGO.transform.right;
            if(dir.x<-1||dir.x>1)
                dir = dir/2;
            if(dir.y<-1||dir.y>1)
                dir = dir/2;
            if(dir.z<-1||dir.z>1)
                dir = dir/2;
        }
        if(Input.GetKey(KeyCode.Q)){
            // dir=new Vector3(1f, 0f, 0f);
            dir+=spiderGO.transform.right;
            if(dir.x<-1||dir.x>1)
                dir = dir/2;
            if(dir.y<-1||dir.y>1)
                dir = dir/2;
            if(dir.z<-1||dir.z>1)
                dir = dir/2;
        }

        if(dir!=Vector3.zero)
        {
            if(moveCube){
                spiderGO.transform.position += dir*Time.deltaTime*speed;
            }
            else{
                terrain.transform.position -= dir*Time.deltaTime*speed;
            }
        }
    }
}

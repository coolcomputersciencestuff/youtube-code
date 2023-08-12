using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// https://www.youtube.com/watch?v=qqOAzn05fvk

public class InverseKinematics : MonoBehaviour
{
    [Range(0, 1)]
    public float all;
    [Range(0, 1)]
    public float time;
    [Range(-1.57f, 1.57f)]
    public float cosV;
    public float heightIntensity;
    public Vector3 guudToePos;
    public Main main;
    public bool enableRot;
    public LayerMask mask;
    public Transform targetPoint, cube;

    public int chainLength;
    public Transform target;
    public Transform toe;
    public Transform pole;

    // public int iterations;
    // public float delta;

    // [Range(0, 1)]
    // public float snapBackStrength;

    Transform[] bones;
    Vector3[] positions;
    float[] bonesLength;
    float completeLength;

    Vector3[] startDirectionSucc;
    Quaternion[] startRotationBone;
    Quaternion startRotationTarget;
    Quaternion startRotationRoot;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        // initial array
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirectionSucc = new Vector3[chainLength+1];
        startRotationBone = new Quaternion[chainLength+1];

        startRotationTarget = target.rotation;
        completeLength = 0;

        // init data

        Transform curr = this.transform;
        for(int i=bones.Length-1; i>=0; i--)
        {
            bones[i] = curr;
            startRotationBone[i] = curr.rotation;

            if(i==bones.Length-1)
            {
                startDirectionSucc[i] = target.position - curr.position;
            }
            else
            {
                startDirectionSucc[i] = bones[i+1].position - curr.position;
                bonesLength[i] = (bones[i+1].position - curr.position).magnitude;
                completeLength += bonesLength[i];
            }
            curr=curr.parent;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool gotPos, legDown;
    public float groundDist;
    public bool otherLegsAreGrounded;

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(toe.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit info, 10, mask))
        {
            // toe.position = info.point;
        }

        groundDist = info.distance;


        Vector3 upTarget = new Vector3(targetPoint.position.x, cube.position.y+1f, targetPoint.position.z);
        // Ray _ray1 = new Ray(upTarget, -targetPoint.transform.up);
        // Ray _ray1 = new Ray(targetPoint.transform.position, -targetPoint.transform.up);
        Ray _ray1 = new Ray(upTarget, -Vector3.up);
        if(Physics.Raycast(_ray1, out RaycastHit _info1, 100, mask))
        {
            // if(!toe.gameObject.name.Contains("front")) // humanoid
                targetPoint.position = new Vector3(targetPoint.position.x, _info1.point.y, targetPoint.position.z);
            
            if(main.drawRays)
                Debug.DrawRay(upTarget, _info1.point-upTarget, Color.red);

            // upTarget = new Vector3(targetPoint.position.x, cube.position.y+1f, targetPoint.position.z);

            // upTarget = new Vector3(
            //  targetPoint.position.x*targetPoint.transform.rotation.x,
            //    (cube.position.y+1f)*targetPoint.transform.rotation.y,
            //  targetPoint.position.z*targetPoint.transform.rotation.z
            //  );

            // Ray _ray1 = new Ray(upTarget, -targetPoint.transform.up);
            // Ray _ray1 = new Ray(targetPoint.transform.position, -targetPoint.transform.up);

            // _ray1 = new Ray(upTarget, -cube.transform.up);
        }

        // Debug.DrawRay(_ray1.origin, _ray1.direction*10, Color.green);

        if(main.drawRays)
            Debug.DrawRay(targetPoint.position, toe.position-targetPoint.position, Color.green);


        // moving the leg

        float _max = 0.3f;

        if(fr(info.distance, 1)==0f&&Vector3.Distance(targetPoint.position, toe.position)>_max)
            legDown=true;

        if(this == main.fl_leg || this == main.br_leg){
            if(fr(main.fr_leg.groundDist, 1)==0f && fr(main.bl_leg.groundDist, 1)==0f){
                otherLegsAreGrounded=true;
                main.fr_leg.otherLegsAreGrounded = main.bl_leg.otherLegsAreGrounded = false;
            }
        }
        if(this == main.fr_leg || this == main.bl_leg){
            if(fr(main.fl_leg.groundDist, 1)==0f && fr(main.br_leg.groundDist, 1)==0f){
                otherLegsAreGrounded=true;
                main.fl_leg.otherLegsAreGrounded = main.br_leg.otherLegsAreGrounded = false;
            }
        }

                // // humanoid
                // if(this == main.br_leg){
                //     if(fr(main.bl_leg.groundDist, 1)==0f){
                //         otherLegsAreGrounded=true;
                //         main.bl_leg.otherLegsAreGrounded = false;
                //     }
                // }
                // if(this == main.bl_leg){
                //     if(fr(main.br_leg.groundDist, 1)==0f){
                //         otherLegsAreGrounded=true;
                //         main.br_leg.otherLegsAreGrounded = false;
                //     }
                // }

        if(!main.video)
        {
            if(otherLegsAreGrounded
            // ||toe.gameObject.name.Contains("front") // humanoid
            )
            {
                if(Vector3.Distance(targetPoint.position, toe.position)>_max||legDown)
                {
                    all+=0.2f;
                    if(fr(info.distance, 1)==0f&&fr(all,1)>0.9f){
                        all=0f;
                        guudToePos=targetPoint.position;
                        legDown=false;
                    }
                }
            }
        }

        if(!gotPos){
            guudToePos = toe.position;
            gotPos=true;
        }

        heightIntensity = 0.5f;
        // all = main.all;
        cosV = 1.57f*2f*all-1.57f;
        time = all;

        Vector3 _p = guudToePos*(1-time) + targetPoint.position*time;
        toe.position = 
        new Vector3(_p.x, 
            _p.y+(Mathf.Cos(cosV)*heightIntensity), 
            // toe.position.y,
            _p.z);

        cube.position = new Vector3(cube.position.x, main.averageToePos.y+1.3f, cube.position.z);

        // Debug.DrawRay(toe.position, Vector3.down*info.distance, Color.red);

        if(Vector3.Distance(toe.transform.position, target.position)>completeLength)
        {
            this.transform.position = target.position;
        }

        // make it that if the distance between the cube and the object forward is smaller push the cube behind


        // float _len = 2f;
        // Ray _ray3 = new Ray(cube.transform.position, -cube.transform.forward);
        // Debug.DrawRay(cube.transform.position, -cube.transform.forward*_len, Color.green);
        // if(Physics.Raycast(_ray3, out RaycastHit _info3, _len))
        // {
        //     Debug.DrawRay(cube.transform.position, _info3.point-cube.transform.position, Color.red);
        //     Debug.Log(_info3.distance);
        //     // if(_info3.distance<0.5f)
        //     //     cube.transform.position-=(-cube.transform.forward)*0.1f;
        // }

    }

    void LateUpdate()
    {
        ResolveInverseKinematics();
    }

    void ResolveInverseKinematics()
    {
        if(target==null)
            return;

        if(bonesLength.Length!=chainLength)
            Init();

        // get position
        for(int i=0; i<bones.Length; i++)
            positions[i] = bones[i].position;

        var rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var rootRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);

        if((target.position - bones[0].position).sqrMagnitude >= completeLength*completeLength)
        {
            var direction = (target.position - positions[0]).normalized;

            for(int i=1; i<positions.Length; i++)
                positions[i] = positions[i-1] + direction*bonesLength[i-1];
        }
        else
        {
            for(int i=0; i<10; i++)
            {
                for(int j=positions.Length-1; j>0; j--)
                {
                    if(j==positions.Length-1)
                        positions[j]=target.position;
                    else
                        positions[j] = positions[j+1] + (positions[j] - positions[j+1]).normalized * bonesLength[j];
                }

                for(int j=1;j<positions.Length; j++)
                {
                    positions[j] = positions[j-1] + (positions[j] - positions[j-1]).normalized * bonesLength[j-1];
                }

                if((positions[positions.Length-1] - target.position).sqrMagnitude < 0.001f * 0.001f)
                    break;
            }
        }

        if(pole!=null)
        {
            for(int i=1; i<positions.Length-1;i++)
            {
                var plane = new Plane(positions[i+1] - positions[i-1], positions[i-1]);
                var projectedPole = plane.ClosestPointOnPlane(pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i-1], projectedPole - positions[i-1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i-1]) + positions[i-1];
            }
        }
        
        // set position
        for(int i=0; i<positions.Length; i++) {
            if(enableRot)
            {
                if(i==positions.Length-1)
                    bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
                else
                    bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i+1] - positions[i]) * startRotationBone[i];
            }
            bones[i].position = positions[i];
        }
    }

    void OnDrawGizmos()
    {
        // return;
        // Transform curr = this.transform;
        // for(int i=0; i<chainLength && curr!=null && curr.parent!=null ; i++)
        // {
        //     float scale = Vector3.Distance(curr.position, curr.parent.position) * 0.1f;
        //     Handles.matrix = Matrix4x4.TRS(curr.position, Quaternion.FromToRotation(Vector3.up, curr.parent.position - curr.position),
        //     new Vector3(scale, Vector3.Distance(curr.parent.position, curr.position), scale));
        //     Handles.color = Color.green;
        //     Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
        //     curr=curr.parent;
        // }
    }

    public float fr(float _f, int _n) { return (float)System.Math.Round(_f, _n); } // float round
}

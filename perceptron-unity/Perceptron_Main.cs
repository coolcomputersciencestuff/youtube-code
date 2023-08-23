using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//make another class that gets functions from this one and that uses the main one in a "simpler typing way", for example in the simpler interface class: few initial start() funcs, and in update() the main train() & guess() func, you know what, mr. shiffman himself did that as he used a sketch.js didn't he ? yeah so do that please to make life easier, and make a show graphics func
public class Perceptron_Main : MonoBehaviour
{
    [Header("Perceptron related vars")]
    public List<float> weights;
    // public List<float> inputs;
    public float sum, learningRate;
    public int output;

    [Header("Other vars")]
    public List<Point> points;
    // public float aestheticsOffset;

    [Header("Graphics related vars")]
    public Sprite sp;
    public Sprite square;

    [Header("Speed & stuff related vars")]
    public int last;
    public float delta;
    public float timer;
    public int dots;

    GameObject line;
    bool stop;

    // Start is called before the first frame update
    void Start()
    {
        if(sp==null) {
            Debug.Log("insert a \"Knob\" sprite in \"sp\" var");
            Debug.Break();
        }
        if(sp==null) {
            Debug.Log("insert a \"square\" sprite in \"Square\" var");
            Debug.Break();
        }

        // some ini vars
            // if(learningRate==0f)
            //     learningRate = 0.0001f;
            // if(dots==0)
            //     dots = 100;
        //

        // initialization

            Dots();

            // // spots for the inputs
            // for(int i=0; i<2; i++)
            //     inputs.Add(Random.Range(-1f, 1f));

            // spots for the weigths
            for(int i=0; i<3; i++)
                weights.Add(0f);

            // Initializing the weights randomly
            for(int i=0; i<weights.Count; i++)
                weights[i] = Random.Range(-1f, 1f);

        //

        // // Guessing
        //     Guess(inputs);
        //     output = ActivationFun(sum);

        // Training
            foreach(Point pt in points) {
                // Train(new List<float>(){pt.x, pt.y} , pt.label);
                Guess(new List<float>(){pt.x, pt.y, pt.bias});

                pt.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().color = 
                (ActivationFun(sum) == pt.label) ? Color.green : Color.red;
                // break;
            }
        //

    }

    // Update is called once per frame
    void Update()
    {
        // a stop
            if(stop)
                return;
            int greens = 0;
            foreach(Point pt in points) {
                if(pt.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().color == Color.green)
                    greens++;
            }
            if(greens==dots) {
                Debug.Log("Done.");
                stop=true;
                // Debug.Break();
                return;
            }

        //

        // up right down left vectors

            Vector3 cmPos1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
            Vector3 cmPos2 = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, -Camera.main.transform.position.z));
            Vector3 cmPos3 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, -Camera.main.transform.position.z));
            Vector3 cmPos4 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));

        //

        timer+=Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Mouse0) || timer>=delta || delta==0)
        {
            // Training
                // foreach(Point pt in points) {
                //     Train(new List<float>(){pt.x, pt.y} , pt.label);
                //     Guess(new List<float>(){pt.x, pt.y});

                //     pt.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().color = 
                //     (ActivationFun(sum) == pt.label) ? Color.green : Color.red;
                //     // break;
                // }

                if(delta==0)
                    last=0;

                // one point at a time
                    for(int i=last; i<points.Count; i++) {

                        Train(new List<float>(){points[i].x, points[i].y, points[i].bias} , points[i].label);
                        Guess(new List<float>(){points[i].x, points[i].y, points[i].bias});

                        points[i].transform.GetChild(0).transform.GetComponent<SpriteRenderer>().color = 
                        (ActivationFun(sum) == points[i].label) ? Color.green : Color.red;

                        last++;
                        if(last==points.Count-1)
                            last=0;

                        // line pos based on last guess

                            twoDots(
                            new Vector3(cmPos1.y, -(weights[2]/weights[1]) - (weights[0]/weights[1]) * cmPos1.y)
                            ,
                            new Vector3(cmPos4.y, -(weights[2]/weights[1]) - (weights[0]/weights[1]) * cmPos4.y)
                            , line, 0f);

                        //

                        if(delta>0)
                            break;
                    }
                //
            //
        }
        if(timer>=delta)
            timer=0f;
    }

    void Train(List<float> _inputs, int correctResult)
    {
        Guess(_inputs);
        output = ActivationFun(sum);
        int error = correctResult - output;

        // tuning weights
        for(int i=0; i<weights.Count; i++)
            weights[i]+=error*_inputs[i]*learningRate;
    }

    void Guess(List<float> _inputs)
    {
        sum=0;
        for(int i=0; i<weights.Count; i++) {
            sum+=_inputs[i]*weights[i];
        }
    }

    int ActivationFun(float sum)
    {
        return (sum>=0) ? 1 : -1;
    }
    
    void Dots()
    {
        // up right down left vectors

            Vector3 cmPos1 = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, -Camera.main.transform.position.z));
            Vector3 cmPos2 = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, -Camera.main.transform.position.z));
            Vector3 cmPos3 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, -Camera.main.transform.position.z));
            Vector3 cmPos4 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -Camera.main.transform.position.z));

        //

        // draw line1

            // GameObject line1 = new GameObject("line1");
            // line1.AddComponent<SpriteRenderer>().sprite=square;
            // // line1.GetComponent<SpriteRenderer>().sortingOrder=10;
            // // "cmPos1.y/2" is the function just like the "bool func = x>y/2;" bellow in create dots
            // twoDots(new Vector3(cmPos1.y/2, cmPos1.y), new Vector3(cmPos4.y/2, cmPos4.y), line1, 0f);
            // // twoDots(new Vector3(cmPos1.y*5, cmPos1.y), new Vector3(cmPos4.y*5, cmPos4.y), line1, 0f);
            // // twoDots(new Vector3(cmPos1.y*-2, cmPos1.y), new Vector3(cmPos4.y*-2, cmPos4.y), line1, 0f);
            // line1.transform.localScale = new Vector3(line1.transform.localScale.x, 0.1f,
            //     line1.transform.localScale.z);

        //

        // draw line2

            GameObject line2 = new GameObject("line2");
            line2.AddComponent<SpriteRenderer>().sprite=square;
            line2.GetComponent<SpriteRenderer>().color=new Color(1, 1, 1, 0.5f);
            // line2.GetComponent<SpriteRenderer>().sortingOrder=10;
            twoDots(new Vector3(cmPos1.y, cmPos1.y), new Vector3(cmPos4.y, cmPos4.y), line2, 0f);
            line2.transform.localScale = new Vector3(line2.transform.localScale.x, 0.1f,
                line2.transform.localScale.z);
            line = line2;

        //

        // create dots

            for(int i=0; i<dots; i++)
            {
                GameObject dot = new GameObject("dot");
                dot.transform.localScale = new Vector3(1.5f, 1.5f);
                dot.AddComponent<SpriteRenderer>().sprite=sp;

                    GameObject sub = new GameObject("sub");
                    sub.transform.localScale = new Vector3(1f, 1f);
                    sub.AddComponent<SpriteRenderer>().sprite=sp;
                    sub.transform.SetParent(dot.transform);
                    sub.transform.position = new Vector3(0, 0);

                        float x = Random.Range(cmPos1.x, cmPos3.x);
                        float y = Random.Range(cmPos1.y, cmPos4.y);
                        dot.transform.position = new Vector3(x, y);
                        bool func = x>y/2;
                        // bool func = x>y*5;
                        // bool func = x>y*-2;
                        int label = (func) ? 1 : -1;
                        dot.GetComponent<SpriteRenderer>().color = (func) ? Color.black : Color.white;

                dot.AddComponent<Point>();
                dot.GetComponent<Point>().x = x;
                dot.GetComponent<Point>().y = y;
                dot.GetComponent<Point>().label = label;
                points.Add(dot.GetComponent<Point>());
            }
        //
    }

    // line position

        void twoDots(Vector3 dot1, Vector3 dot2, GameObject ting, float thickness)
        {
            Vector3 dot1v = dot1;
            Vector3 dot2v = dot2;
            Vector3 dot3v = new Vector3(dot2v.x, dot1v.y, 0f);

            if(dot1v == dot2v)
                return;

            // dot3.transform.position = dot3v;

            float dist1n2 = Vector3.Distance(dot1v, dot2v);

            float cos = Vector3.Distance(dot1v, dot3v)/dist1n2;
            float sin = Vector3.Distance(dot2v, dot3v)/dist1n2;
            float angle = acos(cos);
            float properAngle = 0f;

            properAngle = angle;

            if(dot2v.x < dot1v.x) { // if dot 2 is on dot 1's right
                cos = -cos;
                angle = -angle;

                properAngle = 180f+angle;

                if(dot2v.y < dot1v.y) { // if dot 2 is on dot 1's right and dot 2 is bellow dot 1
                    sin = -sin;
                    angle = -angle;

                    properAngle = 180f+angle;
                }
            }
            else { // if dot 2 is on dot 1's left
                if(dot2v.y < dot1v.y) { // if dot 2 is on dot 1's left and dot 2 is bellow dot 1
                    sin = -sin;
                    angle = -angle;
                    properAngle = 360f+angle;
                }
            }

            ting.transform.position = new Vector3(-(dist1n2/2)*cos+dot2v.x, -(dist1n2/2)*sin+dot2v.y, 0f); // the hard thing
            Vector3 tingS = ting.transform.localScale;
            ting.transform.localScale = new Vector3(dist1n2-thickness, tingS.y, tingS.z);
            ting.transform.eulerAngles = new Vector3(0f, 0f, angle);
        }

        float acos(float cos)
        {
            float rad = 180/Mathf.PI;
            return Mathf.Acos(cos)*rad;
        }

    //
}

public class Point : MonoBehaviour
{
    public float x { get; set; }
    public float y { get; set; }
    public float bias { get; set; } = 1f;
    public int label { get; set; }
}

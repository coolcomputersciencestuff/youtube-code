using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NightSceneScript : MonoBehaviour
{
    public Light fireLight;

    public float intensityRateOfChange;
    public float rangeRateOfChange;
    public TextMeshProUGUI fpsTMP;
    float second;
    int fps;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(fireLight!=null)
        {
            fireLight.intensity = RandomF(fireLight.intensity, 2f, intensityRateOfChange);
            fireLight.range = RandomF(fireLight.range, 8f, rangeRateOfChange);
        }

        if(fpsTMP==null)
            return;

        second+=Time.deltaTime;
        fps++;
        if(second>=1f)
        {
            fpsTMP.text = fps.ToString() + " fps";
            fps=0;
            second=0f;
        }
    }

    float RandomF(float f, float max, float rateOfChange)
    {
        return f>max+1f || f<max-1f ? max+Random.Range(-rateOfChange, rateOfChange) : f+Random.Range(-rateOfChange, rateOfChange);
    }
}

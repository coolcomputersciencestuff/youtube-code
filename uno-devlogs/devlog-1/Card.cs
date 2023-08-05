using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Main_3D main;
    public bool selected;
    public bool expanded;
    public Vector3 iniPos;
    public float expandDir;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(main.selectedCard!=this.gameObject||main.selectedCard==null)
            Unselect();

        float time = 0.3f;

        // wird shit but this is how this shit work :
        //     when using the localScale with the localPosition while the deck is rotated       : shit's fine !
        // and when using the lossyScale with the position      while the deck is not rotated   : shit's fine ! 
        float realScale = this.transform.localScale.x;
        if(selected)
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, iniPos + new Vector3(0, 1.5f*realScale, 0), time);
            // this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        }

        if(expanded)
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, iniPos + new Vector3(0.8f*expandDir*realScale, 0, 0), time);
            // this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        }
        else
        {
            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, iniPos, time);
            // this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        }
    }

    void OnMouseDown()
    {
        if(!main.play) // not not choose a new random rot
            main.Play();
    }

    public void InitVars()
    {
        iniPos = this.transform.localPosition;
    }

    public void Select()
    {
        selected = true;
    }

    void Unselect()
    {
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, iniPos, 0.3f);
        // this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);
        selected = false;
        if(main.selectedCard==null)
            main.Unexpand();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Main_3D : MonoBehaviour
{
    public Transform deck;
    public GameObject selectedCard;
    public GameObject chosenCard;

    public bool selectionDone;
    public bool play;
    public bool arrangeDeck;

    public TextMeshProUGUI fpsTMP;
    float second;
    int fps;

    public GameObject fakeMouse;
    public GameObject pile;

    float lerpTimer;

    float yRot;

    // Start is called before the first frame update
    void Start()
    {
        TiltCards();
        AssignVars();
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        FPS();
        // Mouse();

        if(play)
        {
            // "selectedCard!=null" in order to not nullify the chosen card, and the "chosenCard==null" to not choose the new selected card 
            if(selectedCard!=null&&chosenCard==null)
                chosenCard=selectedCard;
            lerpTimer+=Time.deltaTime;
            chosenCard.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 
            pile.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder+1;
            chosenCard.GetComponent<Card>().enabled = false;
            chosenCard.GetComponent<Collider>().enabled = false;
            chosenCard.transform.position = Vector3.Lerp(chosenCard.transform.position, pile.transform.position, 0.2f);
            chosenCard.transform.rotation = Quaternion.Lerp(chosenCard.transform.rotation, 
                Quaternion.Euler(pile.transform.rotation.eulerAngles.x, pile.transform.rotation.eulerAngles.y, yRot)
                    , 0.2f);
            chosenCard.transform.localScale = Vector3.Lerp(chosenCard.transform.localScale, pile.transform.localScale, 0.2f);
            chosenCard.transform.parent = null;
            if(lerpTimer>=0.2f)
            {
                arrangeDeck = true;
            }
            if(lerpTimer>=1f)
            {
                pile = chosenCard;
                chosenCard = null;
                play=false;
                arrangeDeck=false;
                lerpTimer=0f;
            }
        }

        if(arrangeDeck)
        {
            ArrangeDeck();
        }

        Ray();
    }

    void ArrangeDeck()
    {
        for(int i=0; i<deck.childCount; i++)
        {
            float tilt = 0f;

            if(deck.childCount%2==0)
            {
                if(i<(deck.childCount/2))
                    tilt = i-(deck.childCount/2)+1;
                else
                    tilt = i-(deck.childCount/2);
            }
            else
            {
                tilt = i-(deck.childCount/2);
            }
            
            Vector3 pos = deck.GetChild(i).transform.GetChild(0).transform.localPosition;
            deck.GetChild(i).transform.GetChild(0).transform.localPosition = new Vector3(pos.x, 0, pos.z);

            Quaternion rot = deck.GetChild(i).transform.GetChild(0).transform.localRotation;
            deck.GetChild(i).transform.GetChild(0).transform.localRotation 
            = Quaternion.Lerp(rot, Quaternion.Euler(rot.x, rot.y, -tilt*2f), 0.2f);

            if(deck.childCount%2==0)
            {
                pos = deck.GetChild(i).transform.localPosition;
                deck.GetChild(i).transform.localPosition = Vector3.Lerp(pos,
                    new Vector3((-((deck.childCount/2)-i)*1.4f)+(1.4f/2), pos.y, pos.z), 0.05f);

                deck.GetChild(i).gameObject.GetComponent<Card>().iniPos = 
                new Vector3((-((deck.childCount/2)-i)*1.4f)+(1.4f/2), 
                    deck.GetChild(i).gameObject.GetComponent<Card>().iniPos.y, 
                    deck.GetChild(i).gameObject.GetComponent<Card>().iniPos.z);
            }
            else
            {
                tilt = i-(deck.childCount/2);
                pos = deck.GetChild(i).transform.localPosition;
                deck.GetChild(i).transform.localPosition = deck.GetChild(i).transform.localPosition = Vector3.Lerp(pos,
                    new Vector3(tilt*1.4f, pos.y, pos.z), 0.05f);

                deck.GetChild(i).gameObject.GetComponent<Card>().iniPos = 
                    new Vector3(tilt*1.4f, 
                    deck.GetChild(i).gameObject.GetComponent<Card>().iniPos.y, 
                    deck.GetChild(i).gameObject.GetComponent<Card>().iniPos.z);
            }
        }
    }

    public void Play()
    {
        yRot = Random.Range(0f, 360f);
        play = true;
    }

    void Ray()
    {
        // a bug fix when you select two close cards that in reality their Z positions are identical 
        Ray ray = 
        // new Ray(Camera.main.transform.position, fakeMouse.transform.position-Camera.main.transform.position);
        Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        int rayRange = 20;
        float realScale = deck.GetChild(0).transform.GetChild(0).transform.lossyScale.x;
        hits = Physics.RaycastAll(ray, rayRange*realScale);

        bool stop=false;
        for(int i=0; i<hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if(hit.transform.gameObject.name=="STOP")
                stop=true;
        }
        if(stop)
        {
            selectedCard=null;
            return;
        }

        int biggestOrder = 0;

        for(int i=0; i<hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            
            if(!hit.transform.gameObject.name.Contains("uno"))
                continue;

            int currOrder = hit.transform.GetChild(0).transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
            biggestOrder = currOrder>biggestOrder ? currOrder : biggestOrder;
        }
        
        // &&selectedCard==null prevents the selecting boolean from being true again
        for(int i=0; i<hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            if(!hit.transform.gameObject.name.Contains("uno"))
                continue;

            if(hit.transform.GetChild(0).transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder==biggestOrder)
            {
                Unexpand();
                selectedCard = hit.transform.gameObject;
                selectedCard.GetComponent<Card>().Select();
                // if(!selectionDone)
                    Expand(biggestOrder);
            }
        }

        int hitNum=0;
        for(int i=0; i<hits.Length; i++)
            if(hits[i].transform.gameObject.name.Contains("uno"))
                hitNum++;
        
        if(hitNum==0)
            selectedCard = null;

        // if(selectedCard==null)
        //     selectionDone=false;

        Debug.DrawRay(ray.origin, ray.direction*rayRange*realScale, Color.green);
    }

    void Mouse()
    {
        // fakeMouse.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        // fakeMouse.transform.position = new Vector3(fakeMouse.transform.position.x, fakeMouse.transform.position.y, 5f);
    }

    void Expand(int currOrder)
    {
        for(int i=0; i<deck.childCount; i++)
        {
            if(deck.GetChild(i).transform.gameObject.name=="STOP")
                continue;

            if(deck.GetChild(i).transform.gameObject==selectedCard)
                continue;

            Card c = deck.GetChild(i).transform.GetComponent<Card>();
            c.expanded = true;
            c.expandDir = deck.GetChild(i).transform.GetChild(0).
            transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder<currOrder ? -1 : 1.5f;
        }
    }

    public void Unexpand()
    {
        for(int i=0; i<deck.childCount; i++)
        {
            if(deck.GetChild(i).transform.gameObject.name=="STOP")
                continue;

            Card c = deck.GetChild(i).transform.GetComponent<Card>();
            c.expanded = false;
            // c.expandDir = deck.GetChild(i).transform.gameObject.GetComponent<SpriteRenderer>().sortingOrder<currOrder ? -1 : 1;
        }
    }

    void TiltCards()
    {
        for(int i=0; i<deck.childCount; i++)
        {
            // you are going to forget how you came up with this but it just works and im proud right know lol
            float tilt = i - ((int)(deck.childCount/2));
            deck.GetChild(i).transform.GetChild(0).transform.Rotate(0, 0, -tilt*2f);
            float realScale = deck.GetChild(i).transform.GetChild(0).transform.lossyScale.x;
            deck.GetChild(i).transform.GetChild(0).transform.position += new Vector3(0, (-Mathf.Abs(tilt)/30f)*realScale, 0);
        }
    }

    void FPS()
    {
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

    void AssignVars()
    {
        for(int i=0; i<deck.childCount; i++)
        {
            if(deck.GetChild(i).transform.gameObject.name=="STOP")
                continue;

            deck.GetChild(i).gameObject.GetComponent<Card>().main = this.GetComponent<Main_3D>();
            deck.GetChild(i).gameObject.GetComponent<Card>().InitVars();
        }
    }
}

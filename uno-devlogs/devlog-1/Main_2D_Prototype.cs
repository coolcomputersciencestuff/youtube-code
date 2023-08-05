using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Main_2D_Prototype : MonoBehaviour
{
    public int draw2s;

    public GameObject newCard;
    public List<Transform> decks;
    // public List<GameObject> cards;

    public List<GameObject> turnIndicators;
    public GameObject turnArrow;

    public GameObject centerCard;

    public int centerCardType;
    public Color32 centerCardClr;
    // int prevCardNum;
    int currDeck;

    public List<int> turns;
    // int turn;
    int maxPlayers;

    float turnDir;

    bool drewFirstTime;
    // bool organize;

    bool wild;

    // bool ai;
    // bool playgame;

    // Start is called before the first frame update
    void Start()
    {
        maxPlayers = 4;

        for(int i=0; i<maxPlayers; i++)
            turns.Add(i);

        turnDir = -0.2f;

        Draw();
        drewFirstTime=true;
        HighlightTurn();

        // organize=true;
        OrganizeDeck();
    }

    // Update is called once per frame
    void Update()
    {
        // if(Input.GetKeyDown(KeyCode.P))
        //     playgame = true;

        // ShittyTempAI();

        if(Input.GetKeyDown(KeyCode.A))
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

        if(Done())
        {
            Debug.Log("done");
            return;
        }

        // after a card was played && the turn int incremented
        // HighlightTurn();

        turnArrow.transform.Rotate(0, 0, turnDir);

        // if(Input.GetKeyDown(KeyCode.M))
        // {
        //     turns.Add(turns[0]);
        //     turns.RemoveAt(0);
        //     HighlightTurn();
        // }

        // if(organize)
            OrganizeDeck();

        if(CantPlay())
        {
            if(Input.GetKeyDown(KeyCode.Space)
                // ||(ai&&playgame)
                )
            {
                currDeck = turns[0];

                DrawCard();




                turns.Add(turns[0]);
                turns.RemoveAt(0);

                HighlightTurn();

                // centerCardType=cardType;
                // centerCardClr=cardClr;

                // // updating the center card 
                // centerCard.GetComponent<Image>().color = centerCardClr;
                // centerCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TypeToString(centerCardType);

                // prevCardNum = decks[currDeck].childCount;






                // didnt use the bool cause there is not card that'll be deleted and if there wasnt the bool will not go false apparenly
                OrganizeDeck();


                // playgame=false;
            }
        }

        if(wild)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                centerCardClr=GetColor("red");
            }
            if(Input.GetKeyDown(KeyCode.B))
            {
                centerCardClr=GetColor("blue");
            }
            if(Input.GetKeyDown(KeyCode.G))
            {
                centerCardClr=GetColor("green");
            }
            if(Input.GetKeyDown(KeyCode.Y))
            {
                centerCardClr=GetColor("yellow");
            }

            // if(ai&&playgame)
            // {
            //     centerCardClr=RandomColor();
            // }

            if(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.B) || Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Y)
             // || (ai&&playgame)
             )
            {
                turns.Add(turns[0]);
                turns.RemoveAt(0);

                HighlightTurn();

                // updating the center card 
                centerCard.GetComponent<Image>().color = centerCardClr;
                wild=false;

                // playgame=false;
            }
        }
    }

    // void ShittyTempAI()
    // {
    //     if(turns[0]==2)
    //     {
    //         ai=false;
    //         return;
    //     }

        // ai=true;

        // the ai is glitchy

        // for(int i=0; i<decks[turns[0]].childCount; i++)
        // {
        //     int currCardType = StringToType(decks[turns[0]].transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        //     Color32 currCardClr = decks[turns[0]].transform.GetChild(i).GetComponent<Image>().color;

        //     if(currCardType==centerCardType||currCardClr.ToString()==centerCardClr.ToString()||currCardType==13)
        //     {
        //         GameObject go = decks[turns[0]].transform.GetChild(i).gameObject;
        //         if(playgame)
        //         {
        //             PlayCard(currCardType, centerCardClr, go); // you can get the info without the previous actual on click function
        //             playgame=false;
        //         }
        //     }
        // }
    // }

    void DrawCard()
    {
        GameObject _newCard = Instantiate(newCard);
        _newCard.transform.SetParent(decks[currDeck].transform);
        _newCard.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        _newCard.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        _newCard.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        _newCard.transform.localScale = Vector3.one;

        int cardType = Type();
        Color32 cardClr = RandomColor();
        Color32 white = new Color32(255, 255, 255, 255);
        // make the card colorable if it is or if the center card is a wild one
        cardClr = IsColorable(cardType)||(decks[currDeck].childCount==1&&cardType==13)? cardClr : white;
        decks[currDeck].GetChild(decks[currDeck].childCount-1).GetComponent<Image>().color = cardClr;
        decks[currDeck].GetChild(decks[currDeck].childCount-1).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TypeToString(cardType);

        GameObject go = decks[currDeck].GetChild(decks[currDeck].childCount-1).gameObject;
        go.GetComponent<Button>().onClick.RemoveAllListeners();
        go.GetComponent<Button>().onClick.AddListener(() => PlayCard(cardType, cardClr, go));
    }

    bool CantPlay()
    {
        int play=0;
        
        // so that you can draw only after you choose a color for the wild card you drew
        if(wild)
        {
            return false;        
        }

        for(int i=0; i<decks[turns[0]].childCount;i++)
        {
            int currCardType = StringToType(decks[turns[0]].transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            Color32 currCardClr = decks[turns[0]].transform.GetChild(i).GetComponent<Image>().color;

            if(currCardType==centerCardType||currCardClr.ToString()==centerCardClr.ToString()||currCardType==13)
            {
                play=1;
            }
        }
        return play==0;
    }

    bool Done()
    {
        int play=0;
        // be aware of the last deck which contains the center card which is unique (...==1)
        for(int i=0; i<decks.Count-1;i++)
        {
            if(decks[i].childCount==0)
            {
                play=1;
            }
        }
        return play==1;
    }

    void PlayCard(int cardType, Color32 cardClr, GameObject go)
    {
        int play=0;

        // if(cardType>9)
        // {
        //     // if( cardType==centerCardType|| (cardType==centerCardType&&cardClr.ToString()==centerCardClr.ToString()) )
        //     // {
        //     //     Debug.Log("can");
        //     //     return;
        //     // }

        //     // if(cardType>=13)
        //     // {
        //     //     Debug.Log("can");
        //     //     return;
        //     // }
        //     // if(cardClr.ToString()==centerCardClr.ToString())
        //     // {
        //     //     Debug.Log("can");
        //     //     return;
        //     // }

        //     if(centerCardType>9)
        //     {
        //         // if(cardClr.ToString()==centerCardClr.ToString())
        //         // {
        //         //     Debug.Log("can");
        //         //     return;
        //         // }
        //     }
        //     else // curr card and center card are numbers
        //     {
        //     }
        // }
        // else // got a number
        // {
        //     if(centerCardType>9)
        //     {
        //         // if(cardClr.ToString()==centerCardClr.ToString())
        //         // {
        //         //     Debug.Log("can");
        //         //     return;
        //         // }
        //     }
        //     else // curr card and center card are numbers
        //     {
        //         if(cardType==centerCardType||cardClr.ToString()==centerCardClr.ToString())
        //         {
        //             play=1;
        //         }
        //     }
        // }

        // if there is a reverse on the table || got a number and a number is on the table
        //      allow a reverse or a number
        if( centerCardType==11 || (cardType<=9&&centerCardType<=9) )
        {
            if(cardType==centerCardType||cardClr.ToString()==centerCardClr.ToString())
            {
                play=1;
            }
        }

        // if there is a number on the table
        //      allow a same color card
        if(centerCardType<=9)
        {
            if(cardClr.ToString()==centerCardClr.ToString())
            {
                play=1;
            }
        }

        // allow a same type or same color card
        if(cardType==centerCardType||cardClr.ToString()==centerCardClr.ToString())
        {
            play=1;
        }

        // allow to use the wild card on everything ?
        if(cardType==13)
            play=1;

        if(play==0)
            return;

        currDeck = turns[0];


        // Debug.Log("can");
        // Debug.Log("cant");

        if(cardType==10)
        {
            turns.Add(turns[0]);
            turns.RemoveAt(0);
        }

        if(cardType==11)
        {
            Reverse();
            turns.Reverse();
        }
        else
        {
            if(cardType!=12&&cardType!=13)
            {
                turns.Add(turns[0]);
                turns.RemoveAt(0);   
            }
        }

        if(cardType==12)
        {
            draw2s++;
            turns.Add(turns[0]);
            turns.RemoveAt(0);

            currDeck = turns[0];
            
            play=0;
            for(int i=0; i<decks[turns[0]].childCount;i++)
            {
                int currCardType = StringToType(decks[turns[0]].transform.GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                // Color32 currCardClr = decks[turns[0]].transform.GetChild(i).GetComponent<Image>().color;

                if(currCardType==12)
                {
                    play=1;
                }
            }

            // hasnt got another draw 2
            if(play!=1)
            {
                for(int i=0; i<draw2s; i++)
                {
                    DrawCard();
                    DrawCard();
                }
                draw2s=0;
            }





            // centerCardType=cardType;
            // centerCardClr=cardClr;

            // // updating the center card 
            // centerCard.GetComponent<Image>().color = centerCardClr;
            // centerCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TypeToString(centerCardType);

            // prevCardNum = decks[currDeck].childCount;






            // didnt use the bool cause there is not card that'll be deleted and if there wasnt the bool will not go false apparenly
            OrganizeDeck();

            // hasnt got another draw 2
            if(play!=1)
            {
                turns.Add(turns[0]);
                turns.RemoveAt(0);
            }
        }

        if(cardType==13)
        {
            wild=true;
        }

        HighlightTurn();

        centerCardType=cardType;
        centerCardClr=cardClr;

        // updating the center card 
        centerCard.GetComponent<Image>().color = centerCardClr;
        centerCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TypeToString(centerCardType);

        // prevCardNum = decks[currDeck].childCount;

        Destroy(go);

        // organize = true;
    }

    void OrganizeDeck()
    {
        // -1 to not organize the center card deck
        for(int j=0; j<decks.Count-1; j++)
        {
            // if(prevCardNum!=decks[j].childCount)
            //     organize=false;

            // + 7 (space)
            float maxSpace = (69.1886f + 7f) * 7f;
            float space = maxSpace/decks[j].childCount;
            
            for(int i=0; i<decks[j].childCount;i++)
            {
                decks[j].transform.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(-234f+(i*space), -70f);
            }
        }
        // organize=false;
    }

    void HighlightTurn()
    {
        Color32 red = new Color32(255, 0, 0, 255);
        Color32 green = new Color32(0, 255, 0, 255);

        for(int i=0; i<maxPlayers; i++)
        {
            turnIndicators[i].GetComponent<Image>().color = turns[0]==i ? green : red;
        }
    }

    void Draw()
    {
        for(int j=0; j<decks.Count; j++)
        {
            for(int i=0; i<decks[j].childCount; i++)
            {
                if(drewFirstTime&&decks[j].childCount==1)
                    continue;

                // cards.Add(decks[j].GetChild(i));
                int cardType = Type();
                Color32 cardClr = RandomColor();
                Color32 white = new Color32(255, 255, 255, 255);
                // make the card colorable if it is or if the center card is a wild one
                cardClr = IsColorable(cardType)||(decks[j].childCount==1&&cardType==13)? cardClr : white;
                decks[j].GetChild(i).GetComponent<Image>().color = cardClr;
                decks[j].GetChild(i).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = TypeToString(cardType);


                // if drawing the center card
                if(decks[j].childCount==1)
                {
                    if(drewFirstTime)
                        continue;

                    centerCardType=cardType;
                    centerCardClr=cardClr;

                    if(centerCardType==11)
                    {
                        Reverse();
                        turns.Reverse();
                    }

                    if(centerCardType==10)
                    {
                        turns.Add(turns[0]);
                        turns.RemoveAt(0);
                    }

                    if(centerCardType==12)
                    {
                        draw2s++;
                        
                        int play=0;
                        for(int k=0; k<decks[turns[0]].childCount;k++)
                        {
                            int currCardType = StringToType(decks[turns[0]].transform.GetChild(k).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                            // Color32 currCardClr = decks[turns[0]].transform.GetChild(k).GetComponent<Image>().color;

                            if(currCardType==12)
                            {
                                play=1;
                            }
                        }

                        // hasnt got another draw 2
                        if(play!=1)
                        {
                            for(int l=0; l<draw2s; l++)
                            {
                                DrawCard();
                                DrawCard();
                            }
                            draw2s=0;
                        }

                        // didnt use the bool cause there is not card that'll be deleted and if there wasnt the bool will not go false apparenly
                        OrganizeDeck();

                        // hasnt got another draw 2
                        if(play!=1)
                        {
                            turns.Add(turns[0]);
                            turns.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    GameObject go = decks[j].GetChild(i).gameObject;
                    go.GetComponent<Button>().onClick.RemoveAllListeners();
                    go.GetComponent<Button>().onClick.AddListener(() => PlayCard(cardType, cardClr, go));
                }

            }
        }
    }

    void Reverse()
    {
        bool flipX = turnArrow.GetComponent<SpriteRenderer>().flipX;
        turnArrow.GetComponent<SpriteRenderer>().flipX=!flipX;
        turnDir = -turnDir;
    }

    int StringToType(string str)
    {
        int type = 0;

        switch(str) 
        {
          case "0":
            type = 0;
            break;
          case "1":
            type = 1;
            break;
          case "2":
            type = 2;
            break;
          case "3":
            type = 3;
            break;
          case "4":
            type = 4;
            break;
          case "5":
            type = 5;
            break;
          case "6":
            type = 6;
            break;
          case "7":
            type = 7;
            break;
          case "8":
            type = 8;
            break;
          case "9":
            type = 9;
            break;
          case "skip":
            type = 10;
            break;
          case "reverse":
            type = 11;
            break;
          case "draw2":
            type = 12;
            break;
          case "wild":
            type = 13;
            break;
          // case "wildDraw4":
          //   type = 14;
          //   break;
        }

        return type;
    }

    string TypeToString(int type)
    {
        string typeStr = "";

        switch(type) 
        {
          case 0:
            typeStr = "0";
            break;
          case 1:
            typeStr = "1";
            break;
          case 2:
            typeStr = "2";
            break;
          case 3:
            typeStr = "3";
            break;
          case 4:
            typeStr = "4";
            break;
          case 5:
            typeStr = "5";
            break;
          case 6:
            typeStr = "6";
            break;
          case 7:
            typeStr = "7";
            break;
          case 8:
            typeStr = "8";
            break;
          case 9:
            typeStr = "9";
            break;
          case 10:
            typeStr = "skip";
            break;
          case 11:
            typeStr = "reverse";
            break;
          case 12:
            typeStr = "draw2";
            break;
          case 13:
            typeStr = "wild";
            break;
          // case 14:
          //   typeStr = "wildDraw4";
          //   break;
        }

        return typeStr;
    }

    int Type()
    {
        return Random.Range(0, 14);
    }

    bool IsColorable(int cardType)
    {
        return cardType<13;
    }

    Color32 RandomColor()
    {
        Color32[] colors = {GetColor("red"), GetColor("green"), GetColor("blue"), GetColor("yellow")};
        return colors[Random.Range(0, colors.Length)];
    }

    Color32 GetColor(string clr_str)
    {
        Color32 clr = new Color32(0, 0, 0, 0);

        // schemecolor.com/uno.php
        switch(clr_str) 
        {
          case "red":
            clr = new Color32(215, 38, 0, 255);
            break;
          case "green":
            clr = new Color32(55, 151, 17, 255);
            break;
          case "blue":
            clr = new Color32(9, 86, 191, 255);
            break;
          case "yellow":
            clr = new Color32(236, 212, 7, 255);
            break;
        }

        return clr;
    }
}

using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BoardPath : MonoBehaviour
{
    int i = 0;

    //public CardScript cardScript;

    public GameObject playerPrefab;
    public GameObject cardPrefab;

    int totalDiceValue;
    public int countOfPlayers;
    public DiceBehaviour[] dices;
    public List<PlayerMoving> playerControls;
    public List<GameObject> players;
    bool allDicesLaned;
    bool canBeReset ;
    bool cardActive = false;
    bool cardEndTurn = true;
    int cardType = 0;
    void Start()
    {
        //cardPrefab.transform.SetParent(GameObject.Find("Canvas").transform, false);
        //ShowCard(36);
        //GameObject.Find("Canvas").transform.GetChild(0).gameObject.Variable String

        players = new List<GameObject>();
        for (int n = 0; n < countOfPlayers; n++)
        {
            GameObject playerObject = Instantiate<GameObject>(playerPrefab);
            Vector3 pos = Vector3.zero;
            var modelRenderer = playerObject.GetComponent<Renderer>();
            pos.x = -19f;
            pos.z = 19f;
            pos.y = 0.7f;

            switch (n)
            {
                case 0:
                    pos.z += 1f;
                    pos.x += 1f;
                    modelRenderer.material.SetColor("_Color", Color.red);
                    break;
                case 1:
                    pos.z -= 1f;
                    pos.x += 1f;
                    modelRenderer.material.SetColor("_Color", Color.blue);
                    break;
                case 2:
                    pos.z += 1f;
                    pos.x -= 1f;
                    modelRenderer.material.SetColor("_Color", Color.yellow);
                    break;
                case 3:
                    pos.z -= 1f;
                    pos.x -= 1f;
                    modelRenderer.material.SetColor("_Color", Color.green);
                    break;
                default:
                    break;
            }

            playerObject.transform.position = pos;
            players.Add(playerObject);
            playerControls.Add(playerObject.GetComponentInChildren<PlayerMoving>());
            //playerObject.GetComponentInChildren<CardScript>(); //CardScript
        }

        float nextP = 0;
        foreach (var player in playerControls)
        {
            player.currentRoute = this.GetComponentInChildren<Route>();
            switch (nextP)
            {
                case 0:
                    player.posOnNodeZ = 1f;
                    player.posOnNodeX = 1f;
                    break;
                case 1:
                    player.posOnNodeZ = -1f;
                    player.posOnNodeX = 1f;
                    break;
                case 2:
                    player.posOnNodeZ = 1f;
                    player.posOnNodeX = -1f;
                    break;
                case 3:
                    player.posOnNodeZ = -1f;
                    player.posOnNodeX = -1f;
                    break;
                default:
                    player.posOnNodeZ = 0;
                    player.posOnNodeX = 0;
                    break;

            }
            nextP++;
        }
    }

    void Update()
    {
        
        if (canBeReset && !playerControls[i].GetIsMoving() && !cardActive && cardEndTurn)
        {
            cardEndTurn = false;
            CheckCard(playerControls[i].GetRoutePos());
            
        }
        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Space)) && canBeReset && !playerControls[i].GetIsMoving())
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (GameObject.Find("Canvas").transform.GetChild(1).gameObject.activeSelf == true)
                    ChangeCard(playerControls[i].GetRoutePos(), 2);
                else if (GameObject.Find("Canvas").transform.GetChild(2).gameObject.activeSelf == true)
                    ChangeCard(playerControls[i].GetRoutePos(), 1);
            }
            else if (Input.GetKeyDown(KeyCode.X))
                CheckCard(playerControls[i].GetRoutePos());
            else
            {
                if (cardActive)
                    CheckCard(playerControls[i].GetRoutePos());
                foreach (var dice in dices)
                    dice.Reset();
                canBeReset = false;
                Debug.Log("Dices has been Started/Reseted!");

                i++;
                if (i > countOfPlayers - 1)
                    i = 0;
                cardEndTurn = true;
            }
            
        }
        CheckTurn();
        
    }

    void CheckTurn()
    {
        DiceBehaviour firstDice = (DiceBehaviour)dices.GetValue(0);
        DiceBehaviour secondDice = (DiceBehaviour)dices.GetValue(1);

        foreach (var dice in dices)
            if (dice.name == "First Dice")
                firstDice = dice;
            else
                secondDice = dice;

            if (firstDice.GetLanded() && firstDice.GetThrown() && firstDice.diceValue > 0 &&
                secondDice.GetLanded() && secondDice.GetThrown()  && secondDice.diceValue > 0 && 
                    !canBeReset)
                allDicesLaned = true;
            else
                allDicesLaned = false;


        if (allDicesLaned)
        {
            totalDiceValue = 0;
            foreach (var dice in dices)
            {
                totalDiceValue += dice.diceValue;
                if (dice.diceValue == 0)
                {
                    allDicesLaned = false;
                    Debug.Log(totalDiceValue + " dices Failed");
                    dice.Reset();
                    break;
                }
            }
            Debug.Log(totalDiceValue + " dices has been Rolled!");

            allDicesLaned = false;
            canBeReset = true;

            //if (playerControls[i].GetIsMoving() && playerControls[i].steps <= 1)
                //ShowCard(playerControls[i].GetRoutePos());
            if (!playerControls[i].GetIsMoving())
                playerControls[i].steps = totalDiceValue;
            
            //if (!playerControls[i].GetIsMoving() && playerControls[i].steps == 0)
            //cardPrefab



        }
    }

    CardObject Load(int pos)
    {
        if (File.Exists(Application.dataPath + "/CardsData/" + pos.ToString() + ".txt"))
        {
            string cardString = File.ReadAllText(Application.dataPath + "/CardsData/" + pos.ToString() + ".txt");
            Debug.Log("Loaded " + cardString);

            CardObject cardObject = JsonUtility.FromJson<CardObject>(cardString);
            return cardObject;
        }
        return default;
    }

    void CheckCard(int id)
    {
        
        if (!cardActive)
        {
           
            if (id == 0 || id == 2 || id == 5 || id == 7 || id == 8 || id == 10 || id == 12 || id == 13 || id == 15 || id == 17 || id == 18 || id == 20 || id == 22 || id == 23 || id == 25 || id == 26
                || id == 28 || id == 30 || id == 31 || id == 33 || id == 36 || id == 38)
            {
                GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                cardActive = true;
                cardType = 1;
                GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                ShowPropertyCardFace(id);

            }
        }
        else
        {
            cardActive = false;
            GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(false);
            switch (cardType)
            {
                case 1:
                    GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            cardType = 0;
        }

    }
    void ChangeCard(int id, int childPos)
    {
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(2).gameObject.SetActive(false);
        switch (childPos) 
        {
            case 1:
                ShowPropertyCard(id);
                Debug.Log("Case ShowPropertyCard");
                break;
            case 2:
                ShowPropertyCardFace(id);
                Debug.Log("Case ShowPropertyCardFace");
                break;
            default:
                break;
        }

    }
    void ShowPropertyCard(int id)
    {
        GameObject propertyCard = GameObject.Find("Canvas").transform.GetChild(1).gameObject;

        propertyCard.SetActive(true);


        CardObject cardObject = Load(id);
        var cardTransform = propertyCard.gameObject.transform;
        GameObject header = cardTransform.transform.GetChild(1).gameObject;

        List<GameObject> variableStrings = new List<GameObject>();
        foreach (Transform child in cardTransform.GetChild(3).transform)
        {
            if (child.tag == "Variable String")
            {
                variableStrings.Add(child.gameObject);
                Debug.Log("Added " + child.name);
            }
        }
        Color Col;
        if (ColorUtility.TryParseHtmlString(cardObject.color, out Col))
            header.transform.GetComponent<Image>().color = Col;

        header.transform.GetChild(0).GetComponent<Text>().text = cardObject.titleDeed;


        variableStrings[0].GetComponent<Text>().text = cardObject.rent.ToString() + "$";
        variableStrings[1].GetComponent<Text>().text = cardObject.with1House.ToString() + "$";
        variableStrings[2].GetComponent<Text>().text = cardObject.with2House.ToString() + "$";
        variableStrings[3].GetComponent<Text>().text = cardObject.with3House.ToString() + "$";
        variableStrings[4].GetComponent<Text>().text = cardObject.with4House.ToString() + "$";
        variableStrings[5].GetComponent<Text>().text = cardObject.withHotel.ToString() + "$";
        variableStrings[6].GetComponent<Text>().text = cardObject.morgageValue.ToString() + "$";
        variableStrings[7].GetComponent<Text>().text = cardObject.housesCost.ToString() + "$";
        variableStrings[8].GetComponent<Text>().text = cardObject.housesCost.ToString() + "$ + 4 houses";
    }
    void ShowPropertyCardFace(int id)
    {
        GameObject propertyCard = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        
        propertyCard.SetActive(true);


        CardObject cardObject = Load(id);
        var cardTransform = propertyCard.gameObject.transform;
        GameObject color = cardTransform.transform.GetChild(1).gameObject;
        GameObject title = cardTransform.transform.GetChild(2).gameObject;
        GameObject price = cardTransform.GetChild(3).gameObject;
        Color Col;

        if (ColorUtility.TryParseHtmlString(cardObject.color, out Col))
            color.transform.GetComponent<Image>().color = Col;

        title.GetComponent<Text>().text = cardObject.titleDeed;

        price.GetComponent<Text>().text = cardObject.price.ToString() + "$";

    }
}

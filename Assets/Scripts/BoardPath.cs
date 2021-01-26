using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BoardPath : MonoBehaviour
{
    int i = 0;
    List<int> idsOfProp = new List<int> (){ 0, 2, 5, 7, 8, 10, 12, 13, 15, 17, 18, 20, 22, 23, 25, 26, 28, 30, 31, 33, 36, 38 };
    List<int> idsOfRr = new List<int>() { 4, 14, 24, 34 };


    public GameObject playerPrefab;
    public GameObject cardPrefab;

    int totalDiceValue;
    public int countOfPlayers;
    public DiceBehaviour[] dices;
    public List<PlayerMoving> playerControls;
    public List<GameObject> players;
    public List<PlayerScript> playerScripts;

    public List<PropertyStatus> propertiesStat = new List<PropertyStatus>();
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
            playerScripts.Add(playerObject.GetComponentInChildren<PlayerScript>());
            playerControls.Add(playerObject.GetComponentInChildren<PlayerMoving>());
            playerScripts[n].Player.ID = n+1;
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
        if (Input.GetKeyDown(KeyCode.M))
                Debug.Log("Player ["+ playerScripts[i].Player.ID+"]: "+ playerScripts[i].Player.Money+ "$");

        if (canBeReset && !playerControls[i].GetIsMoving() && !cardActive && cardEndTurn)
        {
            PropertyStatus property = propertiesStat.Find(x => x.id == playerControls[i].GetRoutePos());
            cardEndTurn = false;
            CheckCard(playerControls[i].GetRoutePos());
            if(property.owner != 0)
                PlayerRent(playerControls[i].GetRoutePos());


        }
        if ((Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Space)) && canBeReset && !playerControls[i].GetIsMoving())
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                PropertyStatus property = propertiesStat.Find(x => x.id == playerControls[i].GetRoutePos());

                if (property.isBuyed == false)
                    if (idsOfProp.Contains(playerControls[i].GetRoutePos()))
                    {
                        CardObject cardObject = JsonUtility.FromJson<CardObject>(Load(playerControls[i].GetRoutePos()));
                        property = playerScripts[i].BuyProperty(cardObject, property);
                    }
                    else if (idsOfRr.Contains(playerControls[i].GetRoutePos()))
                    {
                        RailroadObj rrObject = JsonUtility.FromJson<RailroadObj>(Load(playerControls[i].GetRoutePos()));
                        property = playerScripts[i].BuyRailroad(rrObject, property);
                    }

                Debug.Log(property.id + " owner: "+ property.owner);

            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                if (GameObject.Find("Canvas").transform.GetChild(1).gameObject.activeSelf == true || GameObject.Find("Canvas").transform.GetChild(3).gameObject.activeSelf == true)
                    ChangeCard(playerControls[i].GetRoutePos(), 2);
                else if (GameObject.Find("Canvas").transform.GetChild(2).gameObject.activeSelf == true || GameObject.Find("Canvas").transform.GetChild(4).gameObject.activeSelf == true)
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

    string Load(int pos)
    {
        if (File.Exists(Application.dataPath + "/CardsData/" + pos.ToString() + ".txt"))
        {
            string cardString = File.ReadAllText(Application.dataPath + "/CardsData/" + pos.ToString() + ".txt");
            Debug.Log("Loaded " + cardString);

            //CardObject cardObject = JsonUtility.FromJson<CardObject>(cardString);
            return cardString;
        }
        return default;
    }

    void CheckCard(int id)
    {
        
        if (!cardActive)
        {
           
            if (idsOfProp.Contains(id))
            {
                
                GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                cardActive = true;
                cardType = 1;
                GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                if (propertiesStat.Find(x => x.id == id) == default)
                    propertiesStat.Add(new PropertyStatus() { id = id, isBuyed = false, isMortgaged = false, owner = 0, housesCount =  0}); 
                ShowPropertyCardFace(id);

            }
            else if (idsOfRr.Contains(id))
            {
                GameObject.Find("Canvas").transform.GetChild(0).gameObject.SetActive(true);
                cardActive = true;
                cardType = 2;
                GameObject.Find("Canvas").transform.GetChild(3).gameObject.SetActive(false);
                if (propertiesStat.Find(x => x.id == id) == default)
                    propertiesStat.Add(new PropertyStatus() { id = id, isBuyed = false, isMortgaged = false, owner = 0, housesCount = 0});
                ShowRailroadCardFace(id);

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
                case 2:
                    GameObject.Find("Canvas").transform.GetChild(3).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(4).gameObject.SetActive(false);
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
        GameObject.Find("Canvas").transform.GetChild(3).gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.GetChild(4).gameObject.SetActive(false);
        switch (childPos) 
        {
            case 1:
                if (cardType == 1)
                {
                    ShowPropertyCard(id);
                    Debug.Log("Case ShowPropertyCard");
                }
                else if (cardType == 2)
                {
                    ShowRailroadCard(id);
                    Debug.Log("Case ShowRailroadCard");
                }
                break;
            case 2:
                if (cardType == 1)
                {
                    ShowPropertyCardFace(id);
                    Debug.Log("Case ShowPropertyCardFace");
                }
                else if (cardType == 2)
                {
                    ShowRailroadCardFace(id);
                    Debug.Log("Case ShowRailroadCardFace");
                }
                
                break;
            default:
                break;
        }

    }
    void ShowPropertyCard(int id)
    {
        GameObject propertyCard = GameObject.Find("Canvas").transform.GetChild(1).gameObject;

        propertyCard.SetActive(true);


        CardObject cardObject = JsonUtility.FromJson<CardObject>(Load(id));
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


        CardObject cardObject = JsonUtility.FromJson<CardObject>(Load(id));
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

    void ShowRailroadCard(int id)
    {
        GameObject railroadCard = GameObject.Find("Canvas").transform.GetChild(3).gameObject;

        railroadCard.SetActive(true);


        RailroadObj cardObject = JsonUtility.FromJson<RailroadObj>(Load(id));
        var cardTransform = railroadCard.gameObject.transform;
        GameObject title = cardTransform.transform.GetChild(1).gameObject;

        List<GameObject> variableStrings = new List<GameObject>();
        foreach (Transform child in cardTransform.GetChild(3).transform)
        {
            if (child.tag == "Variable String")
            {
                variableStrings.Add(child.gameObject);
                Debug.Log("Added " + child.name);
            }
        }
        title.GetComponent<Text>().text = cardObject.title;

        variableStrings[0].GetComponent<Text>().text = cardObject.rent.ToString() + "$";
        variableStrings[1].GetComponent<Text>().text = cardObject.if2rr.ToString() + "$";
        variableStrings[2].GetComponent<Text>().text = cardObject.if3rr.ToString() + "$";
        variableStrings[3].GetComponent<Text>().text = cardObject.if4rr.ToString() + "$";
        variableStrings[4].GetComponent<Text>().text = cardObject.morgageValue.ToString() + "$";

    }
    void ShowRailroadCardFace (int id)
    {
        GameObject railroadCard = GameObject.Find("Canvas").transform.GetChild(4).gameObject;

        railroadCard.SetActive(true);

        RailroadObj cardObject = JsonUtility.FromJson<RailroadObj>(Load(id));
        var cardTransform = railroadCard.gameObject.transform;
        GameObject title = cardTransform.transform.GetChild(1).gameObject;
        GameObject price = cardTransform.GetChild(2).gameObject;

        title.GetComponent<Text>().text = cardObject.title;
        price.GetComponent<Text>().text = cardObject.price.ToString() + "$";
    }
    public void PlayerRent(int id)
    {
        PropertyStatus property = propertiesStat.Find(x => x.id == playerControls[i].GetRoutePos());
        if (idsOfProp.Contains(id))
        {
            CardObject cardObject = JsonUtility.FromJson<CardObject>(Load(id));
            switch (property.housesCount) 
            {
                case 0:
                    {
                        playerScripts[i].Player.Money -= cardObject.rent;
                        playerScripts[property.owner - 1].Player.Money += cardObject.rent;
                        Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + cardObject.rent + " for renting " + cardObject.titleDeed);

                        break;
                    }
                case 1:
                    {
                        playerScripts[i].Player.Money -= cardObject.with1House;
                        playerScripts[property.owner - 1].Player.Money += cardObject.with1House;
                        Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + cardObject.with1House + " for renting " + cardObject.titleDeed);

                        break;
                    }
                case 2:
                    {
                        playerScripts[i].Player.Money -= cardObject.with2House;
                        playerScripts[property.owner - 1].Player.Money += cardObject.with2House;
                        Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + cardObject.with2House + " for renting " + cardObject.titleDeed);

                        break;
                    }
                case 3:
                    {
                        playerScripts[i].Player.Money -= cardObject.with3House;
                        playerScripts[property.owner - 1].Player.Money += cardObject.with3House;
                        Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + cardObject.with3House + " for renting " + cardObject.titleDeed);

                        break;
                    }
                case 4:
                    {
                        playerScripts[i].Player.Money -= cardObject.with4House;
                        playerScripts[property.owner - 1].Player.Money += cardObject.with4House;
                        Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + cardObject.with4House + " for renting " + cardObject.titleDeed);

                        break;
                    }
                case 5:
                    {
                        playerScripts[i].Player.Money -= cardObject.withHotel;
                        playerScripts[property.owner - 1].Player.Money += cardObject.withHotel;
                        Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + cardObject.withHotel + " for renting " + cardObject.titleDeed);

                        break;
                    }
                default: break;
            }


        }
        else if (idsOfRr.Contains(id))
        {
            RailroadObj rrObject = JsonUtility.FromJson<RailroadObj>(Load(id));
            int count = playerScripts[property.owner - 1].Player.railroads.Count;
            int rent = rrObject.rent;

            for (int i = 1; i <= count; i++)
                rent *= 2;
            playerScripts[i].Player.Money -= rent;
            playerScripts[property.owner - 1].Player.Money += rent;
            Debug.Log("Player " + playerScripts[i].Player.ID + " payed player " + property.owner + ": " + rent + " for renting " + rrObject.title);

        }

    }

}



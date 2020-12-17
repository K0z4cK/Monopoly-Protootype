using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPath : MonoBehaviour
{
    int i = 0;

    public GameObject playerPrefab;

    int totalDiceValue;
    public int countOfPlayers;
    public DiceBehaviour[] dices;
    public List<PlayerMoving> playerControls;
    public List<GameObject> players;
    bool allDicesLaned;
    bool canBeReset ;

    void Start()
    {
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
        
        if (Input.GetKeyDown(KeyCode.Space) && canBeReset && !playerControls[i].GetIsMoving())
        {
                foreach (var dice in dices)
                    dice.Reset();
            canBeReset = false;
            Debug.Log("Dices has been Started/Reseted!");

            i++;
            if (i > countOfPlayers - 1)
                i = 0;
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

            if (!playerControls[i].GetIsMoving())
                playerControls[i].steps = totalDiceValue;

            
        }
    }
}

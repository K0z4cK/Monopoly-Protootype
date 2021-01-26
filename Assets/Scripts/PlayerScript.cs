using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public PlayerObj Player = new PlayerObj();
    // Start is called before the first frame update
    void Start()
    {
        Player.Money = 500;
        Player.properties = new List<CardObject>();
        Player.propertiesStat = new List<PropertyStatus>();
        Player.railroads = new List<RailroadObj>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public PropertyStatus BuyProperty(CardObject cardObj, PropertyStatus propertySt)
    {
        if (Player.Money >= cardObj.price)
        {
            propertySt.isBuyed = true;
            propertySt.owner = Player.ID;
            Player.properties.Add(cardObj);
            Player.propertiesStat.Add(propertySt);
            Player.Money -= cardObj.price;
            Debug.Log("player " + Player.ID + " buyed " + cardObj.titleDeed + " for " + cardObj.price + "| Now he got: " + Player.Money) ;
            return propertySt;
        }
        return propertySt;
    }
    public PropertyStatus BuyRailroad(RailroadObj railroadObj, PropertyStatus propertySt)
    {
        if (Player.Money >= railroadObj.price)
        {
            propertySt.isBuyed = true;
            propertySt.owner = Player.ID;
            Player.railroads.Add(railroadObj);
            Player.propertiesStat.Add(propertySt);
            Player.Money -= railroadObj.price;
            Debug.Log("player " + Player.ID + " buyed " + railroadObj.title + " for " + railroadObj.price + "| Now he got: " + Player.Money);
            return propertySt;
        }
        return propertySt;
    }
}

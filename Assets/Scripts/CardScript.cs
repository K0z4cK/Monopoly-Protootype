using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CardScript : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Save()
    {
       //string m_Path = Application.dataPath;

       // //Output the Game data path to the console
       // Debug.Log("dataPath : " + m_Path);
       // CardObject cardObject = new CardObject 
       // { 
       //     id = 0,
       //     titleDeed = "Mediterranean Avenue",
       //     price = 60,
       //     rent = 2,
       //     with1House = 10,
       //     with2House = 30,
       //     with3House = 90,
       //     with4House = 160,
       //     withHotel = 250,
       //     morgageValue = 30,
       //     housesCost = 50
       // };
       // string json = JsonUtility.ToJson(cardObject);
       // File.WriteAllText(Application.dataPath + "/save.txt", json); 

    }

    public void Load(int pos)
    {
        if(File.Exists(Application.dataPath + "/CardsData/"+pos.ToString() +".txt"))
        {
            string cardString = File.ReadAllText(Application.dataPath + "/CardsData/" + pos.ToString() + ".txt");
            Debug.Log("Loaded " + cardString);

            CardObject cardObject = JsonUtility.FromJson<CardObject>(cardString);
        }
    }

    private class CardObject 
    {
        public int id;
        public string titleDeed;
        public int price;
        public int rent;
        public int with1House;
        public int with2House;
        public int with3House;
        public int with4House;
        public int withHotel;
        public int morgageValue;
        public int housesCost;
         
    }

}

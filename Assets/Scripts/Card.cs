using System.Collections.Generic;
using UnityEngine;

public class Card: MonoBehaviour
{
    public int id = -1;
    public int hp;
    public int atk;
    public int cost;
    public List<string> abils = new List<string>();
    public string[] cards;

    public void init(){
        cards = PlayerHelper.instance.cards;
        string[] card =  cards[id].Split(",");
        hp = int.Parse(card[0]);
        atk = int.Parse(card[1]);
        cost = int.Parse(card[2]);
    }

}
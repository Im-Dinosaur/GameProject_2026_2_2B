using System;

[Serializable]
public class Player
{
    public int player_id;
    public string username;
    public int level;
}

[Serializable]
public class InventoryItem
{
    public int item_id;
    public string name;
    public string description;
    public int value;
    public int quantity;
}

[Serializable]
public class Quest
{
    public int quest_id;
    public string title;
    public string description;
    public int reward_exp;
    public int reward_item_id;
    public string status;
}

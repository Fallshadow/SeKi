using Framework.AnimGraphs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimGraphContainer
{
    private static AnimGraphContainer instance;

    public static AnimGraphContainer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AnimGraphContainer();
            }
            return instance;
        }
    }

    private AnimGraphContainer()
    {
        players = new List<AnimGraphPlayer>(10);
    }

    private List<AnimGraphPlayer> players;

    public void Clear()
    {
        players.Clear();
    }

    public void Register(AnimGraphPlayer player)
    {
        players.Add(player);
    }

    public void Remove(AnimGraphPlayer player)
    {
        players.Remove(player);
    }

    public int Count
    {
        get
        {
            return players.Count;
        }
    }

    public AnimGraphPlayer Get(int index)
    {
        if (players == null || players.Count <= index)
        {
            Debug.Log($"Container is Empty {index}");
            return null;
        }
        
        return players[index];
    }
}

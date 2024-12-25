using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Если класс объявлен с модификатором sealed, то от этого класса нельзя наследовать и создавать производные классы.
// Например, следующий класс не допускает создание наследников
public sealed class GameEnviroment
{
    // Реализация класса как синглтон
    private static GameEnviroment instance;
    private List<GameObject> checkpoints = new List<GameObject>();
    public List<GameObject> Checkpoints
    {
        get
        {
            return checkpoints;
        }
    }

    public static GameEnviroment Singleton { 
        get 
        { 
            if (instance == null) 
            { instance = new GameEnviroment(); 
                instance.Checkpoints.AddRange(GameObject.FindGameObjectsWithTag("Checkpoint")); 
            } 
            return instance; 
        } 
    }

}

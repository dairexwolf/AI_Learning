using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Класс, нужный для управления WorldStates
/// </summary>
public sealed class GWorld
{
    // Синглтоны
    private static readonly GWorld instance = new GWorld();
    private static WorldStates world;

    // Конструктор
    static GWorld()
    {
        world = new WorldStates();
    }
    // Тоже конструктор
    private GWorld()
    {

    }

    /// <summary>
    /// Взять instance
    /// </summary>
    public static GWorld Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Взять WorldStates
    /// </summary>
    /// <returns></returns>
    public WorldStates GetWorld()
    {
        return world;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// �����, ������ ��� ���������� WorldStates
/// </summary>
public sealed class GWorld
{
    // ���������
    private static readonly GWorld instance = new GWorld();
    private static WorldStates world;

    // �����������
    static GWorld()
    {
        world = new WorldStates();
    }
    // ���� �����������
    private GWorld()
    {

    }

    /// <summary>
    /// ����� instance
    /// </summary>
    public static GWorld Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// ����� WorldStates
    /// </summary>
    /// <returns></returns>
    public WorldStates GetWorld()
    {
        return world;
    }
}

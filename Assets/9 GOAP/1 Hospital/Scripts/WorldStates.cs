using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    // Значение WorldState
    public string key;
    // То, что должно ассоциироваться к ключом. По сути, реализация "Словаря"
    public int value;
}

/// <summary>
/// Управляет состоянием кабинок
/// </summary>
public class WorldStates
{
    // Полностью заменяем уже сделаным словарем с уже готовыми методами
    public Dictionary<string, int> states;

    // В конструкторе инициализируем словарь
    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    // Ищем значение по ключу
    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    // Добавить состояние
    void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    // Удалить состояние
    void RemoveState(string key)
    {
        if(HasState(key))
        {
            states.Remove(key);
        }
        else
        {
            Debug.Log("RemoveState: key = " + key + " not found!");
        }
    }

    // Изменить состояние
    public void ModifyState(string key, int value)
    {
        if (HasState(key))
        {
            states[key] += value;
            if (states[key] <= 0)
                RemoveState(key);
        }
        else
            states.Add(key, value);
    }

    public void SetState(string key, int value)
    {
        if (HasState(key))
            states[key] = value;
        else
            AddState(key, value);
    }

    public Dictionary<string, int> GetStates()
    {
        return states;
    }
}

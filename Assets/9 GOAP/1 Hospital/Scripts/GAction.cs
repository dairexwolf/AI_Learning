using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GAction : MonoBehaviour
{
    public string actionName = "Action";
    
    /// <summary>
    /// Сколько стоит действие. Чем больше - тем действие дороже. Абстрактное объяснение - чем болье, тем больше НИП не любит этого делать
    /// </summary>
    public float cost = 1.0f;
    public GameObject target;
    public GameObject targetTag;
    public float duration = 0;
    // Будем испорльзовать их для удобаства, но потом закидывать в Словари
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;

    // В эти словари
    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effects;

    public WorldStates agentBeliefs;

    public bool running = false;

    // Инициализируем словари
    public GAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    // Берем агента и заполняем словари при запуске игры
    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if(preConditions != null)
            foreach(WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }
        if (afterEffects != null)
            foreach (WorldState w in preConditions)
            {
                effects.Add(w.key, w.value);
            }
    }

    // Является ли действие выполнимым?
    public bool IsAchievable()
    {
        return true;
    }

    // Может ли выполняется действие? Проверяет, есть ли требующиеся состояния для начала действия
    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach(KeyValuePair<string, int> p in preconditions)
        {
            if (!conditions.ContainsKey(p.Key))
                return false;
        }
        return true;
    }

    // Методы нужны для каких то проверок
    public abstract bool PrePerform();

    public abstract bool PostPerform();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

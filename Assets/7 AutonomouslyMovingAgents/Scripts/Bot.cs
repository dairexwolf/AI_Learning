using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{

    NavMeshAgent agent;
    public GameObject target;
    DrivePoliceman targetDrive;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        targetDrive = target.GetComponent<DrivePoliceman>();
    }

    // Update is called once per frame
    void Update()
    {
        Evade();
    }

    // ѕреследование цели по заданой координате
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);

    }

    // ”ход от цели (который находитс€ на заданой координате)
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    // ѕреследование цели
    void Pursue()
    {
        // ƒистанци€ до цели
        Vector3 targetDir = target.transform.position - this.transform.position;

        // –ассчитываем угол между вектором вперед у преследовател€ и преследуемого. ѕозвол€ет пон€ть, достаточно ли они идут параллельно, чтобы предсказывать траекторию
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // –ассчитываем угол между вектором движени€ преследующего и дистанцией дл€ цели. ѕозвол€ет пон€ть, спереди или сзади находитс€ цель
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        // ≈сли цель перестала двигатьс€, то преследователь следует за ней
        // » если цель сзади (toTarget > 90) и их пути не пересекаютс€ (relativeHeading < 20)
        if ((toTarget > 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }


        // ѕредсказание магнитуды, как дойти до цели
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // ѕреследование цели с предсказанием.
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    // ”клонение от цели
    void Evade()
    {
        // ƒистанци€ до цели
        Vector3 targetDir = target.transform.position - this.transform.position;

        // –ассчитываем угол между вектором вперед у преследовател€ и преследуемого. ѕозвол€ет пон€ть, достаточно ли они идут параллельно, чтобы предсказывать траекторию
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // –ассчитываем угол между вектором движени€ преследующего и дистанцией дл€ цели. ѕозвол€ет пон€ть, спереди или сзади находитс€ цель
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));
        
        // ≈сли цель перестала двигатьс€, то просто бежим
        // » если цель спереди (toTarget > 90) и их пути не пересекаютс€ (relativeHeading < 20)
        if ((toTarget < 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Flee(target.transform.position);
            return;
        }


        // ѕредсказание магнитуды, как дойти до цели
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // ”ход в сторону, использу€ предсказание
        Flee(target.transform.position + target.transform.forward * lookAhead * 2);
    }

}

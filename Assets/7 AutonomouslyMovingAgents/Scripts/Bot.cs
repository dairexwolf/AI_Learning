using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{

    NavMeshAgent agent;
    public GameObject target;
    DrivePoliceman targetDrive;

    // ���������� ��� ������� Wonder
    /// <summary>
    /// ������ �����
    /// </summary>
    public float wanderRadius = 10f;
    /// <summary>
    /// ��������� �� �������� ������������� �����
    /// </summary>
    public float wanderDistance = 20f;
    /// <summary>
    /// ��������, ������� �������������, ��������� ������ ��� ����� �������������� 
    /// </summary>
    public float wanderJitter = 1f;
    /// <summary>
    /// �����, ���� ������ ������� ���
    /// </summary>
    Vector3 wanderTarget = Vector3.zero;

    /// <summary>
    /// ����������, �� ������� ����� ���
    /// </summary>
    public float seeingDistance = 10f;

    // ������� ��� ��������� ����
    bool coolDown = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        targetDrive = target.GetComponent<DrivePoliceman>();

        // ��������� ����������������� ������ CanSeeMe()
        // StartCoroutine(LogCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!coolDown)
        {
            /*
             * ��� ������������� ������ TargetInRange()
             * if(!TargetInRange())
             *      Wander();
             */
            
            // ���� ����� ����
            if (CanSeeTarget())
            {
                // ���� ����� ���, �� ��� ��������
                if (CanSeeMe())
                {
                    // ��� ��������
                    CleverHide();
                    // ��������� ������� � true
                    coolDown = true;
                    // ����� ���� ������ �������� �������, ������� ��������� ������� � false.
                    // ������������� ������� ������������ ����� �����������
                    Invoke("BehaviorCooldown", 7);
                }
                // ���� ��� �� �����, �� �� ����������
                else
                    Pursue();
            }
            // ����� ������
            else
            {
                Wander();
            }
        }
    }

    /// <summary>
    /// ��������� ������� � false
    /// </summary>
    void BehaviorCooldown()
    {
        coolDown = false;
    }

    /// <summary>
    /// ����� �� �������� ���������
    /// </summary>
    /// <returns></returns>
    /// ���� �� ������������, ��� ��� � ���� ��� � ����������� ������
    bool TargetInRange()
    {
        if (Vector3.Distance(this.transform.position, target.transform.position) < seeingDistance)
            return true;
        return false;
    }

    /// <summary>
    /// ����� �� ��� ������ ���� ����. 
    /// </summary>
    /// <returns> �������� ��� �� ��� �� ����, � ���� �� ����� ����, �� true; ����� false</returns>
    bool CanSeeTarget()
    {
        // ������������� ���������� ��� ����
        RaycastHit rayCastInfo;
        // ��������� �� ����
        Vector3 rayToTarget = target.transform.position - this.transform.position;

        // ������������ ���� ����� �������� �������� ������������� � ���������� ��� ����. ��������� ������, ������� ��� ����� ��������� ����
        float lookAngle = Vector3.Angle(this.transform.forward, transform.TransformVector(rayToTarget));

        // � ���� ���� ������� (lookAngle < 90) � ��������� ������ 10
        if (lookAngle < 80 && rayToTarget.magnitude < seeingDistance)
        {
            // ������� ���
            if (Physics.Raycast(this.transform.position, rayToTarget, out rayCastInfo))
            {
                // ���� ��� ���������� � ����� (����� ���� ��� ��������)
                if (rayCastInfo.transform.gameObject.tag == "cop")
                    // ����� �����
                    return true;
            }
        }

        // ����� �� �����
        return false;

    }

    /// <summary>
    /// �������, ������� ����������, ����� �� ���� ������ ����� ����
    /// </summary>
    /// <returns>True, ���� �����; False, ���� �� �����</returns>
    bool CanSeeMe()
    {
        // ������������� ���������� ��� ����
        RaycastHit rayCastInfo;
        // ��������� �� ����
        Vector3 rayToMe = this.transform.position - target.transform.position;

        // ������������ ���� ����� �������� �������� ������������� � ���������� ��� ����. ��������� ������, ������� ��� ����� ��������� ����
        float lookAngle = Vector3.Angle(target.transform.forward, rayToMe);

        // � ���� ���� ������� (lookAngle < 90) � ��������� ������ 10
        if (lookAngle < 80 && rayToMe.magnitude < seeingDistance)
        {
            // ������� ���
            if (Physics.Raycast(target.transform.position, rayToMe, out rayCastInfo))
            {
                // ���� ��� ���������� � ����� (����� ���� ��� ��������)
                if (rayCastInfo.transform.gameObject.tag == "rob")
                    // ����� �����
                    return true;
            }
        }

        // ����� �� �����
        return false;
    }

    /// <summary>
    /// ������������� ���� �� ������� ����������
    /// </summary>
    /// <param name="location"> ���� ����� ����</param>
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);

    }

    // ���� �� ���� (������� ��������� �� ������� ����������)
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    /// <summary>
    /// ������������� ����
    /// </summary>
    void Pursue()
    {
        // ��������� �� ����
        Vector3 targetDir = target.transform.position - this.transform.position;

        // ������������ ���� ����� �������� ������ � �������������� � �������������. ��������� ������, ���������� �� ��� ���� �����������, ����� ������������� ����������
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // ������������ ���� ����� �������� �������� ������������� � ���������� ��� ����. ��������� ������, ������� ��� ����� ��������� ����
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        // ���� ���� ��������� ���������, �� �������������� ������� �� ���
        // � ���� ���� ����� (toTarget > 90) � �� ���� �� ������������ (relativeHeading < 20)
        if ((toTarget > 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }


        // ������������ ���������, ��� ����� �� ����
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // ������������� ���� � �������������.
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    /// <summary>
    /// ��������� �� ����
    /// </summary>
    void Evade()
    {
        // ��������� �� ����
        Vector3 targetDir = target.transform.position - this.transform.position;

        // ������������ ���� ����� �������� ������ � �������������� � �������������. ��������� ������, ���������� �� ��� ���� �����������, ����� ������������� ����������
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // ������������ ���� ����� �������� �������� ������������� � ���������� ��� ����. ��������� ������, ������� ��� ����� ��������� ����
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        // ���� ���� ��������� ���������, �� ������ �����
        // � ���� ���� ������� (toTarget > 90) � �� ���� �� ������������ (relativeHeading < 20)
        if ((toTarget < 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Flee(target.transform.position);
            return;
        }


        // ������������ ���������, ��� ����� �� ����
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // ���� � �������, ��������� ������������
        Flee(target.transform.position + target.transform.forward * lookAhead * 2);
    }

    /// <summary>
    /// ��������� ����
    /// </summary>
    void Wander()
    {
        // ������������� �����������, ���� ������ ���. �������� ��� ��� ������ �������
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
        // ����������� ��������� �������
        wanderTarget.Normalize();
        // � ������������� � ������ �����
        wanderTarget *= wanderRadius;

        // ����� ����� ���������� ����� ����� ����� � ��������� �����������
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        // ��������� �� � ���������� ����������, ����� ������� �� � NavMeshAgent
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        // � ����� ���� ����
        Seek(targetWorld);
    }

    /// <summary>
    /// ������� ���� ����������
    /// </summary> 
    void Hide()
    {
        // ������������� ����������
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        // ��������� ���� ��������, �� �������� ����� ����������
        int len = World.Instance.GetHidingSpots().Length;

        // ������� ���� ���������� � ����� ����������
        for (int i = 0; i < len; i++)
        {
            Vector3 hideDir = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            // �������� �� 10, ����� ������� ���������� �� �����������
            Vector3 hidePos = World.Instance.GetHidingSpots()[i].transform.position + hideDir.normalized * 10;

            if (Vector3.Distance(this.transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                dist = Vector3.Distance(this.transform.position, hidePos);
            }
        }

        Seek(chosenSpot);
    }

    /// <summary>
    /// ����� ����������� ��������� ��� ��� ������� ����������.
    /// </summary>
    void CleverHide()
    {
        // ������������� ����������
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        // ���������� �����������, ���� ������� ���� ���
        Vector3 chosenDir = Vector3.zero;
        // ��������� ����� �����������
        GameObject chosenGO = World.Instance.GetHidingSpots()[0];

        // ��������� ���� ��������, �� �������� ����� ����������
        int len = World.Instance.GetHidingSpots().Length;

        // ������� ���� ����������� � ����� ����������
        for (int i = 0; i < len; i++)
        {
            Vector3 hideDir = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            // �������� �� 10, ����� ������� ���������� �� �����������
            Vector3 hidePos = World.Instance.GetHidingSpots()[i].transform.position + hideDir.normalized * 10;

            // ���� ��������, �� ���������� ��� � ����� ������������ �������
            if (Vector3.Distance(this.transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                chosenDir = hideDir;
                chosenGO = World.Instance.GetHidingSpots()[i];
                dist = Vector3.Distance(this.transform.position, hidePos);

            }
        }

        // ����� ��������� ���������� �������
        Collider hideCol = chosenGO.GetComponent<Collider>();
        // ������� ������ ���� �� ���������� ����������� � ���������� ��� �������
        Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
        // �������������� ��������� ����
        RaycastHit info;
        float rayDistance = 100f;
        // ������� ���. ������� ��  Physics ���������� � ���, ��� �� ���������� ��� ����������, ����� �����
        hideCol.Raycast(backRay, out info, rayDistance);
        Debug.DrawRay(chosenSpot, -chosenDir.normalized * 10, Color.red);

        // ���������� ���� � �����, ���� ����� ���, ���� ��������� �������� � ����������� chosenDir.
        Seek(info.point + chosenDir.normalized * 5);
    }

    /// <summary>
    /// �������� ��� �������� ������ CanSeeMe
    /// </summary>
    /// <returns></returns>
    IEnumerator LogCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            Debug.Log(CanSeeMe());
        }
    }
}

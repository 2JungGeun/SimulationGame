using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemeyAI : MonoBehaviour
{
    public float range; //적이 타워를 탐색할 수 있는 범위
    public GameObject target; //타겟으로 설정할 타워 오브젝트

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        MoveAI();
    }

    void UpdateTarget()
    {
        if(target == null)
        {
            GameObject[] Towers = GameObject.FindGameObjectsWithTag("Tower");
            float shortDistance = Mathf.Infinity; // 가장 가까운 타워와의 거리, 처음에는 무한대로 설정 -> 무조건 Towers의 0번 인덱스는 target으로 설정하기 위해서 
            GameObject nearTower = null; //가장 가까운 타워 오브젝트 
            
            foreach(GameObject Tower in Towers)
            {
                float DistanceToTower = Vector2.Distance(transform.position, Tower.transform.position); // 타워와의 거리를 계산
                if(DistanceToTower < shortDistance) //타워와의 거리가 가장 가까운 타워와의 거리보다 작다면
                {
                    shortDistance = DistanceToTower; //가장 가까운 타워와의 거리를 변경
                    nearTower = Tower; // 가장 가까운 타워를 변경
                }
            }
            if(nearTower != null && shortDistance <= range) //가장 가까운 타워가 존재하고, 탐색 범위 내에 있다면
            {
                target = nearTower; //가장 가까운 타워를 타겟으로 함
                Debug.Log("Find tower!! Attack!!");
                Destroy(nearTower, 3f);
            }
            else //그 외에는 다시 찾음
            {
                target = null;
            }
        }
    }

    void MoveAI()
    {
        if (target != null) // 타겟이 설정되면
        {
            //타겟의 포지션으로 움직임
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * 1.5f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemeyAI : MonoBehaviour
{
    public float range; //���� Ÿ���� Ž���� �� �ִ� ����
    public GameObject target; //Ÿ������ ������ Ÿ�� ������Ʈ

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
            float shortDistance = Mathf.Infinity; // ���� ����� Ÿ������ �Ÿ�, ó������ ���Ѵ�� ���� -> ������ Towers�� 0�� �ε����� target���� �����ϱ� ���ؼ� 
            GameObject nearTower = null; //���� ����� Ÿ�� ������Ʈ 
            
            foreach(GameObject Tower in Towers)
            {
                float DistanceToTower = Vector2.Distance(transform.position, Tower.transform.position); // Ÿ������ �Ÿ��� ���
                if(DistanceToTower < shortDistance) //Ÿ������ �Ÿ��� ���� ����� Ÿ������ �Ÿ����� �۴ٸ�
                {
                    shortDistance = DistanceToTower; //���� ����� Ÿ������ �Ÿ��� ����
                    nearTower = Tower; // ���� ����� Ÿ���� ����
                }
            }
            if(nearTower != null && shortDistance <= range) //���� ����� Ÿ���� �����ϰ�, Ž�� ���� ���� �ִٸ�
            {
                target = nearTower; //���� ����� Ÿ���� Ÿ������ ��
                Debug.Log("Find tower!! Attack!!");
                Destroy(nearTower, 3f);
            }
            else //�� �ܿ��� �ٽ� ã��
            {
                target = null;
            }
        }
    }

    void MoveAI()
    {
        if (target != null) // Ÿ���� �����Ǹ�
        {
            //Ÿ���� ���������� ������
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * 1.5f);
        }
    }
}

using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMagic : PlayerController
{
    Vector3 mousePosition;

    public GameObject skillRangeA;
    public GameObject skillRangeS;

    float skillCooltime_A = 0;
    float time_A = 5;
    float skillCooltime_S = 0;
    float time_S = 5;
    float skillCooltime_D = 0;
    float time_D = 5;

    void Update()
    {
        PlayerSkillA();
    }

    void PlayerSkillA()
    {
        float maxRange = 10f;
        if (Input.GetKey(KeyCode.A))
        {
            GetMousePosition();

            skillRangeA.SetActive(true);
            Vector3 direction = mousePosition - transform.position;
            float distance = direction.magnitude;
            distance = Mathf.Min(distance, maxRange);
            skillRangeA.transform.position = transform.position + direction.normalized * (distance / 2);
            skillRangeA.transform.localScale = new Vector3(1f, 0.1f, distance);

            skillRangeA.transform.rotation = Quaternion.LookRotation(direction);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            skillRangeA.SetActive(false);
        }
    }

    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            mousePosition = ray.GetPoint(distance);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    private new Transform transform;
    private new Camera camera;

    private Ray ray;

    private int speed = 3;
    private bool isMoving = false;

    void Start()
    {
        transform = GetComponent<Transform>();
        camera = Camera.main;
        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }

    void Update()
    {
        Move();
    }

    Vector3 targetPos;
    void Move() // 움직이는 애니메이션 추가
    {
        if (Input.GetMouseButtonDown(1))
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Ground")))
            {
                targetPos = hit.point;
                targetPos.y = transform.position.y;
                isMoving = true;
            }
        }
        if (isMoving)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.1f);

            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    private new Transform transform;
    private new Camera camera;

    private Ray ray;

    private Vector3 receivePos;
    private Quaternion receiveRot;
    public float damping = 10.0f;

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
        if (pv.IsMine)
        {
            Move();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);

            transform.rotation = Quaternion.Slerp(transform.rotation, receiveRot, Time.deltaTime * damping);
        }
    }

    Vector3 targetPos;
    void Move() // �����̴� �ִϸ��̼� �߰�
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // �ڽ��� ���� ĳ�����ΰ���ڽ��ǵ����͸��ٸ���Ʈ��ũ�������Լ۽�
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    private new Camera camera;
    public Animator animator;
    public Rigidbody rigidbody;

    public Ray ray;

    private Vector3 receivePos;
    private Quaternion receiveRot;
    public float damping = 10.0f;

    public float playerHp = 100;
    public float playerAtk = 10;
    public float defaultSpped = 3;
    public float playerSpeed = 3;

    public bool isMoving = false;
    public bool isStun = false;
    public bool isCasting = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }

    public virtual void Update()
    {
        if (pv.IsMine)
        {
            ray = camera.ScreenPointToRay(Input.mousePosition);
            animator.SetBool("IsRun", isMoving);
            if (!isStun && !isCasting)
                Move();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * damping);
            transform.rotation = Quaternion.Slerp(transform.rotation, receiveRot, Time.deltaTime * damping);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            OnPlayerStun(2f);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            OnPlayerKnockBack(transform);
    }

    Vector3 targetPos;
    void Move()
    {
        if (Input.GetMouseButtonDown(1))
        {
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
            transform.position = Vector3.MoveTowards(transform.position, targetPos, playerSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
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

    // 플레이어에게 피해를 입힐 때 호출
    public void OnHitPlayer(float _damage)
    {
        playerHp -= _damage;
    }

    #region Player Crowd Control

    private Coroutine speedCoroutine;
    public void OnPlayerSpeedDown(float _time, float _moveSpeed)
    {
        if (playerSpeed > _moveSpeed)
        {
            if (speedCoroutine != null)
                StopCoroutine(speedCoroutine);

            speedCoroutine = StartCoroutine(PlayerSpeedDown(_time, _moveSpeed));
        }
        else
        {
            if (speedCoroutine != null)
                StopCoroutine(speedCoroutine);

            speedCoroutine = StartCoroutine(PlayerSpeedDown(_time, playerSpeed));
        }
    }

    IEnumerator PlayerSpeedDown(float _time, float _moveSpeed) // 이동 속도 감소 처리
    {
        playerSpeed = _moveSpeed;
        yield return new WaitForSeconds(_time);
        playerSpeed = defaultSpped;
    }

    private Coroutine stunCoroutine;
    public void OnPlayerStun(float _time)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(PlayerStun(_time));
    }

    IEnumerator PlayerStun(float _time) // 스턴 상태 처리
    {
        isMoving = false;
        isStun = true;
        yield return new WaitForSeconds(_time);
        isStun = false;
    }

    public void OnPlayerKnockBack(Transform _transform)
    {
        Vector3 vec = transform.position - _transform.position;
        vec.x = Mathf.Sign(vec.x) * 5;
        vec.y = 3f;
        vec.z = Mathf.Sign(vec.z) * 5;
        rigidbody.AddForce(vec, ForceMode.Impulse);
        OnPlayerStun(0.5f);
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    private PostProcessVolume postProcessVolume;
    private Vignette vignetteEffect;
    private float vignetteValue = 0f;

    private new Camera camera;
    public Animator animator;
    public Rigidbody rigidbody;

    public Ray ray;
    public Vector3 mousePosition;

    private Vector3 receivePos;
    private Quaternion receiveRot;
    private Vector3 receiveMousePos;
    private bool receiveMoving;

    public float playerMaxHp = 100;
    public float playerHp = 100;
    public float playerAtk = 10;
    public float defaultSpped = 3;
    public float playerSpeed = 3;

    public bool isMoving = false;
    public bool isStun = false;
    public bool isCasting = false;
    public bool isDie = false;

    public virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        postProcessVolume = FindObjectOfType<PostProcessVolume>();
        postProcessVolume.profile.TryGetSettings(out vignetteEffect);
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
        animator.SetBool("IsRun", isMoving);
        if (pv.IsMine)
        {
            if (!isDie)
            {
                mousePosition = GetMousePosition();

                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);

                vignetteEffect.intensity.value = Mathf.Lerp(vignetteEffect.intensity.value, vignetteValue, Time.deltaTime * 5f);

                if (aniInfo.IsName("A") || aniInfo.IsName("S") || aniInfo.IsName("D"))
                    isCasting = true;
                else
                    isCasting = false;

                if (!isStun && !isCasting)
                    Move();
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Slerp(transform.rotation, receiveRot, Time.deltaTime * 10);
            mousePosition = receiveMousePos;
            isMoving = receiveMoving;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            OnPlayerStun(2f);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            OnPlayerKnockBack(transform);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            OnPlayerBlind();
    }

    Vector3 targetPos;
    void Move()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, 1 << LayerMask.NameToLayer("Ground")))
            {
                targetPos = hit.point;
                targetPos.y = 0.25f;
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

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(mousePosition);
            stream.SendNext(isMoving);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
            receiveMousePos = (Vector3)stream.ReceiveNext();
            receiveMoving = (bool)stream.ReceiveNext();
        }
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));

        float distance;

        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        else { return Vector3.zero; }
    }

    public Vector3 GetSkillRange(float _range)
    {
        Vector3 direction = mousePosition - transform.position;
        float distance = direction.magnitude;

        if (distance > _range)
        {
            direction = direction.normalized;
            return transform.position + direction * _range;
        }
        else
            return mousePosition;
    }

    // 플레이어에게 피해를 입힐 때 호출
    public void OnHitPlayer(float _damage)
    {
        playerHp -= _damage;
        if (playerHp < 0)
        {
            isDie = true;
            pv.RPC("PlayAnimation", RpcTarget.All, "Die");
        }
    }

    public void StandUp()
    {
        pv.RPC("PlayAnimation", RpcTarget.All, "StandUp");
    }

    public void ReSpawn()
    {
        isDie = false;
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

    public void OnPlayerBlind()
    {
        StartCoroutine(PlayerBlind());
    }

    IEnumerator PlayerBlind()
    {
        vignetteValue = 1f;
        yield return new WaitForSeconds(6f);
        vignetteValue = 0f;
    }

    #endregion

    [PunRPC]
    public void PlayAnimation(string _ani)
    {
        animator.SetTrigger(_ani);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView pv;
    private CinemachineVirtualCamera virtualCamera;

    private PostProcessVolume postProcessVolume;
    private Vignette vignetteEffect;
    private float vignetteValue = 0.1f;

    private new Camera camera;
    public PlayerUi playerUi;
    public Skilltower skilltower;
    public Animator animator;
    public Rigidbody rigidbody;

    public Ray ray;
    public Vector3 mousePosition;

    private Vector3 receivePos;
    private Quaternion receiveRot;
    private Vector3 receiveMousePos;
    private bool receiveMoving;
    private bool receiveDie;

    public GameObject SkillEffect;

    public Sprite IconImage;
    public Sprite[] skillImages;
    // 방어막 이미지 추가

    public float playerLv = 1;
    public float playerMaxHp = 100;
    public float playerHp = 100;
    public float playerAtk = 15;
    public float defaultSpped = 3;
    public float playerSpeed = 3;
    public int elementalCode = 0;
    public float elementalSetCoolTime = 0;

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
        playerUi = FindObjectOfType<PlayerUi>();
        skilltower = FindObjectOfType<Skilltower>();
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;

            PlayerUiSetting();

            playerUi.playerLv = playerLv;
            playerUi.playerHp = playerHp;
            playerUi.playerMaxHp = playerMaxHp;
        }
    }

    int playerCode = 0;
    public virtual void Update()
    {
        animator.SetBool("IsRun", isMoving);
        if (pv.IsMine)
        {
            if (elementalSetCoolTime >= 0) { elementalSetCoolTime -= Time.deltaTime; }
            if (!isDie)
            {
                mousePosition = GetMousePosition();

                AnimatorStateInfo aniInfo = animator.GetCurrentAnimatorStateInfo(0);

                vignetteEffect.intensity.value = Mathf.Lerp(vignetteEffect.intensity.value, vignetteValue, Time.deltaTime * 5f);

                if (aniInfo.IsName("A") || aniInfo.IsName("S") || aniInfo.IsName("D") || aniInfo.IsName("Pray") || aniInfo.IsName("Pray_Loop"))
                    isCasting = true;
                else
                    isCasting = false;

                if (!isStun && !isCasting)
                    Move();

                if (Input.GetKeyDown(KeyCode.Alpha1))
                    MixSkillCasting(0, elementalCode);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    MixSkillCasting(1, elementalCode);
                UseMixSkills();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    PlayerController[] allPlayer = FindObjectsOfType<PlayerController>();
                    virtualCamera.Follow = allPlayer[playerCode].transform;
                    virtualCamera.LookAt = allPlayer[playerCode].transform;
                    playerCode++;

                    playerUi.playerLvText.text = playerCode + " Tlqkf " + allPlayer.Length;

                    if (playerCode >= allPlayer.Length)
                        playerCode = 0;
                }
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * 10);
            transform.rotation = Quaternion.Slerp(transform.rotation, receiveRot, Time.deltaTime * 10);
            mousePosition = receiveMousePos;
            isMoving = receiveMoving;
            isDie = receiveDie;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
            OnPlayerStun(2f);
        if (Input.GetKeyDown(KeyCode.Keypad1))
            OnPlayerKnockBack(transform);
        if (Input.GetKeyDown(KeyCode.Keypad2))
            OnPlayerBlind();
    }

    public void MixSkillCasting(int _slot, int _set)
    {
        if (skilltower.canAttack)
        {
            if (skilltower.elementalSet[0] == _set || skilltower.elementalSet[1] == _set)
            {
                pv.RPC("PlayAnimation", RpcTarget.All, "StopWait");
                if (skilltower.elementalSet[0] == _set)
                    skilltower.pv.RPC("SetingElemental", RpcTarget.All, 0, 0);
                else
                    skilltower.pv.RPC("SetingElemental", RpcTarget.All, 1, 0);
                pv.RPC("PlaySkillEffect", RpcTarget.All, false);
            }
            else if (skilltower.elementalSet[_slot] == 0 && skilltower.cooldownTime <= 0)
            {
                if (elementalSetCoolTime <= 0)
                {
                    pv.RPC("PlayAnimation", RpcTarget.All, "StartWait");
                    skilltower.pv.RPC("SetingElemental", RpcTarget.All, _slot, _set);
                    pv.RPC("PlaySkillEffect", RpcTarget.All, true);
                    elementalSetCoolTime = 3f;
                }
            }
            else
            {
                return;
            }
        }
    }

    public void UseMixSkills()
    {
        if (skilltower.canUseSkill && skilltower.cooldownTime <= 0)
        {
            switch (skilltower.mixSkillNum)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    if (Input.GetKeyDown(KeyCode.Space))
                        skilltower.laserRange.SetActive(true);
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        skilltower.laserRange.SetActive(false);
                        skilltower.pv.RPC("UseMasterLazer", RpcTarget.All, null);
                    }
                    break;
                case 3:
                    if (Input.GetKey(KeyCode.Space))
                    {
                        skilltower.meteorRange.SetActive(true);
                        skilltower.meteorRange.transform.position = GetSkillRange(20f);
                    }
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        skilltower.meteorRange.SetActive(false);
                        skilltower.pv.RPC("UseMasterMeteor", RpcTarget.All, GetSkillRange(20f));
                    }
                    break;
                case 4:
                    if (Input.GetKey(KeyCode.Space))
                    {
                        skilltower.barricadeRange.SetActive(true);
                        skilltower.barricadeRange.transform.position = new Vector3(GetSkillRange(20f).x, GetSkillRange(20f).y, 0);
                    }
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        skilltower.barricadeRange.SetActive(false);
                        skilltower.pv.RPC("UseMasterBarricade", RpcTarget.All, skilltower.barricadeRange.transform.position);
                    }
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }

        }
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
            stream.SendNext(isDie);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
            receiveMousePos = (Vector3)stream.ReceiveNext();
            receiveMoving = (bool)stream.ReceiveNext();
            receiveDie = (bool)stream.ReceiveNext();
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
        if (pv.IsMine)
        {
            playerHp -= _damage;
            playerUi.playerHp = playerHp;
            playerUi.playerMaxHp = playerMaxHp;

            pv.RPC("PlaySkillEffect", RpcTarget.All, false);
            if (playerHp <= 0)
            {
                isDie = true;
                pv.RPC("PlayAnimation", RpcTarget.All, "Die");
            }
        }
    }

    [PunRPC]
    public void StandUp()
    {
        if (pv.IsMine)
            pv.RPC("PlayAnimation", RpcTarget.All, "StandUp");
    }

    [PunRPC]
    public void PlayerRecovery()
    {
        if (pv.IsMine)
        {
            playerHp += 50;
            if (playerHp > playerMaxHp)
                playerHp = playerMaxHp;
            playerUi.playerHp = playerHp;
            playerUi.playerMaxHp = playerMaxHp;
        }
    }

    public void ReSpawn()
    {
        if (pv.IsMine)
        {
            isDie = false;
            playerHp = playerMaxHp;
            playerUi.playerHp = playerHp;
            playerUi.playerMaxHp = playerMaxHp;

            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
    }

    public void PlayerUiSetting()
    {
        playerUi.StartUISetting(IconImage, skillImages);
    }
    [PunRPC]
    public void PlayerLvUp()
    {
        playerLv++;
        playerUi.playerLv = playerLv;
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
        vignetteValue = 0.1f;
    }

    #endregion

    [PunRPC]
    public void PlayAnimation(string _ani)
    {
        animator.SetTrigger(_ani);
    }

    [PunRPC]
    public void PlaySkillEffect(bool _bool)
    {
        SkillEffect.SetActive(_bool);
    }
}

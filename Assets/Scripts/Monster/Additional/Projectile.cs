using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviourPunCallbacks, IPunObservable
{
    private Vector3 targetPosition; // 목표 위치
    private float launchHeight = 5.5f; // 포물선의 최대 높이
    private float flightDuration;
    private int MonsterDmg;

    // 초기화 메서드: 목표 위치와 투사체 속도를 설정
    public void Initialize(Vector3 target, float projectileSpeed, int damage)
    {
        targetPosition = target; // 목표 위치 설정
        // targetPosition.y += 2;
        flightDuration = projectileSpeed; // 입력받은 속도로 초기화
        MonsterDmg = damage;
        StartCoroutine(Fly()); // 비행 코루틴 시작
    }

    // 비행 코루틴: 목표로 날아가는 투사체의 경로를 정의
    private IEnumerator Fly()
    {
        Vector3 startPosition = transform.position; // 시작 위치
        float startTime = Time.time; // 비행 시작 시간 기록

        // 비행 시간 동안 반복
        while (true)
        {
            float elapsedTime = Time.time - startTime; // 경과 시간 계산

            // 총 비행 시간이 초과하면 종료
            if (elapsedTime > flightDuration)
            {
                break;
            }

            // 진행 비율 t를 계산
            float t = elapsedTime / flightDuration;

            // 포물선의 높이 계산: t에 따라 포물선의 높이를 조정
            float height = Mathf.Sin(t * Mathf.PI) * launchHeight; // 포물선의 높이

            // 새로운 위치 계산: 시작 위치와 목표 위치 사이의 선형 보간
            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t);
            newPosition.y += height; // 포물선 높이 추가

            transform.position = newPosition; // 투사체의 위치 업데이트

            yield return null; // 다음 프레임까지 대기
        }

        // 목표 위치에 도달했을 때 투사체 위치 업데이트
        transform.position = targetPosition;

        photonView.RPC("DestroyObj", RpcTarget.All);
    }
    
    [PunRPC]
    public void DestroyObj()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 이 클라이언트에서 몬스터 위치와 회전을 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 다른 클라이언트에서 몬스터 위치와 회전을 수신
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerScript = other.GetComponent<PlayerController>();
            if (playerScript != null)
            {
                playerScript.OnHitPlayer(MonsterDmg);
            }
        }
        if (other.CompareTag("skilltower"))
        {
            Skilltower skillTowerScript = other.GetComponent<Skilltower>(); 
            if (skillTowerScript != null)
            {
                skillTowerScript.TakeDamage(MonsterDmg);
            }
        }
        if (other.CompareTag("Castle"))
        {
            Wall castleScript = other.GetComponent<Wall>();
            if (castleScript != null)
            {
                castleScript.TakeDamage(MonsterDmg);
            }
        }
        if (other.CompareTag("turret"))
        {
            Turret towerScript = other.GetComponent<Turret>();
            if (towerScript != null)
            {
                towerScript.TakeDamage(MonsterDmg);
            }
        }
    }
}

using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 targetPosition; // 목표 위치
    private float launchHeight = 5f; // 포물선의 최대 높이
    private float flightDuration = 2; // 비행에 걸리는 총 시간
    private int monsterDamage;

    private IceSpirit iceSpirit;

    void Start()
    {
        GameObject iceSpiritObj = GameObject.FindWithTag("IceSpirit");
        iceSpirit = iceSpiritObj.GetComponent<IceSpirit>();
    }

    // 초기화 메서드: 목표 위치와 투사체 속도를 설정
    public void Initialize(Vector3 target, float projectileSpeed, int damage)
    {
        targetPosition = target; // 목표 위치 설정
        flightDuration = projectileSpeed; // 입력받은 속도로 초기화
        monsterDamage = damage;
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

        Destroy(gameObject); // 목표에 도달 후 투사체 삭제
    }

    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.tag == "Player")
        {
            iceSpirit.AttackPlayer(monsterDamage);
        }
    }
}

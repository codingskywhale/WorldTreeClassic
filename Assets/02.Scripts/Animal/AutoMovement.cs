using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class AutoMovement : MonoBehaviour
{
    // 자동 움직임을 구현 (StateMachine 추후 적용하면 좋을듯?)
    // AutoMovement를 상속받는 Air, Ground 타입 만들어보자.
    Vector3 targetPos;
    public float moveDeceleration = 3f;
    public float maxMoveX = 6f;
    public float minMoveZ = 5f;
    public float maxMoveZ = 20f;
    Camera cam;

    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        cam = Camera.main;
        targetPos = SetRandomDestination();
    }

    private void Start()
    {
        StartCoroutine(ApplyChangeDirection());
    }

    private void FixedUpdate()
    {
        Move(targetPos);
    }
    
    //추후 날아다니는 아이들이 추가될 수도 있음.
    protected virtual Vector3 SetRandomDestination()
    {
        Vector3 moveVec;
        if (Random.Range(0f, 1f) > 0.5)
        {
            float randomX = Random.Range(-maxMoveX, maxMoveX);
            float randomZ = Random.Range(minMoveZ, maxMoveZ);
            moveVec = new Vector3(randomX, 0.5f, randomZ);
        }
        else
            moveVec = transform.position;


        return moveVec;
    }

    public void Move(Vector3 targetPos)
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, moveDeceleration);
    }

    IEnumerator ApplyChangeDirection()
    {
        while (true)
        {
            targetPos = SetRandomDestination();
            yield return new WaitForSeconds(1f);    
        }
    }

    // 화면에서 완전히 벗어나지 않도록 설정
    public bool CheckOutofMap()
    {
        Vector3 objectPosition = transform.position;

        // 오브젝트의 중심점을 뷰포트 좌표로 변환합니다.
        Vector3 viewportPosition = cam.WorldToViewportPoint(objectPosition);

        return viewportPosition.x > 1f || viewportPosition.x < 0f;
    }
}

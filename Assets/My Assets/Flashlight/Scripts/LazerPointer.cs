using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    [Header("Refs")]
    public Transform muzzle;                    // 빔 출발 지점
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;             // XR Grab 인터랙터
    public LayerMask hitMask;                   // Ray가 충돌할 레이어들
    public LineRenderer lr;                     // 빔 표시용 라인렌더러

    [Header("Beam Settings")]
    public int maxBounces = 1;                  // 반사 최대 횟수
    public float maxDistance = 30f;             // 최대 거리
    public float width = 0.01f;                 // 라인 굵기
    public bool startOn = true;                 // 기본 켜진 상태 여부
    public float stabilizeFPS = 60f;            // 업데이트 빈도 제한

    private bool _on;                           // 빔 On/Off 상태
    private float _accum;                       // FPS 제한용 누적 시간
    private readonly List<Vector3> _points = new(); // 빔 경로 저장

    void Awake()
    {
        if (!grab) grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (!lr) lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.startWidth = lr.endWidth = width;

        // 트리거(Activate)로 On/Off 토글
        grab.activated.AddListener(_ => Toggle());
        grab.deactivated.AddListener(_ => Toggle());
        _on = startOn;
    }

    void OnDisable()
    {
        grab.activated.RemoveAllListeners();
        grab.deactivated.RemoveAllListeners();
        lr.positionCount = 0;
    }

    // 빔 토글
    void Toggle() => _on = !_on;

    void Update()
    {
        if (!_on || muzzle == null)
        {
            lr.positionCount = 0;
            return;
        }

        // 프레임 제한으로 부드럽게 유지
        _accum += Time.deltaTime;
        if (_accum < 1f / Mathf.Max(10f, stabilizeFPS))
            return;
        _accum = 0f;

        CastBeam();
    }

    // 실제 Raycast 수행
    void CastBeam()
    {
        _points.Clear();
        Vector3 origin = muzzle.position;
        Vector3 dir = muzzle.up;

        _points.Add(origin);

        for (int i = 0; i < maxBounces; i++)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, hitMask, QueryTriggerInteraction.Collide))
            {
                _points.Add(hit.point);

                // LaserSensor에 맞았으면 신호 전달
                var sensor = hit.collider.GetComponentInParent<LaserSensor>();
                if (sensor != null)
                {
                    sensor.RegisterHit();
                    Debug.Log("센서에 레이저 맞음");
                    break;
                }

                // 반사면이면 방향 반전
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("LaserReflect"))
                {
                    origin = hit.point + hit.normal * 0.001f; // 자기충돌 방지용 offset
                    dir = Vector3.Reflect(dir, hit.normal);
                    continue;
                }

                // 일반 벽이면 종료
                break;
            }
            else
            {
                // 아무것도 안 맞으면 직진
                _points.Add(origin + dir * maxDistance);
                break;
            }
        }

        // 라인렌더러에 점들 설정
        lr.positionCount = _points.Count;
        lr.SetPositions(_points.ToArray());
    }
}

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class EndingCredit : MonoBehaviour
{
    private RectTransform rectTransform;
    public GameObject[] Creators;

    private GameObject ground;
    private GameObject init;
    void Awake()
    {
        Time.timeScale = 1f;
        rectTransform = gameObject.GetComponent<RectTransform>();
        Debug.Log("Starting EndingCredit script");
        ground = GameObject.Find("Ground");
        init = GameObject.Find("Inits");

        // ground의 경계 정보 가져오기
        Renderer groundRenderer = ground.GetComponent<Renderer>();
        Vector3 groundMin = groundRenderer.bounds.min;  // ground의 왼쪽 아래 좌표
        Vector3 groundMax = groundRenderer.bounds.max;  // ground의 오른쪽 위 좌표

        int i = 1;
        foreach (var creator in Creators)
        {
            Vector3 topLeftPosition = new Vector3(groundMin.x + i, groundMax.y-0.5f, groundMin.z);
            GameObject createdObject = Instantiate(creator, topLeftPosition, Quaternion.identity, init.transform);

            // 부모로부터 분리 (초기 위치 설정 후)
            createdObject.transform.SetParent(null);

            // 생성된 오브젝트를 좌우 반전
            Vector3 localScale = createdObject.transform.localScale;
            localScale.x = -localScale.x; // x축을 반전
            createdObject.transform.localScale = localScale;

            Animator animator = createdObject.transform.GetChild(0).GetComponent<Animator>();
            if (animator != null)
            {
                // 애니메이션 시작
                animator.speed = 0.6f;
                animator.Play("1_Run");
            }

            i++;
        }
    }

    private void Start() {
    }

    private void Update()
    {
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform is not assigned!");
            return;
        }

        Debug.Log($"Current Position: {rectTransform.anchoredPosition.y}");

        if (rectTransform.anchoredPosition.y >= 10000f)
        {
            Debug.Log("Position reached 3500f, loading Title scene.");
            SceneManager.LoadScene("Title");
        }

        Vector2 newPosition = rectTransform.anchoredPosition;
        newPosition.y += 100f * Time.deltaTime;
        rectTransform.anchoredPosition = newPosition;
    }
    void OnEnable()
    {
        Debug.Log("EndingCredit script enabled");
    }

    void OnDisable()
    {
        Debug.Log("EndingCredit script disabled");
    }
 
}
using System;
using UnityEngine;

public class AllySwap : MonoBehaviour {
	public LayerMask clickableLayer; // 감지할 레이어 설정
	private GameObject unit1;
	private GameObject unit2;
	private bool isMoving = false;
	private Animator animator1;
	private Animator animator2;
	private AllyScan allyScan1;
	private AllyScan allyScan2;
	private AllyScan unitInfo1;
	private AllyScan unitInfo2;

	private GameObject highlight;
	private Vector3 targetPosition1;
	private Vector3 targetPosition2;

	private float originTime;
	
	public GameObject playerObjCircle;
	private GameObject playerObjCircle1;
	private GameObject playerObjCircle2;
	
	private void Awake()
	{
		playerObjCircle1 = Instantiate(playerObjCircle, transform.position, Quaternion.identity);
		playerObjCircle2 = Instantiate(playerObjCircle, transform.position, Quaternion.identity);
		playerObjCircle1.SetActive(false);
		playerObjCircle2.SetActive(false);
	}
	void Update() {
		
		// 게임이 일시 정지된 상태인지 확인
		if (StageC.Instance != null && StageC.Instance.isGamePaused)
		{
			return; // 게임이 일시 정지된 상태에서는 클릭을 무시함
		}
		
		if (Input.GetMouseButtonDown(0) && !isMoving) // 마우스 왼쪽 버튼 클릭 확인 및 이동 중인지 확인
		{
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

			Collider2D collider = Physics2D.OverlapPoint(mousePosition2D, clickableLayer); // 특정 레이어만 감지

			if (collider != null) {
				GameObject clickedObject = collider.gameObject;
				CustomLogger.Log("Clicked on object: " + clickedObject.name, "blue");

				if (unit1 == null)
				{
					originTime = Time.timeScale;
					unit1 = clickedObject;
					if (unit1 != null)
					{
						Time.timeScale = 0.5f;
						playerObjCircle1.SetActive(true);
						playerObjCircle1.transform.position = unit1.transform.position;
					}
					CustomLogger.Log("Selected unit1: " + unit1.name);
				} else {
					unit2 = clickedObject;
					if (unit2 != null)
					{
						Time.timeScale = originTime;
						playerObjCircle2.SetActive(true);
						playerObjCircle2.transform.position = unit2.transform.position;
						CustomLogger.Log("Selected unit2: " + unit2.name);
						targetPosition1 = unit2.transform.position;
						targetPosition2 = unit1.transform.position;
						animator1 = unit1.GetComponent<Animator>();
						animator2 = unit2.GetComponent<Animator>();
						allyScan1 = unit1.GetComponent<AllyScan>();
						allyScan2 = unit2.GetComponent<AllyScan>();
						isMoving = true; // 이동 시작
						// 이동 시작 애니메이션 트리거
						animator1.SetTrigger("Run");
						animator2.SetTrigger("Run");

						// AllyScan 스크립트 비활성화
						if (allyScan1 != null) allyScan1.enabled = false;
						if (allyScan2 != null) allyScan2.enabled = false;
					}
					
				}
			} else {
				CustomLogger.LogWarning("No object clicked");
			}
		}

		if (isMoving) {
			MoveUnits();
		}
	}

	void MoveUnits()
	{
		float speed1 = allyScan1.movementSpeed;
		float speed2 = allyScan2.movementSpeed;
		unit1.transform.position = Vector3.MoveTowards(unit1.transform.position, targetPosition1, speed1 * Time.deltaTime);
		unit2.transform.position = Vector3.MoveTowards(unit2.transform.position, targetPosition2, speed2 * Time.deltaTime);

		animator1.SetTrigger("Idle");
		animator2.SetTrigger("Idle");
		animator1.SetFloat("RunState", 0.5f);
		animator2.SetFloat("RunState", 0.5f);


		if (unit1.transform.position == targetPosition1)
		{
			if (allyScan1 != null) allyScan1.enabled = true;
		}
		if (unit2.transform.position == targetPosition2)
		{
			if (allyScan2 != null) allyScan2.enabled = true;
		}
		if (unit1.transform.position == targetPosition1 && unit2.transform.position == targetPosition2) {
			// 이동 완료 애니메이션 트리거

			// AllyScan 스크립트 활성화

			isMoving = false; // 이동 완료
			playerObjCircle1.SetActive(false);
			playerObjCircle2.SetActive(false);
			unit1 = null;
			unit2 = null;
		}
	}
}

using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
using DefenseScripts;

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

public class CastleWallManager : MonoBehaviour {
	public static CastleWallManager Instance;

    [SerializeField] private DefenseGameDataMB gameData;
    [SerializeField] private float activateShieldValue = 80f; // 실드 활성화 시 설정할 값
    private float extraHealth;

	private HeroManager heroManager;
	public float shield;
	public bool activateShield; // activateShield가 true이면 실드 적용 + hasShield를 true로 변경
	private Coroutine resetShieldCoroutine;

	[SerializeField] private Slider healthSlider;
	[SerializeField] private Slider shieldSlider;

	[SerializeField] private bool hasShield; // hasShield가 false가 되면 즉시 실드 무효화

	private StageC stageC; // StageC 스크립트 참조

	private List<GameObject> wallObjects;

	public float maxHealth;
	public float health;

	public float extraHealth1 {
		get => extraHealth;

		set => extraHealth = value;
	}

	private void Awake() {
		// Load game data
		if (gameData != null) {
			maxHealth = gameData.MaxHealth;
			extraHealth = gameData.ExtraHealth;
			health = gameData.Health;
		}

        Debug.Log("캐슬월에서 가져온 스테이지카운트 : " + gameData.StageCount);
        //1스테이지일 경우에만 health를 풀피로 설정
        if (gameData.StageCount == 1)
        {
            
            maxHealth += extraHealth; //2회차 1스테이지 경우에는 로그라이크 특전 추가체력을 더해줌
            health = maxHealth;
        }

		wallObjects = new List<GameObject>();
		wallObjects.Add(GameObject.FindGameObjectsWithTag("RightWall")[0]);
		wallObjects.Add(GameObject.FindGameObjectsWithTag("LeftWall")[0]);

		// Singleton 패턴을 사용하여 유일한 인스턴스 보장
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}

		shield = 0f;
		hasShield = false;
		activateShield = false;

		InitializeSliders();

		// StageC 스크립트 참조 초기화
		stageC = FindObjectOfType<StageC>();
	}

	private void InitializeSliders() {
		if (healthSlider != null) {
			healthSlider.maxValue = maxHealth;
			healthSlider.value = health;
		}

		if (shieldSlider != null) {
			shieldSlider.maxValue = activateShieldValue; // 실드 슬라이더의 최대값을 activateShieldValue로 설정
			shieldSlider.value = shield;
		}
	}

	private void Update() {
		// activateShield가 true로 설정되었을 때 실드 활성화
		if (activateShield) {
			ActivateShield();
			activateShield = false; // 실행 후 비활성화
		}

		if (!hasShield)
		{
			foreach (var wall in wallObjects) {
				CastleWall cwall = wall.GetComponent<CastleWall>();
				cwall.ChangeWallColor(false); // 실드 활성화 시 색상 변경
			}
		}
		// hasShield가 false가 되었을 때 실드 비활성화
		if (!hasShield && shield > 0)
		{
			SetShield(0); // 실드를 비활성화하고 초기화
			Debug.Log("Shield deactivated.");
		}
	}

	public void ApplyDamage(float damage) {
		if (hasShield) {
			shield -= damage;

			if (shield <= 0) {
				damage = -shield; // 남은 데미지를 체력에 적용
				shield = 0;
				hasShield = false;
				
				Debug.Log("Shield destroyed!");
			} else {
				damage = 0;
			}
		}

		health -= damage;

		if (health <= 0) {
			health = 0;
			HandleGameOver(); // 게임 오버 처리 호출
		}

		UpdateSliders();
	}

	public void EarnShield(float duration, float shieldAmount) {
		Debug.Log("EarnShield called with duration: " + duration + " and shieldAmount: " + shieldAmount);
		hasShield = true;
		// 기존의 실드 초기화 코루틴이 실행 중이면 중지
		if (resetShieldCoroutine != null) {
			StopCoroutine(resetShieldCoroutine);
		}

		foreach (var wall in wallObjects) {
			CastleWall cwall = wall.GetComponent<CastleWall>();
			cwall.ChangeWallColor(true); // 실드 활성화 시 색상 변경
		}
		AddShield(shieldAmount); // 실드 추가
		resetShieldCoroutine = StartCoroutine(ResetEarnShieldAfterDelay(duration)); // 일정 시간 후 실드 초기화
	}

	private IEnumerator ResetEarnShieldAfterDelay(float delay) {
		if (!hasShield)
		{
			yield break;
		}
		yield return new WaitForSeconds(delay);

		foreach (var wall in wallObjects) {
			CastleWall cwall = wall.GetComponent<CastleWall>();
			cwall.ChangeWallColor(false); // 실드 활성화 시 색상 변경
		}

		SetShield(0); // 실드 초기화
		hasShield = false; // 실드 비활성화
		Debug.Log("Shield reset to original values.");
	}

	private void ActivateShield() {
		SetShield(activateShieldValue);
		hasShield = true; // 실드 활성화 시 hasShield를 true로 설정
		Debug.Log("Shield activated with value: " + activateShieldValue);
	}

	public void SetActivateShield(bool value) {
		activateShield = value;
	}

	public void AddShield(float shieldAmount) {
		shield = Mathf.Min(activateShieldValue, shield + shieldAmount);
		hasShield = shield > 0;
		UpdateSliders();
		Debug.Log("Shield added. Current shield: " + shield);
	}

	public void SetShield(float shieldValue) {
		shield = Mathf.Clamp(shieldValue, 0, activateShieldValue);
		hasShield = shield > 0;
		UpdateSliders();
		Debug.Log("Shield set to value: " + shield);
	}

	private void UpdateSliders() {
		if (healthSlider != null) {
			healthSlider.value = health;
		}

		if (shieldSlider != null) {
			shieldSlider.maxValue = activateShieldValue; // 실드 슬라이더의 최대값을 최신 activateShieldValue로 설정
			shieldSlider.value = shield;
		}
	}

	private void HandleGameOver() {
		
		DestroyWallsWithTag("RightWall");
		DestroyWallsWithTag("LeftWall");
		
		Debug.Log("성벽이 파괴되었습니다! 게임 오버!");
		
		if (stageC != null) {
			stageC.ShowGameOverUI(); // StageC에서 게임 오버 UI를 표시하도록 호출
		} else {
			Debug.LogWarning("StageC instance not found.");
		}

		HeroManager.Instance.ClearHeroFormation(); //영웅선택 저장정보 clear
	}
	
	private void DestroyWallsWithTag(string tag) {
		GameObject[] walls = GameObject.FindGameObjectsWithTag(tag);
		foreach (GameObject wall in walls) {
			Destroy(wall);
		}
	}

    public void SaveWallHP()
    {
        CustomLogger.Log("성벽 HP 정보 저장됨 health, mH, eH"+health+","+maxHealth+","+extraHealth);
        gameData.Health = health;
        gameData.MaxHealth = maxHealth;
        gameData.ExtraHealth = extraHealth;
    }

    public float GetHealth() => health;
    public float GetShield() => shield;
}
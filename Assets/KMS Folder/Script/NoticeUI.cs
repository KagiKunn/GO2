using UnityEngine;

using System.Collections;

using TMPro;

public class NoticeUI : MonoBehaviour {
	[Header("SubNotice")] public GameObject subbox;
	public TMP_Text subintext;
	public Animator subani;

	// 코루틴 딜레이
	private WaitForSeconds _UIDelay = new WaitForSeconds(2.0f);
	private WaitForSeconds _UIDelay2 = new WaitForSeconds(0.3f);

	private void Start() {
		subbox.SetActive(false);
	}

	// 서브 메세지 >> String 값을 매개 변수로 받아와서 2초간 출력
	// 사용법 : _notice.SUB("문자열");
	public void SUB(string message) {
		subintext.text = message;
		subbox.SetActive(false);
		StopAllCoroutines();
		StartCoroutine(SUBDelay());
	}

	//  반복되지 않도록 딜레이 설정
	IEnumerator SUBDelay() {
		subbox.SetActive(true);
		subani.SetBool("isOn", true);

		yield return _UIDelay;

		subani.SetBool("isOn", false);

		yield return _UIDelay2;

		subbox.SetActive(false);
	}
}
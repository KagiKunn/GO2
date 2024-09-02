using System;

using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다

public class AllySpawner : MonoBehaviour {
	[SerializeField] public List<GameObject> allies;
	private List<GameObject> leftAllies; //0~
	private List<GameObject> rightAllies; //13~
	[SerializeField] public GameObject defaultObject;
	[SerializeField] private Tilemap tilemap;
	[SerializeField] private bool facingRight;
	[SerializeField] private float xSpacing = 10f; // 타일 크기에 맞게 조정
	[SerializeField] private float ySpacing = 10f; // 타일 크기에 맞게 조정
	private Dictionary<Vector3Int, GameObject> allyPositions = new Dictionary<Vector3Int, GameObject>();

	private List<Triple<int, int, string>> allyUnitList;
	
	private Vector3Int? selectedPosition = null;
	private int maxUnitCount;
	private int currentNumber;

	private Vector3Int gridStart;
	private int row = 7;
	private int column = 2;

	void Awake()
	{
		List<GameObject> leftAllies = new List<GameObject>();
		List<GameObject> rightAllies = new List<GameObject>();
		allyUnitList = PlayerLocalManager.Instance.lUnitList
			.OrderBy(unit => unit.Item2)
			.ToList();
		for (int i = 0; i<28; i++)
		{
			bool founded = false;
			for (int j = 0; j < allyUnitList.Count; j++)
			{
				if (allyUnitList[j].Item2 == i)
				{
					founded = true;
					string name = allyUnitList[j].Item3;
					if (i < 14)
					{
						leftAllies.Add(Resources.Load<GameObject>("Defense/Unit/" + name));
					}
					else
					{
						rightAllies.Add(Resources.Load<GameObject>("Defense/Unit/" + name));
					}
				} 
			}

			if (!founded)
			{
				if (i < 14)
				{
					leftAllies.Add(Resources.Load<GameObject>("Defense/Unit/Default"));
				}
				else
				{
					rightAllies.Add(Resources.Load<GameObject>("Defense/Unit/Default"));
				}
			}
		}

		allies = facingRight ? rightAllies : leftAllies;
		maxUnitCount = allies.Count;
		currentNumber = 0;
		BoundsInt bounds = tilemap.cellBounds;
		gridStart = new Vector3Int(facingRight?bounds.xMax - column - 1 : bounds.xMin + column +1, bounds.yMin + 1, 0); // 오른쪽에서 한 칸 띄운 위치를 기준점으로 설정

		Flip(bounds);
	}

	void Flip(BoundsInt bounds)
	{
		if (facingRight)
		{
			for (int x = 0; x < column; x++) {
				for (int y = 0; y < row; y++) {
					// 타일맵의 셀 위치에 따라 그리드 위치 계산
					Vector3Int gridPosition = new Vector3Int(gridStart.x + x, gridStart.y + y, 0);
					Vector3 worldPosition = tilemap.CellToWorld(gridPosition);

					// 스페이싱 값에 따라 월드 위치 조정
					worldPosition += new Vector3(x * xSpacing, y * ySpacing, 0);

					GameObject ally = null;

					if (currentNumber < maxUnitCount && allies[currentNumber] != null) {
						ally = Instantiate(allies[currentNumber], worldPosition, facingRight ? Quaternion.Euler(0, 180, 0) : Quaternion.identity, transform);
					} else {
						ally = Instantiate(defaultObject, worldPosition, facingRight ? Quaternion.Euler(0, 180, 0) : Quaternion.identity, transform);
					}

					
					AllyScan allyScan = ally.transform.GetChild(0).GetComponent<AllyScan>();
					if (allyScan != null)
					{
						allyScan.Initialized(facingRight);
					}
					else
					{
						Debug.LogWarning("AllyScan component is missing on " + ally.name);
					}
					allyPositions[gridPosition] = ally;
					currentNumber++;
				}
			}
		}
		else
		{
			for (int x = 0; x < column; x++) {
				for (int y = 0; y < row; y++) {
					// 타일맵의 셀 위치에 따라 그리드 위치 계산
					Vector3Int gridPosition = new Vector3Int(gridStart.x - x, gridStart.y + y, 0); 
					Vector3 worldPosition = tilemap.CellToWorld(gridPosition);

					// 스페이싱 값에 따라 월드 위치 조정
					worldPosition += new Vector3(-x * xSpacing, y * ySpacing, 0);

					GameObject ally = null;

					if (currentNumber < maxUnitCount && allies[currentNumber] != null) {
						ally = Instantiate(allies[currentNumber], worldPosition, facingRight ? Quaternion.Euler(0, 180, 0) : Quaternion.identity, transform);
					} else {
						ally = Instantiate(defaultObject, worldPosition, facingRight ? Quaternion.Euler(0, 180, 0) : Quaternion.identity, transform);
					}

					AllyScan allyScan = ally.transform.GetChild(0).GetComponent<AllyScan>();
					if (allyScan != null)
					{
						allyScan.Initialized(facingRight);
					}
					else
					{
						Debug.LogWarning("AllyScan component is missing on " + ally.name);
					}
					allyPositions[gridPosition] = ally;
					currentNumber++;
				}
			}
		}
	}
}
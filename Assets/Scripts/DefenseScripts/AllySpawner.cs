using UnityEngine;
using UnityEngine.Tilemaps;

public class AllySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] allies;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private bool facingRight = false;
    [SerializeField] private float xSpacing = 1.0f;  // 타일 크기에 맞게 조정
    [SerializeField] private float ySpacing = 1.0f;  // 타일 크기에 맞게 조정
    private Vector3[] alliesPosition;

    private int row = 7;
    private int column = 2;

    void Awake()
    {
        int maxUnitCount = allies.Length;
        int currentNumber = 0;
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int gridStart = new Vector3Int(bounds.xMax - column - 1, bounds.yMin + 1, 0);  // 오른쪽에서 한 칸 띄운 위치를 기준점으로 설정

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                if (currentNumber >= maxUnitCount)
                {
                    break;
                }

                // 타일맵의 셀 위치에 따라 그리드 위치 계산
                Vector3Int gridPosition = new Vector3Int(gridStart.x + x, gridStart.y + y, 0);
                Vector3 worldPosition = tilemap.CellToWorld(gridPosition);

                // 스페이싱 값에 따라 월드 위치 조정
                worldPosition += new Vector3(x * xSpacing, y * ySpacing, 0);

                // 계산된 월드 위치에 유닛 생성
                if (facingRight)
                {
                    Instantiate(allies[currentNumber], worldPosition, Quaternion.Euler(0, 180, 0), transform);
                }
                else
                {
                    Instantiate(allies[currentNumber], worldPosition, Quaternion.identity, transform);
                }

                currentNumber++;
            }
        }
    }
}
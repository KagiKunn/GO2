using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class AllySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] allies;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private bool facingRight;
    [SerializeField] private float xSpacing = 0.5f;
    [SerializeField] private float ySpacing = 0.5f;
    private Vector3[] alliesPosition;

    private int row = 8;
    private int column = 2;

    void Awake()
    {
        int maxUnitCount = allies.Length-1;
        int currentNumber = 0;
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int gridCenter = new Vector3Int(bounds.xMin + bounds.size.x / 3, bounds.yMin + bounds.size.y / 2, 0);
        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                if (currentNumber > maxUnitCount)
                {
                    break;
                }
                Vector3Int gridPosition = new Vector3Int(gridCenter.x + x+1, y, 0);
                Vector3 worldPosition = tilemap.CellToWorld(gridPosition);
                worldPosition += new Vector3(x * xSpacing, y * ySpacing, 0);
                if (facingRight) {
                    Instantiate(allies[currentNumber], worldPosition, Quaternion.Euler(0,180,0),transform);

                }else {
                    Instantiate(allies[currentNumber], worldPosition, Quaternion.identity,transform);
                }
                currentNumber++;
            }
            
            
            
        }
    }
}

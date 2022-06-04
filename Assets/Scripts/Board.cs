using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece piece { get; private set; }
    public int score { get; private set; }

    public TetrominoData[] tetraminoes;
    public Vector3Int spawnPos;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public TMP_Text scoreTxt;
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        piece = GetComponent<Piece>();
        score = 0;

        for(int i = 0; i < tetraminoes.Length; i++)
        {
            tetraminoes[i].Initialize();
        }
    }
    private void Start()
    {
        SpawnPiece();
    }

    private void Update()
    {
        scoreTxt.text = "Score: " + score;
    }

    public void SpawnPiece()
    {
        score += 15;
        int random = Random.Range(0, this.tetraminoes.Length);
        TetrominoData data = tetraminoes[random];

        piece.Initialize(this, data, spawnPos);

        if (IsValid(piece, spawnPos))
        {
            Set(piece);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        score = 0;
        tilemap.ClearAllTiles();
    }

    public void Set(Piece piece)
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePos, piece.data.tile);
        }
    }
    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePos, null);
        }
    }
    public bool IsValid(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePos = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePos)){
                return false;
            }

            if (tilemap.HasTile(tilePos))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while(row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                score += 100;
            }
            else
            {
                row++;
            }
        }
    }
    private bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            this.tilemap.SetTile(position, null);
        }

        while(row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}

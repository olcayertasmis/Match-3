using System.Collections.Generic;
using _GameFolder.Scripts.Data;
using _GameFolder.Scripts.Enums;
using _GameFolder.Scripts.ManagerScripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GameFolder.Scripts.GridSystem
{
    public class GridSpawner : MonoBehaviour
    {
        [Header("Cell")]
        public Cell[,] Cells;
        private GameObject _cellPrefab;
        private Transform _cellSpawnTransform;

        [Header("Grid")]
        private GameObject _maskAreaPrefab;
        private int _rowCount;
        private int _columnCount;

        [Header("Fruit")]
        private AllFruits _allFruits;
        private Transform _fruitSpawnTransform;

        [Header("Lists")]
        public List<List<Fruit>> FruitColumns;
        public List<List<Fruit>> FruitRows;

        [Header("Components")]
        private SpriteRenderer _cellSpriteRenderer;
        private GridMovement _gridMovement;

        private void Awake()
        {
            Cells = null;

            var dataManager = Managers.Instance.DataManager;

            _allFruits = dataManager.AllFruits;

            _maskAreaPrefab = dataManager.GridSpawnerData.MaskAreaPrefab;

            _cellSpawnTransform = Instantiate(dataManager.GridSpawnerData.EmptyTransform);
            _cellSpawnTransform.name = "Cells";

            _fruitSpawnTransform = Instantiate(dataManager.GridSpawnerData.EmptyTransform);
            _fruitSpawnTransform.name = "Fruits";

            var activeLevelIndex = dataManager.AllLevels.activeLevel;
            var activeLevel = dataManager.AllLevels.LevelList[activeLevelIndex];

            _rowCount = activeLevel.RowCount;
            _columnCount = activeLevel.ColumnCount;

            _cellPrefab = activeLevel.CellPrefab;
            _cellSpriteRenderer = _cellPrefab.GetComponent<SpriteRenderer>();

            _gridMovement = GetComponent<GridMovement>();
        }

        private void Start()
        {
            InitializeGrid();
        }

        #region Event Listener

        private void OnChangedFruit(Vector2 pos, Fruit fruit)
        {
            if (_gridMovement.GetCurrentMovementDirection() == MovementDirection.Horizontal) // Yatay Liste güncelleme
            {
                FruitColumns[(int)pos.x][(int)pos.y] = fruit;
            }
            else if (_gridMovement.GetCurrentMovementDirection() == MovementDirection.Vertical) // Dikey Liste güncelleme
            {
                FruitRows[(int)pos.y][(int)pos.x] = fruit;
            }
        }

        private void OnMatchedFruit(Vector2 pos, Fruit fruit)
        {
            FruitColumns[(int)pos.x][(int)pos.y] = fruit;
            FruitRows[(int)pos.y][(int)pos.x] = fruit;
        }

        #endregion

        private void InitializeGrid()
        {
            Cells = new Cell[_rowCount, _columnCount];
            FruitColumns = new List<List<Fruit>>();
            FruitRows = new List<List<Fruit>>();

            for (int x = 0; x < _rowCount; x++)
            {
                var column = new List<Fruit>();

                for (int y = 0; y < _columnCount; y++)
                {
                    Vector2 pos = new Vector2(x, y);
                    Cells[x, y] = Cell.SpawnCell(_cellPrefab, new Vector2(x, y), _cellSpawnTransform);

                    int fruitToUse = Random.Range(0, _allFruits.FruitList.Length);
                    var selectedFruit = _allFruits.FruitList[fruitToUse];

                    while (IsSameFruitAdjacent(new Vector2(x, y), selectedFruit))
                    {
                        fruitToUse = Random.Range(0, _allFruits.FruitList.Length);
                        selectedFruit = _allFruits.FruitList[fruitToUse];
                    }

                    var spawnedFruit = Fruit.FruitSpawn(selectedFruit, pos, _fruitSpawnTransform);

                    Cells[x, y].Initialize(spawnedFruit, new Vector2(x, y));
                    Cells[x, y].OnChangedFruit += OnChangedFruit;
                    Cells[x, y].OnMatchedFruit += OnMatchedFruit;

                    column.Add(spawnedFruit);
                }

                FruitColumns.Add(column);
            }

            for (int y = 0; y < _columnCount; y++)
            {
                var row = new List<Fruit>();

                for (int x = 0; x < _rowCount; x++)
                {
                    row.Add(FruitColumns[x][y]);
                }

                FruitRows.Add(row);
            }

            SetMaskArea();
        }

        private void SetMaskArea()
        {
            var centerPos = (Cells[_rowCount - 1, _columnCount - 1].transform.position - Cells[0, 0].transform.position) / 2;
            var cellRadius = _cellSpriteRenderer.bounds.max.y;
            var sizeY = Vector2.Distance(Cells[0, 0].transform.position, Cells[0, _columnCount - 1].transform.position) + cellRadius * 2;
            var sizeX = Vector2.Distance(Cells[0, 0].transform.position, Cells[_rowCount - 1, 0].transform.position) + cellRadius * 2;
            var maskAreaInstance = Instantiate(_maskAreaPrefab);
            maskAreaInstance.transform.position = centerPos;
            maskAreaInstance.transform.localScale = new Vector2(sizeX, sizeY);
        }

        private bool IsSameFruitAdjacent(Vector2 posToCheck, Fruit selectedFruit)
        {
            if (posToCheck.x > 0 && Cells[(int)posToCheck.x - 1, (int)posToCheck.y].fruitInCell.FruitColorType == selectedFruit.FruitColorType)
                return true; // Üstte aynı türde meyve var

            if (posToCheck.y > 0 && Cells[(int)posToCheck.x, (int)posToCheck.y - 1].fruitInCell.FruitColorType == selectedFruit.FruitColorType)
                return true; // Solda aynı türde meyve var

            return false; // Yanında aynı türde meyve yok
        }

        #region DebugControl

        private void ListControl()
        {
            for (int i = 0; i < FruitColumns.Count; i++)
            {
                Debug.Log("Column " + i + " has " + FruitColumns[i].Count + " fruits.");
            }

            foreach (var column in FruitColumns)
            {
                Debug.Log(column);
                foreach (var fruit in column)
                {
                    Debug.Log(fruit.name);
                }
            }

            for (int i = 0; i < FruitRows.Count; i++)
            {
                Debug.Log("Row " + i + " has " + FruitRows[i].Count + " fruits.");
            }

            foreach (var row in FruitRows)
            {
                Debug.Log(row);
                foreach (var fruit in row)
                {
                    Debug.Log(fruit.name);
                }
            }
        }

        #endregion
    } // END CLASS
}
using System.Collections;
using System.Collections.Generic;
using _GameFolder.Scripts.Enums;
using UnityEngine;

namespace _GameFolder.Scripts.Data.LevelSystem
{
    [CreateAssetMenu(fileName = "Level", menuName = "New Level")]
    public class Level : ScriptableObject
    {
        [Header("Level Settings")]
        [SerializeField] private int rowCount;
        [SerializeField] private int columnCount;
        [SerializeField] private List<RequiredValues> requiredValues;
        [SerializeField] private GameObject cellPrefab;


        [System.Serializable]
        public struct RequiredValues
        {
            public List<FruitColor> requiredColor;
            public List<int> requiredColorNumber;
        }

        #region Getters

        public int RowCount => rowCount;
        public int ColumnCount => columnCount;
        public GameObject CellPrefab => cellPrefab;
        public List<RequiredValues> RequiredValuesList => requiredValues;

        #endregion
    } // END CLASS
}
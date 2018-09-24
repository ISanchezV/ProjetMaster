﻿namespace GameCreator.Inventory
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using GameCreator.Core;

    [CreateAssetMenu(fileName = "New Merchant", menuName = "Game Creator/Inventory/Merchant")]
    public class Merchant : ScriptableObject
    {
        [System.Serializable]
        public class Ware
        {
            public ItemHolder item = new ItemHolder();
            public bool limitAmount = false;
            public int maxAmount = 10;
        }

        [System.Serializable]
        public class Warehouse
        {
            public Ware[] wares = new Ware[0];
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        public string uuid = "";
        public string title = "";
        public string description = "";

        [Space]
        public GameObject merchantUI;
        public Warehouse warehouse;
    }
}
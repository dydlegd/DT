using UnityEngine;
using System.Collections;

namespace DYD
{
    public class HistroyRecordManager : Spawn
    {
        public static HistroyRecordManager Instance;
        public const int MAX_RECORD_SUM = 20;

        public Transform prefab;
        private UIGrid mUIGrid;
        public int HistroyRecordCount { get { return mUIGrid.GetChildList().Count; } }
        public void Init()
        {
            Instance = this;
            pool = CreatePool(transform.name);
            CreatePrefab(pool, prefab);

            mUIGrid = GameUtility.FindDeepChild(gameObject, "UIGris").GetComponent<UIGrid>();
        }

        public void AddHistroyRecord(ref HistroyRecordData data)
        {
            if (mUIGrid.GetChildList().Count >= MAX_RECORD_SUM)
            {
                DespawnHistroyRecord(mUIGrid.GetChild(0));                
            }
            SpawnHistroyRecord(ref data);
        }

        private HistroyRecord SpawnHistroyRecord(ref HistroyRecordData data)
        {
            Transform ts = pool.Spawn(prefab, mUIGrid.transform);
            ts.transform.localScale = Vector3.one;
            HistroyRecord hr = ts.GetComponent<HistroyRecord>();
            hr.Init(ref data);
            mUIGrid.AddChild(hr.transform);
            return hr;
        }

        private void DespawnHistroyRecord(Transform ts)
        {
            mUIGrid.RemoveChild(ts);
            pool.Despawn(ts);
        }

        public void ClearAllHistroyRecord()
        {
            while (mUIGrid.GetChildList().Count>0)
            {
                DespawnHistroyRecord(mUIGrid.GetChild(0));
            }
        }

        public HistroyRecordData GetHistroyRecordData(int index)
        {
            if (index>=0 && index<=mUIGrid.GetChildList().Count-1)
            {
                return mUIGrid.GetChild(index).GetComponent<HistroyRecord>().data;
            }
            return null;
        }
    }
}


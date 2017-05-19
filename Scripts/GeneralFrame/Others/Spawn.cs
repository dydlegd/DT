using UnityEngine;
using System.Collections;
using PathologicalGames;

namespace DYD
{

    public class Spawn : MonoBehaviour
    {

        protected SpawnPool pool;

        protected SpawnPool CreatePool(string name)
        {
            name = transform.root.name + name;
            if (PoolManager.Pools.ContainsKey(name))
            {
                Debuger.LogError("The name of \"" + name + "\" pool was exited");
                return null;
            }

            SpawnPool pool = PoolManager.Pools.Create(name);
            pool.group.parent = this.transform;
            pool.group.localPosition = Vector3.zero;
            pool.group.localRotation = Quaternion.identity;
            pool.group.localScale = Vector3.one;

            return pool;
        }

        protected PrefabPool CreatePrefab(SpawnPool pool, Transform trans, int preload)
        {
            if (trans == null)
            {
                Debuger.LogError("The prefab was null");
                return null;
            }
            if (pool == null)
            {
                Debuger.LogError("The pool was null");
                return null;
            }

            //如果该预设已经生成有prefabPool，直接返回改prefabPool即可
            PrefabPool prePool = pool.GetPrefabPool(trans);
            if (prePool != null)
            {
                return prePool;
            }

            //创建预设
            PrefabPool prefabPool = new PrefabPool(trans);
            //缓存池这个Prefab的最大保存数量
            prefabPool.preloadAmount = preload;
            //是否开启缓存池智能自动清理模式
            prefabPool.cullDespawned = true;
            //缓存池自动清理，但是始终保留几个对象不清理
            prefabPool.cullAbove = 200;
            //每过多久执行一遍自动清理(销毁)，单位是秒
            prefabPool.cullDelay = 6;
            //每次自动清理2个游戏对象
            prefabPool.cullMaxPerPass = 2;
            //是否开启实例的限制功能
            //prefabPool.limitInstances = true;
            //限制缓存池里最大的Prefab的数量，它和上面的preloadAmount是有冲突的，如果同时开启则以limitAmout为准
            //prefabPool.limitAmount = 3;
            //如果我们限制了缓存池里面只能有10个Prefab，如果不勾选它，那么你拿第11个的时候就会返回null。如果勾选它在取第11个的时候他会返回给你前10个里最不常用的那个
            //prefabPool.limitFIFO = true;
            //加入此预设
            pool.CreatePrefabPool(prefabPool);

            return prefabPool;
        }

        protected PrefabPool CreatePrefab(SpawnPool pool, Transform trans)
        {
            return CreatePrefab(pool, trans, 5);
        }

        public IEnumerator StartCoroutine(IEnumerator routine, IEnumerator instance)
        {
            if (instance != null) StopCoroutine(instance);
            instance = routine;
            StartCoroutine(instance);
            return instance;
        }
    }
}
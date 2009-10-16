#region File Description
//-----------------------------------------------------------------------------
// SLDictionary.cs
//
// StiLib Two Keys/One Value Dictionary
// Copyright (c) Zhang Li. 2009-02-22.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib 2keys-1value Dictionary
    /// </summary>
    /// <typeparam name="pK">Primary Key Type</typeparam>
    /// <typeparam name="sK">Secondary Key Type</typeparam>
    /// <typeparam name="V">Value Type</typeparam>
    public class SLDictionary<pK, sK, V>
    {
        /// <summary>
        /// Primary Dictionary -- map primary key to value
        /// </summary>
        public Dictionary<pK, V> pDictionary = new Dictionary<pK, V>();
        /// <summary>
        /// map secondary key to primary key
        /// </summary>
        public Dictionary<sK, pK> sTop = new Dictionary<sK, pK>();
        /// <summary>
        /// map primary key to secondary key
        /// </summary>
        public Dictionary<pK, sK> pTos = new Dictionary<pK, sK>();
        object lockobject = new object();


        /// <summary>
        /// Gets/Sets Value from Primary Key
        /// </summary>
        /// <param name="pKey"></param>
        /// <returns></returns>
        public V this[pK pKey]
        {
            get
            {
                V item;
                if (TryGetValue(pKey, out item))
                    return item;
                throw new KeyNotFoundException("Primary Key Not Found: " + pKey.ToString());
            }
            set
            {
                if (ContainsKey(pKey))
                {
                    lock (lockobject)
                    {
                        pDictionary[pKey] = value;
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Primary Key Not Found: " + pKey.ToString());
                }
            }
        }

        /// <summary>
        /// Gets/Sets Value from Secondary Key
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public V this[sK sKey]
        {
            get
            {
                V item;
                if (TryGetValue(sKey, out item))
                    return item;
                throw new KeyNotFoundException("Secondary Key Not Found: " + sKey.ToString());
            }
            set
            {
                if (ContainsKey(sKey))
                {
                    lock (lockobject)
                    {
                        pDictionary[sTop[sKey]] = value;
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Secondary Key Not Found: " + sKey.ToString());
                }
            }
        }

        /// <summary>
        /// Associate Primary and Secondary Keys
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="pKey"></param>
        public void Associate(sK sKey, pK pKey)
        {
            lock (lockobject)
            {
                if (!pDictionary.ContainsKey(pKey))
                    throw new KeyNotFoundException(string.Format("The primary dictionary does not contain the key '{0}' !", pKey));

                if (sTop.ContainsKey(sKey))
                {
                    sTop[sKey] = pKey;
                    pTos[pKey] = sKey;
                }
                else
                {
                    sTop.Add(sKey, pKey);
                    pTos.Add(pKey, sKey);
                }
            }
        }

        /// <summary>
        /// Get Value from Primary Key
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetValue(pK pKey, out V val)
        {
            lock (lockobject)
            {
                if (!pDictionary.TryGetValue(pKey, out val))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get Value from Secondary Key
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetValue(sK sKey, out V val)
        {
            val = default(V);
            lock (lockobject)
            {
                pK pk;
                if (sTop.TryGetValue(sKey, out pk))
                {
                    if (!TryGetValue(pk, out val))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get Secondary Key from Primary Key
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetKey(pK pKey, out sK val)
        {
            val = default(sK);
            lock (lockobject)
            {
                if (pTos.ContainsKey(pKey))
                {
                    val = pTos[pKey];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Primary Key from Secondary Key
        /// </summary>
        /// <param name="sKey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool TryGetKey(sK sKey, out pK val)
        {
            val = default(pK);
            lock (lockobject)
            {
                if (sTop.ContainsKey(sKey))
                {
                    val = sTop[sKey];
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If contains specified primary key
        /// </summary>
        /// <param name="pKey"></param>
        /// <returns></returns>
        public bool ContainsKey(pK pKey)
        {
            V val;
            return TryGetValue(pKey, out val);
        }

        /// <summary>
        /// If contains specified secondary key
        /// </summary>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public bool ContainsKey(sK sKey)
        {
            V val;
            return TryGetValue(sKey, out val);
        }

        /// <summary>
        /// Remove value specified by primary key
        /// </summary>
        /// <param name="pKey"></param>
        public void Remove(pK pKey)
        {
            lock (lockobject)
            {
                if (pTos.ContainsKey(pKey))
                {
                    if (sTop.ContainsKey(pTos[pKey]))
                        sTop.Remove(pTos[pKey]);
                    pTos.Remove(pKey);
                }
                pDictionary.Remove(pKey);
            }
        }

        /// <summary>
        /// Remove value specified by secondary key
        /// </summary>
        /// <param name="sKey"></param>
        public void Remove(sK sKey)
        {
            lock (lockobject)
            {
                if (sTop.ContainsKey(sKey))
                {
                    if (pDictionary.ContainsKey(sTop[sKey]))
                        pDictionary.Remove(sTop[sKey]);
                    if (pTos.ContainsKey(sTop[sKey]))
                        pTos.Remove(sTop[sKey]);
                    sTop.Remove(sKey);
                }
            }
        }

        /// <summary>
        /// Add Primary Key/Value pair
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="val"></param>
        public void Add(pK pKey, V val)
        {
            lock (lockobject)
                pDictionary.Add(pKey, val);
        }

        /// <summary>
        /// Add Primary and Secondary Keys/Value pair
        /// </summary>
        /// <param name="pKey"></param>
        /// <param name="sKey"></param>
        /// <param name="val"></param>
        public void Add(pK pKey, sK sKey, V val)
        {
            lock (lockobject)
                pDictionary.Add(pKey, val);
            Associate(sKey, pKey);
        }

        /// <summary>
        /// Get a clone array of all values
        /// </summary>
        /// <returns></returns>
        public V[] CloneValues()
        {
            lock (lockobject)
            {
                V[] values = new V[pDictionary.Values.Count];
                pDictionary.Values.CopyTo(values, 0);
                return values;
            }
        }

        /// <summary>
        /// Get a clone array of all primary keys
        /// </summary>
        /// <returns></returns>
        public pK[] ClonePrimaryKeys()
        {
            lock (lockobject)
            {
                pK[] values = new pK[pDictionary.Keys.Count];
                pDictionary.Keys.CopyTo(values, 0);
                return values;
            }
        }

        /// <summary>
        /// Get a clone array of all secondary keys
        /// </summary>
        /// <returns></returns>
        public sK[] CloneSecondaryKeys()
        {
            lock (lockobject)
            {
                sK[] values = new sK[sTop.Keys.Count];
                sTop.Keys.CopyTo(values, 0);
                return values;
            }
        }

        /// <summary>
        /// Clear all Keys/Value Pairs
        /// </summary>
        public void Clear()
        {
            lock (lockobject)
            {
                pDictionary.Clear();
                sTop.Clear();
                pTos.Clear();
            }
        }

        /// <summary>
        /// Get the Count of Keys/Value pairs
        /// </summary>
        public int Count
        {
            get
            {
                lock (lockobject)
                    return pDictionary.Count;
            }
        }

        /// <summary>
        /// Get Enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<pK, V>> GetEnumerator()
        {
            lock (lockobject)
                return pDictionary.GetEnumerator();
        }

    }
}

// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityMasterData;
using UnityMasterData.Interfaces;

namespace Chsopoly.MasterData.Collection
{
    public class MasterDataAccessorObjectCollection : IMasterDataAccessorObjectCollection
    {
        private List<IMasterDataAccessorObject> _collection = new List<IMasterDataAccessorObject> ()
        {
            (Activator.CreateInstance(typeof(DAO.Ingame.StageDAO)) as IMasterDataAccessorObject),
            (Activator.CreateInstance(typeof(DAO.Ingame.CharacterDAO)) as IMasterDataAccessorObject),
            (Activator.CreateInstance(typeof(DAO.Ingame.GimmickDAO)) as IMasterDataAccessorObject),
        };

        IEnumerator<IMasterDataAccessorObject> IEnumerable<IMasterDataAccessorObject>.GetEnumerator ()
        {
            return _collection.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _collection.GetEnumerator ();
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace B2BAISERA.Models.EFServer
{
    public partial class CUSTOM_TRANSACTIONDATA
    {
        #region Primitive Properties
    
        public virtual int ID
        {
            get;
            set;
        }
    
        public virtual int TransactionID
        {
            get { return _transactionID; }
            set
            {
                if (_transactionID != value)
                {
                    if (CUSTOM_TRANSACTION != null && CUSTOM_TRANSACTION.ID != value)
                    {
                        CUSTOM_TRANSACTION = null;
                    }
                    _transactionID = value;
                }
            }
        }
        private int _transactionID;
    
        public virtual string TransGUID
        {
            get;
            set;
        }
    
        public virtual string DocumentNumber
        {
            get;
            set;
        }
    
        public virtual string FileType
        {
            get;
            set;
        }
    
        public virtual string IPAddress
        {
            get;
            set;
        }
    
        public virtual string DestinationUser
        {
            get;
            set;
        }
    
        public virtual string Key1
        {
            get;
            set;
        }
    
        public virtual string Key2
        {
            get;
            set;
        }
    
        public virtual string Key3
        {
            get;
            set;
        }
    
        public virtual Nullable<int> DataLength
        {
            get;
            set;
        }
    
        public virtual string RowStatus
        {
            get;
            set;
        }
    
        public virtual string CreatedWho
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> CreatedWhen
        {
            get;
            set;
        }
    
        public virtual string ChangedWho
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> ChangedWhen
        {
            get;
            set;
        }

        #endregion

        #region Navigation Properties
    
        public virtual CUSTOM_TRANSACTION CUSTOM_TRANSACTION
        {
            get { return _cUSTOM_TRANSACTION; }
            set
            {
                if (!ReferenceEquals(_cUSTOM_TRANSACTION, value))
                {
                    var previousValue = _cUSTOM_TRANSACTION;
                    _cUSTOM_TRANSACTION = value;
                    FixupCUSTOM_TRANSACTION(previousValue);
                }
            }
        }
        private CUSTOM_TRANSACTION _cUSTOM_TRANSACTION;
    
        public virtual ICollection<CUSTOM_TRANSACTIONDATADETAIL> CUSTOM_TRANSACTIONDATADETAIL
        {
            get
            {
                if (_cUSTOM_TRANSACTIONDATADETAIL == null)
                {
                    var newCollection = new FixupCollection<CUSTOM_TRANSACTIONDATADETAIL>();
                    newCollection.CollectionChanged += FixupCUSTOM_TRANSACTIONDATADETAIL;
                    _cUSTOM_TRANSACTIONDATADETAIL = newCollection;
                }
                return _cUSTOM_TRANSACTIONDATADETAIL;
            }
            set
            {
                if (!ReferenceEquals(_cUSTOM_TRANSACTIONDATADETAIL, value))
                {
                    var previousValue = _cUSTOM_TRANSACTIONDATADETAIL as FixupCollection<CUSTOM_TRANSACTIONDATADETAIL>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCUSTOM_TRANSACTIONDATADETAIL;
                    }
                    _cUSTOM_TRANSACTIONDATADETAIL = value;
                    var newValue = value as FixupCollection<CUSTOM_TRANSACTIONDATADETAIL>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCUSTOM_TRANSACTIONDATADETAIL;
                    }
                }
            }
        }
        private ICollection<CUSTOM_TRANSACTIONDATADETAIL> _cUSTOM_TRANSACTIONDATADETAIL;
    
        public virtual ICollection<CUSTOM_S02007> CUSTOM_S02007
        {
            get
            {
                if (_cUSTOM_S02007 == null)
                {
                    var newCollection = new FixupCollection<CUSTOM_S02007>();
                    newCollection.CollectionChanged += FixupCUSTOM_S02007;
                    _cUSTOM_S02007 = newCollection;
                }
                return _cUSTOM_S02007;
            }
            set
            {
                if (!ReferenceEquals(_cUSTOM_S02007, value))
                {
                    var previousValue = _cUSTOM_S02007 as FixupCollection<CUSTOM_S02007>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupCUSTOM_S02007;
                    }
                    _cUSTOM_S02007 = value;
                    var newValue = value as FixupCollection<CUSTOM_S02007>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupCUSTOM_S02007;
                    }
                }
            }
        }
        private ICollection<CUSTOM_S02007> _cUSTOM_S02007;

        #endregion

        #region Association Fixup
    
        private void FixupCUSTOM_TRANSACTION(CUSTOM_TRANSACTION previousValue)
        {
            if (previousValue != null && previousValue.CUSTOM_TRANSACTIONDATA.Contains(this))
            {
                previousValue.CUSTOM_TRANSACTIONDATA.Remove(this);
            }
    
            if (CUSTOM_TRANSACTION != null)
            {
                if (!CUSTOM_TRANSACTION.CUSTOM_TRANSACTIONDATA.Contains(this))
                {
                    CUSTOM_TRANSACTION.CUSTOM_TRANSACTIONDATA.Add(this);
                }
                if (TransactionID != CUSTOM_TRANSACTION.ID)
                {
                    TransactionID = CUSTOM_TRANSACTION.ID;
                }
            }
        }
    
        private void FixupCUSTOM_TRANSACTIONDATADETAIL(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CUSTOM_TRANSACTIONDATADETAIL item in e.NewItems)
                {
                    item.CUSTOM_TRANSACTIONDATA = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CUSTOM_TRANSACTIONDATADETAIL item in e.OldItems)
                {
                    if (ReferenceEquals(item.CUSTOM_TRANSACTIONDATA, this))
                    {
                        item.CUSTOM_TRANSACTIONDATA = null;
                    }
                }
            }
        }
    
        private void FixupCUSTOM_S02007(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (CUSTOM_S02007 item in e.NewItems)
                {
                    item.CUSTOM_TRANSACTIONDATA = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (CUSTOM_S02007 item in e.OldItems)
                {
                    if (ReferenceEquals(item.CUSTOM_TRANSACTIONDATA, this))
                    {
                        item.CUSTOM_TRANSACTIONDATA = null;
                    }
                }
            }
        }

        #endregion

    }
}

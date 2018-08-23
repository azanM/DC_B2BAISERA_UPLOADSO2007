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

namespace B2BAISERA.Models.DataAccess
{
    public partial class TransactionDataDetail
    {
        #region Primitive Properties
    
        public virtual int ID
        {
            get;
            set;
        }
    
        public virtual int TransactionDataID
        {
            get { return _transactionDataID; }
            set
            {
                if (_transactionDataID != value)
                {
                    if (TransactionData != null && TransactionData.ID != value)
                    {
                        TransactionData = null;
                    }
                    _transactionDataID = value;
                }
            }
        }
        private int _transactionDataID;
    
        public virtual string Data
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual TransactionData TransactionData
        {
            get { return _transactionData; }
            set
            {
                if (!ReferenceEquals(_transactionData, value))
                {
                    var previousValue = _transactionData;
                    _transactionData = value;
                    FixupTransactionData(previousValue);
                }
            }
        }
        private TransactionData _transactionData;

        #endregion
        #region Association Fixup
    
        private void FixupTransactionData(TransactionData previousValue)
        {
            if (previousValue != null && previousValue.TransactionDataDetails.Contains(this))
            {
                previousValue.TransactionDataDetails.Remove(this);
            }
    
            if (TransactionData != null)
            {
                if (!TransactionData.TransactionDataDetails.Contains(this))
                {
                    TransactionData.TransactionDataDetails.Add(this);
                }
                if (TransactionDataID != TransactionData.ID)
                {
                    TransactionDataID = TransactionData.ID;
                }
            }
        }

        #endregion
    }
}
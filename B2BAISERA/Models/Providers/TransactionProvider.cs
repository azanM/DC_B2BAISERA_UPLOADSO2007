using System;
using System.Collections.Generic;
using System.Linq;
//using B2BAISERA.Models.DataAccess;
//using B2BAISERA.Helper;
//using B2BAISERA.Logic;
using Microsoft.Practices.Unity;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Common;
using B2BAISERA.Models.EFServer;
using B2BAISERA.Helper;
using System.Data.EntityClient;
using System.Data;
using B2BAISERA.B2BAIWsDMZ;
using System.Globalization;
using System.Diagnostics;
using B2BAISERA.Log;

namespace B2BAISERA.Models.Providers
{
    public class TransactionProvider : DataAccessBase
    {
        public TransactionProvider()
            : base()
        {
        }

        public TransactionProvider(EProcEntities context)
            : base(context)
        {
        }

        #region MAIN
        //B2BAISERAEntities ctx = new B2BAISERAEntities(Repository.ConnectionStringEF);

        //public User GetUser(string userName, string password, string clientTag)
        //{
        //    var User = (from o in ctx.Users
        //                where o.UserName == userName && o.Password == password && o.ClientTag == clientTag
        //                select o).FirstOrDefault();

        //    return User;
        //}

        public CUSTOM_USER GetUser(string userName, string password, string clientTag)
        {
            var user = (from o in entities.CUSTOM_USER
                        where o.UserName == userName && o.Password == password && o.ClientTag == clientTag
                        select o).FirstOrDefault();

            return user;
        }

        public string GetLastTicketNo(string fileType)
        {
            var result = "";
            try
            {
                var query = (entities.CUSTOM_LOG
                    .Where(log => (log.Acknowledge == true) && (log.FileType == fileType))
                    .Select(log => new LogViewModel
                    {
                        ID = log.ID,
                        WebServiceName = log.WebServiceName,
                        MethodName = log.MethodName,
                        TicketNo = log.TicketNo,
                        Message = log.Message
                    })
                    ).OrderByDescending(log => log.ID).FirstOrDefault();

                result = query != null ? Convert.ToString(query.TicketNo) : "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        #endregion

        #region UPLOAD

        #region S02007
        public List<CUSTOM_S02007_TEMP> SendActualReceiptInvoice()
        {
            List<CUSTOM_S02007_TEMP> listTempHS = new List<CUSTOM_S02007_TEMP>();
            try
            {
                listTempHS = entities.sp_SendActualReceiptInvoice().ToList();
                return listTempHS;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<CUSTOM_S02007_TEMP> CheckingHistoryHS(List<CUSTOM_S02007_TEMP> tempHS)
        {
            LogEvent logEvent = new LogEvent();
            List<CUSTOM_S02007_TEMP> listDataHS = new List<CUSTOM_S02007_TEMP>();
            try
            {
                if (tempHS.Count > 0)
                {
                    listDataHS = tempHS;
                    var existingRow = (from o in tempHS
                                       where entities.CUSTOM_S02007.Any(e => o.PONUMBER == e.PONUMBER)
                                       select new S02007ViewModel
                                       {
                                           PONUMBER = o.PONUMBER,
                                           VERSIONPOSERA = o.VERSIONPOSERA,
                                           BILLINGNO = o.BILLINGNO,
                                           INVOICERECEIPTDATE = o.INVOICERECEIPTDATE,
                                           DATAVERSION = o.DATAVERSION,
                                           CompanyCodeAI = o.CompanyCodeAI,
                                           payPlan = o.payPlan
                                       }).ToList();

                    for (int i = 0; i < existingRow.Count; i++)
                    {
                        string existPoNumber = existingRow[i].PONUMBER;
                        var q = (from o in entities.CUSTOM_S02007
                                 where o.PONUMBER == existPoNumber
                                 select new S02007ViewModel
                                 {
                                     PONUMBER = o.PONUMBER,
                                     VERSIONPOSERA = o.VERSIONPOSERA,
                                     BILLINGNO = o.BILLINGNO,
                                     INVOICERECEIPTDATE = o.INVOICERECEIPTDATE,
                                     DATAVERSION = o.DATAVERSION,
                                     payPlan=o.payPlan
                                 }).SingleOrDefault();
                        var compare = EqualS02007HS(existingRow[i], q);

                        //jika ada yg beda di salah satu field atau lebih, maka update list 
                        if (!compare)
                        {
                            //removeat(index)
                            var rowDel = (from o in listDataHS
                                          where o.PONUMBER == existingRow[i].PONUMBER
                                          select o).SingleOrDefault();
                            listDataHS.Remove(rowDel);

                            CUSTOM_S02007_TEMP row = new CUSTOM_S02007_TEMP();
                            row.PONUMBER = existingRow[i].PONUMBER;
                            row.VERSIONPOSERA = existingRow[i].VERSIONPOSERA; //q.VERSIONPOSERA != null ? q.VERSIONPOSERA + 1 : existingRow[i].VERSIONPOSERA != null ? existingRow[i].VERSIONPOSERA : 1;
                            row.BILLINGNO = existingRow[i].BILLINGNO;
                            row.INVOICERECEIPTDATE = existingRow[i].INVOICERECEIPTDATE;
                            row.DATAVERSION = q.DATAVERSION != null ? q.DATAVERSION + 1 : existingRow[i].DATAVERSION != null ? existingRow[i].DATAVERSION : 1;
                            row.CompanyCodeAI = existingRow[i].CompanyCodeAI;
                            row.payPlan = existingRow[i].payPlan;
                            listDataHS.Add(row);
                        }
                        else
                        {
                            //removeat(index)
                            var rowDel = (from o in listDataHS
                                          where o.PONUMBER == existingRow[i].PONUMBER
                                          select o).SingleOrDefault();
                            listDataHS.Remove(rowDel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //add task kill : by  fhi 05.06.2014
                logEvent.WriteDBLog("", "UploadS02007_Load", false, "", ex.Message, "S02007", "SERA");
                Process.Start("taskkill.exe", "/f /im B2BAISERA_S02007.exe");
                //end
                throw ex;
            }
            return listDataHS;
        }

        public bool EqualS02007HS(S02007ViewModel item1, S02007ViewModel item2)
        {
            //jika value sama return true, jika value beda return false
            if (item1 == null && item2 == null)
                return true;
            else if ((item1 != null && item2 == null) || (item1 == null && item2 != null))
                return false;

            var PONUMBER1 = !string.IsNullOrEmpty(item1.PONUMBER) ? item1.PONUMBER : "";
            var BILLINGNO1 = !string.IsNullOrEmpty(item1.BILLINGNO) ? item1.BILLINGNO : "";
            var INVOICERECEIPTDATE1 = item1.INVOICERECEIPTDATE != null ? item1.INVOICERECEIPTDATE : Convert.ToDateTime("01/01/1900");
            var PAYPLAN1 = item1.payPlan != null ? item1.payPlan : Convert.ToDateTime("01/01/1900");


            var PONUMBER2 = !string.IsNullOrEmpty(item2.PONUMBER) ? item2.PONUMBER : "";
            var BILLINGNO2 = !string.IsNullOrEmpty(item2.BILLINGNO) ? item2.BILLINGNO : "";
            var INVOICERECEIPTDATE2 = item2.INVOICERECEIPTDATE != null ? item2.INVOICERECEIPTDATE : Convert.ToDateTime("01/01/1900");
            var PAYPLAN2 = item2.payPlan != null ? item2.payPlan : Convert.ToDateTime("01/01/1900");

            return PONUMBER1.Equals(PONUMBER2) &&
                BILLINGNO1.Equals(BILLINGNO2) &&
                INVOICERECEIPTDATE1.Equals(INVOICERECEIPTDATE2) &&
                PAYPLAN1.Equals(PAYPLAN2);
        }

        public string GetKodeCabangAI(string poNumber)
        {
            string result;
            try
            {
                EntityCommand cmd = new EntityCommand("EProcEntities.sp_GetKodeCabangAI", entityConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("PONUMBER", DbType.String).Value = poNumber;
                OpenConnection();
                result = Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                result = "";
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public int InsertLogTransactionS02007(List<CUSTOM_S02007_TEMP> listTempHS)
        {
            int result = 0;
            try
            {
                if (listTempHS.Count > 0)
                {
                    //insert into CUSTOM_TRANSACTION
                    CUSTOM_TRANSACTION transaction = new CUSTOM_TRANSACTION();
                    transaction.TicketNo = "";
                    transaction.ClientTag = "";
                    EntityHelper.SetAuditForInsert(transaction, "SERA");
                    entities.CUSTOM_TRANSACTION.AddObject(transaction);

                    var countListTempHS = listTempHS.Count;
                    //var countListTempIS = listTempIS.Count;

                    for (int i = 0; i < listTempHS.Count; i++)
                    {
                        //insert into CUSTOM_TRANSACTIONDATA
                        CUSTOM_TRANSACTIONDATA transactionData = new CUSTOM_TRANSACTIONDATA();
                        transactionData.CUSTOM_TRANSACTION = transaction;
                        transactionData.TransGUID = Guid.NewGuid().ToString();
                        transactionData.DocumentNumber = listTempHS[i].PONUMBER;
                        transactionData.FileType = "S02007";
                        transactionData.IPAddress = "118.97.80.12"; //IP ADDRESS KOMP SERVER, dan HARUS TERDAFTAR DI DB AI
                        transactionData.DestinationUser = "AI";
                        //transactionData.Key1 = "0002"; //company toyota
                        transactionData.Key1 = listTempHS[i].CompanyCodeAI;
                        transactionData.Key2 = GetKodeCabangAI(listTempHS[i].PONUMBER); //trial: kode cabang AI
                        transactionData.Key3 = listTempHS[i].BILLINGNO; //BILLINGNO
                        transactionData.DataLength = null;
                        transactionData.RowStatus = "";
                        EntityHelper.SetAuditForInsert(transactionData, "SERA");
                        entities.CUSTOM_TRANSACTIONDATA.AddObject(transactionData);

                        //CHECK IF DATA HS BY PONUMBER SUDAH ADA, DELETE DULU BY ID, supaya tidak redundant ponumber nya
                        var poNumb = listTempHS[i].PONUMBER;
                        var query = (from o in entities.CUSTOM_S02007
                                     where o.PONUMBER == poNumb
                                     select o).ToList();
                        if (query.Count > 0)
                        {
                            for (int d = 0; d < query.Count; d++)
                            {
                                //delete
                                var delID = query[d].ID;
                                CUSTOM_S02007 delHS = entities.CUSTOM_S02007.Single(o => o.ID == delID);
                                entities.CUSTOM_S02007.DeleteObject(delHS);
                            }
                        }

                        //insert into CUSTOM_S02007
                        CUSTOM_S02007 DataDetailHS = new CUSTOM_S02007();
                        DataDetailHS.CUSTOM_TRANSACTIONDATA = transactionData;
                        DataDetailHS.PONUMBER = listTempHS[i].PONUMBER;
                        DataDetailHS.VERSIONPOSERA = listTempHS[i].VERSIONPOSERA;
                        DataDetailHS.BILLINGNO = listTempHS[i].BILLINGNO;
                        DataDetailHS.INVOICERECEIPTDATE = listTempHS[i].INVOICERECEIPTDATE;
                        DataDetailHS.DATAVERSION = listTempHS[i].DATAVERSION;
                        DataDetailHS.payPlan = listTempHS[i].payPlan;
                        //start add identitas penambahan row CUSTOM_S02007 : by fhi 05.06.2014
                        DataDetailHS.dibuatOleh = "system";
                        DataDetailHS.dibuatTanggal = DateTime.Now;
                        DataDetailHS.diubahOleh = "system";
                        DataDetailHS.diubahTanggal = DateTime.Now;
                        //end
                        entities.CUSTOM_S02007.AddObject(DataDetailHS);

                        //build HS separator
                        var strHS = ConcateStringHSS02007(listTempHS[i]);

                        //insert into CUSTOM_TRANSACTIONDATADETAIL for HS
                        CUSTOM_TRANSACTIONDATADETAIL transactionDataDetail = new CUSTOM_TRANSACTIONDATADETAIL();
                        transactionDataDetail.CUSTOM_TRANSACTIONDATA = transactionData;
                        transactionDataDetail.Data = strHS;
                        //start add identitas penambahan row CUSTOM_S02007 : by fhi 05.06.2014
                        transactionDataDetail.dibuatOleh = "system";
                        transactionDataDetail.dibuatTanggal = DateTime.Now;
                        transactionDataDetail.diubahOleh = "system";
                        transactionDataDetail.diubahTanggal = DateTime.Now;
                        //end
                        entities.CUSTOM_TRANSACTIONDATADETAIL.AddObject(transactionDataDetail);
                    }
                    entities.SaveChanges();
                    //todo: delete temp table
                    for (int y = 0; y < countListTempHS; y++)
                    {
                        DeleteTempHSS02007(listTempHS[y]);
                    }
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public string ConcateStringHSS02007(CUSTOM_S02007_TEMP HS)
        {
            StringBuilder strHS = new StringBuilder(1000);
            strHS.Append("HS|");
            strHS.Append(HS.PONUMBER);
            strHS.Append("|");
            strHS.Append(HS.VERSIONPOSERA);
            strHS.Append("|");
            strHS.Append(HS.BILLINGNO);
            strHS.Append("|");
            strHS.Append(HS.INVOICERECEIPTDATE == null ? "19000101" : string.Format("{0:yyyyMMdd}", HS.INVOICERECEIPTDATE));
            strHS.Append("|");
            strHS.Append(HS.DATAVERSION);
            strHS.Append("|");
            strHS.Append(HS.payPlan == null ? "19000101" : string.Format("{0:yyyyMMdd}", HS.payPlan));


            return strHS.ToString();
        }

        public string ConcateStringHSS02007(S02007ViewModel HS)
        {
            StringBuilder strHS = new StringBuilder(1000);
            strHS.Append("HS|");
            strHS.Append(HS.PONUMBER);
            strHS.Append("|");
            strHS.Append(HS.VERSIONPOSERA);
            strHS.Append("|");
            strHS.Append(HS.BILLINGNO);
            strHS.Append("|");
            strHS.Append(HS.INVOICERECEIPTDATE == null ? "19000101" : string.Format("{0:yyyyMMdd}", HS.INVOICERECEIPTDATE));
            strHS.Append("|");
            strHS.Append(HS.DATAVERSION);
            strHS.Append("|");
            strHS.Append(HS.payPlan == null ? "19000101" : string.Format("{0:yyyyMMdd}", HS.payPlan));

            return strHS.ToString();
        }

        public int DeleteTempHSS02007(CUSTOM_S02007_TEMP tempHS)
        {
            int result = 1;
            try
            {
                EntityCommand cmd = new EntityCommand("EProcEntities.sp_DeleteTempHSS02007", entityConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("PONUMBER", DbType.String).Value = tempHS.PONUMBER;
                OpenConnection();
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                result = 0;

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public int DeleteAllTempHSS02007()
        {
            int result = 1;
            try
            {
                EntityCommand cmd = new EntityCommand("EProcEntities.sp_DeleteAllTempHSS02007", entityConnection);
                cmd.CommandType = CommandType.StoredProcedure;
                OpenConnection();
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                result = 0;

                throw ex;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }
        
        public TransactionViewModel GetTransactionS02007()
        {
            TransactionViewModel transaction = null;
            try
            {
                DateTime dateNow = DateTime.Now.Date;
                transaction = (from h in entities.CUSTOM_TRANSACTION
                               join d in entities.CUSTOM_TRANSACTIONDATA
                               on h.ID equals d.TransactionID
                               where d.FileType == "S02007" && h.CreatedWhen >= dateNow
                               select new TransactionViewModel()
                               {
                                   ID = h.ID
                               }).OrderByDescending(z => z.ID).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return transaction;
        }
        
        public List<S02007ViewModel> GetTransactionDataDetailHSS02007(int? transactionDataID)
        {
            List<S02007ViewModel> dataHS = null;
            try
            {
                dataHS = (entities.CUSTOM_S02007
                          .Where(o => o.TransactionDataID == transactionDataID)
                          .Select(o => new S02007ViewModel
                          {
                              ID = o.ID,
                              TransactionDataID = o.TransactionDataID == null ? null : o.TransactionDataID,
                              PONUMBER = o.PONUMBER,
                              VERSIONPOSERA = o.VERSIONPOSERA,
                              BILLINGNO = o.BILLINGNO,
                              INVOICERECEIPTDATE = o.INVOICERECEIPTDATE,
                              DATAVERSION = o.DATAVERSION,
                              payPlan=o.payPlan
                          }).ToList());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataHS;
        }

        public wsB2B.TransactionData[] GetTransactionDataArray(int? transactionID)
        {
            wsB2B.TransactionData[] transactionData = null;
            try
            {
                transactionData = (from o in entities.CUSTOM_TRANSACTIONDATA
                                   //join p in entities.CUSTOM_S02001_HS
                                   //on o.ID equals p.TransactionDataID
                                   //join q in entities.CUSTOM_S02001_IS
                                   //on o.ID equals q.TransactionDataID
                                   //join s in entities.CUSTOM_TRANSACTIONDATADETAIL
                                   //on o.ID equals s.TransactionDataID
                                   where o.TransactionID == transactionID
                                   select new wsB2B.TransactionData
                                   {
                                       ID = o.ID,
                                       TransGUID = o.TransGUID,
                                       DocumentNumber = o.DocumentNumber,
                                       FileType = o.FileType,
                                       IPAddress = o.IPAddress,
                                       DestinationUser = o.DestinationUser,
                                       Key1 = o.Key1,
                                       Key2 = o.Key2,
                                       Key3 = o.Key3,
                                       //DataLength = 
                                       //Data = ConcateStringHS()//new ArrayOfString { s.Data, "","" }
                                   }).ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return transactionData;
        }

        #endregion

        #endregion
    }
}
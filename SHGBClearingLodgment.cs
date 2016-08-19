using System;
//UPGRADE_TODO: The package 'shgn' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using shgn;
//UPGRADE_TODO: The type 'shgn.SHGNDateUtil' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using SHGNDateUtil = shgn.SHGNDateUtil;
//UPGRADE_TODO: The type 'shma.enterprise.shared.EnvHelper' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using EnvHelper = SHMA.Enterprise.Shared.EnvHelper;
//UPGRADE_TODO: The type 'shma.enterprise.NameValueCollection' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using NameValueCollection = SHMA.Enterprise.NameValueCollection;
//UPGRADE_TODO: The package 'shma.enterprise.data' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using SHMA.Enterprise.Data;
//UPGRADE_TODO: The package 'shma.bank.gnbk' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using shma.bank.gnbk;
//UPGRADE_TODO: The type 'shma.enterprise.exceptions.ProcessException' could not be found. If it was not included in the conversion, there may be compiler issues. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1262_3"'
using ProcessException = SHMA.Enterprise.Exceptions.ProcessException;
using shma.bank.shgc;
namespace shgb
{
	
	public class SHGBClearingLodgment:ProcessCommand
	{
		private ParameterCollection pDailyTransactionColl = new ParameterCollection();
		private System.String Str_Condition = "A", Str_Message = null;
		private System.DateTime tempDt;
		private System.Collections.ArrayList ArrList = null;
		
		public override System.String processing()
		{
			System.Console.Out.WriteLine("HAQ in the class SHGBDailyTransaction");
			
			SHGBTranCreationUtil TranCreationUtil = new SHGBTranCreationUtil();
			System.Collections.Hashtable gHashTable = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
			//SHPRBalValidityUtil BalanceValidity = new SHPRBalValidityUtil();
			SHGNGetGlobalPara globalPara = new SHGNGetGlobalPara();
			SHGBChqInstValidation gInstValidation = new SHGBChqInstValidation();
			SHGBAuthorityCriteria gAuthorityCriteria = new SHGBAuthorityCriteria();
			EnvHelper gEnvHlp = new EnvHelper();
			SHGNDateUtil DateUtil = new SHGNDateUtil();
			SHGBCalculationMethods gCalcMeth = new SHGBCalculationMethods();
			
			
			System.Object Obj_Reason = null, Obj_Exempt = null, Obj_CurrCode = null;
			System.Object Obj_Bank=null, Obj_Branch=null;
			System.String Str_Bank=null, Str_Branch=null,Str_respTranCode = null;
			System.String Str_Reason = null, Str_Exempt = "N", Str_RsnChrg = null, Str_rsnChrgApply = "Y", Str_vchFlag = null;
			System.String str_OrgaCode = null, str_LocaCode = null, str_Product = null, str_SchemCod = null, str_Number = null, str_TranCode = null;
			System.String str_Narration = null, str_CurrCode = null, str_PetExtCd = null, str_CrtUsr = null, str_PvtVchTyp = null, str_SysCode = null;
			System.String str_InstCode = null, str_InstNo = null, str_Error = null, str_Sign = null, str_FieldComb = null, str_ValueComb = null, sGlaCodeDR = null, sGlaCodeCR = null;
			System.String str_GntrDate = null, str_ValueDate = null, str_CreatDat = null, str_InstDate = null, str_gntrNum = null, str_Sus_Level = null;
			System.String str_LocaCod_Ref = null, str_OrgaCod_Ref = null, Str_creator = null;
			//UPGRADE_TODO: The 'System.DateTime' structure does not have an equivalent to NULL. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1291_3"'
			System.DateTime dt_GntrDate = tempDt, dt_ValueDate = tempDt, dt_CreatDat = tempDt, dt_InstDate = tempDt;
			//UPGRADE_TODO: The 'System.DateTime' structure does not have an equivalent to NULL. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1291_3"'
			System.DateTime dt_GntrDateSql = tempDt, dt_ValueDateSql = tempDt, dt_CreatDatSql = tempDt, dt_InstDateSql = tempDt, dt_SysDate = tempDt;
			
			//StringBuffer Stb_ShowAlert = new StringBuffer();
			
			double dbl_FcAmt = 0, dbl_BcAmt = 0, dbl_ExchRate = 1;
			System.Object Dbl_FcAmt = null, Dbl_ExchRate = null, Dbl_GntrNum = null;
			int int_TranNo = 0, int_gntrNum = 0, int_CountWith = 0;
			System.Object Obj_InsDate = null;
            double dbl_chargeAmt = 0;
			dt_SysDate = SHGNDateUtil.getDate();
			
			NameValueCollection[] nvDt = this.getDataRows();
			
			for (int r = 0; r < this.getSelectedRows().Length; r++)
			{
				if (this.getSelectedRows()[r])
				{
					str_OrgaCode    = ((System.String) nvDt[r].getObject("POR_ORGACODE"));
					str_LocaCode    = ((System.String) nvDt[r].getObject("PLC_LOCACODE"));
					str_LocaCod_Ref = ((System.String) nvDt[r].getObject("PLC_LOCACODE_REF"));
					str_OrgaCod_Ref = ((System.String) nvDt[r].getObject("POR_ORGACODE_REF"));
					str_Product     = (nvDt[r].getObject("DMP_PRODCODE") == null?"":(System.String) nvDt[r].getObject("DMP_PRODCODE"));
					str_SchemCod    = (nvDt[r].getObject("DCS_SCHEMCODE") == null?"":(System.String) nvDt[r].getObject("DCS_SCHEMCODE"));
					str_Number      = (nvDt[r].getObject("MBM_NUMBER") == null?"":(System.String) nvDt[r].getObject("MBM_NUMBER"));
					str_TranCode    = ((System.String) nvDt[r].getObject("PTR_TRANCODE"));
					str_Narration   = ((System.String) nvDt[r].getObject("MGT_NARRATION"));
				    str_CurrCode    = ((System.String) nvDt[r].getObject("PCR_CURRCODE"));
					Obj_CurrCode    = ((System.String) nvDt[r].getObject("PCR_CURRCODE"));
					str_PetExtCd    = ((System.String) nvDt[r].getObject("PET_EXRTCODE"));
					str_CrtUsr      = ((System.String) gEnvHlp.getAttribute("s_SUS_USERCODE"));
					Str_creator     = ((System.String) nvDt[r].getObject("MGT_CREATEUSR"));
					str_PvtVchTyp   = ((System.String) nvDt[r].getObject("PVT_VCHTTYPE"));
					str_SysCode     = ((System.String) nvDt[r].getObject("PSY_SYSTCODE"));
					str_InstCode    = ((System.String) nvDt[r].getObject("PIT_INSTCODE"));
					str_InstNo      = ((System.String) nvDt[r].getObject("MGT_INSTRUMENTNO"));
					dt_GntrDate     = (System.DateTime) nvDt[r].getObject("MGT_GNTRDATE");
					dt_ValueDate    = (System.DateTime) nvDt[r].getObject("MGT_VALUEDAT");
					dt_CreatDat     = (System.DateTime) nvDt[r].getObject("MGT_CREATEDAT");
					dt_InstDate     = (System.DateTime) nvDt[r].getObject("MGT_INSTRDATE");

					Obj_Bank= nvDt[r].getObject("PBN_BNBRCODE_DESBK");
					Obj_Branch= nvDt[r].getObject("PBN_BNBRCODE_DESBN");

					if (Obj_CurrCode != null && Obj_CurrCode.ToString().Trim().Length > 0)
						str_CurrCode = ((System.String) Obj_CurrCode);
					else
						throw new ProcessException("currenct code is going to be null for transaction:"+str_LocaCode+", "+dt_GntrDate+", "+nvDt[r].getObject("MGT_GNTRNUMBER"));
					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
					if (Obj_Bank != null && Obj_Bank.ToString().Trim().Length > 0)
						Str_Bank = ((System.String) Obj_Bank);
					else
						Str_Bank ="";

					if (Obj_Branch != null && Obj_Branch.ToString().Trim().Length > 0)
						Str_Branch = ((System.String) Obj_Branch);
					else 
						Str_Branch="";
					
					if (nvDt[r].getObject("MGT_CHRGSKIP") != null && nvDt[r].getObject("MGT_CHRGSKIP").ToString().Trim().Length > 0)
						Obj_Exempt = nvDt[r].getObject("MGT_CHRGSKIP");
					else
						Obj_Exempt = "";
					
					Obj_Reason = nvDt[r].getObject("PRT_RESNCODE");
					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
					if (Obj_Exempt != null & Obj_Exempt.ToString().Trim().Length > 0)
						Str_Exempt = ((System.String) Obj_Exempt);
					else
						Str_Exempt = "N";
					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
					if (Obj_Reason != null && Obj_Reason.ToString().Trim().Length > 0)
						Str_Reason = ((System.String) Obj_Reason);
					else
						Str_Reason = null;
					
					if (Str_Reason != null && Str_Reason.Trim().Length > 0)
						Str_rsnChrgApply = gCalcMeth.srcRsnType(Str_Reason);
					else
						Str_rsnChrgApply = "Y";
					
					//UPGRADE_TODO: Constructor 'java.sql.Date.Date' was converted to 'System.DateTime.DateTime' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javasqlDateDate_long_3"'
					//UPGRADE_TODO: Method 'java.util.Date.getTime' was converted to 'System.DateTime.Ticks' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilDategetTime_3"'
					dt_GntrDateSql = dt_GntrDate;
					
					//UPGRADE_TODO: Constructor 'java.sql.Date.Date' was converted to 'System.DateTime.DateTime' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javasqlDateDate_long_3"'
					//UPGRADE_TODO: Method 'java.util.Date.getTime' was converted to 'System.DateTime.Ticks' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilDategetTime_3"'
					dt_ValueDateSql = dt_ValueDate;
					//UPGRADE_TODO: Constructor 'java.sql.Date.Date' was converted to 'System.DateTime.DateTime' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javasqlDateDate_long_3"'
					//UPGRADE_TODO: Method 'java.util.Date.getTime' was converted to 'System.DateTime.Ticks' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilDategetTime_3"'
					dt_CreatDatSql = dt_CreatDat;
					
					//UPGRADE_TODO: The 'System.DateTime' structure does not have an equivalent to NULL. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1291_3"'
					if (dt_InstDate.ToString() != null)
					{
						//UPGRADE_TODO: Constructor 'java.sql.Date.Date' was converted to 'System.DateTime.DateTime' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javasqlDateDate_long_3"'
						//UPGRADE_TODO: Method 'java.util.Date.getTime' was converted to 'System.DateTime.Ticks' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilDategetTime_3"'
						dt_InstDateSql = dt_InstDate;
					}
					
					Dbl_GntrNum = nvDt[r].getObject("MGT_GNTRNUMBER");
					Dbl_ExchRate = nvDt[r].getObject("MGT_EXCHGRATE");
					
					Dbl_FcAmt = nvDt[r].getObject("MGT_AMTFC");
					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
                    if (Dbl_ExchRate != null && Dbl_ExchRate.ToString().Trim().Length > 0)
                    {
                        //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
                        dbl_ExchRate = System.Double.Parse(Dbl_ExchRate.ToString()); //.doubleValue();
                    }
                    else
                    {
                        dbl_ExchRate = 1;
                    }
					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
					if (Dbl_FcAmt != null && Dbl_FcAmt.ToString().Trim().Length > 0)
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
						dbl_FcAmt = System.Double.Parse(Dbl_FcAmt.ToString()); //.doubleValue();
					}
					
					dbl_BcAmt = dbl_FcAmt * dbl_ExchRate;


					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
					if (Dbl_GntrNum != null && Dbl_GntrNum.ToString().Trim().Length > 0)
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
						if (Dbl_GntrNum.ToString().LastIndexOf((System.Char) '.') > - 1)
						{
							//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
							int_gntrNum = System.Int32.Parse(Dbl_GntrNum.ToString().Substring(0, (Dbl_GntrNum.ToString().LastIndexOf((System.Char) '.')) - (0)));
						}
						//(int)Dbl_GntrNum.doubleValue();          
						else
						{
							//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
							int_gntrNum = System.Int32.Parse(Dbl_GntrNum.ToString());
						} //(int)Dbl_GntrNum.doubleValue();                        
					}
					
					int_TranNo = int_gntrNum;


                    /*Extra Check for Base Currency Null handling*/

                    string CheckBase = " select COUNT(*) CNT from BN_MS_GT_GENERALTRANSACTION where POR_ORGACODE=? and PLC_LOCACODE=? and MGT_GNTRDATE=? and MGT_GNTRNUMBER=? and case when MGT_AMTBC is null then 0 else MGT_AMTBC end =0 ";
                    pDailyTransactionColl.clear();
                    pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
                    pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
                    pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
                    pDailyTransactionColl.puts("@mgt_gntrnumber", int_TranNo, Types.INTEGER);

                    rowset rsCheckBase = DB.executeQuery(CheckBase, pDailyTransactionColl);


                    if (rsCheckBase.next() && rsCheckBase.getInt("CNT") > 0)
                    {
                        CheckBase = " update BN_MS_GT_GENERALTRANSACTION set MGT_AMTBC=? where POR_ORGACODE=? and PLC_LOCACODE=? and MGT_GNTRDATE=? and MGT_GNTRNUMBER=? ";

                        pDailyTransactionColl.clear();
                        pDailyTransactionColl.puts("@MGT_AMTBC", dbl_BcAmt, Types.DOUBLE);

                        pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
                        pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
                        pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
                        pDailyTransactionColl.puts("@mgt_gntrnumber", int_TranNo, Types.INTEGER);
                        DB.executeDML(CheckBase, pDailyTransactionColl);
                    }


                    /*End*/
					
					System.Console.Out.WriteLine("HAQ tran no " + int_TranNo);
					
					System.String sUserLevel = "select sus_levelcode from sh_sm_us_user where sus_usercode = ? ";
					
					pDailyTransactionColl.clear();
					pDailyTransactionColl.puts("@sus_usercode", str_CrtUsr, Types.VARCHAR);
					
					rowset rUserLevel = DB.executeQuery(sUserLevel, pDailyTransactionColl);
					rUserLevel.next();
					
					str_Sus_Level = rUserLevel.getString("sus_levelcode");
					System.Console.Out.WriteLine("HAQ user level " + str_Sus_Level);


				//  10-12-2007 - getting voucher flag
					ArrList    = null;
					ArrList    = gCalcMeth.sourceTransactionType(str_TranCode);
					Str_vchFlag= (System.String) ArrList[14];
					
					gHashTable.Clear();
					gHashTable["POR_ORGACODE"] = str_OrgaCode;
					gHashTable["PLC_LOCACODE"] = str_LocaCode;
					gHashTable["MGT_GNTRDATE"] = dt_GntrDateSql;
					gHashTable["POR_ORGACODE_REF"] = str_OrgaCod_Ref;
					gHashTable["PLC_LOCACODE_REF"] = str_LocaCod_Ref;
					gHashTable["DMP_PRODCODE"] = (str_Product == null?"":str_Product);
					gHashTable["DCS_SCHEMCODE"] = (str_SchemCod == null?"":str_SchemCod);
					gHashTable["MBM_NUMBER"] = (str_Number == null?"":str_Number);
					gHashTable["PTR_TRANCODE"] = str_TranCode;
					gHashTable["PCR_CURRCODE"] = str_CurrCode;
					gHashTable["PIT_INSTCODE"] = ""; //str_InstCode == null?"":str_InstCode);
					gHashTable["SUS_LEVELCODE"] = str_Sus_Level;
					gHashTable["MGT_VALUEDAT"] = dt_ValueDateSql;
					gHashTable["PET_EXRTCODE"] = str_PetExtCd;
					gHashTable["MGT_EXCHGRATE"] = (double) dbl_ExchRate;
					gHashTable["MGT_AMTFC"] = (double) dbl_FcAmt;
					gHashTable["MGT_AMTBC"] = (double) dbl_BcAmt;
					gHashTable["PVT_VCHTTYPE"] = str_PvtVchTyp;
					gHashTable["PSY_SYSTCODE"] = str_SysCode;
					gHashTable["MGT_POSTUSR"] = str_CrtUsr;
					gHashTable["MGT_POSTDAT"] = dt_SysDate;
					gHashTable["PBN_BNBRCODE_DESBK"] = Str_Bank;
					gHashTable["PBN_BNBRCODE_DESBN"] = Str_Branch;
					gHashTable["MGT_GNTRNUMBER"] = int_gntrNum;
					gHashTable["PTR_PLUSMINACNTBAL"] = "N";
					gHashTable["MGT_CREATEUSR"] = str_CrtUsr;
					
					//UPGRADE_TODO: The equivalent in .NET for method 'java.sql.Date.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
					System.Console.Out.WriteLine("HAQ getting tran count for " + str_OrgaCode + "-" + str_LocaCode + "-" + dt_GntrDateSql + "-" + int_gntrNum);
					System.String sTransCheck = " select count(*) cn from bn_ms_gt_generaltransaction " + "  where por_orgacode = ? and plc_locacode = ? " + "   and  mgt_gntrdate = ? and mgt_gntrnumber = ? ";
					
					pDailyTransactionColl.clear();
					pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
					pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
					pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
					pDailyTransactionColl.puts("@mgt_gntrnumber", int_gntrNum, Types.INTEGER);
					
					System.Console.Out.WriteLine("HAQ getting current transaction");
					rowset rTransCheck = DB.executeQuery(sTransCheck, pDailyTransactionColl);
					rTransCheck.next();
					
					if (rTransCheck.getInt("cn") == 1)
					{
						Str_Condition = "B";
						System.Console.Out.WriteLine("HAQ getting actual transaction");
						if ((str_PvtVchTyp == null || str_PvtVchTyp.Equals("")) && (str_SysCode == null || str_SysCode.Equals("")))
						{
							throw new ProcessException(" must define voucher type and system id against Transaction Type ");
						}
						
						if (dbl_FcAmt <= 0)
						{
							throw new ProcessException("Transaction amount should not be Negative or Zero Value");
						}
						

						System.Console.Out.WriteLine("HAQ updating gen tran with GL, varifier and status info");
						sGlaCodeDR = TranCreationUtil.mGetGlCodeDR(str_OrgaCode, str_TranCode, gHashTable, pDailyTransactionColl);

					//for getting and inserting Profit Center Code
						String profitCentre = null;
						
						System.Collections.Hashtable Hash_RespCollection = shma.bank.shgc.TransactionTypeInfo.fsgetTransTypeInfo(str_TranCode);
						System.String Str_crcCriteria  = (System.String) Hash_RespCollection["PTR_CRCCRITERIA"];

						if(Str_crcCriteria.Equals("R"))
						{
							String Str_acntNature = shma.bank.shgc.AccountTypeInfo.fsgetAccountNature(str_OrgaCode,sGlaCodeDR);
				
							if((Str_acntNature != null) && (Str_acntNature.Equals("EXP") || Str_acntNature.Equals("REV")))
							{
								profitCentre = shma.bank.gnbk.generalMethods.getGlbValue("PROFITCNTR","NOMIACNT").Replace("'","");
							}
							else
							{
								profitCentre = shma.bank.gnbk.generalMethods.getGlbValue("PROFITCNTR","GENBANK").Replace("'","");
							}
						}
					//for getting and inserting Profit Center Code
						
						sGlaCodeCR = TranCreationUtil.mGetGlCodeCR(str_OrgaCode, str_TranCode, gHashTable, pDailyTransactionColl);
						System.String sUpdateTrn = " update bn_ms_gt_generaltransaction set pca_glaccode = ?, mgt_drcr = 'DR', " + "        mgt_verifyusr  = ?, mgt_verifydat = ?, " + "        mgt_status     = 'N', mgt_glvchflag = ?,PPF_PRFCCODE = ? " + "  where por_orgacode   = ? and plc_locacode = ? and mgt_gntrdate = ? " + "    and mgt_gntrnumber = ? and mgt_gntrsubnumber = 1 ";
						
						pDailyTransactionColl.clear();
						pDailyTransactionColl.puts("@pca_glaccode",  sGlaCodeDR, Types.VARCHAR);
						pDailyTransactionColl.puts("@mgt_verifyusr", str_CrtUsr, Types.VARCHAR);
						pDailyTransactionColl.puts("@mgt_verifydat", dt_CreatDatSql, Types.DATE);
						pDailyTransactionColl.puts("@mgt_glvchflag", Str_vchFlag, Types.VARCHAR);
						pDailyTransactionColl.puts("@PPF_PRFCCODE",  profitCentre,Types.VARCHAR);
						pDailyTransactionColl.puts("@por_orgacode",  str_OrgaCode, Types.VARCHAR);
						pDailyTransactionColl.puts("@plc_locacode",  str_LocaCode, Types.VARCHAR);
						pDailyTransactionColl.puts("@mgt_gntrdate",  dt_GntrDateSql, Types.DATE);
						pDailyTransactionColl.puts("@mgt_gntrnumber",int_TranNo, Types.INTEGER);
						
						DB.executeDML(sUpdateTrn, pDailyTransactionColl);
						
						System.Console.Out.WriteLine("HAQ creating gen tran serial 2");
						if (str_Product.Equals(""))
							str_Product = null;
						System.Console.Out.WriteLine("HAQ value for str_Product : " + str_Product);
						if (str_SchemCod.Equals(""))
							str_SchemCod = null;
						System.Console.Out.WriteLine("HAQ value for str_SchemCod : " + str_SchemCod);
						if (str_Number.Equals(""))
							str_Number = null;
						System.Console.Out.WriteLine("HAQ value for str_Number : " + str_Number);
						
						TranCreationUtil.setAdtCreatedUser(str_CrtUsr);
						TranCreationUtil.setAdtCreatedDate(SHMA.CodeVision.Business.DateUtil.getCurrentDateTime());

						TranCreationUtil.mGeneralTransaction(dt_GntrDateSql, int_TranNo, 2, str_OrgaCode, str_TranCode, str_LocaCode, str_Product, str_SchemCod, str_Number, dt_ValueDateSql, str_Narration, "CR", sGlaCodeCR, str_CurrCode, str_PetExtCd, dbl_ExchRate, dbl_BcAmt, dbl_FcAmt, 0, 0, str_CrtUsr, dt_CreatDatSql, null, "N", str_CrtUsr, dt_CreatDatSql, str_PvtVchTyp, str_SysCode, null, null, tempDt, str_OrgaCod_Ref, str_LocaCod_Ref, pDailyTransactionColl);
						System.Console.Out.WriteLine("HAQ transaction created");
					}
					
					System.Console.Out.WriteLine("HAQ in str_ClrDep str_ClrDepGL");
					System.String strCountDetail = " select Count(*) cn,sum(mtd_instamtfc) amt from BN_MS_TD_GENERALTRANDETAIL " + "  where  por_orgacode = ? and plc_locacode = ? " + "    and  mgt_gntrdate = ? and mgt_gntrnumber = ? ";
					
					pDailyTransactionColl.clear();
					pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
					pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
					pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
					pDailyTransactionColl.puts("@mgt_gntrnumber", int_TranNo, Types.INTEGER);
					
					rowset rCountDetail = DB.executeQuery(strCountDetail, pDailyTransactionColl);
					rCountDetail.next();
					
					if (rCountDetail.getInt("cn") > 0)
					{
						if (rCountDetail.getDouble("amt") != dbl_FcAmt)
						{
							throw new ProcessException(" Clearing deposit slip amount is not matching with the detail amount. ");
						}
						
						//*****************************************************************************************************************
						//*** updating transaction detail with cust a/c number ************************************************************
						System.Console.Out.WriteLine("HAQ * * * code start for Tran Gen Detail updatation with a/c no");
						System.String strGetDepAcnt = " select POR_ORGACODE_REF, PLC_LOCACODE_REF, " + " DMP_PRODCODE, DCS_SCHEMCODE, MBM_NUMBER " + " from bn_ms_gt_generaltransaction " + " where  por_orgacode = ? and plc_locacode = ? " + "  and   mgt_gntrdate = ? and mgt_gntrnumber = ? ";
						//UPGRADE_TODO: The equivalent in .NET for method 'java.sql.Date.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
						System.Console.Out.WriteLine("HAQ strGetDepAcnt " + strGetDepAcnt + " " + str_OrgaCode + " " + str_LocaCode + " " + dt_GntrDateSql + " " + int_TranNo);
						
						pDailyTransactionColl.clear();
						pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
						pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
						pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
						pDailyTransactionColl.puts("@mgt_gntrnumber", int_TranNo, Types.INTEGER);
						
						rowset rGetDepAcnt = DB.executeQuery(strGetDepAcnt, pDailyTransactionColl);

						if (rGetDepAcnt.next())
						{
							System.String strUpdateTranDetail = " update  BN_MS_TD_GENERALTRANDETAIL " + " set  POR_ORGACODE_REF = ?, PLC_LOCACODE_REF = ? ," + " DMP_PRODCODE_REF = ?, DCS_SCHEMCODE_REF = ?,  MBM_NUMBER_REF = ? " + " where  por_orgacode = ? and plc_locacode = ? " + " and  mgt_gntrdate = ? and mgt_gntrnumber = ? ";
							
							//UPGRADE_TODO: The equivalent in .NET for method 'java.sql.Date.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
							System.Console.Out.WriteLine("HAQ detail tran upd data " + strUpdateTranDetail + "-" + rGetDepAcnt.getString(1) + "-" + rGetDepAcnt.getString(2) + "-" + rGetDepAcnt.getString(3) + "-" + rGetDepAcnt.getString(4) + "-" + rGetDepAcnt.getString(5) + "-" + str_OrgaCode + "-" + str_LocaCode + "-" + dt_GntrDateSql + "-" + int_TranNo);
							
							pDailyTransactionColl.clear();
							pDailyTransactionColl.puts("@POR_ORGACODE_REF", rGetDepAcnt.getString(1), Types.VARCHAR);
							pDailyTransactionColl.puts("@PLC_LOCACODE_REF", rGetDepAcnt.getString(2), Types.VARCHAR);
							pDailyTransactionColl.puts("@DMP_PRODCODE_REF", rGetDepAcnt.getString(3), Types.VARCHAR);
							pDailyTransactionColl.puts("@DCS_SCHEMCODE_REF", rGetDepAcnt.getString(4), Types.VARCHAR);
							pDailyTransactionColl.puts("@MBM_NUMBER_REF", rGetDepAcnt.getString(5), Types.VARCHAR);
							pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
							pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
							pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
							pDailyTransactionColl.puts("@mgt_gntrnumber", int_TranNo, Types.INTEGER);
							
							DB.executeDML(strUpdateTranDetail, pDailyTransactionColl);
							System.Console.Out.WriteLine("HAQ tran detail updated successfully");
						}
						System.Console.Out.WriteLine("HAQ x x x code end for Tran Gen Detail updatation");
						//*****************************************************************************************************************
					}
					else
					{
						if (Str_Condition.Equals("A"))
							Str_Message = " Transaction " + int_TranNo + " saved; but Detail Required;";
						else
							Str_Message = " Transaction " + int_TranNo + "; but Detail Required;";
						
						return Str_Message; //+ BalanceValidity.getMessValidity() + gInstValidation.getInsMessage();
					}

				//this method is for temporary authorization for hiding transaction from main screen - 17-09-2007
					shma.bank.shgc.GenTransactionInfo.fsReadyForAuthUpd(str_OrgaCode,str_LocaCode,dt_GntrDateSql,int_TranNo);
					
					System.Console.Out.WriteLine("HAQ checking authority criteria for " + str_SysCode + "-" + str_TranCode + "-" + dbl_FcAmt);
				
				//Authority Criteria - new functionality other than PKR (Audit Objection) - 18-03-2010//
				    gAuthorityCriteria.isValueExists(true);	
					gAuthorityCriteria.setAmtType("BC");//authority criteria amount type BC/FC
					gAuthorityCriteria.setFCAmount(dbl_FcAmt);//authority criteria amount FC
					gAuthorityCriteria.setBCAmount(dbl_BcAmt);//authority criteria amount BC
				//new functionality other than PKR (Audit Objection)//

					if (!gAuthorityCriteria.getAuthorityCriteria(str_SysCode, str_TranCode, gHashTable, dbl_FcAmt))
					{
						if (Str_Condition.Equals("A"))
							Str_Message = " Transaction " + int_TranNo + " saved; but Authorization Required;";
						else
							Str_Message = " Transaction " + int_TranNo + "; but Authorization Required;";
						
						return Str_Message; //+ BalanceValidity.getMessValidity() + gInstValidation.getInsMessage();
					}
					
					System.Console.Out.WriteLine("HAQ checking authority criteria cleared");
					
					if ((str_Product != null && str_Product.Trim().Length > 0) && (str_SchemCod != null && str_SchemCod.Trim().Length > 0))
					{
						System.Text.StringBuilder sSchColValue = new System.Text.StringBuilder();
						sSchColValue.Remove(0, sSchColValue.Length - 0);
						sSchColValue.Append(" select case when DCS_SUBACCOUNT is null then 'N' else DCS_SUBACCOUNT end SubAcnt ");
						sSchColValue.Append("   from bn_pd_sc_scheme ");
						sSchColValue.Append("  where dmp_prodcode = ? and dcs_schemcode = ? ");
					
						pDailyTransactionColl.clear();
						pDailyTransactionColl.puts("@dmp_prodcode", str_Product);
						pDailyTransactionColl.puts("@dcs_schemcode", str_SchemCod);
					
						rowset rSchValue = DB.executeQuery(sSchColValue.ToString(), pDailyTransactionColl);
						rSchValue.next();
					
						if (rSchValue.getString("SubAcnt").Equals("Y"))
						{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.sql.Date.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043_3"'
							System.Console.Out.WriteLine("HAQ creating sub account for : " + str_OrgaCod_Ref + "-" + str_LocaCod_Ref + "-" + str_Product + "-" + str_SchemCod + "-" + str_Number + "-" + dt_GntrDateSql + "-" + dbl_FcAmt + "-" + str_TranCode + "-" + int_TranNo);
							SubAcntTransaction.subAcntTran(str_OrgaCod_Ref, str_LocaCod_Ref, str_Product, str_SchemCod, str_Number, dt_GntrDateSql, (double) dbl_FcAmt, str_TranCode, int_TranNo);
						}
					
						//10-04-2006 - chargesDeduction on Transaction Type
						//if (Str_Exempt.Equals("N") && Str_rsnChrgApply.Equals("Y")) -- it has been closed because of applying exempt functionality - 22-04-2009
						
                        //Block as per qa feroze bhai said[29032016] 
                        //if (Str_rsnChrgApply.Equals("Y"))
                        //Work Exampt Charges On Screen [29032016-90266]
                        if (Str_Exempt.Equals("N"))
						{
							if(!str_LocaCode.Equals(str_LocaCod_Ref))
							{
								gHashTable["CMP_BRANCHCUST"] = "N";
							}
							else
							{
								gHashTable["CMP_BRANCHCUST"] = "Y";	
							}

                            /*
							    gnbkTransactionChargesDed chargesDed = new gnbkTransactionChargesDed();
							
							    chargesDed.setExemptCharge(Str_Exempt);
							    chargesDed.mainProcess(TranCreationUtil,true,gHashTable);
							*/
                              
                            //start - new method here for charges By Taha

                            System.Collections.Hashtable HasCollection = shma.bank.shgc.TransactionTypeInfo.fsgetTransTypeInfo(str_TranCode);
                       
                            //**********System Code, VoucherType and PlusMinus sign on the basis of responding transaction code********//
                            System.String Str_systemCode   = (System.String)HasCollection["PSY_SYSTCODE"];
                            System.String Str_voucherType  = (System.String)HasCollection["PVT_VCHTTYPE"];
                            System.String Str_narration    = (System.String)HasCollection["PTR_NARRATION"];
                            System.String Str_PlusMinus    = (System.String)HasCollection["PTR_PLUSMINACNTBAL"];
                            System.String Str_vouchFlag    = (System.String)HasCollection["PTR_GLVCHFLAG"];
                            System.String Str_chrgApplied  = (System.String)HasCollection["PTR_CHRGAPPLICABLE"];
                            System.String Str_chrgTranCode = (System.String)HasCollection["PTR_CHRGTRANTYPE"];
                            System.String Str_crcCriteria  = (System.String)HasCollection["PTR_CRCCRITERIA"];
                            System.String str_LogOnApply   = (System.String)HasCollection["PTR_CHRGLOGAPPLY"];
                            if (Str_chrgApplied.Equals("Y"))
                            {
                                TransactionVariables Transactionvariables = new TransactionVariables();
                                System.String Str_chargeProd_2 = str_Product != null && str_Product.Trim().Length > 0 ? str_Product : Transactionvariables.getProdCode();
                                System.String Str_chargeScheme_2 = str_SchemCod != null && str_SchemCod.Trim().Length > 0 ? str_SchemCod : Transactionvariables.getSchCode();


                                Transactionvariables.setOrgaCode(str_OrgaCode);
                                Transactionvariables.setLocaCode(str_LocaCode);
                                Transactionvariables.setTransDate(dt_SysDate);
                                Transactionvariables.setValueDate(dt_ValueDate);
                                Transactionvariables.setOrgaCodeRef(str_OrgaCod_Ref);
                                Transactionvariables.setLocaCodeRef(str_LocaCod_Ref);
                                Transactionvariables.setProdCode(str_Product);
                                Transactionvariables.setSchCode(str_SchemCod);
                                Transactionvariables.setAcntNumber(str_Number);
                                Transactionvariables.setTransCode(str_TranCode);
                                Transactionvariables.setSystemCode(Str_systemCode);
                                Transactionvariables.setVoucherType(Str_voucherType);
                                Transactionvariables.setCurrCode(str_CurrCode);
                                Transactionvariables.setExchangeRate(dbl_ExchRate == 0.0 ? 1 : dbl_ExchRate);
                                Transactionvariables.setFCAmount(dbl_FcAmt);
                                Transactionvariables.setBCAmount(dbl_BcAmt);
                                Transactionvariables.setCreateDate(dt_SysDate);
                                Transactionvariables.setVerifyDate(dt_SysDate);
                                Transactionvariables.setPostDate(dt_SysDate);
                                Transactionvariables.setNarration(Str_narration);
                                Transactionvariables.setGLVchFlag(Str_vouchFlag);
                                Transactionvariables.setExchangeType(str_PetExtCd);
                                Transactionvariables.setPlusMinusSign(Str_PlusMinus);
                                Transactionvariables.setBMPost("P");
                                Transactionvariables.setStatus("P");
                                Transactionvariables.setAdtCreatedUser(str_CrtUsr);
                                Transactionvariables.setAdtCreatedDate(SHMA.CodeVision.Business.DateUtil.getCurrentDateTime());
                                Transactionvariables.setAdtModifiedUser(str_CrtUsr);
                                Transactionvariables.setAdtModifiedDate(SHMA.CodeVision.Business.DateUtil.getCurrentDateTime());
                                Transactionvariables.setChargeProdCode(Str_chargeProd_2);
                                Transactionvariables.setChargeSchemCode(Str_chargeScheme_2);
                                Transactionvariables.setCalculationAmountFC(dbl_FcAmt);
                                Transactionvariables.setCalculationAmountBC(dbl_BcAmt);
                                Transactionvariables.setProcessDate(dt_SysDate);
                                Transactionvariables.setChargeTranCode2(Str_chrgTranCode);
                                Transactionvariables.setShortBalanceCharge(str_LogOnApply);
                                Transactionvariables.setHashCollection(gHashTable);

                                dbl_chargeAmt = ChargesTransActivity.fssetChargeTransVariableWithLog(Transactionvariables);
                            }
                            
                            
                       //   double dbl_chargeAmt = chargesDed.getChargeAmt();

                            //end -  - new method here for charges

							//Irfan Ali 09-06-2011
                            //System.Collections.Hashtable Hash_RespCollection = shma.bank.shgc.TransactionTypeInfo.fsgetTransTypeInfo(str_TranCode);
								
                            ////**********System Code, VoucherType and PlusMinus sign on the basis of responding transaction code********//
                            //System.String Str_systemCode   = (System.String) Hash_RespCollection["PSY_SYSTCODE"];
                            //System.String Str_voucherType  = (System.String) Hash_RespCollection["PVT_VCHTTYPE"];
                            //System.String Str_narration    = (System.String) Hash_RespCollection["PTR_NARRATION"];
                            //System.String Str_vouchFlag    = (System.String) Hash_RespCollection["PTR_GLVCHFLAG"]; 
                            //System.String Str_chrgApplied  = (System.String) Hash_RespCollection["PTR_CHRGAPPLICABLE"];
                            //System.String Str_chrgTranCode = (System.String) Hash_RespCollection["PTR_CHRGTRANTYPE"];
                            //System.String Str_crcCriteria  = (System.String) Hash_RespCollection["PTR_CRCCRITERIA"];
	
							//ADDED BY FARHAN MANZAR 24-APRIL-2012
							//CHECK ADDED if dbl_chargeAmt not > 0 then no charges effect with zero
							if (str_LocaCode != str_LocaCod_Ref && Str_chrgApplied.Equals("Y") && dbl_chargeAmt > 0)
							{
									System.Text.StringBuilder Sb_transType = new System.Text.StringBuilder(); 

									Sb_transType.Remove(0, Sb_transType.Length);
									Sb_transType.Append(" select PTR_TRANCODE_EVENT1 from PR_GN_TR_TRANSACTIONTYPE ");
									Sb_transType.Append("   where PTR_TRANCODE = ? ");
										
									pDailyTransactionColl.clear();
									pDailyTransactionColl.puts("@PTR_TRANCODE", Str_chrgTranCode, Types.VARCHAR);
										
									rowset rs = DB.executeQuery(Sb_transType.ToString(), pDailyTransactionColl);
			
									if (!rs.next())
									{
										throw new ProcessException("Responding Transaction type not defined in transaction type setup. ");
									}
									else
									{	
										Str_respTranCode = rs.getString("PTR_TRANCODE_EVENT1");
									}

									rs.close(); 


								shma.bank.shgc.TransactionVariables RespTransVariable = new shma.bank.shgc.TransactionVariables();


								//*********** setting of transaction variables for making transaction in case of transaction fee***********//
								RespTransVariable.setOrgaCode(str_OrgaCode);
								RespTransVariable.setLocaCode(str_LocaCod_Ref);
								RespTransVariable.setTransDate(dt_GntrDateSql);
								RespTransVariable.setValueDate(dt_GntrDateSql);
								RespTransVariable.setOrgaCodeRef(str_OrgaCod_Ref);
								RespTransVariable.setLocaCodeRef(str_LocaCod_Ref);
								RespTransVariable.setProdCode((str_Product == null?"":str_Product));
								RespTransVariable.setSchCode((str_SchemCod == null?"":str_SchemCod));
								RespTransVariable.setAcntNumber(str_Number == null?"":str_Number);
								RespTransVariable.setTransCode(Str_respTranCode);
								RespTransVariable.setSystemCode(Str_systemCode);
								RespTransVariable.setVoucherType(Str_voucherType);
								RespTransVariable.setCurrCode(str_CurrCode);
								RespTransVariable.setExchangeRate((double) dbl_ExchRate);
								RespTransVariable.setFCAmount((double) dbl_chargeAmt);
								RespTransVariable.setBCAmount((double) dbl_chargeAmt);
								RespTransVariable.setCreateUsr(str_CrtUsr);
								RespTransVariable.setCreateDate(dt_SysDate);
								RespTransVariable.setVerifyUsr(str_CrtUsr);
								RespTransVariable.setVerifyDate(dt_SysDate);
								RespTransVariable.setPostUsr(str_CrtUsr);
								RespTransVariable.setPostDate(dt_SysDate);
								RespTransVariable.setLocaCodeReq(str_LocaCode);
								//ADDED BY FARHAN MANZAR ON 10-FEB-2012 FOR NARRATION WORK
								RespTransVariable.setNarration(str_OrgaCode + "~" + str_LocaCode  + "~" + dt_GntrDateSql.ToShortDateString() + "~" + str_TranCode + "~" + str_Narration);
								RespTransVariable.setGLVchFlag(Str_vouchFlag);
								RespTransVariable.setExchangeType(str_PetExtCd);
								RespTransVariable.setChargeTranCode2(Str_chrgTranCode); //for tax deduction
								RespTransVariable.setBMPost("P");
								RespTransVariable.setStatus("P");
								//RespTransVariable.setCardType(TransVariable.getCardType());
								//RespTransVariable.setCardNumber(TransVariable.getCardNumber());

								//****************** setting of keys and values in collection for reading matrix************//
								//RespTransVariable.setHashCollection(RespTransVariable.getHashCollection());
								RespTransVariable.setHashCollection("PTR_TRANCODE", str_TranCode); //Transaction Code
								RespTransVariable.setHashCollection("PVT_VCHTTYPE", Str_voucherType); //Voucher Type
								RespTransVariable.setHashCollection("PSY_SYSTCODE", Str_systemCode); //System Code
								RespTransVariable.setHashCollection("DMP_PRODCODE",str_Product);
								RespTransVariable.setHashCollection("DCS_SCHEMCODE",str_SchemCod);	
								RespTransVariable.setHashCollection(gHashTable); 
								//******* Calling of GL Matrix ********//
								shma.bank.shgc.GLMatrixInfo.fsgetGLMatrix(RespTransVariable);
							}

						}
						//// ended here
					}						
						System.String sUpdTrans = " update bn_ms_gt_generaltransaction " + 
								"    set mgt_bmpost = 'P', mgt_postusr = ?, mgt_postdat = ?, " + 
								"        mgt_status = 'P', " +
								"		 adt_modifieduser = ?, adt_modifieddate = ? " +	
								"  where por_orgacode = ? and plc_locacode = ? " + 
								"    and mgt_gntrdate = ? and mgt_gntrnumber = ? ";
					
						pDailyTransactionColl.clear();
						pDailyTransactionColl.puts("@mgt_postusr", str_CrtUsr, Types.VARCHAR);
						pDailyTransactionColl.puts("@mgt_postdat", dt_SysDate, Types.DATE);
						pDailyTransactionColl.puts("@adt_modifieduser",str_CrtUsr,Types.VARCHAR);
						pDailyTransactionColl.puts("@adt_modifieddate",SHMA.CodeVision.Business.DateUtil.getCurrentDateTime(),Types.TIMESTAMP);
						pDailyTransactionColl.puts("@por_orgacode", str_OrgaCode, Types.VARCHAR);
						pDailyTransactionColl.puts("@plc_locacode", str_LocaCode, Types.VARCHAR);
						pDailyTransactionColl.puts("@mgt_gntrdate", dt_GntrDateSql, Types.DATE);
						pDailyTransactionColl.puts("@mgt_gntrnumber", int_TranNo, Types.INTEGER);
					
						DB.executeDML(sUpdTrans, pDailyTransactionColl);

				//************************************************** MOBIZ RELATED WORK STARTS HERE **************************************************//
					if(shma.bank.Mobiz.MobizUtility.MobizSupport())
					{
						//Mobiz is enabled for this transaction or not
						if(shma.bank.Mobiz.MobizedTransaction.isTransactionMobizEnabled(str_TranCode))
						{
							//Transaction Information
							NameValueCollection Collection = new NameValueCollection();
									
							//Get Transaction Date and Time - Point No 76
							Collection.Add("POR_ORGACODE_TR", str_OrgaCode);
							Collection.Add("PLC_LOCACODE_TR", str_LocaCode);
							//Account Information
							Collection.Add("POR_ORGACODE_AC", str_OrgaCod_Ref);
							Collection.Add("PLC_LOCACODE_AC", str_LocaCod_Ref);
							Collection.Add("DMP_PRODCODE", str_Product);
							Collection.Add("DCS_SCHEMCODE", str_SchemCod);
							Collection.Add("MBM_NUMBER", str_Number);
							//More columns
							Collection.Add("PTR_TRANCODE", str_TranCode);
							Collection.Add("PCR_CURRCODE", str_CurrCode);
							Collection.Add("ACT_TRANAMT",dbl_FcAmt);

							DateTime Dt_SysDateTime = SHMA.CodeVision.Business.DateUtil.getCurrentDateTime(); //dateTime Variable
							Collection.Add("ACT_TRANDTTM",Dt_SysDateTime); //datetime
								
							Collection.Add("ACT_TRANDATE",Dt_SysDateTime.Day+"/"+Dt_SysDateTime.Month+"/"+Dt_SysDateTime.Year); //date
							Collection.Add("ACT_TRANTIME",Dt_SysDateTime.Hour+":"+Dt_SysDateTime.Minute+":"+Dt_SysDateTime.Second); //time

							//Customer Info
							//Collection.Add("PCT_CSTYCODE", objMessageField.getPCTCSTYCODE());
							//Collection.Add("CMP_CUSTCODE", objMessageField.getCMPCUSTCODE());

							//Columns required for Mobiz Template for transactions (e.g. POS, Withdrawls etc.)
									
							shma.bank.Mobiz.MobizUtility.sendMessageToMobiz(Collection, str_TranCode);
						}
					}
					//************************************************** MOBIZ RELATED WORK STARTS HERE **************************************************//
				}
			}
			
			if (Str_Condition.Equals("A"))
				Str_Message = " Transaction " + int_TranNo + " saved successfully;";
			else
				Str_Message = " Transaction " + int_TranNo + ";";
			
			return Str_Message; //+ BalanceValidity.getMessValidity() + gInstValidation.getInsMessage();
		}
		private System.String[] targetTransType(System.String sourceTransType, ParameterCollection ParaColl)
		{
			System.String[] transTypeArr = new System.String[4];
			
			System.Text.StringBuilder targetType = new System.Text.StringBuilder();
			targetType.Remove(0, targetType.Length - 0);
			targetType.Append(" select ptr_chrgtrantype,case when ptr_chrgapplicable is null then 'N' else ptr_chrgapplicable end ptr_chrgapplicable, ");
			targetType.Append("        case ptr_plusminacntbal when 'P' then '+' when 'M' then '-' else 'N' end ptr_plusminacntbal, ");
			targetType.Append("        case when ptr_glvchflag is null then 'N' else ptr_glvchflag end ptr_glvchflag ");
			targetType.Append("   from pr_gn_tr_transactiontype ");
			targetType.Append("  where ptr_trancode = ? ");
			
			ParaColl.clear();
			ParaColl.puts("@ptr_trancode", sourceTransType);
			
			rowset rTargetType = DB.executeQuery(targetType.ToString(), ParaColl);
			
			if (!rTargetType.next())
				throw new ProcessException(" Source transaction code is not proper defined in setup. ");
			else
			{
				if (rTargetType.getString("ptr_chrgtrantype") == null)
					transTypeArr[0] = null;
				else
					transTypeArr[0] = rTargetType.getString("ptr_chrgtrantype");
				
				transTypeArr[1] = rTargetType.getString("ptr_chrgapplicable");
				transTypeArr[2] = rTargetType.getString("ptr_plusminacntbal");
				transTypeArr[3] = rTargetType.getString("ptr_glvchflag");
			}
			
			return transTypeArr;
		}
	}
}
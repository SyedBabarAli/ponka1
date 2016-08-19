/*************************************************************************
 *  General Process for Incidental Remaining Charges 
 *  
 * Steps:
 *	
 *	1 - execute on day end
 *	2 - Select all the accounts, for incidental remaining charges to be deducted
 *	3 - if balance available then mark staus "F" = Full and deduct all remaining amount
 *	4 - if balance not available then mark staus "P" = Partial and deduct available amount
 *	5 - Execute GL Matrix
 * 
 *	Created by: Irfan Ali 31-01-2011
 *
 *************************************************************************/

using System;
using ProcessException = SHMA.Enterprise.Exceptions.ProcessException;
using SHMA.Enterprise.Data;
using SHMA.Enterprise.Shared;
using NameValueCollection = SHMA.Enterprise.NameValueCollection;
using shgn;
using shgb;
using generalMethods = shma.bank.gnbk.generalMethods;
using EnvHelper = SHMA.Enterprise.Shared.EnvHelper;
using SHGNGetGlobalPara = shgn.SHGNGetGlobalPara;
using shma.bank.shgc;

namespace shma.bank.shgc
{
	/// <summary>
	/// To execute Remaining charges on the accounts periodically .
	/// Stand alone process require new screen with inputs 
	/// LocationFrom,LocationTo 
	/// </summary>
	/// 

	public class SHGBChargesRecovery:ProcessCommand
	{
		private ParameterCollection pQuery = new ParameterCollection();
		private String	Str_locaCodeFrom = null, Str_locaCodeTo = null, Str_processId = null, Str_incidChargesTransCode = null;
		private String	Str_fedallowed = null, Str_ChrgFreq = null;
		private double  dbl_fedoncharge = 0, dbl_fcAmountDR = 0, dbl_bcAmountDR = 0,exchRate =0;
		private double dblFEDAmt=0,dbl_RmainAmt = 0,dbl_RemainingAmount = 0,dbl_NewBalance = 0,dbl_RCharg=0;
		private int int_decimalPlaces = 0, int_decimalPlacesBC = 0;
		private DateTime	Dt_systemDate, Dt_processDate,Dt_processDateTO, Dt_systemDate_2, Dt_processDate_2,Dt_DueDate ;
		private string str_tranDesc = null;
		

		public override System.String processing()
		{

			String vchtType="";
			String systCode="";

			double dbl_availBalance = 0;	//Account Balance
			double dbl_remainingChgAmt = 0;	//gross Amount - Adjust Amount
			double dbl_currentBalance = 0;	//dbl_availBalance - dbl_remainingChgAmt
			double dbl_deductedAmount = 0;
			int	   intCnt			  = 0;	

			//*********************	S	E	S	S	I	O	N	~~~	V	A	L	U	E	S	****************//
			shgb.SHGBCalculationMethods calcMethdsUtil = new shgb.SHGBCalculationMethods ();

			EnvHelper EnvHlp = new EnvHelper();
			DateTime systemDate = SHGNDateUtil.parseDate((String)EnvHlp.getAttribute("s_SYSDATE"));
			String orgaCode = (String)EnvHlp.getAttribute("s_POR_ORGACODE");
			//String locaCode = (String)EnvHlp.getAttribute("s_PLC_LOCACODE");
			String sessionUser = (String) EnvHlp.getAttribute("s_SUS_USERCODE");
			//String acntYear="";//(String)EnvHlp.getAttribute("s_PFS_ACNTYEAR");

			//*********************	G	L	O	B	A	L  ~~~	V	A	L	U	E	S	****************//
			
			String str_MINBAL   = generalMethods.getGlbValue("ACRBALTYPE", "MINBAL");
			String str_AVGBAL   = generalMethods.getGlbValue("ACRBALTYPE", "AVGBAL");
			String perdType     = generalMethods.getGlbValue("GLOBAL", "PERIODTYPE");
			String exrtRateType = generalMethods.getGlbValue("VCHRDEFVAL", "RATETYPE");
			String prodLimit    = generalMethods.getGlbValue("PRODLIMT", "ACNTMINBAL");
			String defaultCurr  = generalMethods.getGlbValue("VCHRDEFVAL", "CURRENCY");
			String statusCode   = generalMethods.getGlbValue("ACNTSTATUS", "NORMAL");

			String str_FreqYear = generalMethods.getGlbValue("FREQUENCY" ,	"YEARLY").Replace("'","");

			//double exchRate		= generalMethods.getGlbValue("VCHRDEFVAL", "EXCHRATE");

			//if (exchRate.LastIndexOf("'") > - 1)
			//	exchRate = exchRate.Substring(exchRate.IndexOf("'") + 1, (exchRate.LastIndexOf("'")) - (exchRate.IndexOf("'") + 1));
			
			//*********************	O	B	J	E	C	T	~~~	I	N	S	T	A	N	C	E	S	****************//

			TransactionVariables Obj_CreationOfTrans = new TransactionVariables();

			//*********************	S	C	R	E	E	N ~~~ V	A	L	U	E	S	****************//

			NameValueCollection[] screen = this.getDataRows();
			//String prodCode = (String)screen[0].getObject("DMP_PRODCODE");
			//String schemCodeFrom = (String)screen[0].getObject("DCS_SCHEMCODE");
			//String schemCodeTo = (String)screen[0].getObject("DCS_SCHEMCODETO");
			//String transCode = (String) screen[0].getObject("PTR_TRANCODE");

			Object Obj_locaCodeFrom = screen[0].getObject("LOCACODE_FROM");
			Object Obj_locaCodeTo   = screen[0].getObject("LOCACODE_TO");
			Object Obj_processId	= "1018"; 
			Object Obj_systemDate	= screen[0].getObject("PROCESS_DATE");
			//Object Obj_processDate  = screen[0].getObject("PROCESS_DATE_FROM");
						
			if (Obj_locaCodeFrom != null && Obj_locaCodeFrom.ToString().Trim().Length > 0)
				Str_locaCodeFrom = (System.String) Obj_locaCodeFrom;

			if (Obj_locaCodeTo != null && Obj_locaCodeTo.ToString().Trim().Length > 0)
				Str_locaCodeTo = (System.String) Obj_locaCodeTo;

			if (Obj_processId != null && Obj_processId.ToString().Trim().Length > 0)
				Str_processId = (System.String) Obj_processId;

			if (Obj_systemDate != null && Obj_systemDate.ToString().Trim().Length > 0)
			{
				Console.WriteLine(Obj_systemDate.GetType());
				Dt_systemDate_2 = (System.DateTime)Obj_systemDate;
			}
			
			//if (Obj_processDate != null && Obj_processDate.ToString().Trim().Length > 0)
			//{
			//	Dt_processDate_2 = (System.DateTime)Obj_processDate;
			//}
			
			Dt_systemDate_2 = systemDate;

			 
			//Accrual Voucher Transaction Type
			Str_incidChargesTransCode = shma.bank.shgc.AllowableProductSchemeInfo.fsgetTransactionCode(Str_processId);

			//fill voucher type and system code
			rowset rsTransactionType = getVoucherTypeAndSystemCode(Str_incidChargesTransCode);

			if (rsTransactionType.next())
			{
				vchtType = rsTransactionType.getString("PVT_VCHTTYPE");
				systCode = rsTransactionType.getString("PSY_SYSTCODE");
			}

			//********* Branch Cursor *********//
			//rowset rs = shma.bank.shgc.LocationsInfo.fsgetLocationInfo(orgaCode,Str_locaCodeFrom,Str_locaCodeTo);
			/*
			rowset rs = shma.bank.shgc.LocationsInfo.fsgetLocationInfo(orgaCode,Str_locaCodeFrom,Str_locaCodeTo,Str_processId,Dt_systemDate_2);

			if (!rs.next())
				throw new ProcessException("Branch is not ready for (Incidental Charges).");
			else
				rs.previous();
			*/
			System.Text.StringBuilder Sb_incidentalCharges = new System.Text.StringBuilder();

			Sb_incidentalCharges.Remove(0, Sb_incidentalCharges.Length);
			Sb_incidentalCharges.Append(" select  * from  BN_MS_CH_CHARGESDETL ");
			Sb_incidentalCharges.Append("  where POR_ORGACODE  = ?  ");
			Sb_incidentalCharges.Append("	 and PLC_LOCACODE between ? and ?  ");
			Sb_incidentalCharges.Append("    and  MCH_RECIEVEDSTATUS != 'F' ");
			Sb_incidentalCharges.Append("    and  CASE WHEN MCH_PROCESSONEOD IS NULL THEN 'N' ELSE MCH_PROCESSONEOD END = 'Y' ");
			Sb_incidentalCharges.Append("  order by mch_serialno ");
				
			pQuery.clear();
			pQuery.puts("@por_orgacode", orgaCode, Types.VARCHAR);
			pQuery.puts("@plc_locacode", Str_locaCodeFrom, Types.VARCHAR);
			pQuery.puts("@plc_locacode2", Str_locaCodeTo, Types.VARCHAR);
			
			rowset rs = DB.executeQuery(Sb_incidentalCharges.ToString(), pQuery);

			while (rs.next()) //Charges loop starting rs
			{
				String FEDExempt = getLocationPara(rs.getString("por_orgacode"),rs.getString("plc_locacode"));

				Str_ChrgFreq = rs.getString("pfm_fmodecode_narrat");

				if(rs.getString("mch_duedate") != null)
					Dt_DueDate = rs.getDate("mch_duedate");

				BankMasterInfo BankInfo = new BankMasterInfo(rs.getString("por_orgacode"),rs.getString("plc_locacode"),rs.getString("DMP_PRODCODE"),rs.getString("DCS_SCHEMCODE"),rs.getString("MBM_NUMBER"));
				//BankInfo.fsgetAcntBalance(Dt_systemDate_2);
                BankInfo.fsgetAcntBalance(Dt_systemDate_2, Str_incidChargesTransCode);

				String chrgCode = getChargeCode(rs.getString("pch_chrgcode")); 
				int_decimalPlaces   = MiscellaneousMethods.fsgetDecimalPlacesFC(rs.getString("PCH_CHRGCODE"),rs.getString("PCR_CURRCODE"));
				int_decimalPlacesBC = MiscellaneousMethods.fsgetDecimalPlacesBC(rs.getString("PCH_CHRGCODE"),rs.getString("PCR_CURRCODE"));

				dbl_availBalance = BankInfo.getAvailableBal();	

				if(dbl_availBalance > 0)
				{
					dbl_remainingChgAmt = rs.getDouble("MCH_GRSAMTFC") - rs.getDouble("MCH_ADJAMTFC");

					dbl_RCharg=dbl_remainingChgAmt;
								
					//hash table for field value combination in charge setup
					System.Collections.Hashtable hashValues = new System.Collections.Hashtable();
					hashValues.Clear();

					double dbl_fedincidentalamount=0;
					double dbl_fedAmount = 0;	
					
					//Added by Ahsan (19-Jan-2014) FED Province Wise Work
					LocationsInfo obj_LocationsInfo = new LocationsInfo(rs.getString("por_orgacode"), rs.getString("plc_locacode"));
					double dbl_FedOnBranch = 0;
					String fed_Exempt = null;

					fed_Exempt = obj_LocationsInfo.getFedExempt();

					dbl_FedOnBranch = obj_LocationsInfo.getFEDOnCharge();

					if (fed_Exempt != null && fed_Exempt=="N")
					{
						dbl_fedoncharge = dbl_FedOnBranch > 0 ? dbl_FedOnBranch : dbl_fedoncharge;
					}
					//End
								
////					if (Str_fedallowed.Equals("Y") && FEDExempt.Equals("N"))
////					{
////						dbl_fedincidentalamount = Math.Round(dbl_deductedAmount+((dbl_deductedAmount*dbl_fedoncharge)/100),int_decimalPlaces);
////						dbl_fedAmount = Math.Round(dbl_fedincidentalamount - dbl_deductedAmount,int_decimalPlaces);
////
////						dbl_fedincidentalamount = Math.Round(dbl_deductedAmount - dbl_fedAmount,int_decimalPlaces);
////
////						hashValues.Add("MBM_FEDAMOUNT", dbl_fedAmount);
////						hashValues.Add("MBM_LASTINCDCHRAMOUNT", dbl_fedincidentalamount);
////						hashValues.Add("MBM_TOTCHRGAMOUNT", dbl_deductedAmount);
////					}
////					else
////					{
////						hashValues.Add("MBM_FEDAMOUNT", 0);
////						hashValues.Add("MBM_LASTINCDCHRAMOUNT", dbl_deductedAmount);
////						hashValues.Add("MBM_TOTCHRGAMOUNT", dbl_deductedAmount);
////					}
					//**********************end FED ALLOWED**********//

					dblFEDAmt = Math.Round((dbl_fedoncharge * dbl_remainingChgAmt / 100),int_decimalPlaces);

					dbl_RemainingAmount = 0;
					double dbl_CalcAmt = 0;
					dbl_RmainAmt = dbl_remainingChgAmt;

					if(Str_fedallowed.Equals("Y") && FEDExempt.Equals("N"))
					{
						dbl_RmainAmt = dbl_remainingChgAmt + dblFEDAmt;
						dbl_RemainingAmount = dbl_RmainAmt;
						dbl_fedincidentalamount = dbl_RmainAmt; 
						if (dbl_availBalance > 0 && dbl_availBalance < dbl_fedincidentalamount)
						{   
							dbl_fedincidentalamount = dbl_fedincidentalamount - dblFEDAmt;
							dbl_remainingChgAmt = dbl_availBalance;
							dbl_NewBalance = dbl_availBalance - dbl_remainingChgAmt ;
											
							dbl_RmainAmt= Math.Round(((dbl_remainingChgAmt /(dbl_fedoncharge +100))*100),int_decimalPlaces);
							dblFEDAmt = Math.Round(dbl_remainingChgAmt - dbl_RmainAmt,int_decimalPlaces);
							dbl_RemainingAmount = dbl_fedincidentalamount;
							dbl_CalcAmt = dbl_RmainAmt;

							//TransVariable.setNarration(TransVariable.getNarration() + " Partially Recovered , Remaining " + Convert.ToString(Math.Round(dbl_RemainingAmount - dbl_CalcAmt +Math.Round((dbl_fedoncharge * (dbl_RemainingAmount - dbl_CalcAmt) / 100),int_decimalPlaces),2))+" Only.");
							
							//TransVariable.setNarration(TransVariable.getNarration() + " Partially Recovered , Remaining " + Convert.ToString(dbl_RemainingAmount - dbl_CalcAmt)+" Only.");

						}
						else if(dbl_availBalance >= dbl_fedincidentalamount)
						{
							dbl_remainingChgAmt = dbl_RmainAmt;
							dbl_NewBalance =  dbl_availBalance - dbl_remainingChgAmt;
							dbl_RmainAmt = dbl_remainingChgAmt - dblFEDAmt;
							dbl_RemainingAmount = dbl_remainingChgAmt;
							dbl_CalcAmt = dbl_remainingChgAmt;
						}
						else if (dbl_availBalance <= 0)
						{
							dbl_RemainingAmount = dbl_remainingChgAmt;
							dbl_RmainAmt = 0;
							dblFEDAmt = 0;
							//bln_Transaction = false;
							dbl_remainingChgAmt =0;
							dbl_CalcAmt = 0;
						}
						else
						{
							dbl_fedincidentalamount = Math.Round(dbl_remainingChgAmt - dblFEDAmt ,int_decimalPlaces);
						}

						hashValues.Add("MBM_FEDAMOUNT", dblFEDAmt );
						hashValues.Add("MBM_LASTINCDCHRAMOUNT", dbl_RmainAmt );
						//TransVariable.setHashCollection("MBM_TOTFEDCHRGAMOUNT", dbl_remainingChgAmt);
						hashValues.Add("MBM_TOTCHRGAMOUNT", dbl_remainingChgAmt );

						dbl_currentBalance = dbl_availBalance - (dbl_RCharg+Math.Round((dbl_fedoncharge * dbl_RCharg / 100),int_decimalPlaces));

					}
					else
					{
						if (dbl_availBalance > 0 && dbl_availBalance < dbl_remainingChgAmt)
						{
							dbl_RmainAmt  = dbl_availBalance;
							dbl_RemainingAmount = dbl_remainingChgAmt;
							dbl_remainingChgAmt = dbl_availBalance;
							dbl_NewBalance = dbl_availBalance - dbl_remainingChgAmt;
							dbl_CalcAmt = dbl_RmainAmt;

							//Str_narration = " Partially Recovered , Remaining " + Convert.ToString(dbl_RemainingAmount - dbl_CalcAmt)+" Only.";
							//TransVariable.setNarration(TransVariable.getNarration() + " Partially Recovered , Remaining " + Convert.ToString(dbl_RemainingAmount - dbl_CalcAmt)+" Only.");
							//TransVariable.setNarration(TransVariable.getNarration() + " Partially Recovered , Remaining " + Convert.ToString(Math.Round(dbl_RemainingAmount - dbl_CalcAmt,2))+" Only.");
						}
						else if(dbl_availBalance > dbl_remainingChgAmt)
						{
							dbl_RemainingAmount = dbl_remainingChgAmt;
							dbl_RmainAmt = dbl_remainingChgAmt;
							dbl_NewBalance = dbl_availBalance - dbl_remainingChgAmt;
							dbl_CalcAmt = dbl_RmainAmt;
						}
						else if (dbl_availBalance <= 0)
						{
							//bln_Transaction = false;
							dbl_RmainAmt  = 0;
							dbl_RemainingAmount = dbl_remainingChgAmt;
							dbl_NewBalance = dbl_availBalance;
							dbl_CalcAmt = 0;
						}
						
						

						hashValues.Add("MBM_FEDAMOUNT", 0 );
						//TransVariable.Add("MBM_TOTFEDCHRGAMOUNT", dbl_remainingChgAmt);
						hashValues.Add("MBM_LASTINCDCHRAMOUNT", dbl_remainingChgAmt );
						hashValues.Add("MBM_TOTCHRGAMOUNT", dbl_remainingChgAmt );

						dbl_currentBalance = dbl_availBalance - dbl_RCharg;

					}

					dbl_deductedAmount=dbl_remainingChgAmt;

					//dbl_currentBalance = dbl_availBalance - (dbl_RCharg+Math.Round((dbl_fedoncharge * dbl_RCharg / 100),int_decimalPlaces));

					if(dbl_currentBalance >= 0)
					{
						updateChargesDetail(dbl_RmainAmt,"F",rs.getString("POR_ORGACODE"),rs.getString("PLC_LOCACODE"),rs.getString("DMP_PRODCODE"),rs.getString("DCS_SCHEMCODE"),rs.getString("MBM_NUMBER"),rs.getString("MCH_SERIALNO"));				
						//dbl_deductedAmount = dbl_remainingChgAmt;
					}
					else
					{
						updateChargesDetail(dbl_RmainAmt,"P",rs.getString("POR_ORGACODE"),rs.getString("PLC_LOCACODE"),rs.getString("DMP_PRODCODE"),rs.getString("DCS_SCHEMCODE"),rs.getString("MBM_NUMBER"),rs.getString("MCH_SERIALNO"));			
						//dbl_deductedAmount = dbl_availBalance;
					}

					exchRate = calcMethdsUtil.getExchangeRate(rs.getString("PCR_CURRCODE"),exrtRateType, defaultCurr);

					hashValues.Add("SYSDATE",Dt_systemDate_2);
					hashValues.Add("POR_ORGACODE", orgaCode);
					hashValues.Add("PLC_LOCACODE", rs.getString("PLC_LOCACODE"));
					hashValues.Add("POR_ORGACODE_REF", rs.getString("por_orgacode"));
					hashValues.Add("PLC_LOCACODE_REF", rs.getString("plc_locacode"));
					hashValues.Add("DMP_PRODCODE", rs.getString("DMP_PRODCODE"));
					hashValues.Add("DCS_SCHEMCODE", rs.getString("DCS_SCHEMCODE"));
					hashValues.Add("PCR_CURRCODE", rs.getString("PCR_CURRCODE"));
					hashValues.Add("PCH_CHRGCODE", rs.getString("PCH_CHRGCODE"));
					hashValues.Add("PSY_SYSTCODE", systCode);
					hashValues.Add("PVT_VCHTTYPE", vchtType);
					hashValues.Add("PCH_FEDALLOWED",Str_fedallowed);
					hashValues.Add("PLC_FEDEXEMPT",FEDExempt);
					hashValues.Add("MBM_EXCHRATE", exchRate);

					//*********	- Execute GL Matrix *********//
						
					//pick up the exchage rate to be passed in to the matrix
					
					Obj_CreationOfTrans.setOrgaCode(rs.getString("POR_ORGACODE"));		//Transaction Organization
					Obj_CreationOfTrans.setLocaCode(rs.getString("plc_locacode"));		//Transaction Location
					Obj_CreationOfTrans.setTransDate(Dt_systemDate_2);	//Transaction Date
					Obj_CreationOfTrans.setTransCode(rs.getString("PTR_TRANCODE"));		//Transaction Code / Type PTR_TRANCODE_REVL
					Obj_CreationOfTrans.setOrgaCodeRef(rs.getString("POR_ORGACODE"));//Account Organization
					Obj_CreationOfTrans.setLocaCodeRef(rs.getString("plc_locacode"));//Account Location
					Obj_CreationOfTrans.setProdCode(rs.getString("DMP_PRODCODE"));		//Product
					Obj_CreationOfTrans.setSchCode(rs.getString("DCS_SCHEMCODE"));		//Scheme
					Obj_CreationOfTrans.setAcntNumber(rs.getString("MBM_NUMBER"));		//Account
					Obj_CreationOfTrans.setValueDate(Dt_systemDate_2);	//Valuation Date

					//ParaCollection.clear();

					pQuery.clear();
					pQuery.puts("@PTR_TRANCODE",rs.getString("PTR_TRANCODE"),Types.VARCHAR);

                    rowset rst = DB.executeQuery("SELECT PTR_TRANDESC FROM PR_GN_TR_TRANSACTIONTYPE WHERE PTR_TRANCODE = ? ",pQuery);

					if (!rst.next())
					{
						//throw new System.Exception(" No data found transaction description .");
					}
					else
					{
						str_tranDesc = rst.getString("PTR_TRANDESC");
					}

					//if (Str_ChrgFreq == null)
					//	Obj_CreationOfTrans.setNarration("charges auto recording");	//Narration
					
					string str_remarks = dbl_currentBalance<0?" partially recovered. Remaining:"+Math.Round(Math.Abs(dbl_currentBalance),2)+" only":"fully recovered.";

					if((Str_ChrgFreq != null && Str_ChrgFreq.Trim().Length > 0) && str_FreqYear.Equals(Str_ChrgFreq))
					{
						Obj_CreationOfTrans.setNarration(str_tranDesc+" for the Year of " + Dt_DueDate.Year.ToString()+str_remarks);	//Narration
					}
					else
					{
						Obj_CreationOfTrans.setNarration(str_tranDesc+" for the month of " +Dt_DueDate.Month.ToString()+","+Dt_DueDate.Year.ToString()+str_remarks);//Narration
					}

					Obj_CreationOfTrans.setCurrCode(rs.getString("PCR_CURRCODE"));		//Currency Code
					Obj_CreationOfTrans.setExchangeType(exrtRateType);  //Exchange Rate Type
					Obj_CreationOfTrans.setExchangeRate(exchRate);  //Exchange Rate glb_curr, glb_rateType, glb_rate
					Obj_CreationOfTrans.setFCAmount(dbl_deductedAmount);			//FC Amount
					Obj_CreationOfTrans.setBCAmount(Math.Round((dbl_deductedAmount* exchRate),int_decimalPlacesBC));	//BC Amount
					Obj_CreationOfTrans.setDNAmount(0);					//Denomination Amount
					Obj_CreationOfTrans.setRPAmount(0);					//Reporting Amount
					Obj_CreationOfTrans.setBMPost("P");				//BM Post
					Obj_CreationOfTrans.setStatus("P");					//Status
					Obj_CreationOfTrans.setCreateUsr(sessionUser);		//Create User
					Obj_CreationOfTrans.setCreateDate(Dt_systemDate_2);  //Create Date
					Obj_CreationOfTrans.setVerifyUsr(sessionUser);		//Verify User 
					Obj_CreationOfTrans.setVerifyDate(Dt_systemDate_2);	//Verify Date
					Obj_CreationOfTrans.setPostUsr(sessionUser);
					Obj_CreationOfTrans.setPostDate(Dt_systemDate_2);
					Obj_CreationOfTrans.setStatus("P");
					Obj_CreationOfTrans.setVoucherType(vchtType);	//Voucher Type 
					Obj_CreationOfTrans.setSystemCode(systCode);		//System Code pSysCd, pVchTyp
					Obj_CreationOfTrans.setGLVchFlag("P");
					//Obj_CreationOfTrans.setGLCode(str_GLCode);
					Obj_CreationOfTrans.setHashCollection(hashValues); //gNmValColl
					Obj_CreationOfTrans.setAdtCreatedUser(sessionUser);
					Obj_CreationOfTrans.setAdtCreatedDate(SHMA.CodeVision.Business.DateUtil.getCurrentDateTime());
   									
					int int_gntrNumber = shma.bank.shgc.GLMatrixInfo.fsgetGLSQLMatrix(Obj_CreationOfTrans);

					dbl_fcAmountDR = 0;
					dbl_bcAmountDR = 0;

					dbl_fcAmountDR = (double)Obj_CreationOfTrans.getHashCollection("AMOUNTDRFC");
					dbl_bcAmountDR = (double)Obj_CreationOfTrans.getHashCollection("AMOUNTDRBC");

					//BankInfo.fsupdateAcntIncidental(dbl_fcAmountDR>0?dbl_fcAmountDR:(dbl_deductedAmount),dbl_fcAmountDR>0?dbl_bcAmountDR:(dbl_deductedAmount*exchRate),"-",Dt_processDateTO);
                    BankInfo.fsupdateAcntIncidental(dbl_fcAmountDR > 0 ? dbl_fcAmountDR : (dbl_deductedAmount), dbl_fcAmountDR > 0 ? dbl_bcAmountDR : (dbl_deductedAmount * exchRate), "-", Dt_systemDate_2);
                    //BankInfo.fsupdateAcntIncidental(dbl_fcAmountDR > 0 ? dbl_fcAmountDR : (dbl_deductedAmount), dbl_fcAmountDR > 0 ? dbl_bcAmountDR : (dbl_deductedAmount * System.Double.Parse(exchRate)), "-", Dt_systemDate_2);
                    
					
					intCnt++;
				}
			}	
			rs.close();

			return "Total " + intCnt + " record(s) processed successfully";
		}
	
		
		private void updateChargesDetail(Double adjAmt,String status, String orgacode, String locaCode, String prodCode, String schemCode,String number,String srno) 
		{
			
			System.Text.StringBuilder Sb_bankMaster = new System.Text.StringBuilder(); 
			
			Sb_bankMaster.Remove(0, Sb_bankMaster.Length);
			Sb_bankMaster.Append(" update BN_MS_CH_CHARGESDETL  ");
			Sb_bankMaster.Append("  set MCH_ADJAMTFC = MCH_ADJAMTFC + ? , ");
			Sb_bankMaster.Append("		MCH_ADJAMTBC = MCH_ADJAMTBC + ? , ");
			Sb_bankMaster.Append("		MCH_RECIEVEDSTATUS = ?		");
			Sb_bankMaster.Append("  where POR_ORGACODE = ?			");
			Sb_bankMaster.Append("    and PLC_LOCACODE = ?          ");
			Sb_bankMaster.Append("    and DMP_PRODCODE = ?          ");
			Sb_bankMaster.Append("    and DCS_SCHEMCODE = ?			");
			Sb_bankMaster.Append("    and MBM_NUMBER = ?			");
			Sb_bankMaster.Append("    and MCH_SERIALNO = ?			");
											
			pQuery.clear();
			pQuery.puts("@MCH_ADJAMTFC",		adjAmt, Types.DOUBLE);
			pQuery.puts("@MCH_ADJAMTBC",		adjAmt, Types.DOUBLE);
			pQuery.puts("@MCH_RECIEVEDSTATUS",	status, Types.VARCHAR);
			pQuery.puts("@POR_ORGACODE",		orgacode, Types.VARCHAR);
			pQuery.puts("@PLC_LOCACODE",		locaCode, Types.VARCHAR);
			pQuery.puts("@DMP_PRODCODE",		prodCode, Types.VARCHAR);
			pQuery.puts("@DCS_SCHEMCODE",		schemCode, Types.VARCHAR);
			pQuery.puts("@MBM_NUMBER",			number, Types.VARCHAR);
			pQuery.puts("@MCH_SERIALNO",		srno, Types.VARCHAR);

			DB.executeDML(Sb_bankMaster.ToString(), pQuery);
			
		}

		private rowset getVoucherTypeAndSystemCode(String transCode)
		{
			System.Text.StringBuilder Sb_transType = new System.Text.StringBuilder(); 

			Sb_transType.Remove(0, Sb_transType.Length);
			Sb_transType.Append(" select PSY_SYSTCODE, PVT_VCHTTYPE from PR_GN_TR_TRANSACTIONTYPE ");
			Sb_transType.Append("   where PTR_TRANCODE = ? ");
											
			pQuery.clear();
			pQuery.puts("@PTR_TRANCODE", transCode, Types.VARCHAR);
											
			rowset rs = DB.executeQuery(Sb_transType.ToString(), pQuery);
				
			if (!rs.next())
			{
				throw new ProcessException("Transaction type not defined in transaction type setup. ");
			}
			else
			{
				rs.previous();
			}
				
			return rs;
		}

		private String getLocationPara(String orgaCode, String locaCode)
		{
			String strFEDExempt= "N";

			System.Text.StringBuilder Sb_LocationPara = new System.Text.StringBuilder();

			Sb_LocationPara.Remove(0, Sb_LocationPara.Length);
			Sb_LocationPara.Append(" select case when plc_fedexempt is null then 'N' else plc_fedexempt end plc_fedexempt ");
			Sb_LocationPara.Append("   from pr_gn_lc_location ");
			Sb_LocationPara.Append("  where por_orgacode = ? ");
			Sb_LocationPara.Append("    and plc_locacode = ? ");
					
			pQuery.clear();
			pQuery.puts("@por_orgacode", orgaCode, Types.VARCHAR);
			pQuery.puts("@plc_locacode", locaCode, Types.VARCHAR);

			rowset rs = DB.executeQuery(Sb_LocationPara.ToString(),pQuery);
						
			if (!rs.next())
			{  
				strFEDExempt = "N";
			}
			else
			{
				strFEDExempt = rs.getString("plc_fedexempt");
			}
			
			rs.close();
			
			return strFEDExempt;
		}

		private String getChargeCode(String chrgCode)
		{
			String strCHRGCODE="";
			System.Text.StringBuilder Sb_AcntChrg = new System.Text.StringBuilder();
			Sb_AcntChrg.Remove(0, Sb_AcntChrg.Length);
			Sb_AcntChrg.Append(" select PCH_CHRGCODE,PCH_FEDONCHARGE, ");
			Sb_AcntChrg.Append(" CASE WHEN PCH_FEDALLOWED IS NULL THEN 'N' ELSE PCH_FEDALLOWED END PCH_FEDALLOWED from PR_GN_CH_CHARGES");
			Sb_AcntChrg.Append("  WHERE PCH_CHRGNATURE= 'I' and PCH_CHRGCODE = '"+ chrgCode + " '");//PSY_SYSTCODE='"+ systCode + " '");
						
			rowset rs = DB.executeQuery(Sb_AcntChrg.ToString());
						
			if (!rs.next())
				throw new System.Exception(" Charge not defined in Charge Setup");
			else
			{
				strCHRGCODE     = rs.getString("PCH_CHRGCODE");
				Str_fedallowed  = rs.getString("PCH_FEDALLOWED");  
				dbl_fedoncharge = rs.getDouble("PCH_FEDONCHARGE"); 
			}
			rs.close();
			return strCHRGCODE;
		}
	}
}


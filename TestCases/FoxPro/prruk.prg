********************************************
*** ����������� ���, ��, ���, ��
********************************************

Set Talk Off
Set Safety Off
Set ENGINEBEHAVIOR 70

If Used('GEN_ZDN1')
Else
	Close Databases
Endif

Dimension cMes[12]
cMes[1]=[������]
cMes[2]=[�������]
cMes[3]=[����]
cMes[4]=[������]
cMes[5]=[���]
cMes[6]=[����]
cMes[7]=[����]
cMes[8]=[������]
cMes[9]=[��������]
cMes[10]=[�������]
cMes[11]=[������]
cMes[12]=[�������]

*!*	*** ����� ���� ��� ���������� �������
	rMes=Month(loZDN.pdata1)
	rGod=Year(loZDN.pdata1)
*!*	l_data1=FU_OLD_D(Date(),0)
*!*	l_data2=FU_OLD_D(Date(),1)-1

*!*	Do Form men_dt With l_data1,l_data2
*!*	l_data1=n_data
*!*	l_data2=k_data
*!*	If Month(l_data1)=Month(l_data2) And Year(l_data1)=Year(l_data2)
*!*		rMes=Month(l_data1)
*!*		rGod=Year(l_data1)
*!*	Else
*!*		=Messagebox('��������! ������ ������ ���� � �������� 1-�� ������!!!')
*!*		Return
*!*	Endif
*!*	*** ���������� ���������� ��� ������� �������
If Vartype(loZDN)<>'U'
	loZDN.mes1=rMes
	loZDN.mes2=rMes
	loZDN.god=rGod
*!*		loZDN.pdata1=l_data1
*!*		loZDN.pdata2=l_data2
	loZDN.protokol='' && �������� ���������� �������
*** ������ ������ �������� � ������� GEN_ZDN1 � GEN_ZDNH
	loZDN.obnzdn
Endif
**********************************************
*** �������� ������� �������� ����������� *******************************************
DECLARE ms_no_work[1]
ms_no_work[1]=0
lst_pst='  ASCAN(ms_no_work,k_post)=0' && ������� �� �������� �������� �����������
** ���� ����� ����� �������� ������ ��� ����������� �� ������� ����� ����������� 
*********************************************************************************************

************************* ���� ���ר�� ����������� ******************************************

* �������� ���������� ������
pSrv=[e:\vfp_sk\srv\db\]
LPathXls='I:\rabp\prr\'
lPathRep='I:\rabp\rep\'
Set Procedure To e:\vfp_sk\srv\re\pris_pst Additive
&& ���� � ����� ������������

Use &pSrv.S21 Order k_s21 In 0 Noupdate Shared
IF !USED('s1')
	Use &pSrv.S1 In 0 Noupdate Shared Order k_s1
endif	
SET ORDER TO k_s1 IN s1
*** �������� ������� �������� ����������� *************************************************
*** ����������� ������� ����������� �� ������� �� ���� �������� ���������
SELECT k_s1 FROM s1 WHERE !EMPTY(z_no_work) INTO ARRAY ms_no_work GROUP BY k_s1
****************************************************************************************
*******************************************************************************************

Use &pSrv.s_post In 0 Shared Noupdate
* ����� ��� �������� ������
otms=Right(Str(rGod,4),2)+Iif(rMes<10, [0]+Str(rMes,1), Str(rMes,2))
FlItg=[prr]+otms

*Set Default To &LPathXls

* ������� �� ����� �������������
Create Table (FlItg) Free (usl c(1), addm c(50), nkv c(15), fio c(35), k_s4 N(6), plos N(7,2), jilr N(3),;
	mespr c(6), dtn d, dtk d, prich c(40), sumvd N(9,2), sum_2 N(9,2),;
	volvd N(11,5), vol_2 N(11,5), k_post N(3), tpis c(2))

For rn=1 To 3
	Do Case
	Case rn=1
		rnn=[���: ]
		pRab=[e:\vfp_sk\db]+Ltrim(Str(rn))+[\]  && ���� � ������� ��������
	Case rn=2
		rnn=[�/�: ]
		pRab=[F:\vfp_sk\db]+Ltrim(Str(rn))+[\]  && ���� � ������� ��������
	Case rn=3
		rnn=[���: ]
		pRab=[G:\vfp_sk\db]+Ltrim(Str(rn))+[\]  && ���� � ������� ��������
	Endcase
	IF RMES<>MO_MES OR RGOD<>MO_GOD
		PRAB=PRAB+[A]+STR(RGOD,4)+[\KV]+STR(CEILING(RMES/3),1)+[\]
	ENDIF 

*** �������� ������ �� ***
	Wait rnn+[�������� ������ �� ]+cMes(rMes)+[ ]+Right(Str(rGod,4),2) Window Nowait
	Use &pRab.r4_5 In 0 Noupdate Shared Order k_s4_gm
	Use &pRab.r4_5_1 In 0 Noupdate Shared Order k_s4_gm &&(k_s4)+STR(god,4)+STR(mes,2)+str(k_s15)+str(k_s1)+str(k_s18)
	Use &pRab.r4_5_1p Order k_s4_gm In 0 Noupdate       && STR(k_s4)+STR(god,4)+STR(mes,2)+STR(k_s15)+STR(k_s1)
	Use &pRab.r4_5s3 Order k_s4_gm In 0 Noupdate        && STR(k_s4)+STR(god,4)+STR(mes,2)+STR(k_s15)
	Wait rnn+[ ����-��� ������ �/�������� �� ]+cMes(rMes)+[ ]+Right(Str(rGod,4),2) Window Nowait

* ����� ���������� �� �/������ �� �������� ��� � �����
	Select k_s4, k_s15, k_s1, Sum(sn_dengi) As sn_dengi, Sum(pe_dengi) As pe_dengi From r4_5_1;
		WHERE k_s65=2 And god=rGod And mes=rMes And pe_dengi-sn_dengi<>0;
		GROUP By k_s4, k_s15, k_s1 Order By k_s4, k_s15, k_s1 Into Cursor c451
	Count To kz
	ikz=Iif(Int(kz/100)=0,1,Int(kz/100))
	Select c451
	Go Top
	Store 0 To ls, i
	Scan
		If k_s4<>ls
			ls=k_s4
			Store [] To dm,kv
			Select r4_5
			If Seek(Str(ls)+Str(rGod,4)+Str(rMes,2))
				dm=Trim(t_s5_name)+Iif(Val(t_dom)>0,;
					STR(Val(t_dom),4)+Substr(Trim(t_dom),Len(Ltrim(Str(Val(t_dom))))+1),;
					[ ]+Trim(t_dom))+Iif(Empty(t_kor), [], [ ���.]+Trim(t_kor))
				kv=Iif(Val(t_nomer_kv)>0,;
					STR(Val(t_nomer_kv),4)+Substr(Trim(t_nomer_kv),Len(Ltrim(Str(Val(t_nomer_kv))))+1),;
					[ ]+Trim(t_nomer_kv))+Iif(Empty(t_nomer_ko), [], [ (]+Trim(t_nomer_ko)+[)])
			Endif
		Endif
		Select c451
		Do Case
		Case Inlist(k_s15,12,15,19,69) Or Between(k_s15,76,83)  Or Between(k_s15,176,183) Or Between(k_s15,376,383)  Or Between(k_s15,476,483) && ���
			Do AddRec With 'G', 1
		Case Inlist(k_s15,7,18)                                                          && �����.
			Do AddRec With 'G', 2
		Case Inlist(k_s15,10,310,110,410)                                                        && ���
			Do AddRec With 'H', 1
		Case Inlist(k_s15,11,20,53,311,320,353)                                          && ��
			Do AddRec With 'H', 2
		Endcase
		Select c451
		i=i+1
		If i%ikz=0
			Wait rnn+[���������� ���������� �� �/��������: ]+Str(i*100/kz,5)+[%] Window Nowait
		Endif
	Endscan
	Use In r4_5
	Use In r4_5_1
	Use In r4_5_1p
	Use In r4_5s3
	Use In c451
Next
Use In s_post
Use In S21

Select Distinct usl, k_post, tpis From (FlItg) Into Cursor cps WHERE k_post>0 AND &lst_pst
Select cps
Go Top
Scan
	pst=k_post
	tus=usl
	ist=Trim(tpis)
	=Seek(pst,[s1])
	cFileR=[prr]+otms+[_]+Padl(Ltrim(Str(pst,3)),3,'0')+Iif(tus='G','gv','hv')+Iif(Len(ist)>0,[_]+ist,[])+[.xls]
	Wait Iif(tus='G','�� ','�� ')+Padl(Ltrim(Str(pst,3)),3,'0')+[ ������������ ��������� XLS-����� ] Window Nowait
	Select addm, nkv, fio, k_s4, plos, jilr, mespr, Dtoc(dtn) As datn, Dtoc(dtk) As datk, prich,;
		SUM(sumvd) As sumvd, Sum(sum_2) As sum_2;
		FROM (FlItg) Where usl=tus And k_post=pst And Iif(Empty(ist),Empty(tpis),tpis=ist);
		GROUP By addm, nkv, fio, k_s4, plos, jilr, mespr, dtn, dtk, prich Order By addm, nkv, k_s4, dtn;
		INTO Cursor pruk

*   COPY FILE [d:\RABp\rep\prr.xls] TO (cFileR)
	Copy File (lPathRep+[prr.xls]) To LPathXls+(cFileR)

	ap=Createobject("Excel.Application")
	ap.Visible=.F.
	ap.WorkBooks.Open(LPathXls+cFileR)
 
 If Vartype(loZDN)<>'U'
		loZDN.protokol=loZDN.protokol+'����������� ���� '+LPathXls+cFileR+Chr(13)
	Endif

	ap.Sheets(1).Range("A1").Select
	ap.Application.Selection.Value=S1.Name
	ap.Sheets(1).Range("F1").Select
	ap.Application.Selection.Value=cMes(rMes)+[ ]+Str(rGod,4)
	ap.Sheets(1).Range("J1").Select
	Do Case
	Case ist='ts'
		ap.Application.Selection.Value=[��� "����"]
	Case ist='tt'
		ap.Application.Selection.Value=[��� "���"]
	Case ist='ge'
		ap.Application.Selection.Value=[����� "��"]
	Endcase
	ap.Sheets(1).Range("J3").Select
	ap.Application.Selection.Value=Iif(tus='G', [���], [���])
	ap.Sheets(1).Range("K3").Select
	ap.Application.Selection.Value=Iif(tus='G', [���������], [��])
	Store 0 To i
	Store 0 To sumvdS, sum_2S
	Select pruk
	Go Top
	Scan
		i=i+1
		ap.Sheets(1).Range("A"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=addm
		ap.Sheets(1).Range("B"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=Alltrim(nkv)
		ap.Sheets(1).Range("C"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=fio
		ap.Sheets(1).Range("D"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=Iif(plos#0, plos, [])
		ap.Sheets(1).Range("E"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=Iif(jilr#0, jilr, [])
		ap.Sheets(1).Range("F"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=mespr
		ap.Sheets(1).Range("G"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=datn
		ap.Sheets(1).Range("H"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=datk
		ap.Sheets(1).Range("I"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=prich
		ap.Application.Selection.BorderS(10).LineStyle=1
		ap.Sheets(1).Range("J"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=Iif(sumvd#0, sumvd, [])
		ap.Sheets(1).Range("K"+Alltrim(Str(3+i))).Select
		ap.Application.Selection.Value=Iif(sum_2#0, sum_2, [])
		ap.Application.Selection.BorderS(10).LineStyle=1
		sumvdS=sumvdS+sumvd
		sum_2S=sum_2S+sum_2
	Endscan
	i=i+2
	ap.Sheets(1).Range("J"+Alltrim(Str(3+i))).Select
	ap.Application.Selection.Value=sumvdS
	ap.Sheets(1).Range("K"+Alltrim(Str(3+i))).Select
	ap.Application.Selection.Value=sum_2S
	ap.Sheets(1).Range("J"+Alltrim(Str(3+i))+":K"+Alltrim(Str(3+i))).Select
	ap.Application.Selection.Font.Bold=.T.
	ap.Sheets(1).Range("A1").Select
	ap.ActiveWorkBook.Save
	ap.WorkBooks.Close
	ap.Application.Quit
	Release ap
	Use In pruk
	Select cps
Endscan
Use In cps
Use In S1
Wait Clear

** ������ � �������� �������
Procedure AddRec
Parameters tipu,vidu

pst=pris_pst(k_s1, r4_5.k_s1sod, r4_5.k_s6_1, r4_5.k_s8)
If c451.pe_dengi#0
	Select r4_5_1p
	If Seek(Str(ls)+Str(rGod,4)+Str(rMes,2)+Str(c451.k_s15)+Str(c451.k_s1))
		Do While !Eof() And k_s4=ls And god=rGod And mes=rMes And k_s15=c451.k_s15 And k_s1=c451.k_s1
			If pe_dengi#0
				t21=[]
				If Int(k_s21_all)=1
					If Seek(k_s21_all*10000-10000,[s1])
						t21=S1.Name
					Endif
				Else
					If Seek(k_s21_all*10000-210000,[s21])
						t21=S21.Name
					Endif
				Endif
				Select (FlItg)
				Append Blank
				Replace usl With tipu, addm With dm, nkv With kv, fio With r4_5.fio, k_s4 With ls,;
					plos With r4_5.pl_ras_stl, jilr With r4_5.kol_jilr,;
					mespr With Left(cMes[r4_5_1p.mes_t],3)+[.]+Right(Str(r4_5_1p.god_t,4),2),;
					dtn With Iif(Empty(r4_5_1p.dt1) Or r4_5_1p.dt1>r4_5_1p.dt2, r4_5_1p.data1, r4_5_1p.dt1),;
					dtk With Iif(Empty(r4_5_1p.dt2) Or r4_5_1p.dt1>r4_5_1p.dt2, r4_5_1p.data2, r4_5_1p.dt2),;
					prich With t21, k_post With pst
				Do Case
				Case Inlist(c451.k_s1,650,651,652)
					Replace tpis With 'ts'
				Case Inlist(c451.k_s1,655,656)
					Replace tpis With 'tt'
				Case Inlist(c451.k_s1,670,671)
					Replace tpis With 'ge'
				Endcase
				If vidu=1
					Replace sumvd With r4_5_1p.pe_dengi, volvd With r4_5_1p.perashod
				Else
					Replace sum_2 With r4_5_1p.pe_dengi, vol_2 With r4_5_1p.perashod
				Endif
			Endif
			Select r4_5_1p
			Skip
		Enddo
	Endif
Endif
If c451.sn_dengi#0
	Select r4_5s3
	If Seek(Str(ls)+Str(rGod,4)+Str(rMes,2)+Str(c451.k_s15))
		Do While !Eof() And k_s4=ls And god=rGod And mes=rMes And k_s15=c451.k_s15
			If sn_dengi#0 And k_s1=c451.k_s1
				Select (FlItg)
				Append Blank
				Replace usl With tipu, addm With dm, nkv With kv, fio With r4_5.fio, k_s4 With ls,;
					plos With r4_5.pl_ras_stl, jilr With r4_5.kol_jilr,;
					mespr With Left(cMes[r4_5s3.mes_t],3)+[.]+Right(Str(r4_5s3.god_t,4),2),;
					dtn With Iif(Empty(r4_5s3.p_data1), r4_5s3.dt1, r4_5s3.p_data1),;
					dtk With Iif(Empty(r4_5s3.p_data2), r4_5s3.dt2, r4_5s3.p_data2),;
					prich With r4_5s3.t_s21_name, k_post With pst
				Do Case
				Case Inlist(c451.k_s1,650,651,652)
					Replace tpis With 'ts'
				Case Inlist(c451.k_s1,655,656)
					Replace tpis With 'tt'
				Case Inlist(c451.k_s1,670,671)
					Replace tpis With 'ge'
				Endcase
				If vidu=1
					Replace sumvd With -r4_5s3.sn_summa, volvd With -r4_5s3.snrashod
				Else
					Replace sum_2 With -r4_5s3.sn_summa, vol_2 With -r4_5s3.snrashod
				Endif
			Endif
			Select r4_5s3
			Skip
		Enddo
	Endif
Endif

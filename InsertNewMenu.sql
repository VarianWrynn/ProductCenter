/****** Script for SelectTopNRows command from SSMS  ******/
  
  --Insert Admin:Insert Into User_Role_Relation values('519AD3B1-9661-4610-ABDE-A06FC775AC11','6E2A2D5D-A4FB-478C-8F4E-D0464C4DCC34');
  
 use PermaisuriCMS
  
 Insert Menu_Resource ([MenuID],[ParentMenuID],[MenuName],[SortNo],[MenuUrl],[Visible],[Created_By],[Created_On]) 
 values('20010','200','Sales by Channel',220,'SalesByChannel',1,'admin',getdate())
 
 insert Role_Menu_Relation(MR_ID,Role_GUID) values(9,'6E2A2D5D-A4FB-478C-8F4E-D0464C4DCC34')
 
  Insert Menu_Resource ([MenuID],[ParentMenuID],[MenuName],[SortNo],[MenuUrl],[Visible],[Created_By],[Created_On]) 
 values('70020','700','User Management',260,'/Users/MenuManagement',1,'admin',getdate())
 
 
 insert Role_Menu_Relation(MR_ID,Role_GUID) values(13,'6E2A2D5D-A4FB-478C-8F4E-D0464C4DCC34')
 
 
 select * from Menu_Resource  where parentMenuID = '700'
 
 
 select * from Security_Role


select * from  User_Profile

SELECT * FROM (  SELECT ROW_NUMBER() OVER (order by T.Display_Name desc)AS Row, T.*  from User_Profile T  )
 TT WHERE TT.Row between 1 and 10
 
 
 
 select * from User_Role_Relation
 
 
 select * from Role_Menu_Relation
 
 
 

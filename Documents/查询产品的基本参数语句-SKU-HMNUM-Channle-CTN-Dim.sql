select WP.SKU ,C.ChannelName,WP.ProductName,B.BrandName,wp.upc,SC.SalePrice,
	SM.MaterialName,SKUC.ColourName,
	HM.HMNUM,
	SKUHM.R_QTY as Pieces,
	HM.MasterPack,
	HM.ProductName as HMName,HM.StockKey,
	HM.IsGroup,
	HC.FirstCost,HC.LandedCost,HC.EstimateFreight,
	CTN.CTNWeight,
	cast(CTN.CTNLength as nvarchar(200)) +' x '+	cast(CTN.CTNWidth as nvarchar(200))+' x '+cast(CTN.CTNHeight as nvarchar(200)) as 'Carton LxWxH',
	
	cast(Dim.DimLength as nvarchar(200)) +' x '+	cast(Dim.DimWidth as nvarchar(200))+' x '+cast(Dim.DimHeight as nvarchar(200)) as 'Dim LxWxH'
	,
	WP.SKUID,
	HM.ProductID,
	SC.SKUCostID,
	HC.HMCostID
	
	from CMS_SKU WP
	inner join Channel C on WP.ChannelID = C.ChannelID
	inner join Brand B on B.BrandID  = WP.BrandID
	Inner Join CMS_SKU_Costing SC on SC.SKUCostID = WP.SKUCostID
	left join CMS_SKU_Material SM on SM.MaterialID = WP.MaterialID
	Left Join CMS_SKU_Colour SKUC on SKUC.ColourID = WP.ColourID
	inner join SKU_HM_Relation SKUHM on SKUHM.SKUID = WP.SKUID
	inner join CMS_HMNUM HM on HM.ProductID = SKUHM.ProductID
	left join CMS_HM_Costing HC on HC.HMCostID = HM.HMCostID
	left join CMS_ProductCTN CTN on CTN.ProductID =  HM.ProductID
	Left join CMS_ProductDimension Dim on Dim.ProductID =HM.ProductID
	
	where WP.ChannelID in (12,46) order by 2
	 
	

	
	

	

	
	
	--ÎÞÖØ¸´
	select  SKU,ChannelID, Count(*) from CMS_SKU 
	group by SKU,ChannelID   Having count(*) >1
	
	
	
	
	





	


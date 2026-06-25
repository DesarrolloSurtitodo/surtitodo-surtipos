CREATE PROCEDURE sap.sp_CheckNegativeStockByAgroupDocument
    @DocumentAgroupId INT
AS
BEGIN
    SET NOCOUNT ON;


DECLARE @ItemList      NVARCHAR(MAX) = '';
DECLARE @WarehouseList NVARCHAR(MAX) = '';
DECLARE @SQL           NVARCHAR(MAX);

SELECT @ItemList = STUFF((
    SELECT
        DISTINCT ',''''' + T1.ItemCode + ''''''
    FROM sap.DocumentAgroup AS T0
    INNER JOIN sap.DocumentAgroupLines AS T1 ON T0.Id = T1.DocumentAgroupId
    WHERE
        T0.IntegrationStatus = 'E'
        AND T0.IntegrationCode = -10
        AND T0.Id = @DocumentAgroupId
    FOR XML PATH(''), TYPE
).value('.', 'NVARCHAR(MAX)')
, 1, 1, '');

SELECT @WarehouseList = STUFF((
    SELECT
        DISTINCT ',''''' + T1.WarehouseCode + ''''''
    FROM sap.DocumentAgroup AS T0
    INNER JOIN sap.DocumentAgroupLines AS T1 ON T0.Id = T1.DocumentAgroupId
    WHERE
        T0.IntegrationStatus = 'E'
        AND T0.IntegrationCode = -10
        AND T0.Id = @DocumentAgroupId
    FOR XML PATH(''), TYPE
).value('.', 'NVARCHAR(MAX)')
, 1, 1, '');


--PRINT 'Items: '      + @ItemList;
--PRINT 'WarehouseCode: '      + @WarehouseList;
SET @WarehouseList = '''''301'''''

PRINT 'WarehouseCode: '      + @WarehouseList;

SET @SQL = N'
SELECT
    -- TF.WarehouseCode,
		301 AS WarehouseCode,
    TF.ItemCode,
    TF.Quantity,
    TF.Inventory,
    TF.Difference
FROM (

    SELECT
    Lines.WarehouseCode,
    Lines.ItemCode,
    Lines.Quantity,
    CAST(TI.Existencia AS INT) AS Inventory,
    CAST(TI.Existencia - Lines.Quantity AS INT) AS Difference
FROM (
    SELECT
        T1.WarehouseCode,
        T1.ItemCode,
        T1.Quantity
    FROM sap.DocumentAgroup AS T0
    INNER JOIN sap.DocumentAgroupLines AS T1 ON T0.Id = T1.DocumentAgroupId
    WHERE T0.IntegrationStatus = ''E''
      AND T0.IntegrationCode   = -10
) AS Lines
INNER JOIN OPENQUERY(AWS_HANA, ''
    SELECT
        T15."WhsCode"    AS "CodigoAlmacen",
        T0."ItemCode"    AS "CodigoArticulo",
        T00."ItemName"   AS "DescripcionArticulo",
        SUM(T0."OnHand") AS "Existencia"
    FROM       "SURTITODO".OITW   T0
    INNER JOIN "SURTITODO"."OITM" T00 ON T0."ItemCode" = T00."ItemCode"
    INNER JOIN "SURTITODO"."OWHS" T15 ON T15."WhsCode" = T0."WhsCode"
    INNER JOIN "SURTITODO"."ITM1" T6  ON T6."ItemCode" = T00."ItemCode"
    WHERE T6."PriceList" = 1
      AND T0."ItemCode"  IN (' + @ItemList + N')
      AND T15."WhsCode"  IN (' + @WarehouseList + N')
    GROUP BY
        T15."WhsCode",
        T0."ItemCode",
        T00."ItemName"
'') AS TI
    ON
    -- TI.CodigoAlmacen  = Lines.WarehouseCode
    TI.CodigoAlmacen  = 301
    AND TI.CodigoArticulo = Lines.ItemCode

) AS TF
WHERE TF.Difference < 0
';

PRINT @SQL;
EXEC sp_executesql @SQL;

END


Create PROCEDURE [dbo].[GetFaceAttendanceForwarding]
    @MonthID INT,
    @YearID INT,
    @BranchID INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LateComingAppID INT, 
            @EarlyGoingAppID INT, 
            @EarlyComingAppID INT, 
            @LateGoingAppID INT,
            @LateGraceMinute INT, 
            @EarlyGoingGraceMinute INT, 
            @EarlyComingGraceMinute INT, 
            @LateGoingGraceMinute INT,
            @LateComingPercent DECIMAL(5, 2), 
            @EarlyGoingPercent DECIMAL(5, 2), 
            @EarlyComingPercent DECIMAL(5, 2), 
            @LateGoingPercent DECIMAL(5, 2);

    -- Fetch values from HR_GlobalSettings
    SELECT TOP 1 
        @LateComingAppID = LateAppID,
        @EarlyGoingAppID = EarlyGoingAppID,
        @LateGraceMinute = LateGraceMinute,    
        @LateComingPercent = LateValueofHours,
        @EarlyGoingGraceMinute = EarlyGoingGraceMinute,
        @EarlyGoingPercent = EarlyGoingValueofHours,
        @EarlyComingAppID = EarlyComingAppID,
        @EarlyComingGraceMinute = EarlyComingGraceMinute,
        @EarlyComingPercent = EarlyComingValueofHours,
        @LateGoingAppID = LateSeatingAppID,
        @LateGoingGraceMinute = LateSeatingGraceMinute,
        @LateGoingPercent = LateSeatingValueofHours
    FROM HR_GlobalSettings;

    SELECT 
        fa.EmployeeID, 
        fa.MarkDate, 
        FORMAT(fa.InTime, 'HH:mm') AS InTime, 
        FORMAT(fa.OutTime, 'HH:mm') AS OutTime,
        DATEDIFF(MINUTE, fa.InTime, fa.OutTime) / 60 AS DHours,
        DATEDIFF(MINUTE, fa.InTime, fa.OutTime) % 60 AS DMinutes,  
        CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS FromDutyTime,
        CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS ToDutyTime,
        
        -- Late Coming Calculation with grace period
        CASE 
            WHEN CAST(fa.InTime AS TIME) > DATEADD(MINUTE, @LateGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))
            AND @LateComingAppID = 1 -- Only calculate if APPID is 1
            THEN CEILING(DATEDIFF(MINUTE, DATEADD(MINUTE, @LateGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME)), CAST(fa.InTime AS TIME)) / 60.0)
            ELSE 0
        END AS LateComingGraceTime,

        -- Early Going Calculation with grace period
        CASE 
            WHEN CAST(fa.OutTime AS TIME) < DATEADD(MINUTE, -@EarlyGoingGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))
            AND @EarlyGoingAppID = 1 -- Only calculate if APPID is 1
            THEN CEILING(DATEDIFF(MINUTE, CAST(fa.OutTime AS TIME), DATEADD(MINUTE, -@EarlyGoingGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))) / 60.0)
            ELSE 0
        END AS EarlyGoingGraceTime,

        -- Per Day Salary Calculation
        FORMAT(CAST(hrsd.SalaryAmount AS FLOAT) / 
        (CASE 
            WHEN MONTH(fa.MarkDate) IN (4, 6, 9, 11) THEN 30.0  
            WHEN MONTH(fa.MarkDate) = 2 THEN 
                CASE 
                    WHEN YEAR(fa.MarkDate) % 4 = 0 AND (YEAR(fa.MarkDate) % 100 != 0 OR YEAR(fa.MarkDate) % 400 = 0) THEN 29.0 
                    ELSE 28.0 
                END
            ELSE 31.0 
        END), 'N2') AS PerDaySalary,

        -- Late Coming Deduction Calculation
        FORMAT(
            (CAST(hrsd.SalaryAmount AS FLOAT) / 
            (CASE 
                WHEN MONTH(fa.MarkDate) IN (4, 6, 9, 11) THEN 30.0
                WHEN MONTH(fa.MarkDate) = 2 THEN 
                    CASE 
                        WHEN YEAR(fa.MarkDate) % 4 = 0 AND (YEAR(fa.MarkDate) % 100 != 0 OR YEAR(fa.MarkDate) % 400 = 0) THEN 29.0
                        ELSE 28.0
                    END
                ELSE 31.0
            END) * 
            @LateComingPercent / 100) * 
            CASE 
                WHEN CAST(fa.InTime AS TIME) > DATEADD(MINUTE, @LateGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))
                AND @LateComingAppID = 1 -- Only calculate if APPID is 1
                THEN CEILING(DATEDIFF(MINUTE, DATEADD(MINUTE, @LateGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME)), CAST(fa.InTime AS TIME)) / 60.0)
                ELSE 0
            END, 'N2') AS LateComingDeduction,

        -- Early Going Deduction Calculation
        FORMAT(
            (CAST(hrsd.SalaryAmount AS FLOAT) / 
            (CASE 
                WHEN MONTH(fa.MarkDate) IN (4, 6, 9, 11) THEN 30.0
                WHEN MONTH(fa.MarkDate) = 2 THEN 
                    CASE 
                        WHEN YEAR(fa.MarkDate) % 4 = 0 AND (YEAR(fa.MarkDate) % 100 != 0 OR YEAR(fa.MarkDate) % 400 = 0) THEN 29.0
                        ELSE 28.0
                    END
                ELSE 31.0
            END) * 
            @EarlyGoingPercent / 100) * 
            CASE 
                WHEN CAST(fa.OutTime AS TIME) < DATEADD(MINUTE, -@EarlyGoingGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))
                AND @EarlyGoingAppID = 1 -- Only calculate if APPID is 1
                THEN CEILING(DATEDIFF(MINUTE, CAST(fa.OutTime AS TIME), DATEADD(MINUTE, -@EarlyGoingGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))) / 60.0)
                ELSE 0
            END, 'N2') AS EarlyGoingDeduction,

        -- Early Coming Grace Time Calculation
        CASE 
            WHEN CAST(fa.InTime AS TIME) < DATEADD(MINUTE, -@EarlyComingGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))
            AND @EarlyComingAppID = 1 -- Only calculate if APPID is 1
            THEN CEILING(DATEDIFF(MINUTE, CAST(fa.InTime AS TIME), CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME)) / 60.0)
            ELSE 0
        END AS EarlyComingGraceTime,

        -- Late Going Grace Time Calculation
        CASE 
            WHEN CAST(fa.OutTime AS TIME) > CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME)
            AND @LateGoingAppID = 1 -- Only calculate if APPID is 1
            THEN CEILING(DATEDIFF(MINUTE, CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME), CAST(fa.OutTime AS TIME)) / 60.0)
            ELSE 0
        END AS LateGoingGraceTime,

        -- Early Coming Overtime Amount Calculation
    FORMAT(
        (CAST(hrsd.SalaryAmount AS FLOAT) / 
        (CASE 
            WHEN MONTH(fa.MarkDate) IN (4, 6, 9, 11) THEN 30.0  
            WHEN MONTH(fa.MarkDate) = 2 THEN 
                CASE 
                    WHEN YEAR(fa.MarkDate) % 4 = 0 AND (YEAR(fa.MarkDate) % 100 != 0 OR YEAR(fa.MarkDate) % 400 = 0) THEN 29.0 
                    ELSE 28.0 
                END
            ELSE 31.0 
        END) * 
        @EarlyComingPercent / 100) * 
        CASE 
            WHEN CAST(fa.InTime AS TIME) < DATEADD(MINUTE, -@EarlyComingGraceMinute, CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME))
            AND @EarlyComingAppID = 1 -- Only calculate if APPID is 1
            THEN CEILING(DATEDIFF(MINUTE, CAST(fa.InTime AS TIME), CAST(CONCAT(RIGHT('00' + CAST(hre.FromDutyTime AS VARCHAR(2)), 2), ':00') AS TIME)) / 60.0)
            ELSE 0
        END, 'N2') AS EarlyComingAmount,

    -- Late Going Amount Calculation
    FORMAT(
        (CAST(hrsd.SalaryAmount AS FLOAT) / 
        (CASE 
            WHEN MONTH(fa.MarkDate) IN (4, 6, 9, 11) THEN 30.0
            WHEN MONTH(fa.MarkDate) = 2 THEN 
                CASE 
                    WHEN YEAR(fa.MarkDate) % 4 = 0 AND (YEAR(fa.MarkDate) % 100 != 0 OR YEAR(fa.MarkDate) % 400 = 0) THEN 29.0
                    ELSE 28.0
                END
            ELSE 31.0
        END) * 
        @LateGoingPercent / 100) * 
        CASE 
            WHEN CAST(fa.OutTime AS TIME) > CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME)
            AND @LateGoingAppID = 1 -- Only calculate if APPID is 1
            THEN CEILING(DATEDIFF(MINUTE, CAST(CONCAT(RIGHT('00' + CAST(hre.ToDutyTime AS VARCHAR(2)), 2), ':00') AS TIME), CAST(fa.OutTime AS TIME)) / 60.0)
            ELSE 0
        END, 'N2') AS LateGoingAmount

    FROM
         CR_FaceAttendances fa
    JOIN 
        HR_Employees hre ON fa.EmployeeID = hre.EmployeeID
    JOIN 
        HR_Salarys hrs ON fa.EmployeeID = hrs.EmployeeID
    JOIN 
        HR_SalaryDetails hrsd ON hrs.SalaryID = hrsd.SalaryID and SalaryTypeID=1
    WHERE 
        MONTH(fa.MarkDate) = @MonthID 
        AND YEAR(fa.MarkDate) = @YearID
        AND hre.BranchTypeID = @BranchID
        AND fa.MarkDate <= GETDATE(); 
END;
GO

Create PROCEDURE [dbo].[GetMonthlySalarySheet]
	@BranchID NVARCHAR(10),
    @EmployeeID NVARCHAR(10), -- Parameter to specify the EmployeeID
    @Month NVARCHAR(2),        -- Parameter for the month (e.g., '10' for October)
    @Year NVARCHAR(4)          -- Parameter for the year (e.g., '2024')
AS
BEGIN
    DECLARE @cols AS NVARCHAR(MAX),
            @query AS NVARCHAR(MAX),
            @EmployeeName NVARCHAR(MAX);

    -- Fetch EmployeeName by concatenating FirstName, FatherName, and FamilyName
    SELECT @EmployeeName = (FirstName + ' ' + FatherName + ' ' + FamilyName), @BranchID = BranchTypeID
    FROM HR_Employees
    WHERE EmployeeID = @EmployeeID and FinalApprovalID=1 and DeleteYNID != 1;

    -- Include Salary Types
    SELECT @cols = STUFF((
        SELECT DISTINCT ',' + QUOTENAME('Salary_' + SalaryTypeName) 
        FROM Settings_SalaryTypes where DeleteYNID != 1
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    -- Step 1: Retrieve distinct column names with unique prefixes for all types
    SELECT @cols = @cols + ',' + STUFF((
        SELECT DISTINCT ',' + QUOTENAME('AdditionalAllowance_' + AddionalAllowanceTypeName) 
        FROM Settings_AddionalAllowanceTypes where DeleteYNID != 1
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    -- Include OverTime Types
    SELECT @cols = @cols + ',' + STUFF((
        SELECT DISTINCT ',' + QUOTENAME('OverTime_' + OverTimeTypeName) 
        FROM Settings_OverTimeTypes where DeleteYNID != 1
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    -- Include Deduction Types
    SELECT @cols = @cols + ',' + STUFF((
        SELECT DISTINCT ',' + QUOTENAME('Deduction_' + DeductionTypeName) 
        FROM Settings_DeductionTypes where DeleteYNID != 1
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    -- Include Fixed Deduction Types
    SELECT @cols = @cols + ',' + STUFF((
        SELECT DISTINCT ',' + QUOTENAME('FixedDeduction_' + FixedDeductionTypeName) 
        FROM Settings_FixedDeductionTypes where DeleteYNID != 1
        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 1, '');

    -- Step 2: Create a dynamic SQL query to pivot the result ensuring all types appear in the same row
    SET @query = '
        SELECT ''' + @EmployeeID + ''' AS EmployeeID, ''' + @EmployeeName + ''' AS EmployeeName,
		(SELECT MonthTypeName From Settings_MonthTypes Where MonthTypeID=''' + @Month + ''') AS MonthName,''' + @BranchID + ''' AS BranchID,''' + @Month + ''' AS MonthID,''' + @Year + ''' AS Year,* FROM 
        ( 
            SELECT ''Salary_'' + SalaryTypeName AS TypeName, 
                ISNULL((SELECT hsd.SalaryAmount 
                         FROM HR_SalaryDetails hsd
                         INNER JOIN HR_Salarys hs ON hs.SalaryID = hsd.SalaryID
                         WHERE hsd.SalaryTypeID = sst.SalaryTypeID AND FinalApprovalID=1 
                           AND hs.EmployeeID = ''' + @EmployeeID + '''), 
                       0) AS Value
            FROM Settings_SalaryTypes sst 

            UNION ALL
            
            SELECT ''AdditionalAllowance_'' + AddionalAllowanceTypeName AS TypeName, 
                ISNULL((SELECT haad.AddionalAllowanceAmount 
                         FROM HR_AddionalAllowanceDetails haad
                         INNER JOIN HR_AddionalAllowances haa ON haa.AddionalAllowanceID = haad.AddionalAllowanceID
                         WHERE haad.AddionalAllowanceTypeID = saat.AddionalAllowanceTypeID AND FinalApprovalID=1
                           AND haa.EmployeeID = ''' + @EmployeeID + ''' 
                           AND MonthTypeID = ''' + @Month + ''' 
                           AND Year = ''' + @Year + '''), 
                   0) AS Value
            FROM Settings_AddionalAllowanceTypes saat 

            UNION ALL
            
             SELECT ''Deduction_'' + DeductionTypeName AS TypeName,
                (SELECT SUM(hd.Amount) 
                        FROM HR_Deductions hd
                        WHERE hd.EmployeeID = ''' + @EmployeeID + ''' 
                          AND hd.Month = ''' + @Month + ''' 
                          AND hd.Year = ''' + @Year + ''' 
                          AND hd.DeductionTypeID = sd.DeductionTypeID AND FinalApprovalID=1 AND DeleteYNID != 1
                    )AS Value
            FROM Settings_DeductionTypes sd

            UNION ALL
            
            SELECT ''OverTime_'' + sott.OverTimeTypeName AS TypeName, 
                (SELECT SUM(hot.Amount) 
                 FROM HR_OverTimes hot
                 WHERE hot.OverTimeTypeID = sott.OverTimeTypeID AND FinalApprovalID=1 AND DeleteYNID != 1
                   AND hot.EmployeeID = ''' + @EmployeeID + ''' 
                   AND hot.MonthTypeID = ''' + @Month + ''' 
                   AND hot.Year = ''' + @Year + ''') AS Value
            FROM Settings_OverTimeTypes sott

            UNION ALL
            
            SELECT ''FixedDeduction_'' + sfdt.FixedDeductionTypeName AS TypeName, 
                ISNULL((SELECT hfdd.FixedDeductionAmount 
                         FROM HR_FixedDeductionDetails hfdd
                         INNER JOIN HR_FixedDeductions hfd ON hfd.FixedDeductionID = hfdd.FixedDeductionTypeID
                         WHERE hfdd.FixedDeductionTypeID = sfdt.FixedDeductionTypeID AND FinalApprovalID=1
                           AND hfd.EmployeeID = ''' + @EmployeeID + '''), 
                   0) AS Value
            FROM Settings_FixedDeductionTypes sfdt

        ) src    
        PIVOT
        (
            MAX(Value)
            FOR TypeName IN (' + @cols + ')
        ) AS pvt'; 

    -- Step 3: Execute the dynamic SQL
    EXEC sp_executesql @query;
END

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190701014528_init')
BEGIN
    CREATE TABLE [Prices] (
        [PriceId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [PriceUSD] decimal(9, 4) NOT NULL,
        [PriceUSDMove] int NOT NULL,
        [MarketCapUSD] int NOT NULL,
        [MarketCapUSDMove] int NOT NULL,
        [VolumeUSD] int NOT NULL,
        [VolumeUSDMove] int NOT NULL,
        [PriceETH] decimal(25, 18) NOT NULL,
        [PriceETHMove] int NOT NULL,
        [PriceBTC] decimal(16, 8) NOT NULL,
        [PriceBTCMove] int NOT NULL,
        CONSTRAINT [PK_Prices] PRIMARY KEY ([PriceId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190701014528_init')
BEGIN
    CREATE TABLE [Requests] (
        [RequestId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [User] nvarchar(max) NULL,
        [Type] int NOT NULL,
        [Response] int NOT NULL,
        CONSTRAINT [PK_Requests] PRIMARY KEY ([RequestId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190701014528_init')
BEGIN
    CREATE TABLE [Stats] (
        [StatId] int NOT NULL IDENTITY,
        [Date] datetime2 NOT NULL,
        [Transactions] int NOT NULL,
        [TxAvgDay] decimal(9, 4) NOT NULL,
        [TxAvgMove] int NOT NULL,
        [Supply] decimal(25, 18) NOT NULL,
        [Circulation] decimal(25, 18) NOT NULL,
        [Burned] decimal(25, 18) NOT NULL,
        [BurnLast1H] decimal(25, 18) NOT NULL,
        [BurnLast1HMove] int NOT NULL,
        [BurnLast24H] decimal(25, 18) NOT NULL,
        [BurnLast24HMove] int NOT NULL,
        [BurnAvgDay] decimal(25, 18) NOT NULL,
        [BurnAvgDayMove] int NOT NULL,
        CONSTRAINT [PK_Stats] PRIMARY KEY ([StatId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190701014528_init')
BEGIN
    CREATE TABLE [Transactions] (
        [TransactionId] int NOT NULL IDENTITY,
        [BlockNumber] nvarchar(max) NULL,
        [TimeStamp] datetimeoffset NOT NULL,
        [Hash] nvarchar(max) NULL,
        [To] nvarchar(max) NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_Transactions] PRIMARY KEY ([TransactionId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190701014528_init')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190701014528_init', N'2.2.1-servicing-10028');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Stats] ADD [Group] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [Base] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [Group] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [MarketCapUSDPct] decimal(5, 4) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [PriceBTCPct] decimal(5, 4) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [PriceBTCWeighted] decimal(16, 8) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [PriceETHPct] decimal(5, 4) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [PriceETHWeighted] decimal(25, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [PriceUSDPct] decimal(5, 4) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [PriceUSDWeighted] decimal(9, 4) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [Source] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    ALTER TABLE [Prices] ADD [VolumeUSDPct] decimal(5, 4) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703004104_more1')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190703004104_more1', N'2.2.1-servicing-10028');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703035517_more2')
BEGIN
    ALTER TABLE [Prices] ADD [MarketCapUSDWeighted] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190703035517_more2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190703035517_more2', N'2.2.1-servicing-10028');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190704155912_droppcts')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prices]') AND [c].[name] = N'MarketCapUSDPct');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Prices] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Prices] DROP COLUMN [MarketCapUSDPct];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190704155912_droppcts')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prices]') AND [c].[name] = N'PriceBTCPct');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Prices] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Prices] DROP COLUMN [PriceBTCPct];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190704155912_droppcts')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prices]') AND [c].[name] = N'PriceETHPct');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Prices] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Prices] DROP COLUMN [PriceETHPct];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190704155912_droppcts')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prices]') AND [c].[name] = N'PriceUSDPct');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Prices] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Prices] DROP COLUMN [PriceUSDPct];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190704155912_droppcts')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190704155912_droppcts', N'2.2.1-servicing-10028');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713025521_indexes')
BEGIN
    CREATE INDEX [IX_Transactions_TimeStamp] ON [Transactions] ([TimeStamp]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713025521_indexes')
BEGIN
    CREATE INDEX [IX_Stats_Date] ON [Stats] ([Date]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713025521_indexes')
BEGIN
    CREATE INDEX [IX_Requests_Date] ON [Requests] ([Date]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713025521_indexes')
BEGIN
    CREATE INDEX [IX_Requests_Response_Type] ON [Requests] ([Response], [Type]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713025521_indexes')
BEGIN
    CREATE INDEX [IX_Prices_Group] ON [Prices] ([Group]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713025521_indexes')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190713025521_indexes', N'2.2.1-servicing-10028');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713124149_indexes2')
BEGIN
    CREATE INDEX [IX_Prices_Date] ON [Prices] ([Date]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190713124149_indexes2')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190713124149_indexes2', N'2.2.1-servicing-10028');
END;

GO


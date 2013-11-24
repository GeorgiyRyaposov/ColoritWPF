
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 11/23/2013 08:58:00
-- Generated from EDMX file: C:\Users\SxWx\Documents\Visual Studio 2010\Projects\ColoritWPF\ColoritWPF\ColorITModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ColoritDb];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Client_Deposit]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Deposit] DROP CONSTRAINT [FK_Client_Deposit];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Client_Loan]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Loan] DROP CONSTRAINT [FK_Client_Loan];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Client_SaleDocument]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[SaleDocument] DROP CONSTRAINT [FK_Client_SaleDocument];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Group_Product]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Product] DROP CONSTRAINT [FK_Group_Product];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_MoveProduct_Product]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[MoveProduct] DROP CONSTRAINT [FK_MoveProduct_Product];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_MoveProductDocument_MoveProduct]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[MoveProduct] DROP CONSTRAINT [FK_MoveProductDocument_MoveProduct];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Producers_Product]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Product] DROP CONSTRAINT [FK_Producers_Product];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Purchase_Product]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Purchase] DROP CONSTRAINT [FK_Purchase_Product];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Purchase_PurchaseDocument]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Purchase] DROP CONSTRAINT [FK_Purchase_PurchaseDocument];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_PurchaseDocument_Client]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[PurchaseDocument] DROP CONSTRAINT [FK_PurchaseDocument_Client];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Sale_Product]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Sale] DROP CONSTRAINT [FK_Sale_Product];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_Sale_SaleDocument]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Sale] DROP CONSTRAINT [FK_Sale_SaleDocument];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_PK_Paints_CarModels]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Paints] DROP CONSTRAINT [FK_PK_Paints_CarModels];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_PK_Paints_Clients]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Paints] DROP CONSTRAINT [FK_PK_Paints_Clients];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[FK_PK_Paints_PaintName]', 'F') IS NOT NULL
    ALTER TABLE [ColorITModelStoreContainer].[Paints] DROP CONSTRAINT [FK_PK_Paints_PaintName];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[ColorITModelStoreContainer].[CarModels]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[CarModels];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Client]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Client];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Density]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Density];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Deposit]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Deposit];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Group]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Group];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Loan]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Loan];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[MoveProduct]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[MoveProduct];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[MoveProductDocument]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[MoveProductDocument];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[PaintName]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[PaintName];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Paints]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Paints];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Producers]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Producers];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Product]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Product];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Purchase]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Purchase];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[PurchaseDocument]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[PurchaseDocument];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Sale]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Sale];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[SaleDocument]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[SaleDocument];
GO
IF OBJECT_ID(N'[ColorITModelStoreContainer].[Settings]', 'U') IS NOT NULL
    DROP TABLE [ColorITModelStoreContainer].[Settings];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Client'
CREATE TABLE [dbo].[Client] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(50)  NOT NULL,
    [Balance] decimal(19,4)  NOT NULL,
    [Info] nvarchar(100)  NULL,
    [Discount] float  NOT NULL,
    [PhoneNumber] nvarchar(100)  NULL,
    [PrivatePerson] bit  NOT NULL
);
GO

-- Creating table 'Deposit'
CREATE TABLE [dbo].[Deposit] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [ClientID] int  NOT NULL,
    [Amount] decimal(19,4)  NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'Group'
CREATE TABLE [dbo].[Group] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'Loan'
CREATE TABLE [dbo].[Loan] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [ClientID] int  NOT NULL,
    [Amount] decimal(19,4)  NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'Product'
CREATE TABLE [dbo].[Product] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(255)  NOT NULL,
    [SelfCost] decimal(19,4)  NOT NULL,
    [Cost] decimal(19,4)  NOT NULL,
    [Warehouse] float  NOT NULL,
    [Storage] float  NOT NULL,
    [Bottled] bit  NOT NULL,
    [MaxDiscount] float  NOT NULL,
    [Group] int  NULL,
    [ProducerId] int  NOT NULL
);
GO

-- Creating table 'CarModels'
CREATE TABLE [dbo].[CarModels] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ModelName] nvarchar(32)  NOT NULL
);
GO

-- Creating table 'PaintName'
CREATE TABLE [dbo].[PaintName] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(64)  NOT NULL,
    [Cost] decimal(19,4)  NOT NULL,
    [Container] decimal(19,4)  NOT NULL,
    [Work] decimal(19,4)  NOT NULL,
    [Census1] float  NOT NULL,
    [Census2] float  NOT NULL,
    [ThreeLayers] bit  NOT NULL,
    [Package] bit  NOT NULL,
    [PaintType] nvarchar(50)  NULL,
    [L2KType] nvarchar(50)  NULL,
    [MaxDiscount] float  NOT NULL
);
GO

-- Creating table 'Paints'
CREATE TABLE [dbo].[Paints] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [CarModelID] int  NULL,
    [PaintCode] nvarchar(32)  NULL,
    [NameID] int  NOT NULL,
    [Amount] float  NOT NULL,
    [Sum] decimal(19,4)  NOT NULL,
    [ClientID] int  NOT NULL,
    [DocState] bit  NOT NULL,
    [PhoneNumber] nvarchar(100)  NULL,
    [ServiceByCode] bit  NOT NULL,
    [ServiceSelection] bit  NOT NULL,
    [ServiceColorist] bit  NOT NULL,
    [AmountPolish] float  NOT NULL,
    [Prepay] decimal(19,4)  NOT NULL,
    [Total] decimal(19,4)  NOT NULL,
    [IsPreorder] bit  NOT NULL
);
GO

-- Creating table 'Settings'
CREATE TABLE [dbo].[Settings] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [ByCodeCost] decimal(19,4)  NOT NULL,
    [SelectionCost] decimal(19,4)  NOT NULL,
    [SelectionAndThreeLayers] decimal(19,4)  NOT NULL,
    [Cash] decimal(19,4)  NOT NULL,
    [CashInBox] decimal(19,4)  NOT NULL
);
GO

-- Creating table 'Density'
CREATE TABLE [dbo].[Density] (
    [ID] int IDENTITY(1,1) NOT NULL,
    [Type] int  NOT NULL,
    [Name] nvarchar(100)  NOT NULL,
    [DensityValue] float  NOT NULL,
    [AccordingThinner] int  NOT NULL,
    [AccordingHardener] int  NOT NULL,
    [ProportionThinner] float  NOT NULL,
    [ProportionHardener] float  NOT NULL
);
GO

-- Creating table 'Producers'
CREATE TABLE [dbo].[Producers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(255)  NOT NULL
);
GO

-- Creating table 'SaleDocument'
CREATE TABLE [dbo].[SaleDocument] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [ClientId] int  NOT NULL,
    [SaleListNumber] int  NOT NULL,
    [Total] decimal(19,4)  NOT NULL,
    [CleanTotal] decimal(19,4)  NOT NULL,
    [Confirmed] bit  NOT NULL,
    [Comments] nvarchar(255)  NULL,
    [Prepay] bit  NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [ClientBalancePartInTotal] decimal(19,4)  NOT NULL
);
GO

-- Creating table 'Sale'
CREATE TABLE [dbo].[Sale] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [ProductID] bigint  NOT NULL,
    [Amount] int  NOT NULL,
    [Discount] float  NULL,
    [SaleListNumber] bigint  NOT NULL,
    [Cost] decimal(19,4)  NOT NULL
);
GO

-- Creating table 'Purchase'
CREATE TABLE [dbo].[Purchase] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [ProductID] bigint  NOT NULL,
    [SelfCost] decimal(19,4)  NOT NULL,
    [ToWarehouse] float  NOT NULL,
    [ToStorage] float  NOT NULL,
    [Date] datetime  NULL,
    [Cost] decimal(19,4)  NOT NULL,
    [ListNumber] bigint  NOT NULL,
    [Amount] int  NOT NULL
);
GO

-- Creating table 'MoveProduct'
CREATE TABLE [dbo].[MoveProduct] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [ProductID] bigint  NOT NULL,
    [Amount] int  NOT NULL,
    [DocNumber] bigint  NOT NULL
);
GO

-- Creating table 'MoveProductDocument'
CREATE TABLE [dbo].[MoveProductDocument] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [DocumentNumber] int  NOT NULL,
    [ToStorage] bit  NOT NULL,
    [ToWarehouse] bit  NOT NULL,
    [Date] datetime  NOT NULL,
    [Comment] nvarchar(100)  NULL,
    [Confirmed] bit  NOT NULL
);
GO

-- Creating table 'PurchaseDocument'
CREATE TABLE [dbo].[PurchaseDocument] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [DocumentNumber] int  NOT NULL,
    [Date] datetime  NOT NULL,
    [Total] decimal(19,4)  NOT NULL,
    [CleanTotal] decimal(19,4)  NOT NULL,
    [ClientId] int  NOT NULL,
    [Confirmed] bit  NOT NULL,
    [Prepay] bit  NOT NULL,
    [Comment] nvarchar(100)  NULL,
    [ToWarehouse] bit  NOT NULL,
    [ToStorage] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'Client'
ALTER TABLE [dbo].[Client]
ADD CONSTRAINT [PK_Client]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Deposit'
ALTER TABLE [dbo].[Deposit]
ADD CONSTRAINT [PK_Deposit]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Group'
ALTER TABLE [dbo].[Group]
ADD CONSTRAINT [PK_Group]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Loan'
ALTER TABLE [dbo].[Loan]
ADD CONSTRAINT [PK_Loan]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Product'
ALTER TABLE [dbo].[Product]
ADD CONSTRAINT [PK_Product]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'CarModels'
ALTER TABLE [dbo].[CarModels]
ADD CONSTRAINT [PK_CarModels]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'PaintName'
ALTER TABLE [dbo].[PaintName]
ADD CONSTRAINT [PK_PaintName]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Paints'
ALTER TABLE [dbo].[Paints]
ADD CONSTRAINT [PK_Paints]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Settings'
ALTER TABLE [dbo].[Settings]
ADD CONSTRAINT [PK_Settings]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Density'
ALTER TABLE [dbo].[Density]
ADD CONSTRAINT [PK_Density]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'Producers'
ALTER TABLE [dbo].[Producers]
ADD CONSTRAINT [PK_Producers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SaleDocument'
ALTER TABLE [dbo].[SaleDocument]
ADD CONSTRAINT [PK_SaleDocument]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ID] in table 'Sale'
ALTER TABLE [dbo].[Sale]
ADD CONSTRAINT [PK_Sale]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Purchase'
ALTER TABLE [dbo].[Purchase]
ADD CONSTRAINT [PK_Purchase]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'MoveProduct'
ALTER TABLE [dbo].[MoveProduct]
ADD CONSTRAINT [PK_MoveProduct]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [Id] in table 'MoveProductDocument'
ALTER TABLE [dbo].[MoveProductDocument]
ADD CONSTRAINT [PK_MoveProductDocument]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PurchaseDocument'
ALTER TABLE [dbo].[PurchaseDocument]
ADD CONSTRAINT [PK_PurchaseDocument]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ClientID] in table 'Deposit'
ALTER TABLE [dbo].[Deposit]
ADD CONSTRAINT [FK_Client_Deposit]
    FOREIGN KEY ([ClientID])
    REFERENCES [dbo].[Client]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Client_Deposit'
CREATE INDEX [IX_FK_Client_Deposit]
ON [dbo].[Deposit]
    ([ClientID]);
GO

-- Creating foreign key on [ClientID] in table 'Loan'
ALTER TABLE [dbo].[Loan]
ADD CONSTRAINT [FK_Client_Loan]
    FOREIGN KEY ([ClientID])
    REFERENCES [dbo].[Client]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Client_Loan'
CREATE INDEX [IX_FK_Client_Loan]
ON [dbo].[Loan]
    ([ClientID]);
GO

-- Creating foreign key on [Group] in table 'Product'
ALTER TABLE [dbo].[Product]
ADD CONSTRAINT [FK_Group_Product]
    FOREIGN KEY ([Group])
    REFERENCES [dbo].[Group]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Group_Product'
CREATE INDEX [IX_FK_Group_Product]
ON [dbo].[Product]
    ([Group]);
GO

-- Creating foreign key on [CarModelID] in table 'Paints'
ALTER TABLE [dbo].[Paints]
ADD CONSTRAINT [FK_PK_Paints_CarModels]
    FOREIGN KEY ([CarModelID])
    REFERENCES [dbo].[CarModels]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PK_Paints_CarModels'
CREATE INDEX [IX_FK_PK_Paints_CarModels]
ON [dbo].[Paints]
    ([CarModelID]);
GO

-- Creating foreign key on [ClientID] in table 'Paints'
ALTER TABLE [dbo].[Paints]
ADD CONSTRAINT [FK_PK_Paints_Clients]
    FOREIGN KEY ([ClientID])
    REFERENCES [dbo].[Client]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PK_Paints_Clients'
CREATE INDEX [IX_FK_PK_Paints_Clients]
ON [dbo].[Paints]
    ([ClientID]);
GO

-- Creating foreign key on [NameID] in table 'Paints'
ALTER TABLE [dbo].[Paints]
ADD CONSTRAINT [FK_PK_Paints_PaintName]
    FOREIGN KEY ([NameID])
    REFERENCES [dbo].[PaintName]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PK_Paints_PaintName'
CREATE INDEX [IX_FK_PK_Paints_PaintName]
ON [dbo].[Paints]
    ([NameID]);
GO

-- Creating foreign key on [ProducerId] in table 'Product'
ALTER TABLE [dbo].[Product]
ADD CONSTRAINT [FK_Producers_Product]
    FOREIGN KEY ([ProducerId])
    REFERENCES [dbo].[Producers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Producers_Product'
CREATE INDEX [IX_FK_Producers_Product]
ON [dbo].[Product]
    ([ProducerId]);
GO

-- Creating foreign key on [ClientId] in table 'SaleDocument'
ALTER TABLE [dbo].[SaleDocument]
ADD CONSTRAINT [FK_Client_SaleDocument]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Client]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Client_SaleDocument'
CREATE INDEX [IX_FK_Client_SaleDocument]
ON [dbo].[SaleDocument]
    ([ClientId]);
GO

-- Creating foreign key on [ProductID] in table 'Sale'
ALTER TABLE [dbo].[Sale]
ADD CONSTRAINT [FK_Sale_Product]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[Product]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Sale_Product'
CREATE INDEX [IX_FK_Sale_Product]
ON [dbo].[Sale]
    ([ProductID]);
GO

-- Creating foreign key on [ProductID] in table 'Purchase'
ALTER TABLE [dbo].[Purchase]
ADD CONSTRAINT [FK_Purchase_Product]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[Product]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Purchase_Product'
CREATE INDEX [IX_FK_Purchase_Product]
ON [dbo].[Purchase]
    ([ProductID]);
GO

-- Creating foreign key on [ProductID] in table 'MoveProduct'
ALTER TABLE [dbo].[MoveProduct]
ADD CONSTRAINT [FK_MoveProduct_Product]
    FOREIGN KEY ([ProductID])
    REFERENCES [dbo].[Product]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MoveProduct_Product'
CREATE INDEX [IX_FK_MoveProduct_Product]
ON [dbo].[MoveProduct]
    ([ProductID]);
GO

-- Creating foreign key on [DocNumber] in table 'MoveProduct'
ALTER TABLE [dbo].[MoveProduct]
ADD CONSTRAINT [FK_MoveProductDocument_MoveProduct]
    FOREIGN KEY ([DocNumber])
    REFERENCES [dbo].[MoveProductDocument]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MoveProductDocument_MoveProduct'
CREATE INDEX [IX_FK_MoveProductDocument_MoveProduct]
ON [dbo].[MoveProduct]
    ([DocNumber]);
GO

-- Creating foreign key on [SaleListNumber] in table 'Sale'
ALTER TABLE [dbo].[Sale]
ADD CONSTRAINT [FK_Sale_SaleDocument]
    FOREIGN KEY ([SaleListNumber])
    REFERENCES [dbo].[SaleDocument]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Sale_SaleDocument'
CREATE INDEX [IX_FK_Sale_SaleDocument]
ON [dbo].[Sale]
    ([SaleListNumber]);
GO

-- Creating foreign key on [ClientId] in table 'PurchaseDocument'
ALTER TABLE [dbo].[PurchaseDocument]
ADD CONSTRAINT [FK_PurchaseDocument_Client]
    FOREIGN KEY ([ClientId])
    REFERENCES [dbo].[Client]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PurchaseDocument_Client'
CREATE INDEX [IX_FK_PurchaseDocument_Client]
ON [dbo].[PurchaseDocument]
    ([ClientId]);
GO

-- Creating foreign key on [ListNumber] in table 'Purchase'
ALTER TABLE [dbo].[Purchase]
ADD CONSTRAINT [FK_Purchase_PurchaseDocument]
    FOREIGN KEY ([ListNumber])
    REFERENCES [dbo].[PurchaseDocument]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_Purchase_PurchaseDocument'
CREATE INDEX [IX_FK_Purchase_PurchaseDocument]
ON [dbo].[Purchase]
    ([ListNumber]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
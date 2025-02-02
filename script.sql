USE [master]
GO
/****** Object:  Database [mposLOC]    Script Date: 5/10/2019 11:14:12 ******/
CREATE DATABASE [mposLOC]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'mPOS_New', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQL\MSSQL\DATA\mposLOC.mdf' , SIZE = 24576KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'mPOS_New_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQL\MSSQL\DATA\mposLOC_log.ldf' , SIZE = 15040KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [mposLOC] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [mposLOC].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [mposLOC] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [mposLOC] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [mposLOC] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [mposLOC] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [mposLOC] SET ARITHABORT OFF 
GO
ALTER DATABASE [mposLOC] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [mposLOC] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [mposLOC] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [mposLOC] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [mposLOC] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [mposLOC] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [mposLOC] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [mposLOC] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [mposLOC] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [mposLOC] SET  DISABLE_BROKER 
GO
ALTER DATABASE [mposLOC] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [mposLOC] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [mposLOC] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [mposLOC] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [mposLOC] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [mposLOC] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [mposLOC] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [mposLOC] SET RECOVERY FULL 
GO
ALTER DATABASE [mposLOC] SET  MULTI_USER 
GO
ALTER DATABASE [mposLOC] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [mposLOC] SET DB_CHAINING OFF 
GO
ALTER DATABASE [mposLOC] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [mposLOC] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [mposLOC] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'mposLOC', N'ON'
GO
USE [mposLOC]
GO
/****** Object:  UserDefinedFunction [dbo].[AllVIPs_PurchaseProductQty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[AllVIPs_PurchaseProductQty]
(
	
	@fromDate datetime,
	@toDate datetime
)
RETURNS int
AS
BEGIN
	
	DECLARE @pQty int

	select @pQty = Count(TD.ProductId)
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id inner join Product as P on P.Id = TD.ProductId
	where (T.Type = 'Sale' or T.Type = 'Credit')and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0
	
	RETURN @pQty

END

GO
/****** Object:  UserDefinedFunction [dbo].[CheckNewVIP]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CheckNewVIP]
(
	@customerId int,
	@customerType int,
	@fromDate datetime,
	@toDate datetime
)
RETURNS varchar(50)
AS
BEGIN
	DECLARE @NewVIP varchar(50)
	DECLARE @count int
	SET @NewVIP = '';
	select @count = Count(*)
	from Customer as Cus
	where Cus.Id = @customerId and CAST(Cus.PromoteDate as date) >= CAST(@fromDate as date) and CAST(Cus.PromoteDate as date) <= CAST(@toDate as date)
	if @count = 1 
	Begin
		Set @NewVIP = 'This Weeks New VIP'
	End

	RETURN @NewVIP
End

GO
/****** Object:  UserDefinedFunction [dbo].[GetBrandName]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetBrandName]
(
 @BrandId int
)
RETURNS varchar(200)
AS
BEGIN
	Declare @BName varchar(200)
	select @BName = B.Name
	From Brand as B
	where B.Id = @BrandId
	RETURN @BName
END

GO
/****** Object:  UserDefinedFunction [dbo].[GetGWPGiftSetInvoiceAmount]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetGWPGiftSetInvoiceAmount]
(
	@GiftSystemId int,
	@CustomerTypeId int,
	@fromDate datetime,
	@toDate datetime,
	@CounterId int
)
RETURNS bigint
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Amount bigint

	Select @Amount = SUM(T.TotalAmount)
	From [Transaction] as T 
	inner join [AttachGiftSystemForTransaction] as AG on T.Id = AG.TransactionId
	inner join Customer as C on T.CustomerId = C.Id
	Where AG.AttachGiftSystemId = @GiftSystemId and 
	--C.CustomerTypeId = @CustomerTypeId and
	 CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0
	and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
	and ((@CustomerTypeId=1 and t.DateTime >= C.PromoteDate )   or (@CustomerTypeId=2 and t.DateTime  < C.PromoteDate))

	if (@Amount is null)
	Begin
		Set @Amount = 0
	End
	RETURN @Amount

END

GO
/****** Object:  UserDefinedFunction [dbo].[GetGWPGiftSetQty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetGWPGiftSetQty]
(
	@GiftSystemId int,
	@CustomerTypeId int,
	@fromDate datetime,
	@toDate datetime,
	@CounterId int
)
RETURNS int
AS
BEGIN
	-- Declare the return variable here
	DECLARE @GiftSetQty int

	
	SELECT @GiftSetQty = COUNT(*)
	FROM AttachGiftSystemForTransaction as AG 
	inner join [Transaction] as T on T.Id = AG.TransactionId
	inner join Customer as C on C.Id = T.CustomerId
	WHERE AttachGiftSystemId = @GiftSystemId and 
	--C.CustomerTypeId = @CustomerTypeId 
	CAST(T.DateTime as date) >= CAST(@fromDate as date) 
	and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
	and ((@CustomerTypeId=1 and t.DateTime >= C.PromoteDate )   or (@CustomerTypeId=2 and t.DateTime  < C.PromoteDate))

	RETURN @GiftSetQty

END

GO
/****** Object:  UserDefinedFunction [dbo].[GetGWPName]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GetGWPName]
(
	@productId int,
	@transactionId varchar(50)
	
)
RETURNS varchar(200)
AS
BEGIN
	Declare @GiftName varchar(200)

	--Declare @AttachGId int
	DECLARE @AttachGId TABLE (Id int)
	insert into @AttachGId
	select A.AttachGiftSystemId
	from AttachGiftSystemForTransaction as A
	where A.TransactionId = @transactionId
	
	Declare @Gift TABLE (Name varchar(200), productId int)
	insert into @Gift
	select G.Name, G.GiftProductId
	from GiftSystem as G inner join @AttachGId as a on G.Id = a.Id

	select @GiftName = Name
	from @Gift
	where productId = @productId
		
	RETURN @GiftName
END

GO
/****** Object:  UserDefinedFunction [dbo].[VIP_GWP_Qty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[VIP_GWP_Qty]
(
	@customerId int,
	@customerType int,
	@fromDate datetime,
	@toDate datetime
)
RETURNS  int
AS
BEGIN
	
	DECLARE @Qty int
	select @Qty = Count(P.Id)
	from [Transaction] as T inner join Customer as Cus on T.CustomerId = Cus.Id 
	inner join AttachGiftSystemForTransaction as ATG on ATG.TransactionId = T.Id 
	inner join GiftSystem as GS on GS.Id = ATG.AttachGiftSystemId
	inner join Product as P on P.Id = GS.GiftProductId
	left join WrapperItem as Wp on Wp.ParentProductId = P.Id
	where
	 --Cus.CustomerTypeId = @customerType and
	  GS.Type = 'GWP' and T.CustomerId = @customerId and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
	and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0
	and ((@customerType=1 and cast(t.DateTime as date) >= cast(Cus.PromoteDate as date) )   or (@customerType=2 
	and cast(t.DateTime as date)  < cast(Cus.PromoteDate as date)) or (@customerType=2 and Cus.PromoteDate is null))
	Group By T.CustomerId
	
	RETURN @Qty
END

GO
/****** Object:  UserDefinedFunction [dbo].[VIP_Novelty_Qty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[VIP_Novelty_Qty]
(
	@customerId int,
	@customerType int,
	@fromDate datetime,
	@toDate datetime
)
RETURNS INT
AS
BEGIN
	DECLARE @Qty int
	select @Qty = Count(*)
	from [Transaction] as T 
	inner join Customer as Cus on Cus.Id = T.CustomerId 
	inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId
	inner join ProductInNovelty as PN on PN.ProductId = P.Id
	inner join NoveltySystem as NS on NS.Id = PN.NoveltySystemId
	where 
	--Cus.CustomerTypeId = @customerType and T.CustomerId = @customerId and
	 CAST(T.DateTime as date) >= CAST(@fromDate as date) 
	and CAST(T.DateTime as date) <= CAST(@toDate as date) 
	and CAST(NS.ValidFrom as date) <= CAST(T.DateTime as date) 
	and CAST(NS.ValidTo as date) >= CAST(T.DateTime as date) and T.IsDeleted = 0
	and T.CustomerId = @customerId
	and ((@customerType=1 and cast(t.DateTime as date) >= cast(Cus.PromoteDate as date) )  
	 or ((@customerType=2 and cast(t.DateTime as date)  < cast(Cus.PromoteDate as date)) or @customerType=2 and Cus.PromoteDate is null))
	Group By T.CustomerId
	
	RETURN @Qty
END

GO
/****** Object:  UserDefinedFunction [dbo].[VIP_PurchaseProductQty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[VIP_PurchaseProductQty]
(
	@customerId int,
	@customerType int,
	@fromDate datetime,
	@toDate datetime
)
RETURNS int
AS
BEGIN
	
	DECLARE @pQty int

	select @pQty = Sum(TD.Qty)
	from [Transaction] as T inner join Customer as Cus on T.CustomerId = Cus.Id inner join TransactionDetail as TD on TD.TransactionId = T.Id
	 left join Product as P on P.Id = TD.ProductId
	where 
	--Cus.CustomerTypeId = @customerType and
	 (T.Type = 'Sale' or T.Type = 'Credit') and T.CustomerId = @customerId and CAST(T.DateTime as date) >= CAST(@fromDate as date)
	 and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and T.IsComplete = 1 and TD.IsDeleted = 0
	 	and ((@customerType=1 and cast(t.DateTime as date) >= cast(Cus.PromoteDate as date))   or 
		--(@customerType=2 and cast (t.DateTime as date)  < cast(Cus.PromoteDate as date)))
		(@customerType=2 and Cus.PromoteDate is null))
	Group By Cus.Id
	RETURN @pQty

END

GO
/****** Object:  UserDefinedFunction [dbo].[VIP_RefundProductQty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[VIP_RefundProductQty]
(
	@customerId int,
	@customerType int,
	@fromDate datetime,
	@toDate datetime
)
RETURNS int
AS
BEGIN
	
	DECLARE @pQty int

	select @pQty = Sum(TD.Qty)
	from [Transaction] as T inner join Customer as Cus on T.CustomerId = Cus.Id
	 inner join TransactionDetail as TD on TD.TransactionId = T.Id left join Product as P on P.Id = TD.ProductId
	where 
	--Cus.CustomerTypeId = @customerType and 
	(T.Type = 'Refund' or T.Type = 'CreditRefund') and T.CustomerId = @customerId
	 and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0
 	and ((@customerType=1 and cast(t.DateTime as date) >= cast(Cus.PromoteDate as date) )   or (@customerType=2 and cast(t.DateTime as date)  < cast(Cus.PromoteDate as date)))
	Group By Cus.Id
	if @pQty is null
	Begin 
		Set @pQty = 0
	End
	RETURN @pQty

END

GO
/****** Object:  Table [dbo].[AttachGiftSystemForTransaction]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AttachGiftSystemForTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttachGiftSystemId] [int] NOT NULL,
	[TransactionId] [varchar](20) NOT NULL,
 CONSTRAINT [PK_AttachGiftForTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Authorize]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Authorize](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[licenseKey] [varchar](max) NULL,
	[macAddress] [varchar](max) NULL,
 CONSTRAINT [PK_Authorize] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Brand]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Brand](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
 CONSTRAINT [PK_Brand] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[City]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[City](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CityName] [varchar](100) NULL,
 CONSTRAINT [PK_City] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ConsignmentCounter]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConsignmentCounter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[CounterLocation] [nvarchar](200) NULL,
 CONSTRAINT [PK_ConsignmentCounter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Counter]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Counter](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Counter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Currency]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Country] [nvarchar](50) NULL,
	[Symbol] [varchar](5) NULL,
	[CurrencyCode] [varchar](20) NULL,
	[LatestExchangeRate] [int] NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerTypeId] [int] NULL,
	[Title] [varchar](5) NULL,
	[Name] [nvarchar](200) NULL,
	[PhoneNumber] [varchar](50) NULL,
	[Address] [nvarchar](200) NULL,
	[NRC] [varchar](20) NULL,
	[Email] [varchar](100) NULL,
	[CityId] [int] NULL,
	[TownShip] [varchar](200) NULL,
	[Gender] [varchar](10) NULL,
	[Birthday] [date] NULL,
	[PromoteDate] [datetime] NULL,
	[StartDate] [datetime] NULL,
	[VIPMemberId] [varchar](200) NULL,
	[RuleId] [int] NULL,
	[CustomerCode] [varchar](50) NULL,
	[PreferContact] [nvarchar](100) NULL,
	[VipStartedShop] [varchar](50) NULL,
 CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerType]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [varchar](200) NOT NULL,
 CONSTRAINT [PK_CustomerType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DailyRecord]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DailyRecord](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CounterId] [int] NULL,
	[StartDateTime] [datetime] NULL,
	[EndDateTime] [datetime] NULL,
	[OpeningBalance] [bigint] NULL,
	[ClosingBalance] [bigint] NULL,
	[AccuralAmount] [bigint] NULL,
	[DifferenceAmount] [bigint] NULL,
	[Comment] [nvarchar](max) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_DailyRecord] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DeleteLog]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DeleteLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NULL,
	[CounterId] [int] NULL,
	[TransactionId] [varchar](20) NULL,
	[TransactionDetailId] [bigint] NULL,
	[IsParent] [bit] NULL,
	[DeletedDate] [datetime] NULL,
 CONSTRAINT [PK_DeleteLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExchangeRateForTransaction]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExchangeRateForTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[TransactionId] [varchar](20) NOT NULL,
	[ExchangeRate] [int] NOT NULL,
 CONSTRAINT [PK_ExchangeRateForTransaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GiftCard]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GiftCard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CardNumber] [varchar](200) NULL,
	[Amount] [bigint] NULL,
	[IsUsed] [bit] NOT NULL,
	[ExpireDate] [datetime] NULL,
	[CustomerId] [int] NULL,
	[IsUsedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_GiftCard] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GiftCardInTransaction]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GiftCardInTransaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[GiftCardId] [int] NOT NULL,
	[TransactionId] [varchar](20) NOT NULL,
 CONSTRAINT [PK_GiftCardInCustomer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GiftSystem]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GiftSystem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Name] [varchar](200) NULL,
	[MustBuyCostFrom] [bigint] NULL,
	[MustBuyCostTo] [bigint] NULL,
	[MustIncludeProductId] [bigint] NULL,
	[FilterBrandId] [int] NULL,
	[FilterCategoryId] [int] NULL,
	[FilterSubCategoryId] [int] NULL,
	[ValidFrom] [datetime] NOT NULL,
	[ValidTo] [datetime] NOT NULL,
	[UsePromotionQty] [bit] NOT NULL,
	[PromotionQty] [int] NULL,
	[GiftProductId] [bigint] NULL,
	[PriceForGiftProduct] [bigint] NOT NULL,
	[GiftCashAmount] [bigint] NULL,
	[DiscountPercentForTransaction] [int] NULL,
	[UseBrandFilter] [bit] NULL,
	[UseCategoryFilter] [bit] NULL,
	[UseSubCategoryFilter] [bit] NULL,
	[UseProductFilter] [bit] NULL,
	[IsActive] [bit] NULL,
	[UseSizeFilter] [bit] NULL,
	[UseQtyFilter] [bit] NULL,
	[FilterSize] [int] NULL,
	[FilterQty] [int] NULL,
 CONSTRAINT [PK_GiftSystem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Loc_CustomerPoint]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Loc_CustomerPoint](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[OldPoint] [int] NULL,
	[TotalRedeemPoint] [int] NULL,
 CONSTRAINT [PK_Loc_CustomerPoint] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Loc_PointRedeemHistory]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Loc_PointRedeemHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[RedeemPoint] [int] NOT NULL,
	[RedeemAmount] [int] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[CounterId] [int] NOT NULL,
	[CasherId] [int] NOT NULL,
 CONSTRAINT [PK_Loc_PointRedeemHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MainPurchase]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MainPurchase](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierId] [int] NULL,
	[Date] [datetime] NULL,
	[VoucherNo] [nvarchar](50) NULL,
	[TotalAmount] [bigint] NULL,
	[Cash] [bigint] NULL,
	[OldCreditAmount] [bigint] NULL,
	[IsActive] [bit] NULL,
	[DiscountAmount] [int] NULL,
 CONSTRAINT [PK_MainPurchase] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[NoveltySystem]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoveltySystem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[BrandId] [int] NULL,
	[ValidFrom] [datetime] NULL,
	[ValidTo] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_NoveltySystem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointDeductionPercentage_History]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointDeductionPercentage_History](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DiscountRate] [decimal](5, 2) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[UserId] [int] NOT NULL,
	[Counter] [bigint] NOT NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_PointDeductionPercentage_History] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[ProductCode] [varchar](50) NOT NULL,
	[Barcode] [varchar](50) NOT NULL,
	[Price] [bigint] NOT NULL,
	[Qty] [int] NULL,
	[BrandId] [int] NULL,
	[ProductLocation] [nvarchar](200) NULL,
	[ProductCategoryId] [int] NULL,
	[ProductSubCategoryId] [int] NULL,
	[UnitId] [int] NULL,
	[TaxId] [int] NULL,
	[MinStockQty] [int] NULL,
	[DiscountRate] [decimal](5, 2) NOT NULL,
	[IsWrapper] [bit] NULL,
	[IsConsignment] [bit] NULL,
	[IsDiscontinue] [bit] NULL,
	[IsPromotionProduct] [bit] NULL,
	[IsNovelty] [bit] NULL,
	[ConsignmentPrice] [bigint] NULL,
	[ConsignmentCounterId] [int] NULL,
	[Size] [nvarchar](50) NULL,
	[PurchasePrice] [bigint] NULL,
	[IsNotifyMinStock] [bit] NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductInfo]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProductInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [bigint] NULL,
	[Supplier] [nvarchar](200) NULL,
	[ManufacturedCode] [bigint] NULL,
	[LocationInWareHose] [varchar](200) NULL,
	[Length] [int] NULL,
	[Width] [int] NULL,
	[Height] [int] NULL,
	[Weight] [int] NULL,
	[Unit] [varchar](10) NULL,
 CONSTRAINT [PK_ProductInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProductInNovelty]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductInNovelty](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[NoveltySystemId] [bigint] NULL,
	[ProductId] [bigint] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_ProductInNovelty] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductPriceChange]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductPriceChange](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Price] [bigint] NULL,
	[OldPrice] [bigint] NULL,
	[UpdateDate] [datetime] NULL,
	[UserID] [int] NULL,
	[ProductId] [bigint] NULL,
 CONSTRAINT [PK_ProductPriceChange] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductSubCategory]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSubCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[ProductCategoryId] [int] NULL,
 CONSTRAINT [PK_ProductType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PurchaseDetail]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MainPurchaseId] [int] NULL,
	[ProductId] [bigint] NULL,
	[Qty] [int] NULL,
	[UnitPrice] [int] NULL,
 CONSTRAINT [PK_PurchaseDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RoleManagement]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RoleManagement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RuleFeature] [varchar](100) NOT NULL,
	[UserRoleId] [int] NOT NULL,
	[IsAllowed] [bit] NOT NULL,
 CONSTRAINT [PK_RoleManagement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Setting]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Setting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [varchar](max) NULL,
	[Value] [varchar](max) NULL,
 CONSTRAINT [PK_Setting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Shop]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Shop](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ShopName] [varchar](max) NOT NULL,
	[Address] [varchar](max) NULL,
	[PhoneNumber] [varchar](200) NULL,
	[OpeningHours] [varchar](200) NULL,
	[CityId] [int] NOT NULL,
	[ShortCode] [varchar](2) NOT NULL,
	[IsDefaultShop] [bit] NULL,
 CONSTRAINT [PK_Shop] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SPDetail]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SPDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionDetailID] [bigint] NULL,
	[ParentProductID] [bigint] NULL,
	[ChildProductID] [bigint] NULL,
	[Price] [bigint] NULL,
	[DiscountRate] [decimal](15, 3) NULL,
	[SPDetailID] [varchar](50) NULL,
 CONSTRAINT [PK_SPDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Supplier]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Supplier](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[PhoneNumber] [nvarchar](50) NULL,
	[Address] [nvarchar](200) NULL,
	[Email] [varchar](100) NULL,
	[ContactPerson] [nvarchar](200) NULL,
 CONSTRAINT [PK_Supplier] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tax]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tax](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[TaxPercent] [decimal](5, 2) NULL,
 CONSTRAINT [PK_Tax] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [varchar](20) NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL CONSTRAINT [DF_Transaction_UpdatedDate]  DEFAULT (getdate()),
	[UserId] [int] NOT NULL,
	[CounterId] [int] NOT NULL,
	[Type] [varchar](20) NULL,
	[IsPaid] [bit] NULL,
	[IsComplete] [bit] NULL,
	[IsActive] [bit] NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_Transaction_IsDeleted]  DEFAULT ((0)),
	[Loc_IsCalculatePoint] [bit] NOT NULL CONSTRAINT [DF_Transaction_Loc_CalculatePoint]  DEFAULT ((1)),
	[PaymentTypeId] [int] NULL,
	[TaxAmount] [int] NULL,
	[DiscountAmount] [int] NULL,
	[TotalAmount] [decimal](18, 2) NULL,
	[RecieveAmount] [decimal](18, 2) NULL,
	[ParentId] [varchar](20) NULL,
	[GiftCardId] [int] NULL,
	[CustomerId] [int] NULL,
	[ReceivedCurrencyId] [int] NULL,
	[ShopId] [int] NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionDetail]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionId] [varchar](20) NULL,
	[ProductId] [bigint] NULL,
	[Qty] [int] NULL,
	[UnitPrice] [bigint] NULL,
	[DiscountRate] [decimal](5, 2) NOT NULL,
	[TaxRate] [decimal](5, 2) NOT NULL,
	[TotalAmount] [bigint] NULL,
	[IsDeleted] [bit] NULL,
	[IsDeductedBy] [decimal](5, 2) NULL,
 CONSTRAINT [PK_TransactionDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Unit]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UnitName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Unit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsePrePaidDebt]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UsePrePaidDebt](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreditTransactionId] [varchar](20) NULL,
	[PrePaidDebtTransactionId] [varchar](20) NULL,
	[UseAmount] [int] NULL,
	[CashierId] [int] NULL,
	[CounterId] [int] NULL,
 CONSTRAINT [PK_UsePrePaidDebt] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[UserRoleId] [int] NULL,
	[Password] [varchar](max) NULL,
	[DateTime] [datetime] NULL CONSTRAINT [DF_User_DateTime]  DEFAULT (getdate()),
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [varchar](50) NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VIPMemberRule]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VIPMemberRule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RuleName] [nvarchar](200) NOT NULL,
	[Amount] [bigint] NOT NULL,
	[Remark] [nvarchar](400) NULL,
	[IsCalculatePoints] [bit] NULL,
 CONSTRAINT [PK_VIPMemberRule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WrapperItem]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WrapperItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentProductId] [bigint] NULL,
	[ChildProductId] [bigint] NULL,
 CONSTRAINT [PK_WrapperItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[AttachGiftSystemForTransaction]  WITH CHECK ADD  CONSTRAINT [FK_AttachGiftForTransaction_GiftSystem] FOREIGN KEY([AttachGiftSystemId])
REFERENCES [dbo].[GiftSystem] ([Id])
GO
ALTER TABLE [dbo].[AttachGiftSystemForTransaction] CHECK CONSTRAINT [FK_AttachGiftForTransaction_GiftSystem]
GO
ALTER TABLE [dbo].[AttachGiftSystemForTransaction]  WITH CHECK ADD  CONSTRAINT [FK_AttachGiftForTransaction_Transaction] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[AttachGiftSystemForTransaction] CHECK CONSTRAINT [FK_AttachGiftForTransaction_Transaction]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD FOREIGN KEY([RuleId])
REFERENCES [dbo].[VIPMemberRule] ([Id])
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_City] FOREIGN KEY([CityId])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_City]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_CustomerType] FOREIGN KEY([CustomerTypeId])
REFERENCES [dbo].[CustomerType] ([Id])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_CustomerType]
GO
ALTER TABLE [dbo].[DailyRecord]  WITH CHECK ADD  CONSTRAINT [FK_DailyRecord_Counter] FOREIGN KEY([CounterId])
REFERENCES [dbo].[Counter] ([Id])
GO
ALTER TABLE [dbo].[DailyRecord] CHECK CONSTRAINT [FK_DailyRecord_Counter]
GO
ALTER TABLE [dbo].[DeleteLog]  WITH CHECK ADD  CONSTRAINT [FK_DeleteLog_Counter] FOREIGN KEY([CounterId])
REFERENCES [dbo].[Counter] ([Id])
GO
ALTER TABLE [dbo].[DeleteLog] CHECK CONSTRAINT [FK_DeleteLog_Counter]
GO
ALTER TABLE [dbo].[DeleteLog]  WITH CHECK ADD  CONSTRAINT [FK_DeleteLog_Transaction] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[DeleteLog] CHECK CONSTRAINT [FK_DeleteLog_Transaction]
GO
ALTER TABLE [dbo].[DeleteLog]  WITH CHECK ADD  CONSTRAINT [FK_DeleteLog_TransactionDetail] FOREIGN KEY([TransactionDetailId])
REFERENCES [dbo].[TransactionDetail] ([Id])
GO
ALTER TABLE [dbo].[DeleteLog] CHECK CONSTRAINT [FK_DeleteLog_TransactionDetail]
GO
ALTER TABLE [dbo].[DeleteLog]  WITH CHECK ADD  CONSTRAINT [FK_DeleteLog_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[DeleteLog] CHECK CONSTRAINT [FK_DeleteLog_User]
GO
ALTER TABLE [dbo].[ExchangeRateForTransaction]  WITH CHECK ADD  CONSTRAINT [FK_ExchangeRateForTransaction_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[ExchangeRateForTransaction] CHECK CONSTRAINT [FK_ExchangeRateForTransaction_Currency]
GO
ALTER TABLE [dbo].[ExchangeRateForTransaction]  WITH CHECK ADD  CONSTRAINT [FK_ExchangeRateForTransaction_Transaction] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[ExchangeRateForTransaction] CHECK CONSTRAINT [FK_ExchangeRateForTransaction_Transaction]
GO
ALTER TABLE [dbo].[GiftCard]  WITH CHECK ADD  CONSTRAINT [FK_GiftCard_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[GiftCard] CHECK CONSTRAINT [FK_GiftCard_Customer]
GO
ALTER TABLE [dbo].[GiftCardInTransaction]  WITH CHECK ADD  CONSTRAINT [FK_GiftCardInCustomer_GiftCard] FOREIGN KEY([GiftCardId])
REFERENCES [dbo].[GiftCard] ([Id])
GO
ALTER TABLE [dbo].[GiftCardInTransaction] CHECK CONSTRAINT [FK_GiftCardInCustomer_GiftCard]
GO
ALTER TABLE [dbo].[GiftCardInTransaction]  WITH CHECK ADD  CONSTRAINT [FK_GiftCardInCustomer_GiftCardInCustomer] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[GiftCardInTransaction] CHECK CONSTRAINT [FK_GiftCardInCustomer_GiftCardInCustomer]
GO
ALTER TABLE [dbo].[GiftSystem]  WITH CHECK ADD  CONSTRAINT [FK_GiftSystem_Brand] FOREIGN KEY([FilterBrandId])
REFERENCES [dbo].[Brand] ([Id])
GO
ALTER TABLE [dbo].[GiftSystem] CHECK CONSTRAINT [FK_GiftSystem_Brand]
GO
ALTER TABLE [dbo].[GiftSystem]  WITH CHECK ADD  CONSTRAINT [FK_GiftSystem_Product] FOREIGN KEY([MustIncludeProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[GiftSystem] CHECK CONSTRAINT [FK_GiftSystem_Product]
GO
ALTER TABLE [dbo].[GiftSystem]  WITH CHECK ADD  CONSTRAINT [FK_GiftSystem_Product1] FOREIGN KEY([GiftProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[GiftSystem] CHECK CONSTRAINT [FK_GiftSystem_Product1]
GO
ALTER TABLE [dbo].[GiftSystem]  WITH CHECK ADD  CONSTRAINT [FK_GiftSystem_ProductCategory] FOREIGN KEY([FilterCategoryId])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[GiftSystem] CHECK CONSTRAINT [FK_GiftSystem_ProductCategory]
GO
ALTER TABLE [dbo].[GiftSystem]  WITH CHECK ADD  CONSTRAINT [FK_GiftSystem_ProductSubCategory] FOREIGN KEY([FilterSubCategoryId])
REFERENCES [dbo].[ProductSubCategory] ([Id])
GO
ALTER TABLE [dbo].[GiftSystem] CHECK CONSTRAINT [FK_GiftSystem_ProductSubCategory]
GO
ALTER TABLE [dbo].[Loc_CustomerPoint]  WITH CHECK ADD  CONSTRAINT [FK_Loc_CustomerPoint_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[Loc_CustomerPoint] CHECK CONSTRAINT [FK_Loc_CustomerPoint_Customer]
GO
ALTER TABLE [dbo].[Loc_PointRedeemHistory]  WITH CHECK ADD  CONSTRAINT [FK_Loc_PointRedeemHistory_Counter] FOREIGN KEY([CounterId])
REFERENCES [dbo].[Counter] ([Id])
GO
ALTER TABLE [dbo].[Loc_PointRedeemHistory] CHECK CONSTRAINT [FK_Loc_PointRedeemHistory_Counter]
GO
ALTER TABLE [dbo].[Loc_PointRedeemHistory]  WITH CHECK ADD  CONSTRAINT [FK_Loc_PointRedeemHistory_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[Loc_PointRedeemHistory] CHECK CONSTRAINT [FK_Loc_PointRedeemHistory_Customer]
GO
ALTER TABLE [dbo].[Loc_PointRedeemHistory]  WITH CHECK ADD  CONSTRAINT [FK_Loc_PointRedeemHistory_User] FOREIGN KEY([CasherId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Loc_PointRedeemHistory] CHECK CONSTRAINT [FK_Loc_PointRedeemHistory_User]
GO
ALTER TABLE [dbo].[MainPurchase]  WITH CHECK ADD  CONSTRAINT [FK_MainPurchase_Supplier] FOREIGN KEY([SupplierId])
REFERENCES [dbo].[Supplier] ([Id])
GO
ALTER TABLE [dbo].[MainPurchase] CHECK CONSTRAINT [FK_MainPurchase_Supplier]
GO
ALTER TABLE [dbo].[NoveltySystem]  WITH CHECK ADD  CONSTRAINT [FK_NoveltySystem_Brand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([Id])
GO
ALTER TABLE [dbo].[NoveltySystem] CHECK CONSTRAINT [FK_NoveltySystem_Brand]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Brand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[Brand] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Brand]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ConsignmentCounter] FOREIGN KEY([ConsignmentCounterId])
REFERENCES [dbo].[ConsignmentCounter] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ConsignmentCounter]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY([ProductCategoryId])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductCategory]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_ProductType] FOREIGN KEY([ProductSubCategoryId])
REFERENCES [dbo].[ProductSubCategory] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_ProductType]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Tax] FOREIGN KEY([TaxId])
REFERENCES [dbo].[Tax] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Tax]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Unit] FOREIGN KEY([UnitId])
REFERENCES [dbo].[Unit] ([Id])
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_Product_Unit]
GO
ALTER TABLE [dbo].[ProductInNovelty]  WITH CHECK ADD  CONSTRAINT [FK_ProductInNovelty_NoveltySystem] FOREIGN KEY([NoveltySystemId])
REFERENCES [dbo].[NoveltySystem] ([Id])
GO
ALTER TABLE [dbo].[ProductInNovelty] CHECK CONSTRAINT [FK_ProductInNovelty_NoveltySystem]
GO
ALTER TABLE [dbo].[ProductInNovelty]  WITH CHECK ADD  CONSTRAINT [FK_ProductInNovelty_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductInNovelty] CHECK CONSTRAINT [FK_ProductInNovelty_Product]
GO
ALTER TABLE [dbo].[ProductPriceChange]  WITH CHECK ADD  CONSTRAINT [FK_ProductPriceChange_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductPriceChange] CHECK CONSTRAINT [FK_ProductPriceChange_Product]
GO
ALTER TABLE [dbo].[ProductPriceChange]  WITH CHECK ADD  CONSTRAINT [FK_ProductPriceChange_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[ProductPriceChange] CHECK CONSTRAINT [FK_ProductPriceChange_User]
GO
ALTER TABLE [dbo].[ProductSubCategory]  WITH CHECK ADD  CONSTRAINT [FK_ProductSubCategory_ProductCategory] FOREIGN KEY([ProductCategoryId])
REFERENCES [dbo].[ProductCategory] ([Id])
GO
ALTER TABLE [dbo].[ProductSubCategory] CHECK CONSTRAINT [FK_ProductSubCategory_ProductCategory]
GO
ALTER TABLE [dbo].[PurchaseDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetail_MainPurchase] FOREIGN KEY([MainPurchaseId])
REFERENCES [dbo].[MainPurchase] ([Id])
GO
ALTER TABLE [dbo].[PurchaseDetail] CHECK CONSTRAINT [FK_PurchaseDetail_MainPurchase]
GO
ALTER TABLE [dbo].[PurchaseDetail]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[PurchaseDetail] CHECK CONSTRAINT [FK_PurchaseDetail_Product]
GO
ALTER TABLE [dbo].[RoleManagement]  WITH CHECK ADD  CONSTRAINT [FK_RoleManagement_UserRole] FOREIGN KEY([UserRoleId])
REFERENCES [dbo].[UserRole] ([Id])
GO
ALTER TABLE [dbo].[RoleManagement] CHECK CONSTRAINT [FK_RoleManagement_UserRole]
GO
ALTER TABLE [dbo].[Shop]  WITH CHECK ADD  CONSTRAINT [FK_Shop_City] FOREIGN KEY([CityId])
REFERENCES [dbo].[City] ([Id])
GO
ALTER TABLE [dbo].[Shop] CHECK CONSTRAINT [FK_Shop_City]
GO
ALTER TABLE [dbo].[SPDetail]  WITH CHECK ADD  CONSTRAINT [FK_SPDetail_Product] FOREIGN KEY([ParentProductID])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[SPDetail] CHECK CONSTRAINT [FK_SPDetail_Product]
GO
ALTER TABLE [dbo].[SPDetail]  WITH CHECK ADD  CONSTRAINT [FK_SPDetail_Product1] FOREIGN KEY([ChildProductID])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[SPDetail] CHECK CONSTRAINT [FK_SPDetail_Product1]
GO
ALTER TABLE [dbo].[SPDetail]  WITH CHECK ADD  CONSTRAINT [FK_SPDetail_TransactionDetail] FOREIGN KEY([TransactionDetailID])
REFERENCES [dbo].[TransactionDetail] ([Id])
GO
ALTER TABLE [dbo].[SPDetail] CHECK CONSTRAINT [FK_SPDetail_TransactionDetail]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Counter] FOREIGN KEY([CounterId])
REFERENCES [dbo].[Counter] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Counter]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Currency] FOREIGN KEY([ReceivedCurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Currency]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customer] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Customer]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_GiftCard] FOREIGN KEY([GiftCardId])
REFERENCES [dbo].[GiftCard] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_GiftCard]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_PaymentType] FOREIGN KEY([PaymentTypeId])
REFERENCES [dbo].[PaymentType] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_PaymentType]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Transaction] FOREIGN KEY([ParentId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Transaction]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_User]
GO
ALTER TABLE [dbo].[TransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_TransactionDetail_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[TransactionDetail] CHECK CONSTRAINT [FK_TransactionDetail_Product]
GO
ALTER TABLE [dbo].[TransactionDetail]  WITH CHECK ADD  CONSTRAINT [FK_TransactionDetail_Transaction] FOREIGN KEY([TransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[TransactionDetail] CHECK CONSTRAINT [FK_TransactionDetail_Transaction]
GO
ALTER TABLE [dbo].[UsePrePaidDebt]  WITH CHECK ADD  CONSTRAINT [FK_UsePrePaidDebt_Counter] FOREIGN KEY([CounterId])
REFERENCES [dbo].[Counter] ([Id])
GO
ALTER TABLE [dbo].[UsePrePaidDebt] CHECK CONSTRAINT [FK_UsePrePaidDebt_Counter]
GO
ALTER TABLE [dbo].[UsePrePaidDebt]  WITH CHECK ADD  CONSTRAINT [FK_UsePrePaidDebt_Transaction] FOREIGN KEY([CreditTransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[UsePrePaidDebt] CHECK CONSTRAINT [FK_UsePrePaidDebt_Transaction]
GO
ALTER TABLE [dbo].[UsePrePaidDebt]  WITH CHECK ADD  CONSTRAINT [FK_UsePrePaidDebt_Transaction1] FOREIGN KEY([PrePaidDebtTransactionId])
REFERENCES [dbo].[Transaction] ([Id])
GO
ALTER TABLE [dbo].[UsePrePaidDebt] CHECK CONSTRAINT [FK_UsePrePaidDebt_Transaction1]
GO
ALTER TABLE [dbo].[UsePrePaidDebt]  WITH CHECK ADD  CONSTRAINT [FK_UsePrePaidDebt_User] FOREIGN KEY([CashierId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UsePrePaidDebt] CHECK CONSTRAINT [FK_UsePrePaidDebt_User]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_UserRole] FOREIGN KEY([UserRoleId])
REFERENCES [dbo].[UserRole] ([Id])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserRole]
GO
ALTER TABLE [dbo].[WrapperItem]  WITH CHECK ADD  CONSTRAINT [FK_WrapperItem_Product] FOREIGN KEY([ParentProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[WrapperItem] CHECK CONSTRAINT [FK_WrapperItem_Product]
GO
ALTER TABLE [dbo].[WrapperItem]  WITH CHECK ADD  CONSTRAINT [FK_WrapperItem_Product1] FOREIGN KEY([ChildProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[WrapperItem] CHECK CONSTRAINT [FK_WrapperItem_Product1]
GO
/****** Object:  StoredProcedure [dbo].[AverageMonthlySaleReport]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[AverageMonthlySaleReport]
@Year Datetime,
@ProductId bigint
as
begin


Declare  @MonthlySale Table (SaleDate datetime,ProductId bigint, Qty int,TotalAmount bigint)

Insert Into @MonthlySale

select CAST(t.DateTime as date) as SaleDate,pd.Id as ProductId ,Sum(td.Qty) as Qty, Sum( (td.Qty*td.UnitPrice)) as TotalAmount   from Product as pd 

inner join TransactionDetail as td  on td.ProductId=pd.Id

inner join [Transaction] as t on t.Id=td.TransactionId

inner join Unit as u on u.Id=pd.UnitId

where

pd.Id=@ProductId and YEAR(t.DateTime)=YEAR(@Year) and t.IsDeleted=0 and td.IsDeleted =0 and t.IsComplete=1

group by pd.Id,CAST(t.DateTime as date)


Select MONTH(SaleDate) as SaleMonth,ProductId,SUM(Qty) as TotalQty,SUM(TotalAmount) as TotalAmount  from @MonthlySale

Group by MONTH(SaleDate),ProductId

end

GO
/****** Object:  StoredProcedure [dbo].[AverageMonthlySaleReportBrandId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[AverageMonthlySaleReportBrandId]
@Year Datetime,
@BrandId int
as
begin

Declare  @MonthlySale Table (SaleDate datetime,ProductId bigint,Name nvarchar(200),ProductCode nvarchar(200),ProductUnit nvarchar(50),Price bigint, Qty int,TotalAmount bigint)

Insert Into @MonthlySale

			select CAST(t.DateTime as date) as SaleDate,pd.Id as ProductId,pd.Name as Name,pd.ProductCode,u.UnitName as ProductUnit,pd.Price ,Sum(td.Qty) as Qty, Sum( (td.Qty*td.UnitPrice)) as TotalAmount   from Product as pd 

			inner join TransactionDetail as td  on td.ProductId=pd.Id

			inner join [Transaction] as t on t.Id=td.TransactionId

			inner join Unit as u on u.Id=pd.UnitId

			where 						
			pd.BrandId=@BrandId  and YEAR(t.DateTime)=YEAR(@Year) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)   and   (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and t.IsDeleted=0 and td.IsDeleted =0 and t.IsComplete=1

			group by pd.Id,CAST(t.DateTime as date),pd.Name,pd.ProductCode,u.UnitName,pd.Price
			
		

Declare  @TotalAmount Table (TotalAmount int,TotalQty float,PId bigint,PCode nvarchar(200))
Insert Into @TotalAmount
select SUM(TotalAmount)AS TotalAmount,SUM(Qty) AS Qty,ProductId,ProductCode from @MonthlySale Group BY ProductId,ProductCode

--select * from @TotalAmount
--order by PId


Declare  @MonthlySalebymonth Table (PName nvarchar(200),PId bigint,PUnit nvarchar(50),Price bigint,Jan int,Feb int,Mar int,Apr int,May int,Jun int,July int,Aug int,Sep int,Oct int,Nov int,Dece int)
Insert Into @MonthlySalebymonth
select *
from
(
  select Name,ProductId,ProductUnit,Price,DATENAME(month, SaleDate) AS SaleMonth,
    ISNULL(Qty,0) as Qty
  from @MonthlySale


) src
pivot
(
  sum(Qty)
  for SaleMonth in (January,February,March,April,May,June,July,August,September,October,November,December)
) piv;



SELECT t1.PName,t2.PCode,t1.PId,t1.PUnit,t1.Price,ISNULL(t1.Jan,0) AS January ,ISNULL(t1.Feb,0) AS February,ISNULL(t1.Mar,0) AS March,ISNULL(t1.Apr,0) AS April,ISNULL(t1.May,0) AS May,ISNULL(t1.Jun,0) AS June,
ISNULL(t1.July,0) AS July,ISNULL(t1.Aug,0) AS August ,ISNULL(t1.Sep,0) AS September,ISNULL(t1.Oct,0) AS October,ISNULL(t1.Nov,0) AS November,ISNULL(t1.Dece,0) AS December,t2.TotalQty,CAST(t2.TotalQty / 12 AS DECIMAL(18,2)) AS AvgQty,t2.TotalAmount 
FROM @MonthlySalebymonth t1,@TotalAmount t2
WHERE t1.PId=t2.PId
Order by t1.PId


end

GO
/****** Object:  StoredProcedure [dbo].[AverageMonthlySaleReportByBrandIdAndCounterId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[AverageMonthlySaleReportByBrandIdAndCounterId]
@Year Datetime,
@BrandId int,
@CounterId int
as
begin

Declare  @MonthlySale Table (SaleDate datetime,ProductId bigint,Name nvarchar(200),ProductCode nvarchar(200),ProductUnit nvarchar(50),Price bigint, Qty int,TotalAmount bigint)

Insert Into @MonthlySale

			select CAST(t.DateTime as date) as SaleDate,pd.Id as ProductId,pd.Name as Name,pd.ProductCode,u.UnitName as ProductUnit,pd.Price ,Sum(td.Qty) as Qty, Sum( (td.Qty*td.UnitPrice)) as TotalAmount   from Product as pd 

			inner join TransactionDetail as td  on td.ProductId=pd.Id

			inner join [Transaction] as t on t.Id=td.TransactionId

			inner join Unit as u on u.Id=pd.UnitId

			where 						
			t.CounterId=@CounterId and pd.BrandId=@BrandId  and YEAR(t.DateTime)=YEAR(@Year)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6) and   (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and t.IsDeleted=0 and td.IsDeleted =0 and t.IsComplete=1

			group by pd.Id,CAST(t.DateTime as date),pd.Name,pd.ProductCode,u.UnitName,pd.Price
			
		

Declare  @TotalAmount Table (TotalAmount int,TotalQty float,PId bigint,PCode nvarchar(200))
Insert Into @TotalAmount
select SUM(TotalAmount)AS TotalAmount,SUM(Qty) AS Qty,ProductId,ProductCode from @MonthlySale Group BY ProductId,ProductCode

--select * from @TotalAmount
--order by PId


Declare  @MonthlySalebymonth Table (PName nvarchar(200),PId bigint,PUnit nvarchar(50),Price bigint,Jan int,Feb int,Mar int,Apr int,May int,Jun int,July int,Aug int,Sep int,Oct int,Nov int,Dece int)
Insert Into @MonthlySalebymonth
select *
from
(
  select Name,ProductId,ProductUnit,Price,DATENAME(month, SaleDate) AS SaleMonth,
    ISNULL(Qty,0) as Qty
  from @MonthlySale


) src
pivot
(
  sum(Qty)
  for SaleMonth in (January,February,March,April,May,June,July,August,September,October,November,December)
) piv;



SELECT t1.PName,t2.PCode,t1.PId,t1.PUnit,t1.Price,ISNULL(t1.Jan,0) AS January ,ISNULL(t1.Feb,0) AS February,ISNULL(t1.Mar,0) AS March,ISNULL(t1.Apr,0) AS April,ISNULL(t1.May,0) AS May,ISNULL(t1.Jun,0) AS June,
ISNULL(t1.July,0) AS July,ISNULL(t1.Aug,0) AS August ,ISNULL(t1.Sep,0) AS September,ISNULL(t1.Oct,0) AS October,ISNULL(t1.Nov,0) AS November,ISNULL(t1.Dece,0) AS December,t2.TotalQty,CAST(t2.TotalQty / 12 AS DECIMAL(18,2)) AS AvgQty,t2.TotalAmount 
FROM @MonthlySalebymonth t1,@TotalAmount t2
WHERE t1.PId=t2.PId
Order by t1.PId


end

GO
/****** Object:  StoredProcedure [dbo].[AverageMonthlySaleReportByDateTime]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[AverageMonthlySaleReportByDateTime]
@Year Datetime
as
begin

Declare  @MonthlySale Table (SaleDate datetime,ProductId bigint,Name nvarchar(200),ProductCode nvarchar(200),ProductUnit nvarchar(50),Price bigint, Qty int,TotalAmount bigint)

Insert Into @MonthlySale

			select CAST(t.DateTime as date) as SaleDate,pd.Id as ProductId,pd.Name as Name,pd.ProductCode,u.UnitName as ProductUnit,pd.Price ,Sum(td.Qty) as Qty, Sum( (td.Qty*td.UnitPrice)) as TotalAmount   from Product as pd 

			inner join TransactionDetail as td  on td.ProductId=pd.Id

			inner join [Transaction] as t on t.Id=td.TransactionId

			inner join Unit as u on u.Id=pd.UnitId

			where 						
			 YEAR(t.DateTime)=YEAR(@Year) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6) and    (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and t.IsDeleted=0 and td.IsDeleted =0 and t.IsComplete=1

			group by pd.Id,CAST(t.DateTime as date),pd.Name,pd.ProductCode,u.UnitName,pd.Price
			
		

Declare  @TotalAmount Table (TotalAmount int,TotalQty float,PId bigint,PCode nvarchar(200))
Insert Into @TotalAmount
select SUM(TotalAmount)AS TotalAmount,SUM(Qty) AS Qty,ProductId,ProductCode from @MonthlySale Group BY ProductId,ProductCode

--select * from @TotalAmount
--order by PId


Declare  @MonthlySalebymonth Table (PName nvarchar(200),PId bigint,PUnit nvarchar(50),Price bigint,Jan int,Feb int,Mar int,Apr int,May int,Jun int,July int,Aug int,Sep int,Oct int,Nov int,Dece int)
Insert Into @MonthlySalebymonth
select *
from
(
  select Name,ProductId,ProductUnit,Price,DATENAME(month, SaleDate) AS SaleMonth,
    ISNULL(Qty,0) as Qty
  from @MonthlySale


) src
pivot
(
  sum(Qty)
  for SaleMonth in (January,February,March,April,May,June,July,August,September,October,November,December)
) piv;



SELECT t1.PName,t2.PCode,t1.PId,t1.PUnit,t1.Price,ISNULL(t1.Jan,0) AS January ,ISNULL(t1.Feb,0) AS February,ISNULL(t1.Mar,0) AS March,ISNULL(t1.Apr,0) AS April,ISNULL(t1.May,0) AS May,ISNULL(t1.Jun,0) AS June,
ISNULL(t1.July,0) AS July,ISNULL(t1.Aug,0) AS August ,ISNULL(t1.Sep,0) AS September,ISNULL(t1.Oct,0) AS October,ISNULL(t1.Nov,0) AS November,ISNULL(t1.Dece,0) AS December,t2.TotalQty,CAST(t2.TotalQty / 12 AS DECIMAL(18,2)) AS AvgQty,t2.TotalAmount 
FROM @MonthlySalebymonth t1,@TotalAmount t2
WHERE t1.PId=t2.PId
Order by t1.PId


end

GO
/****** Object:  StoredProcedure [dbo].[AverageMonthlySaleReportCounterId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[AverageMonthlySaleReportCounterId]
@Year Datetime,
@CounterId int
as
begin

Declare  @MonthlySale Table (SaleDate datetime,ProductId bigint,Name nvarchar(200),ProductCode nvarchar(200),ProductUnit nvarchar(50),Price bigint, Qty int,TotalAmount bigint)

Insert Into @MonthlySale

			select CAST(t.DateTime as date) as SaleDate,pd.Id as ProductId,pd.Name as Name,pd.ProductCode,u.UnitName as ProductUnit,pd.Price ,Sum(td.Qty) as Qty, Sum( (td.Qty*td.UnitPrice)) as TotalAmount   from Product as pd 

			inner join TransactionDetail as td  on td.ProductId=pd.Id

			inner join [Transaction] as t on t.Id=td.TransactionId

			inner join Unit as u on u.Id=pd.UnitId

			where 						
			t.CounterId=@CounterId and YEAR(t.DateTime)=YEAR(@Year)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6) 
			 and   (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and t.IsDeleted=0 
             and t.IsComplete=1 
			 --or  td.IsDeleted is null 

			group by pd.Id,CAST(t.DateTime as date),pd.Name,pd.ProductCode,u.UnitName,pd.Price
			
		

Declare  @TotalAmount Table (TotalAmount int,TotalQty float,PId bigint,PCode nvarchar(200))
Insert Into @TotalAmount
select SUM(TotalAmount)AS TotalAmount,SUM(Qty) AS Qty,ProductId,ProductCode from @MonthlySale Group BY ProductId,ProductCode

--select * from @TotalAmount
--order by PId


Declare  @MonthlySalebymonth Table (PName nvarchar(200),PId bigint,PUnit nvarchar(50),Price bigint,Jan int,Feb int,Mar int,Apr int,May int,Jun int,July int,Aug int,Sep int,Oct int,Nov int,Dece int)
Insert Into @MonthlySalebymonth
select *
from
(
  select Name,ProductId,ProductUnit,Price,DATENAME(month, SaleDate) AS SaleMonth,
    ISNULL(Qty,0) as Qty
  from @MonthlySale


) src
pivot
(
  sum(Qty)
  for SaleMonth in (January,February,March,April,May,June,July,August,September,October,November,December)
) piv;



SELECT t1.PName,t2.PCode,t1.PId,t1.PUnit,t1.Price,ISNULL(t1.Jan,0) AS January ,ISNULL(t1.Feb,0) AS February,ISNULL(t1.Mar,0) AS March,ISNULL(t1.Apr,0) AS April,ISNULL(t1.May,0) AS May,ISNULL(t1.Jun,0) AS June,
ISNULL(t1.July,0) AS July,ISNULL(t1.Aug,0) AS August ,ISNULL(t1.Sep,0) AS September,ISNULL(t1.Oct,0) AS October,ISNULL(t1.Nov,0) AS November,ISNULL(t1.Dece,0) AS December,t2.TotalQty,CAST(t2.TotalQty / 12 AS DECIMAL(18,2)) AS AvgQty,t2.TotalAmount 
FROM @MonthlySalebymonth t1,@TotalAmount t2
WHERE t1.PId=t2.PId
Order by t1.PId


end

GO
/****** Object:  StoredProcedure [dbo].[ClearDBConnections]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ClearDBConnections]  
AS  
BEGIN  
   
ALTER DATABASE mPOS_New  
SET OFFLINE WITH ROLLBACK IMMEDIATE  
ALTER DATABASE mPOS_New  
SET ONLINE  
  
END

GO
/****** Object:  StoredProcedure [dbo].[CustomerAutoID]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Alter date: <Alter Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CustomerAutoID]
       @IssueDate datetime
      ,@ShopCode varchar(5)
AS
BEGIN
		
	DECLARE @NEWID VARCHAR(20);		
	SELECT @NEWID = ('Cu' + @ShopCode + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
    CONVERT(VARCHAR,N.OID + 1)) FROM (
    SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
    SELECT SUBSTRING(CustomerCode, 5, LEN(CustomerCode)) as TID FROM [Customer] Where SUBSTRING(CustomerCode,0,3) = 'Cu' And SUBSTRING(CustomerCode,3,2) = @ShopCode
) AS T 
) AS N
Select @NEWID
END

GO
/****** Object:  StoredProcedure [dbo].[DeleteWrapperItem]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  proc [dbo].[DeleteWrapperItem]
@Id int,
@ChildProductId int,
@ParentProductId int
as
begin
delete from WrapperItem where Id=@Id AND ParentProductId=@ParentProductId AND ChildProductId=@ChildProductId
end

GO
/****** Object:  StoredProcedure [dbo].[ExportDatabase]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[ExportDatabase]
	@Path varchar(Max),
	@BackUpName varchar(500)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    BACKUP DATABASE POS TO  DISK = @Path WITH NOFORMAT, NOINIT,  NAME = @BackUpName, SKIP, NOREWIND, NOUNLOAD,  STATS = 10

END

GO
/****** Object:  StoredProcedure [dbo].[Get_DamageListStockIn]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Get_DamageListStockIn]
@fromDate datetime,
@toDate datetime
as
begin

select pd.Name,pd.ProductCode,stnD.MFD,stn.ReceiveDate,wh.Name as Warehouse,stnD.Price,stnD.DemageQty,(stnD.DemageQty * stnD.Price) as TotalCost,usr.Name as RegisterName  from StockInDetail as stnD left join 
StockIn as stn on stnD.StockInId=stn.Id left join Product	as pd on stnD.ProductId=pd.Id 
left join dbo.[User] as usr on stn.UserId=usr.Id
left join	 WareHouse as wh on stn.WareHouseId=wh.Id

where CAST(stn.DateTime as Date) >=CAST(@fromDate as Date) and CAST(stn.DateTime as Date)<=CAST(@toDate as Date) and stnD.DemageQty!=0 and stnD.IsDeleted=0

end

GO
/****** Object:  StoredProcedure [dbo].[Get_WarehouseDamagelist]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[Get_WarehouseDamagelist]
@fromDate datetime,
@toDate datetime


as
begin

select  pd.Name, pd.ProductCode,stnD.MFD,stn.ReceiveDate,dm.ResponsibleName,wh.Name as Warehouse,usr.Name as RegisterName, dm.DateTime as DamageDate,stnD.Price,dmDetail.Qty,(dmDetail.Qty*stnD.Price) as TotalCost,
dm.Reason from Damage as dm left join DamageDetail as dmDetail on dm.Id=dmDetail.DamageId left join  StockInDetail as stnD on stnD.Id=dmDetail.StockInDetailId left join 
 StockIn as stn on stn.Id=stnD.StockInId
left join Product as pd on stnD.ProductId=pd.Id
left join dbo.[User] as usr on dm.UserId=usr.Id
left join WareHouse as wh on dm.WareHouseId=wh.Id


 where CAST(dm.DateTime as Date) >=CAST(@fromDate as Date) and CAST(dm.DateTime as Date)<=CAST(@toDate as Date) and dm.IsDeleted=0

RETURN 0
end

GO
/****** Object:  StoredProcedure [dbo].[GetExpiryDatelist]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetExpiryDatelist]
as
begin
select pd.ProductCode,pd.Name as [Product Name],stnD.MFD,stnD.ExpDate,stn.ReceiveDate,DATEDIFF(DAYOFYEAR,GETDATE(),stnD.ExpDate) as [Leave Days],stnD.CurrentQty,stnD.Price from StockInDetail as stnD left join StockIn as stn on stnD.StockInId=stn.Id left join
Product as pd on stnD.ProductId=pd.Id where DATEDIFF(DAYOFYEAR,GETDATE(),stnD.ExpDate)<=365 and stnD.CurrentQty > 0 and stnD.IsDeleted=0 
end

GO
/****** Object:  StoredProcedure [dbo].[GetGWPSetQtyAndAmount]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetGWPSetQtyAndAmount]
	@customerType int,
	@fromDate datetime,
	@toDate datetime,
	@CounterId int
AS
BEGIN
	select g.Id, g.Name, dbo.GetGWPGiftSetQty(g.Id,@customerType,@fromDate,@toDate,@CounterId) as Qty, 
	dbo.GetGWPGiftSetInvoiceAmount(g.Id,@customerType,@fromDate,@toDate,@CounterId) as Amount
	from GiftSystem as g
	where  ((CAST(@fromDate as date) <= CAST(g.ValidFrom as date) and CAST(@toDate as date) >= CAST(g.ValidFrom as date)) 
	or (CAST(g.ValidFrom as date) <= CAST( @fromDate as date) and CAST(g.ValidTo as date) >= CAST(@fromDate as date))) and G.IsActive = 1 and 
	CAST(@fromDate as date)<= CAST(@toDate as date)
END

GO
/****** Object:  StoredProcedure [dbo].[GetGWPTransactions]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetGWPTransactions] --1,'2018-04-01','2018-05-31',0
	@customerType int,
	@fromDate datetime,
	@toDate datetime,
	@CounterId int
AS
declare @customerTypel int
set @customerTypel=@customerType
declare	@fromDatel datetime
set @fromDatel=@fromDate
declare	@toDatel datetime
set @toDatel=@toDate
declare	@CounterIdl int
set @CounterIdl=@CounterId
if @customerTypel = 1 
Begin

	set nocount on
	set arithabort on 
	select Cus.Name as Name, T.Id as InvoiceNo, TD.ProductId as productId, dbo.GetGWPName(TD.ProductId, T.Id) as GiftName, 
	TD.Qty as Qty, TD.DiscountRate as Dis, TD.TotalAmount as Total
	from [Transaction] as T inner join Customer as Cus on Cus.Id = T.CustomerId
	inner join TransactionDetail as TD on TD.TransactionId = T.Id
	where Cus.CustomerTypeId = @customerTypel and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) 
	and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and TD.IsDeleted = 0 and ((@CounterIdl=0 and 1=1) or (@CounterIdl!=0 and t.CounterId =@CounterIdl))
	and (t.DateTime >= cus.PromoteDate )

end
else
Begin
	set nocount on
	set arithabort on 
	select Cus.Name as Name, T.Id as InvoiceNo, TD.ProductId as productId, dbo.GetGWPName(TD.ProductId, T.Id) as GiftName, TD.Qty as Qty, TD.DiscountRate as Dis, TD.TotalAmount as Total
	from [Transaction] as T inner join Customer as Cus on Cus.Id = T.CustomerId
	inner join TransactionDetail as TD on TD.TransactionId = T.Id
	where 
	--(Cus.CustomerTypeId = @customerType or Cus.CustomerTypeId is null) and
	 (T.Type = 'Sale' or T.Type = 'Credit') 
	and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and TD.IsDeleted = 0 
	and ((@CounterIdl=0 and 1=1) or (@CounterIdl!=0 and t.CounterId =@CounterIdl)) 
	and (t.DateTime  < Cus.PromoteDate)

End
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetInvoiceNoForST]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetInvoiceNoForST]
	@ShopCode varchar(5)
AS
	DECLARE @NEWID VARCHAR(10);		
	SELECT @NEWID = ('ST' + @ShopCode + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
	CONVERT(VARCHAR,N.OID + 1)) FROM (
	SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
	SELECT SUBSTRING(Id, 5, LEN(Id)) as TID FROM StockTransfer Where SUBSTRING(Id,0,3) = 'ST' And SUBSTRING(Id,3,2) = @ShopCode
	) AS T 
	) AS N
	select @NEWID
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetNoveliesSaleByCTypte]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveliesSaleByCTypte]
@Type nvarchar(50),
@BrandId int,
@ValidFrom DateTime,
@ValidTo DateTime,
@CityId int,
@CounterId int
as
begin
if(@Type='ALL')
select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
inner join shop as s on s.Id=t.ShopId
left join Customer as c on c.Id=t.CustomerId
where nv.BrandId=@BrandId and pin.IsDeleted=0
and Cast(t.UpdatedDate As date) between Cast(@ValidFrom as date) and Cast(@ValidTo As date)
and Cast(nv.ValidFrom As date) = Cast(@ValidFrom as date) and Cast(nv.ValidTo As date) = Cast(@ValidTo As date)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))


group by (pd.ProductCode),pd.Name,td.UnitPrice

else if(@Type='VIP')
select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
inner join shop as s on s.Id=t.ShopId
left join Customer as c on c.Id=t.CustomerId
where c.CustomerTypeId=1 and nv.BrandId=@BrandId and pin.IsDeleted=0
and Cast(t.UpdatedDate As date) between Cast(@ValidFrom as date) and Cast(@ValidTo As date)
and Cast(nv.ValidFrom As date) = Cast(@ValidFrom as date) and Cast(nv.ValidTo As date) = Cast(@ValidTo As date)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
group by (pd.ProductCode),pd.Name,td.UnitPrice

else if (@Type)='NonVIP'

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
inner join shop as s on s.Id=t.ShopId
left  join Customer as c on c.Id=t.CustomerId
where c.CustomerTypeId=2 and nv.BrandId=@BrandId and pin.IsDeleted=0
and Cast(t.UpdatedDate As date) between Cast(@ValidFrom as date) and Cast(@ValidTo As date)
and Cast(nv.ValidFrom As date) = Cast(@ValidFrom as date) and Cast(nv.ValidTo As date) = Cast(@ValidTo As date)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))


group by (pd.ProductCode),pd.Name,td.UnitPrice

end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveliesSaleByCTypte1]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveliesSaleByCTypte1]
@Type nvarchar(50),
@BrandId int,
@ValidFrom DateTime,
@ValidTo DateTime
as
begin
if(@Type='ALL')
select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId
where nv.BrandId=@BrandId and Cast(t.UpdatedDate As date) between Cast(@ValidFrom as date) and Cast(@ValidTo As date)


group by (pd.ProductCode),pd.Name,td.UnitPrice

else if(@Type='VIP')
select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId
where c.CustomerTypeId=1 and nv.BrandId=@BrandId and Cast(t.UpdatedDate As date) between Cast(@ValidFrom as date) and Cast(@ValidTo As date)
group by (pd.ProductCode),pd.Name,td.UnitPrice

else if (@Type)='NonVIP'

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId
where c.CustomerTypeId=2 and nv.BrandId=@BrandId and Cast(t.UpdatedDate As date) between Cast(@ValidFrom as date) and Cast(@ValidTo As date)


group by (pd.ProductCode),pd.Name,td.UnitPrice

end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltiesSale]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltiesSale]
as
begin
select nv.BrandId,b.Name from NoveltySystem  as nv inner join Brand as b on b.Id =nv.BrandId  group by nv.BrandId,b.Name
end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltySaleByBrandId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltySaleByBrandId]
@BrandId int,
@CityId int,
@CounterId int
as
begin

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
inner join shop as s on s.Id=t.ShopId
left join Customer as c on c.Id=t.CustomerId

where nv.BrandId=@BrandId and pin.IsDeleted=0	and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
group by (pd.ProductCode),pd.Name,td.UnitPrice




end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltySaleByBrandId_Result]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltySaleByBrandId_Result]
@BrandId int
as
begin

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId

where nv.BrandId=@BrandId and pin.IsDeleted=0
group by (pd.ProductCode),pd.Name,td.UnitPrice




end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltySaleByCType_Result]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltySaleByCType_Result]
@Type nvarchar(50)
as
begin
if(@Type='ALL')
select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id 
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId



group by (pd.ProductCode),pd.Name,td.UnitPrice

else if(@Type='VIP')
select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId
where c.CustomerTypeId=1 
group by (pd.ProductCode),pd.Name,td.UnitPrice

else if (@Type)='NonVIP'

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId
where c.CustomerTypeId=2


group by (pd.ProductCode),pd.Name,td.UnitPrice

end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltySaleByDate]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltySaleByDate]
@BrandId int,
@FromDate datetime,
@ToDate datetime,
@CityId int,
@CounterId int
as
begin

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
inner join shop as s on s.Id=t.ShopId
left join Customer as c on c.Id=t.CustomerId

where nv.BrandId=@BrandId and pin.IsDeleted=0 and Cast (nv.ValidFrom as Date)=Cast (@FromDate as Date) and Cast(nv.ValidTo as Date)=Cast( @ToDate as Date)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
group by (pd.ProductCode),pd.Name,td.UnitPrice


end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltySaleByDate_Result]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltySaleByDate_Result]
@BrandId int,
@FromDate datetime,
@ToDate datetime
as
begin

select pd.ProductCode,pd.Name,td.UnitPrice,Sum(td.Qty) as TotalQty, Sum(td.TotalAmount) as TotalAmount from NoveltySystem as nv 
inner join ProductInNovelty as pin on pin.NoveltySystemId=nv.Id
left join [TransactionDetail] as td on td.ProductId=pin.ProductId
inner join Product as pd on td.ProductId=pd.Id
left join [Transaction] as t on t.Id=td.TransactionId
left join Customer as c on c.Id=t.CustomerId

where nv.BrandId=@BrandId and pin.IsDeleted=0 and Cast (nv.ValidFrom as Date)=Cast (@FromDate as Date) and Cast(nv.ValidTo as Date)=Cast( @ToDate as Date)
group by (pd.ProductCode),pd.Name,td.UnitPrice


end

GO
/****** Object:  StoredProcedure [dbo].[GetNoveltySaleDate]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[GetNoveltySaleDate]

@BrandId int
as
begin
select distinct Cast (ValidFrom as Date) as ValidFrom,Cast (ValidTo as Date) as ValidTo from NoveltySystem ns,ProductInNovelty pin
where BrandId=@BrandId and ns.Id=pin.NoveltySystemId and pin.IsDeleted=0



end

GO
/****** Object:  StoredProcedure [dbo].[GetProductReport]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[GetProductReport]
as
begin
select p.ProductCode,p.Name,b.Name as[Brand Name],p.Qty,pC.Name as [Segment Name],pSubC.Name as [SubSegment Name],p.IsDiscontinue  from Product as p
 left join Brand  as b  on p.BrandId=b.Id
 left join ProductCategory as pC on p.ProductCategoryId=pC.Id
 left join ProductSubCategory as pSubC on p.ProductSubCategoryId=pSubC.Id
end

GO
/****** Object:  StoredProcedure [dbo].[GetSaleByRange]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleByRange]
	@fromDate datetime,
	@toDate datetime,
	@year datetime
AS
	Declare  @SaleByFromToDate Table (Id int, FTSTP bigint, FTDST bigint)
	insert into @SaleByFromToDate
	select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as STP, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	right join Brand as B on B.Id = P.BrandId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0
	Group By P.BrandId

	Declare @SaleByStartYear Table (Id int, STSTP bigint, STDST bigint)
	insert into @SaleByStartYear
	select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as STP, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@year as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0
	Group By P.BrandId

	select Br.Id,Br.Name, A.FTSTP,A.FTDST,B.STSTP,B.STDST
	From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id

	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleByRangeWithDiscountedValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleByRangeWithDiscountedValue]
@fromDate datetime,
	@toDate datetime,
	@year varchar(20),
	@CityId int,
	@CountryId int
	
AS
declare @fromdatel datetime
declare @todatel datetime
declare @yearl varchar(20)
declare @CityIdl bit
declare @CountryIdl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @yearl=@year
set @CityIdl=@CityId
set @CountryIdl=@CountryId
	Declare  @SaleByFromToDate Table (Id int, PeriodTotal bigint)
	insert into @SaleByFromToDate
	select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
	right join Brand as B on B.Id = P.BrandId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0


) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))
	Group By P.BrandId
	
	Declare @SaleByStartYear Table (Id int, StartYearlyTotal bigint)
	insert into @SaleByStartYear
	select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
	right join Brand as B on B.Id = P.BrandId
	where (T.Type = 'Sale' or T.Type = 'Credit')
	 and CAST(T.DateTime as date) >= CAST('4-1-'+@yearl as date) 
	 and CAST(T.DateTime as date) <= CAST(@todatel as date)
	  and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted 


= 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

	Group By P.BrandId

	select Br.Id,Br.Name,A.PeriodTotal,B.StartYearlyTotal
	From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id
	where Br.Name != 'Special Promotion'

	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleByRangeWithDiscountedValueAndSaleTrueValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleByRangeWithDiscountedValueAndSaleTrueValue]
	@fromDate datetime,
	@toDate datetime,
	@year varchar(20)
	
AS
	Declare  @SaleByFromToDate Table (Id int, FTSTP bigint, FTDST bigint)
	insert into @SaleByFromToDate
	select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as STP, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	right join Brand as B on B.Id = P.BrandId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6) and TD.IsDeleted = 0
	Group By P.BrandId

	Declare @SaleByStartYear Table (Id int, STSTP bigint, STDST bigint)
	insert into @SaleByStartYear
	select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as STP, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@year as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6) and TD.IsDeleted = 0
	Group By P.BrandId

	select Br.Id,Br.Name, A.FTSTP,A.FTDST,B.STSTP,B.STDST
	From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id
	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleByRangeWithSaleTrueValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleByRangeWithSaleTrueValue]
@fromDate datetime,
	@toDate datetime,
	@year varchar(20),
	@CityId int,
	@CountryId int
	
AS
declare @fromdatel datetime
declare @todatel datetime
declare @yearl varchar(20)
declare @CityIdl bit
declare @CountryIdl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @yearl=@year
set @CityIdl=@CityId
set @CountryIdl=@CountryId
	Declare  @SaleByFromToDate Table (Id int, PeriodTotal bigint)
	insert into @SaleByFromToDate
	select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as STP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	inner join shop as s on s.Id=t.ShopId
	right join Brand as B on B.Id = P.BrandId


	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date)
	 and T.IsDeleted = 0 and B.Name != 'Special Promotion' and ( TD.IsDeleted = 0) and T.IsComplete = 1 

and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))
	Group By P.BrandId

	Declare @SaleByStartYear Table (Id int, StartYearlyTotal bigint)
	insert into @SaleByStartYear
	select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as STP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	inner join shop as s on s.Id=t.ShopId
	right join Brand as B on B.Id = P.BrandId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@yearl as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted = 0) and T.IsComplete = 


1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

	Group By P.BrandId

	select Br.Id,Br.Name, A.PeriodTotal,B.StartYearlyTotal
	From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id
	where Br.Name != 'Special Promotion'
	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleBySegmentWithDiscountedValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE[dbo].[GetSaleBySegmentWithDiscountedValue]
@fromDate datetime,
	@toDate datetime,
	@year varchar(20),
	@CityId int,
	@CountryId int
	
AS
declare @fromdatel datetime
declare @todatel datetime
declare @yearl varchar(20)
declare @CityIdl bit
declare @CountryIdl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @yearl=@year
set @CityIdl=@CityId
set @CountryIdl=@CountryId
	Declare  @SaleByFromToDate Table (Id int, PeriodTotal bigint)
	insert into @SaleByFromToDate
	select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
	right join ProductCategory as C on C.Id = P.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0


) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

	Group By P.ProductCategoryId
	
	Declare @SaleByStartYear Table (Id int, StartYearlyTotal bigint)
	insert into @SaleByStartYear
	select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
			inner join shop as s on s.Id=t.ShopId
	right join ProductCategory as C on C.Id = P.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@yearl as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted 


= 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

	Group By P.ProductCategoryId

	select Br.Id,Br.Name,A.PeriodTotal,B.StartYearlyTotal
	From ProductCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id
	where Br.Name != 'Special Promotion'
	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleBySegmentWithSaleTrueValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleBySegmentWithSaleTrueValue]
	@fromDate datetime,
	@toDate datetime,
	@year varchar(20),
	@CityId int,
	@CountryId int

AS
declare @fromdatel datetime
declare @todatel datetime
declare @yearl varchar(20)
declare @cityidl int
declare @countryidl int
set @fromDatel=@fromDate
set @todatel=@toDate
set @yearl=@year
set @cityidl=@CityId
set @countryidl=@CountryId
	Declare  @SaleByFromToDate Table (Id int, PeriodTotal bigint)
	insert into @SaleByFromToDate
	select  P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as STP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	inner join shop as s on s.Id=t.ShopId
	right join ProductCategory as C on C.Id = P.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0

) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@cityidl != 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@countryidl != 0 and 1=1) or (@countryidl!=0 and t.CounterId=@countryidl))

	Group By P.ProductCategoryId

	Declare @SaleByStartYear Table (Id int, StartYearlyTotal bigint)
	insert into @SaleByStartYear
	select  P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as STP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	inner join shop as s on s.Id=t.ShopId
	right join ProductCategory as C on C.Id = P.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@yearl as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted 

= 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@cityidl != 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@countryidl != 0 and 1=1) or (@countryidl!=0 and t.CounterId=@countryidl))

	Group By P.ProductCategoryId

	select Br.Id,Br.Name, A.PeriodTotal,B.StartYearlyTotal
	From ProductCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id
	Where Br.Name != 'Special Promotion'
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleBySubSegmentWithDiscountedValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleBySubSegmentWithDiscountedValue]
	@fromDate datetime,
	@toDate datetime,
	@year varchar(20),
	@CityId int,
	@CountryId int
	
AS
declare @fromdatel datetime
declare @todatel datetime
declare @yearl varchar(20)
declare @CityIdl bit
declare @CountryIdl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @yearl=@year
set @CityIdl=@CityId
set @CountryIdl=@CountryId
	Declare  @SaleByFromToDate Table (Id int, PeriodTotal bigint)
	insert into @SaleByFromToDate
	select P.ProductSubCategoryId as CId, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	inner join shop as s on s.Id=t.ShopId
	right join ProductSubCategory as C on C.Id = p.ProductSubCategoryId inner join ProductCategory as PC on PC.Id = C.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and PC.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 

0)
	and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryId = 0 and 1=1) or (@CountryId!=0 and t.CounterId=@CountryId))

	Group By P.ProductSubCategoryId
	
	Declare @SaleByStartYear Table (Id int, StartYearlyTotal bigint)
	insert into @SaleByStartYear
	select P.ProductSubCategoryId as CId, Sum(TD.TotalAmount) as DSTP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
	right join ProductSubCategory as C on C.Id = p.ProductSubCategoryId inner join ProductCategory as PC on PC.Id = C.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@yearl as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and PC.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted

= 0)
	and ((@CityIdl = 0 and 1=1) or (@CityIdl!=0 and s.CityId=@CityIdl)) and ((@CountryId = 0 and 1=1) or (@CountryId!=0 and t.CounterId=@CountryId))
	Group By P.ProductSubCategoryId

	select Br.Id,Br.Name,A.PeriodTotal,B.StartYearlyTotal
	From ProductSubCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id inner join ProductCategory as PC on PC.Id = Br.ProductCategoryId
	where PC.Name != 'Special Promotion'
	
	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleBySubSegmentWithSaleTrueValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleBySubSegmentWithSaleTrueValue]
	@fromDate datetime,
	@toDate datetime,
	@year varchar(20)
AS
	declare @fromDatel datetime
	declare @toDatel datetime
	declare @yearl varchar(20)
	set @fromDatel=@fromDate
	set @toDatel=@toDate
	set @yearl=@year
	Declare  @SaleByFromToDate Table (Id int, PeriodTotal bigint)
	insert into @SaleByFromToDate
	select  P.ProductSubCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as STP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	right join ProductSubCategory as C on C.Id = p.ProductSubCategoryId inner join ProductCategory as PC on PC.Id = C.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and PC.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted 
= 
0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
	Group By P.ProductSubCategoryId

	Declare @SaleByStartYear Table (Id int, StartYearlyTotal bigint)
	insert into @SaleByStartYear
	select  P.ProductSubCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as STP
	from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
	inner join Product as P on P.Id = TD.ProductId	
	right join ProductSubCategory as C on C.Id = p.ProductSubCategoryId inner join ProductCategory as PC on PC.Id = C.ProductCategoryId
	where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST('4-1-'+@yearl as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and PC.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted= 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
	Group By P.ProductSubCategoryId

	select Br.Id,Br.Name, A.PeriodTotal,B.StartYearlyTotal
	From ProductSubCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
	Full outer join @SaleByStartYear as B on Br.Id = B.Id inner join ProductCategory as PC on PC.Id = Br.ProductCategoryId
	where PC.Name != 'Special Promotion'
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[GetSaleSpecialPromotionByCustomerId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleSpecialPromotionByCustomerId] 
	@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@bId int,
	@IsSaleTruePrice bit,
	@CityId int,
	@CounterId int
AS
declare @fromdatel datetime
declare @todatel datetime
declare @custypel int
declare @bIdl int
declare @IsSaleTruePricel bit
declare @cityidl int
declare @CounterIdl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @custypel=@cusType
set @bIdl=@bId
set @IsSaleTruePricel=@IsSaleTruePrice
set @cityidl=@CityId
set @CounterIdl=@CounterId
BEGIN
	Declare  @SaleSP Table (Id int, Total bigint, Qty int)
	Declare  @RefundSP Table (Id int, Total bigint,Qty int)
	if @IsSaleTruePricel = 1
		Begin
		if @custypel != 1
		Begin
			insert @SaleSP
			select  Br.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) 
			and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and Br.Id = @bIdl and (TD.IsDeleted IS NULL 
			OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			insert @RefundSP
			select  Br.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and Br.Id = @bIdl and (TD.IsDeleted 

			IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From Brand as Br  Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
		else
		Begin
			insert @SaleSP
			select  Br.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and Br.Id = 

			@bIdl and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			insert @RefundSP
			select  Br.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and 

			Br.Id = @bIdl and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From Brand as Br Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
	End
	Else
	Begin
		if @custypel != 1
		Begin
			insert @SaleSP
			select  Br.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and Br.Id = @bIdl and (TD.IsDeleted IS NULL 

			OR TD.IsDeleted = 0) and T.IsComplete = 1
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			insert @RefundSP
			select  Br.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and Br.Id = @bIdl and (TD.IsDeleted 

			IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From Brand as Br  Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
		else
		Begin
			insert @SaleSP
			select  Br.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and Br.Id = 

			@bIdl and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			insert @RefundSP
			select  Br.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and 

			Br.Id = @bIdl and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl !=0 and t.CounterId=@CounterIdl))
			Group By Br.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From Brand as Br Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
	End
END

GO
/****** Object:  StoredProcedure [dbo].[GetSaleSpecialPromotionSegmentByCustomerId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleSpecialPromotionSegmentByCustomerId] 
	@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@bId int,
	@IsSaleTruePrice bit,
	@CityId int,
	@CounterId int
AS
declare @fromdatel datetime
declare @todatel datetime
declare @custypel int
declare @bIdl int
declare @IsSaleTruePricel bit
declare @cityidl int
declare @counteridl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @custypel=@cusType
set @bIdl=@bId
set @IsSaleTruePricel=@IsSaleTruePrice
set @cityidl=@CityId
set @counteridl=@CounterId

BEGIN
	Declare  @SaleSP Table (Id int, Total bigint, Qty int)
	Declare  @RefundSP Table (Id int, Total bigint,Qty int)
	if @IsSaleTruePricel = 1
		Begin
		if @custypel != 1
		Begin
			insert @SaleSP
			select  PC.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0   and (TD.IsDeleted IS NULL OR TD.IsDeleted 


= 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			insert @RefundSP
			select  PC.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and (TD.IsDeleted IS NULL OR 
			TD.IsDeleted = 0) 
			and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductCategory as Br  Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
		else
		Begin
			insert @SaleSP
			select  PC.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel 
			 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			 and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			insert @RefundSP
			select  PC.Id, Sum((SP.Price - (SP.Price* (SP.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and 


(TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductCategory as Br Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
	End
	Else
	Begin
		if @custypel != 1
		Begin
			insert @SaleSP
			select  PC.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
				inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0  and (TD.IsDeleted IS NULL OR TD.IsDeleted =


 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			insert @RefundSP
			select  PC.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
				inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0  
			and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductCategory as Br  Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
		else
		Begin
			insert @SaleSP
			select  PC.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join shop as s on s.Id=t.ShopId
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel
			 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			 and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			insert @RefundSP
			select  PC.Id, Sum(SP.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId 
			inner join shop as s on s.Id=t.ShopId
			inner join SPDetail as SP on SP.TransactionDetailID = TD.Id
			inner join Product as Pr on Pr.Id = SP.ChildProductID inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductCategory as PC on Pr.ProductCategoryId = PC.Id
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and 


(TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@cityidl=0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@counteridl =0 and 1=1) or (@counteridl !=0 and t.CounterId=@counteridl))
			Group By PC.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductCategory as Br Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
	End
END

GO
/****** Object:  StoredProcedure [dbo].[GetSaleSpecialPromotionSubSegmentByCustomerId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GetSaleSpecialPromotionSubSegmentByCustomerId] 
@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@bId int,
	@IsSaleTruePrice bit,
	@CityId int,
	@CountryId int
AS
declare @fromdatel datetime
declare @todatel datetime
declare @custypel int
declare @bIdl int
declare @IsSaleTruePricel bit
declare @cityidl int
declare @CountryIdl int

set @fromDatel=@fromDate
set @todatel=@toDate
set @custypel=@cusType
set @bIdl=@bId
set @IsSaleTruePricel=@IsSaleTruePrice
set @cityidl=@CityId
set @CountryIdl=@CountryId
BEGIN
	Declare  @SaleSP Table (Id int, Total bigint, Qty int)
	Declare  @RefundSP Table (Id int, Total bigint,Qty int)
	if @IsSaleTruePricel = 1
		Begin
		if @custypel != 1
		Begin
			insert @SaleSP
			select  PCS.Id, Sum((Pr.Price - (Pr.Price* (P.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId
			inner join shop as s on s.Id=t.ShopId
			 inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId inner join ProductSubCategory as PCS on PCS.Id = 

Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and (TD.IsDeleted IS NULL OR TD.IsDeleted =


 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			insert @RefundSP
			select  PCS.Id, Sum((Pr.Price - (Pr.Price* (P.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId
				inner join shop as s on s.Id=t.ShopId
			 inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id =

 Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))


			Group By PCS.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductSubCategory as Br  Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
		else
		Begin
			insert @SaleSP
			select  PCS.Id, Sum((Pr.Price - (Pr.Price* (P.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId
			inner join shop as s on s.Id=t.ShopId
			 inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id =

 Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel 
			and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			insert @RefundSP
			select  PCS.Id, Sum((Pr.Price - (Pr.Price* (P.DiscountRate/100)))*TD.Qty) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId 
			inner join shop as s on s.Id=t.ShopId
			inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id =
 
Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and 


(TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
  and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductSubCategory as Br Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
	End
	Else
	Begin
		if @custypel != 1
		Begin
			insert @SaleSP
			select  PCS.Id, Sum(Pr.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId 
				inner join shop as s on s.Id=t.ShopId
			inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id = 

Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and (TD.IsDeleted IS NULL OR TD.IsDeleted =


 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
  and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			insert @RefundSP
			select  PCS.Id, Sum(Pr.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId
				inner join shop as s on s.Id=t.ShopId
			 inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id =

 Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0  and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			 and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductSubCategory as Br  Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
		else
		Begin
			insert @SaleSP
			select  PCS.Id, Sum(Pr.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId 
				inner join shop as s on s.Id=t.ShopId
			inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id = 

Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			  and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			insert @RefundSP
			select  PCS.Id, Sum(Pr.Price) as Total, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.productId = P.Id
			inner join Brand as B on B.Id = P.BrandId 
				inner join shop as s on s.Id=t.ShopId
			inner join WrapperItem as W on W.ParentProductId = P.Id inner join Product as Pr on Pr.Id = W.ChildProductId inner join Brand as Br on Br.Id = Pr.BrandId inner join Customer as C on C.Id = T.CustomerId  inner join ProductSubCategory as PCS on PCS.Id = 

Pr.ProductSubCategoryId
			where B.Name = 'Special Promotion' and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromdatel as date) and CAST(T.DateTime as date) <= CAST(@todatel as date) and T.IsDeleted = 0 and C.CustomerTypeId = @custypel and 


(TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
	  and ((@cityidl = 0 and 1=1) or (@cityidl!=0 and s.CityId=@cityidl)) and ((@CountryIdl = 0 and 1=1) or (@CountryIdl!=0 and t.CounterId=@CountryIdl))

			Group By PCS.Id

			select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductSubCategory as Br Full outer join @SaleSp as A on A.Id = Br.Id
			Full Outer join @RefundSP B on B.Id = Br.Id
			where Br.Id = @bIdl

		End
	End
END

GO
/****** Object:  StoredProcedure [dbo].[InsertDraft]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertDraft]
			@DateTime datetime
           ,@UserId int
           ,@CounterId int
           ,@Type varchar(20)
           ,@IsPaid bit
           ,@IsActive bit
           ,@PaymentTypeId int
           ,@TaxAmount int
           ,@DiscountAmount int
           ,@TotalAmount decimal(18,2)
           ,@RecieveAmount decimal(18,2)           
           ,@GiftCardId int
           ,@CustomerId int
		   ,@ShopCode varchar(2)		   
		   ,@ShopId int

AS
BEGIN
	DECLARE @NEWID VARCHAR(10);		
	SELECT @NEWID = ('DF' + @ShopCode + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
CONVERT(VARCHAR,N.OID + 1)) FROM (
SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
SELECT SUBSTRING(Id, 5, LEN(Id)) as TID FROM [Transaction] Where SUBSTRING(Id,0,3) = 'DF' And SUBSTRING(Id,3,2) = @ShopCode
) AS T 
) AS N

INSERT INTO [dbo].[Transaction]
           ([Id]
           ,[DateTime]
		   ,[UpdatedDate]
           ,[UserId]
           ,[CounterId]
           ,[Type]
           ,[IsPaid]
		   ,[IsComplete]
           ,[IsActive]
           ,[PaymentTypeId]
           ,[TaxAmount]
           ,[DiscountAmount]
           ,[TotalAmount]
           ,[RecieveAmount]         
           ,[GiftCardId]
           ,[CustomerId]
		   ,[ShopId])
     VALUES
           (@NEWID
           ,@DateTime
		   ,getDate()
           ,@UserId
           ,@CounterId
           ,@Type
           ,@IsPaid
		   ,0
           ,@IsActive
           ,@PaymentTypeId
           ,@TaxAmount
           ,@DiscountAmount
           ,@TotalAmount
           ,@RecieveAmount
           
           ,@GiftCardId
           ,@CustomerId
		   ,@ShopId)

Select @NEWID

END

GO
/****** Object:  StoredProcedure [dbo].[InsertRefundTransaction]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertRefundTransaction]
			@DateTime datetime
           ,@UserId int
           ,@CounterId int
		   ,@Type varchar(20)
           ,@IsPaid bit
           ,@IsActive bit
           ,@PaymentTypeId int
           ,@TaxAmount int
           ,@DiscountAmount int
           ,@TotalAmount decimal(18,2)
           ,@RecieveAmount decimal(18,2)           
		   ,@ParentId varchar(20)
           ,@GiftCardId int
           ,@CustomerId int
		   ,@ShopCode varchar(2)		   
		   ,@ShopId int

AS
BEGIN
	DECLARE @NEWID VARCHAR(10);		
	SELECT @NEWID = ('RF' + @ShopCode + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
CONVERT(VARCHAR,N.OID + 1)) FROM (
SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
SELECT SUBSTRING(Id, 5, LEN(Id)) as TID FROM [Transaction] Where SUBSTRING(Id,0,3) = 'RF' And SUBSTRING(Id,3,2) = @ShopCode
) AS T  
) AS N

INSERT INTO [dbo].[Transaction]
           ([Id]
           ,[DateTime]
		   ,[UpdatedDate]
           ,[UserId]
           ,[CounterId]
           ,[Type]
           ,[IsPaid]
		   ,[IsComplete]
           ,[IsActive]
           ,[PaymentTypeId]
           ,[TaxAmount]
           ,[DiscountAmount]
           ,[TotalAmount]
           ,[RecieveAmount]           
		   ,[ParentId]
           ,[GiftCardId]
           ,[CustomerId]
		   ,[ShopId])
     VALUES
           (@NEWID
           ,@DateTime
		   ,GETDATE()
           ,@UserId
           ,@CounterId
           ,@Type
           ,@IsPaid
		   ,1
           ,@IsActive
           ,@PaymentTypeId
           ,@TaxAmount
           ,@DiscountAmount
           ,@TotalAmount
           ,@RecieveAmount           
		   ,@ParentId
           ,@GiftCardId
           ,@CustomerId
		   ,@ShopId)

Select @NEWID

END

GO
/****** Object:  StoredProcedure [dbo].[insertSPDetail]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[insertSPDetail]
	@TransactionDetailID bigint,
	@ParentProductID bigint,
	@ChildProductID bigint,
	@Price bigint,
	@DiscountRate decimal(15,2),
	@ShopCode varchar(20)
	
	
AS
BEGIN
	DECLARE @NEWID VARCHAR(10);		
	SELECT @NEWID = ('SP' + @ShopCode + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
	CONVERT(VARCHAR,N.OID + 1)) FROM (
	SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
	SELECT SUBSTRING(SPDetailID, 5, LEN(SPDetailID)) as TID FROM SPDetail Where SUBSTRING(SPDetailID,0,3) = 'SP' And SUBSTRING(SPDetailID,3,2) = @ShopCode
	) AS T 
	) AS N

   INSERT INTO [dbo].[SPDetail]
           ([TransactionDetailID]
		   ,[ParentProductID]
		   ,[ChildProductID]
		   ,[Price]
		   ,[DiscountRate]
		   ,[SPDetailID])
     VALUES
           (@TransactionDetailID
		   ,@ParentProductID
		   ,@ChildProductID
		   ,@Price
		   ,@DiscountRate
		   ,@NEWID)

END

GO
/****** Object:  StoredProcedure [dbo].[InsertStockTransfer]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertStockTransfer]
	
			@DateTime datetime
           
           ,@IsFOC bit
		   ,@ShopId int
		   ,@WareHouseId int
		   ,@TransferDate datetime
		   ,@userId int
		   ,@status varchar(50)
           ,@Remark nvarchar(200)
           
		   

AS
BEGIN

	DECLARE @NEWID VARCHAR(20);		
	SELECT @NEWID = ('ST'  + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
	CONVERT(VARCHAR,N.OID + 1)) FROM (
	SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
	SELECT SUBSTRING(Id, 5, LEN(Id)) as TID FROM StockTransfer Where SUBSTRING(Id,0,3) = 'ST' 
	) AS T 
	) AS N
	
	
INSERT INTO [dbo].[StockTransfer]
           ([Id]
           ,[DateTime]
		   
		   ,[IsFOC]
		   ,[ShopId]
		   ,[WareHouseId]
		   ,[TransferDate]
		   ,[UserId]
		   ,[Status]
		   ,[Remark])		   
     VALUES
           (@NEWID
           ,@DateTime
		   
		   
		   ,@IsFOC
		   ,@ShopId
		   ,@WareHouseId
		   ,@TransferDate
		  
		   ,@userId
		   ,@status
		   ,@Remark)
		   

Select @NEWID

END

GO
/****** Object:  StoredProcedure [dbo].[InsertTransaction]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertTransaction]
			@DateTime datetime
           ,@UserId int
           ,@CounterId int
           ,@Type varchar(20)
           ,@IsPaid bit
           ,@IsActive bit
           ,@PaymentTypeId int
           ,@TaxAmount int
           ,@DiscountAmount int
           ,@TotalAmount decimal(18,2)
           ,@RecieveAmount decimal(18,2)
           ,@GiftCardId int
           ,@CustomerId int
		   ,@ShopCode varchar(5)
		   ,@ShopId int

AS
BEGIN

	DECLARE @NEWID VARCHAR(10);		
	SELECT @NEWID = ('TS' + @ShopCode + replicate('0', 6 - len(CONVERT(VARCHAR,N.OID + 1))) +
CONVERT(VARCHAR,N.OID + 1)) FROM (
SELECT CASE WHEN MAX(T.TID) IS null then 0 else MAX(T.TID) end as OID FROM (
SELECT SUBSTRING(Id, 5, LEN(Id)) as TID FROM [Transaction] Where SUBSTRING(Id,0,3) = 'TS' And SUBSTRING(Id,3,2) = @ShopCode
) AS T 
) AS N
	
	
	
INSERT INTO [dbo].[Transaction]
           ([Id]
           ,[DateTime]
		   ,[UpdatedDate]
           ,[UserId]
           ,[CounterId]
           ,[Type]
           ,[IsPaid]
		   ,[IsComplete]
           ,[IsActive]
           ,[PaymentTypeId]
           ,[TaxAmount]
           ,[DiscountAmount]
           ,[TotalAmount]
           ,[RecieveAmount]
           ,[GiftCardId]
           ,[CustomerId]
		   ,[ShopId])
     VALUES
           (@NEWID
           ,@DateTime
		   ,GetDate()
           ,@UserId
           ,@CounterId
           ,@Type
           ,@IsPaid
		   ,1
           ,@IsActive
           ,@PaymentTypeId
           ,@TaxAmount
           ,@DiscountAmount
           ,@TotalAmount
           ,@RecieveAmount
           ,@GiftCardId
           ,@CustomerId
		   ,@ShopId)

Select @NEWID

END

GO
/****** Object:  StoredProcedure [dbo].[InsertTransactionDetail]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[InsertTransactionDetail]
	@TransactionId varchar(20),
	@ProductId int,
	@Qty int,
	@UnitPrice int,
	@DiscountRate float,
	@TaxRate float,
	@TotalAmount int,
	@IsDeleted bit,
	@IsDeductedBy float
AS
BEGIN
	INSERT INTo[TransactionDetail]
(
	[TransactionDetail].[TransactionId],
	[TransactionDetail].[ProductId],
	[TransactionDetail].[Qty],
	[TransactionDetail].[UnitPrice],
	[TransactionDetail].[DiscountRate],
	[TransactionDetail].[TaxRate],
	[TransactionDetail].[TotalAmount],
	[TransactionDetail].[IsDeleted],
	[TransactionDetail].[IsDeductedBy]
)	
VALUES
(
	@TransactionId,
	@ProductId,
	@Qty,
	@UnitPrice,
	@DiscountRate,
	@TaxRate,
	@TotalAmount,
	@IsDeleted,
	@IsDeductedBy
	
	

);
SELECT SCOPE_IDENTITY();
END

GO
/****** Object:  StoredProcedure [dbo].[Paid]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Paid]
	@paid bit,
	@Id varchar(50)
AS
	UPDATE [Transaction] Set IsPaid = @paid, UpdatedDate = getDate() where Id = @Id
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[ProductReportByBId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[ProductReportByBId]
@BrandId int
AS
begin	

select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty ,pd.IsDiscontinue from Product as pd 
left join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.BrandId=@BrandId

end

GO
/****** Object:  StoredProcedure [dbo].[ProductReportByCId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[ProductReportByCId]
@MainCategoryId int	
AS
begin	
select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty,pd.IsDiscontinue from Product as pd 
left join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.ProductCategoryId=@MainCategoryId
end

GO
/****** Object:  StoredProcedure [dbo].[ProductReportByCIdAndBId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[ProductReportByCIdAndBId]
@BrandId int,
@MainCategoryId int
AS
begin

select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty,pd.IsDiscontinue from Product as pd 
left join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.ProductCategoryId=@MainCategoryId and pd.BrandId=@BrandId

end

GO
/****** Object:  StoredProcedure [dbo].[ProductReportBySCIdAndCId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[ProductReportBySCIdAndCId]
@MainCategoryId int,
@SubCategoryId int
AS
begin	
BEGIN

if(@SubCategoryId=1)

select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty ,pd.IsDiscontinue from Product as pd 
left  join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.ProductCategoryId=@MainCategoryId and pd.ProductSubCategoryId is Null

else
select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty ,pd.IsDiscontinue from Product as pd 
left join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.ProductCategoryId=@MainCategoryId and pd.ProductSubCategoryId=@SubCategoryId
End
end

GO
/****** Object:  StoredProcedure [dbo].[ProductReportBySCIdAndCIdAndBId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Proc [dbo].[ProductReportBySCIdAndCIdAndBId]
@MainCategoryId int,
@SubCategoryId int,
@BrandId int
AS
begin	
BEGIN

if(@SubCategoryId=1)

select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty ,pd.IsDiscontinue from Product as pd 
left join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.ProductCategoryId=@MainCategoryId and pd.ProductSubCategoryId is Null and pd.BrandId=@BrandId

else
select pd.ProductCode,pd.Name,PC.Name as [Segment Name],PdSC.Name as [SubSegment Name],bd.Name as [Brand Name],pd.Qty ,pd.IsDiscontinue from Product as pd 
left join  ProductCategory as PC on pd.ProductCategoryId=PC.Id 
left join  ProductSubCategory as PdSC on pd.ProductSubCategoryId=PdSC.Id
left join Brand as bd on pd.BrandId=bd.Id Where pd.ProductCategoryId=@MainCategoryId and pd.ProductSubCategoryId=@SubCategoryId and pd.BrandId=@BrandId
END
End

GO
/****** Object:  StoredProcedure [dbo].[RefundItemList]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[RefundItemList]
	 @fromDate datetime,
	@toDate datetime
AS
	Select TD.ProductId as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, SUM(TD.TotalAmount) as ItemTotalAmount
	from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
	where (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date)
	Group By TD.ProductId, P.Name;
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[SaleBreakDownByRangeWithSaleTrueValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaleBreakDownByRangeWithSaleTrueValue]
	@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@isSp bit,
	@CityId int,
	@CounterId int
AS
	Declare  @SaleByFromToDate Table (Id int, Total bigint, Qty int)
	Declare  @RefundByFromToDate Table (Id int, Total bigint,Qty int)
	if @isSp = 0
		Begin
		if @cusType != 1 
		Begin
	
		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
		and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name !=  'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted =0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
		and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR 
		TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where  Br.Name != 'Special Promotion'

		end
		else
		Begin

	
		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
		and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted
		IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and 
		CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)
		 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name != 'Special Promotion'

		End
	End
	else
	Begin 
		Begin
		if @cusType != 1 
		Begin
	
		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
		and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name =  'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted =0)
		 and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
			inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date)
		 and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, Sum(A.Total) as TotalSale, Sum(A.Qty) as SaleQty,Sum(B.Total) as TotalRefund, Sum(B.Qty) as RefundQty
		From Brand as Br left join @SaleByFromToDate as A on Br.Id = A.Id	
		left join @RefundByFromToDate as B on Br.Id = B.Id
		where  Br.Name = 'Special Promotion'
		Group By Br.Id, Br.Name

		end
		else
		Begin

	
		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
		and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' and (TD.IsDeleted 
IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
	  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
		and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)
		 and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, Sum(A.Total) as TotalSale, Sum(A.Qty) as SaleQty, Sum(B.Total) as TotalRefund, Sum(B.Qty) as RefundQty
		From Brand as Br left join @SaleByFromToDate as A on Br.Id = A.Id	
		left  join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name = 'Special Promotion'
		Group By Br.Id, Br.Name

		End
	End
	End

GO
/****** Object:  StoredProcedure [dbo].[SaleBreakDownByRangeWithUnitValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaleBreakDownByRangeWithUnitValue]
	@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@isSp bit,
	@CityId int,
	@CounterId int
AS
	Declare  @SaleByFromToDate Table (Id int, Total bigint, Qty int)
	Declare  @RefundByFromToDate Table (Id int, Total bigint,Qty int)
	if @isSp != 1
	Begin 
		if @cusType != 1 
		Begin

		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 
0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name != 'Special Promotion'

		end
		else
		Begin

		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
			inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted
 IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name != 'Special Promotion'

		End
	End
	Else
	Begin
		if @cusType != 1 
		Begin

		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice * TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name =  'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 
0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name =  'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name = 'Special Promotion'

		end
		else
		Begin

		insert into @SaleByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name =  'Special Promotion' and (TD.IsDeleted
 IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

	
		insert into @RefundByFromToDate
		select P.BrandId as BId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	
		inner join shop as s on s.Id=t.ShopId
		right join Brand as B on B.Id = P.BrandId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name =  'Special Promotion'
		 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.BrandId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From Brand as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name = 'Special Promotion'

		End
	End

GO
/****** Object:  StoredProcedure [dbo].[SaleBreakDownBySegmentWithSaleTrueValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaleBreakDownBySegmentWithSaleTrueValue]
	@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@isSp bit,
	@CityId int,
	@CounterId int
AS
	Declare  @SaleByFromToDate Table (Id int, Total bigint, Qty int)
	Declare  @RefundByFromToDate Table (Id int, Total bigint,Qty int)
	if @isSP != 1
	Begin
		if @cusType != 1 
			Begin

			insert into @SaleByFromToDate
			select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
			inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
			inner join shop as s on s.Id=t.ShopId
			right join ProductCategory as C on C.Id = P.ProductCategoryId
			where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted =
 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
			Group By P.ProductCategoryId
	
			insert into @RefundByFromToDate
			select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
			inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
			inner join shop as s on s.Id=t.ShopId
			right join ProductCategory as C on C.Id = P.ProductCategoryId
			where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion'
			 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))

			Group BY P.ProductCategoryId

			select C.Id as Id,C.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductCategory as C Full outer join @SaleByFromToDate as A on C.Id = A.Id	
			Full outer join @RefundByFromToDate as B on C.Id = B.Id 
			where C.Name != 'Special Promotion'

			end
		else
			Begin

				insert into @SaleByFromToDate
				select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
				from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
				inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
				inner join shop as s on s.Id=t.ShopId
				right join ProductCategory as C on C.Id = P.ProductCategoryId
				inner join Customer as Cus on Cus.Id = T.CustomerId
				where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion'
				 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
				   and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
				Group By P.ProductCategoryId

	
				insert into @RefundByFromToDate
				select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
				from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
				inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
				inner join shop as s on s.Id=t.ShopId
				right join ProductCategory as C on C.Id = P.ProductCategoryId
				inner join Customer as Cus on Cus.Id = T.CustomerId
				where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' 
				and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
				  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
				Group BY P.ProductCategoryId

				select C.Id as Id, C.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
				From ProductCategory as C Full outer join @SaleByFromToDate as A on C.Id = A.Id	
				Full outer join @RefundByFromToDate as B on C.Id = B.Id 
				where  C.Name != 'Special Promotion'

				End
	End
	Else
	Begin
		if @cusType != 1 
			Begin

			insert into @SaleByFromToDate
			select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
			inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
			inner join shop as s on s.Id=t.ShopId
			right join ProductCategory as C on C.Id = P.ProductCategoryId
			where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 
0)  and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
	  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
			Group By P.ProductCategoryId

	
			insert into @RefundByFromToDate
			select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
			from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
			inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
			inner join shop as s on s.Id=t.ShopId
			right join ProductCategory as C on C.Id = P.ProductCategoryId
			where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' 
			and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
			Group BY P.ProductCategoryId

			select C.Id as Id,C.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
			From ProductCategory as C Full outer join @SaleByFromToDate as A on C.Id = A.Id	
			Full outer join @RefundByFromToDate as B on C.Id = B.Id  
			where C.Name = 'Special Promotion'

			end
		else
			Begin

				insert into @SaleByFromToDate
				select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
				from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
				inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
					inner join shop as s on s.Id=t.ShopId
				right join ProductCategory as C on C.Id = P.ProductCategoryId
				inner join Customer as Cus on Cus.Id = T.CustomerId 
				where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' 
				and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
  and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
				Group By P.ProductCategoryId

	
				insert into @RefundByFromToDate
				select P.ProductCategoryId as CId, Sum(TD.TotalAmount) as DSTP, Sum(TD.Qty) as Qty
				from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
				inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
					inner join shop as s on s.Id=t.ShopId
				right join ProductCategory as C on C.Id = P.ProductCategoryId
				inner join Customer as Cus on Cus.Id = T.CustomerId
				where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion'
				 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)  and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
				Group BY P.ProductCategoryId

				select C.Id as Id,C.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
				From ProductCategory as C Full outer join @SaleByFromToDate as A on C.Id = A.Id	
				Full outer join @RefundByFromToDate as B on C.Id = B.Id 
				where C.Name = 'Special Promotion'

			End
	End

GO
/****** Object:  StoredProcedure [dbo].[SaleBreakDownBySegmentWithUnitValue]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaleBreakDownBySegmentWithUnitValue]
	@fromDate datetime,
	@toDate datetime,
	@cusType int,
	@isSp bit,
	@CityId int,
	@CounterId int
AS
	Declare  @SaleByFromToDate Table (Id int, Total bigint, Qty int)
	Declare  @RefundByFromToDate Table (Id int, Total bigint,Qty int)

	if @isSp != 1
	Begin 
		if @cusType != 1 
		Begin

		insert into @SaleByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId inner join Brand as B on B.Id = P.BrandId	
		inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 
0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.ProductCategoryId

	
		insert into @RefundByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
		inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion'
		 and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))

		Group BY P.ProductCategoryId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From ProductCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name != 'Special Promotion'

		end
		else
		Begin

		insert into @SaleByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId inner join Brand as B on B.Id = P.BrandId	
			inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' and (TD.IsDeleted
 IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))

		Group By P.ProductCategoryId

	
		insert into @RefundByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId inner join Brand as B on B.Id = P.BrandId	
			inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name != 'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))

		Group BY P.ProductCategoryId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From ProductCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name != 'Special Promotion'

		End
	End

	else
	Begin 
		if @cusType != 1 
		Begin

		insert into @SaleByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId inner join Brand as B on B.Id = P.BrandId	
		inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		where (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0
) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.ProductCategoryId

	
		insert into @RefundByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId inner join Brand as B on B.Id = P.BrandId	 
			inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		where (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' 
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))

		Group BY P.ProductCategoryId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From ProductCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name = 'Special Promotion'

		end
		else
		Begin

		insert into @SaleByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
		inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion' and (TD.IsDeleted 
IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
	 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group By P.ProductCategoryId

	
		insert into @RefundByFromToDate
		select P.ProductCategoryId as CId, Sum(TD.UnitPrice *TD.Qty) as DSTP, Sum(TD.Qty) as Qty
		from [Transaction] as T inner join TransactionDetail as TD on TD.TransactionId = T.Id
		inner join Product as P on P.Id = TD.ProductId	inner join Brand as B on B.Id = P.BrandId
		inner join shop as s on s.Id=t.ShopId
		right join ProductCategory as C on C.Id = P.ProductCategoryId
		inner join Customer as Cus on Cus.Id = T.CustomerId
		where Cus.CustomerTypeId = 1 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and T.IsDeleted = 0 and B.Name = 'Special Promotion'
		and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.IsComplete = 1 and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId)) and ((@CounterId =0 and 1=1) or (@CounterId !=0 and t.CounterId=@CounterId))
		Group BY P.ProductCategoryId

		select Br.Id as Id,Br.Name as Name, A.Total as TotalSale, A.Qty as SaleQty,B.Total as TotalRefund, B.Qty as RefundQty
		From ProductCategory as Br Full outer join @SaleByFromToDate as A on Br.Id = A.Id	
		Full outer join @RefundByFromToDate as B on Br.Id = B.Id
		where Br.Name = 'Special Promotion'

		End
	End

GO
/****** Object:  StoredProcedure [dbo].[SaleItemListByDate]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaleItemListByDate]
 @fromDate datetime,
 @toDate datetime
AS

	Select TD.ProductId as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, SUM(TD.TotalAmount) as ItemTotalAmount
	from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
	where T.IsDeleted = 0 and T.Type = 'Sale' or T.Type = 'Credit' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date)
	Group By TD.ProductId, P.Name;
	
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[SelectItemListByDate]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectItemListByDate]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@CounterId int
AS
	 Begin 


	 If (@IsSale = 1)
	 	 if(@CounterId = 0)
	Select P.ProductCode as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, (TD.UnitPrice - (TD.UnitPrice * (TD.DiscountRate/100))) as UnitPrice, SUM(TD.TotalAmount) as ItemTotalAmount, T.PaymentTypeId as PaymentTypeId, P.Size as Size
	from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
	where T.IsDeleted = 0 and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)
	Group By P.ProductCode, P.Name, TD.UnitPrice, T.PaymentTypeId, P.Size,TD.DiscountRate;

		else
		Select P.ProductCode as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, (TD.UnitPrice - (TD.UnitPrice * (TD.DiscountRate/100))) as UnitPrice, SUM(TD.TotalAmount) as ItemTotalAmount, T.PaymentTypeId as PaymentTypeId, P.Size as Size
	from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
	where T.IsDeleted = 0 and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
	and CAST(T.DateTime as date) <= CAST(@toDate as date) and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (t.CounterId=@CounterId)
	Group By P.ProductCode, P.Name, TD.UnitPrice, T.PaymentTypeId, P.Size,TD.DiscountRate;

   Else
    if(@CounterId = 0)
   Select P.ProductCode as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, (TD.UnitPrice - (TD.UnitPrice * (TD.DiscountRate/100))) as UnitPrice, SUM(TD.TotalAmount) as ItemTotalAmount, T.PaymentTypeId as PaymentTypeId, P.Size as Size
	from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
	where  T.IsDeleted = 0 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0)
	Group By P.ProductCode, P.Name, TD.UnitPrice, T.PaymentTypeId, P.Size,TD.DiscountRate;
		else
		Select P.ProductCode as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, (TD.UnitPrice - (TD.UnitPrice * (TD.DiscountRate/100))) as UnitPrice, SUM(TD.TotalAmount) as ItemTotalAmount, T.PaymentTypeId as PaymentTypeId, P.Size as Size
	from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
	where  T.IsDeleted = 0 and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) 
	and CAST(T.DateTime as date) <= CAST(@toDate as date) and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (t.CounterId=@CounterId)
	Group By P.ProductCode, P.Name, TD.UnitPrice, T.PaymentTypeId, P.Size,TD.DiscountRate;
   End
   RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[SelectTaxesListByDate]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SelectTaxesListByDate]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit
AS
   Begin If (@IsSale = 1)
	SELECT CAST(T.DateTime as date) as TDate, SUM(T.TaxAmount) as Amount
	FROM [Transaction] AS T
	WHERE (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date)
	GROUP BY CAST(T.DateTime as date)
   Else
    SELECT CAST(T.DateTime as date) as TDate, SUM(T.TaxAmount) as Amount
	FROM [Transaction] AS T
	WHERE (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate  as date) and CAST(T.DateTime as date) <= CAST(@toDate as date)
	GROUP BY CAST(T.DateTime as date)
   End

GO
/****** Object:  StoredProcedure [dbo].[Top100SaleItemList]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Top100SaleItemList]--'2017-01-01','2018-06-30',1,1000,0,0
 @fromDate datetime,
 @toDate datetime,
 @IsAmount bit,
 @num int,
 @CityId int,
 @CounterId int
AS

declare @fromDatel datetime
set @fromDatel=@fromDate


declare @toDatel datetime
set @toDatel=@toDate

declare @IsAmountl bit
set @IsAmountl=@IsAmount
declare @numl int
set @numl=@num
declare @CityIdl int
set @CityIdl=@CityId
declare @CounterIdl int
set @CounterIdl=@CounterId



begin
Declare @TempTable Table (ItemId varchar(25),ItemName varchar(200),ItemQty int,ItemTotalAmount bigint)
 if(@IsAmountl = 1) 

	Begin
	SET NOCOUNT on
	set arithabort on
		insert into @TempTable
		Select Top (@numl) P.ProductCode as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, 
		SUM(TD.TotalAmount) as ItemTotalAmount
		from [Transaction] as T 
		inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
		inner join Shop as s on s.Id=t.ShopId
		where (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) 
		and CAST(T.DateTime as date) <= CAST(@toDatel as date) and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.
		IsComplete = 1 and ((@CityIdl =0 and 1=1) or (@CityIdl!= 0 and s.CityId=@CityIdl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl!= 0 and T.CounterId=@CounterIdl))
		Group By P.ProductCode, P.Name
		Order By SUM(TD.TotalAmount) DESC,SUM(TD.Qty) Desc;
	End
 Else
	Begin
	SET NOCOUNT on
	set arithabort on
	insert into @TempTable
		Select Top (@numl) P.ProductCode as ItemId, P.Name as ItemName, SUM(TD.Qty) as ItemQty, 
		SUM(TD.TotalAmount) as ItemTotalAmount
		from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join Shop as s on s.Id=t.ShopId
		where (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) 
		and CAST(T.DateTime as date) <= CAST(@toDatel as date) and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and T.
		IsComplete = 1 and ((@CityIdl =0 and 1=1) or (@CityIdl!= 0 and s.CityId=@CityIdl)) and ((@CounterIdl =0 and 1=1) or (@CounterIdl!= 0 and T.CounterId=@CounterIdl))
		Group By P.ProductCode, P.Name
		Order By SUM(TD.Qty) DESC,Sum(TD.TotalAmount) Desc;
	End	
Select   ItemId,ItemName,ItemQty,ItemTotalAmount from @TempTable
end
delete @TempTable

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailByItem]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailByItem]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@MainCategoryId int,
	@SubCategoryId int,
	@BrandId int
AS
	
	Begin If(@SubCategoryId = 1)
		set @SubCategoryId = null
	End
	Begin If(@BrandId = 1)
		set @BrandId = null
	End
	
Begin If (@IsSale = 1)	
		Begin If(@MainCategoryId = 0 and @BrandId = 0)

			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) 
		
		Else If(@MainCategoryId =0 and @BrandId != 0)

			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId
		
		Else If(@MainCategoryId != 0 and @SubCategoryId =0 and @BrandId = 0 )
			
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1  and  (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId

		Else If(@MainCategoryId != 0 and @SubCategoryId != 0 and @BrandId = 0)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId

		Else If(@MainCategoryId !=0 and @SubCategoryId = 0 and @BrandId !=0)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and P.BrandId = @BrandId

		Else If(@MainCategoryId != 0 and @SubCategoryId != 0 and @BrandId != 0 )
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId and P.BrandId = @BrandId
		End	
	--Refund	
	Else
		Begin If(@MainCategoryId = 0 and @BrandId = 0)

			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date)
		
		Else If(@MainCategoryId =0 and @BrandId != 0)

			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId
		
		Else If(@MainCategoryId != 0 and @SubCategoryId =0 and @BrandId = 0 )
			
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId

		Else If(@MainCategoryId != 0 and @SubCategoryId != 0 and @BrandId = 0)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId

		Else If(@MainCategoryId !=0 and @SubCategoryId = 0 and @BrandId !=0)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and P.BrandId = @BrandId

		Else If(@MainCategoryId != 0 and @SubCategoryId != 0 and @BrandId != 0 )
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on P.Id = TD.ProductId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  T.Type = 'Refund' and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId and P.BrandId = @BrandId
		End	
				
	End

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailReport]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailReport]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@CityId int,
	@CounterId int
	
AS
	If(@IsSale = 1)
	Begin 
		Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
		from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
		inner join shop as s on s.Id=t.ShopId
		where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and (TD.
IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
	End
	Else
	Begin
		Select  P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
		from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
		inner join shop as s on s.Id=t.ShopId
		where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund')
		 and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and (TD.IsDeleted IS NULL 
		 OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
		and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
	End 
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailReportByBId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailReportByBId]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@BrandId int,
	@CityId int,
	@CounterId int
	
AS
   
	If(@IsSale = 1)
	Begin 
		Begin If(@BrandId = 1)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
			and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
		Else
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
				 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
			and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
		End
	End
	Else
	Begin
		Begin If(@BrandId = 1)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and (TD.IsDeleted IS 
NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		Else
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and (TD.IsDeleted 
IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		End
	End 
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailReportByBIdAndCId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailReportByBIdAndCId]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@BrandId int,
	@MainCategoryId int,
	@CityId int,
	@CounterId int
	
AS
   
	If(@IsSale = 1)
	Begin 
		Begin If(@BrandId = 1)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and  (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
		Else
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
				inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1  and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
				and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
		End 
	End
	Else
	Begin
		Begin If(@BrandId = 1)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		Else
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		End
	End 
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailReportByBIdAndCIdAndSCId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailReportByBIdAndCIdAndSCId]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@BrandId int,
	@MainCategoryId int,
	@SubCategoryId int,
	@CityId int,
	@CounterId int
	
AS
   
	If(@IsSale = 1)
	Begin 
		Begin If(@BrandId = 1)
		    Begin If (@SubCategoryId = 1)
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
				inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId Is Null and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
				and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))

			Else
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
					inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
					and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))

			End
			
		Else
			Begin If (@SubCategoryId = 1)
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
					inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId Is Null and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
					and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))

			Else
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
				inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
					and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))

			End
			
		End
	End
	Else
	Begin
		Begin If(@BrandId = 1)
		    Begin If (@SubCategoryId = 1)
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
					inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId Is Null and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
				and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
			Else
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
				inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId Is Null and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
					and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
			End
			
		Else
			Begin If (@SubCategoryId = 1)
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
					inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId Is Null and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
					and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
			Else
				Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
				from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
				inner join shop as s on s.Id=t.ShopId
				where (T.IsDeleted IS NULL or T.IsDeleted = 0) and  (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.BrandId = @BrandId and P.ProductCategoryId = @MainCategoryId and P.ProductSubCategoryId = @SubCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
					and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
			End
			
		End
		
	End 
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailReportByCId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailReportByCId]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@MainCategoryId int,
	@CityId int,
	@CounterId int
	
AS
	If(@IsSale = 1)
	Begin 
		Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
				inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Slae' or T.Type = 'Credit' or T.Type = 'GiftCard')
			and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
	End
	Else
	Begin
		Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
		from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join shop as s on s.Id=t.ShopId
		where (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductCategoryId = @MainCategoryId and
 (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
 and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
	End 
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[TransactionDetailReportBySCIdAndCId]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransactionDetailReportBySCIdAndCId]
	@fromDate datetime,
	@toDate datetime,
	@IsSale bit,
	@SubCategoryId int,
	@MainCategoryId int,
	@CityId int,
	@CounterId int
	
AS
   
	If(@IsSale = 1)
	Begin 
		Begin If(@SubCategoryId = 1)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductSubCategoryId Is Null and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
			and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
		Else
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			inner join shop as s on s.Id=t.ShopId
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and T.IsComplete = 1 and (T.Type = 'Sale' or T.Type = 'Credit' or T.Type = 'GiftCard') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductSubCategoryId = @SubCategoryId and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
			and ((@CityId=0 and 1=1) or (@CityId!=0 and s.CityId=@CityId))  
			and ((@CounterId=0 and 1=1) or (@CounterId!=0 and t.CounterId=@CounterId))
		End
	End
	Else
	Begin
		Begin If(@SubCategoryId = 1)
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductSubCategoryId Is Null and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		Else
			Select P.ProductCode as ItemId, P.Name as ItemName, TD.TransactionId as TransactionId, TD.Qty as Qty, TD.TotalAmount, T.Type as TransactionType, T.DateTime as TransactionDate
			from [Transaction] as T inner join TransactionDetail as TD on T.Id = TD.TransactionId inner join Product as P on TD.ProductId = P.Id
			where (T.IsDeleted IS NULL or T.IsDeleted = 0) and (T.Type = 'Refund' or T.Type = 'CreditRefund') and CAST(T.DateTime as date) >= CAST(@fromDate as date) and CAST(T.DateTime as date) <= CAST(@toDate as date) and P.ProductSubCategoryId = @SubCategoryId 
and P.ProductCategoryId = @MainCategoryId and (TD.IsDeleted IS NULL OR TD.IsDeleted = 0) and (T.PaymentTypeId != 4 and T.PaymentTypeId != 6)
		End
	End 
RETURN 0

GO
/****** Object:  StoredProcedure [dbo].[VIPReportForNoveltyAndGWP]    Script Date: 5/10/2019 11:14:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[VIPReportForNoveltyAndGWP]
	
	@customerType int,
	@fromDate datetime,
	@toDate datetime,
	@CounterId int,
	@CityId int
AS
	declare @customerTypel int
	set @customerTypel=@customerType
	declare @fromDatel datetime
	set @fromDatel=@fromDate
	declare @toDatel datetime
	set @toDatel=@toDate
	declare @CounterIdl int
	set @CounterIdl=@CounterId
	declare @CityIdl int
	set @CityIdl=@CityId

	select Cus.Title + ' ' +  Cus.Name  as Name, COUNT(T.Id) as InvoiceQty, SUM(T.TotalAmount) as Total, 
	(dbo.VIP_PurchaseProductQty(T.CustomerId, @customerTypel, @fromDatel, @toDatel) - dbo.VIP_RefundProductQty(T.CustomerId, @customerTypel, @fromDatel, @toDatel))

 as productQty, IsNull(dbo.VIP_Novelty_Qty(T.CustomerId, @customerTypel, @fromDatel, @toDatel),0) as NV_Qty, 
 IsNull(dbo.VIP_GWP_Qty(T.CustomerId, @customerTypel, @fromDatel, @toDatel),0) as GWPQty, 
 dbo.CheckNewVIP(T.CustomerId, @customerTypel, @fromDatel, @toDatel) as IsVIP
	from [Transaction] as T 
	inner join Customer as Cus on T.CustomerId = Cus.Id 
	
	where 
	 ((@CounterIdl=0 and 1=1 ) or (@CounterIdl!=0 and T.CounterId=@CounterIdl))  
	and ((@CityId=0 and 1=1) or (@CityId!=0 and Cus.CityId=@CityIdl))
	and Cus.CustomerTypeId = @customerTypel and 
	 (T.Type = 'Sale' or T.Type = 'Credit') and CAST(T.DateTime as date) >= CAST(@fromDatel as date) 
	and CAST(T.DateTime as date) <= CAST(@toDatel as date) and T.IsDeleted = 0 and T.IsComplete = 1
	and ((@customerTypel=1 and cast(t.DateTime as date) >=  cast( Cus.PromoteDate as date) )   
	or ((@customerTypel=2 and cast(t.DateTime as date)   <  cast( Cus.PromoteDate as date)  ) or (@customerTypel=2 and cus.PromoteDate is null))) 
	Group By T.CustomerId, Cus.Name,Cus.Title

GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'GWP or PWP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'GiftSystem', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Transaction, Refund,Draft, Debt,GiftCard' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transaction', @level2type=N'COLUMN',@level2name=N'Type'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'just for Credit Transaction' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transaction', @level2type=N'COLUMN',@level2name=N'IsPaid'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'If false, store as draft' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transaction', @level2type=N'COLUMN',@level2name=N'IsComplete'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Use only for Refund Transaction' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Transaction', @level2type=N'COLUMN',@level2name=N'ParentId'
GO
USE [master]
GO
ALTER DATABASE [mposLOC] SET  READ_WRITE 
GO

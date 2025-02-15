USE [master]
GO
/****** Object:  Database [CodeWorks]    Script Date: 2024/09/26 01:27:48 ******/
CREATE DATABASE [CodeWorks]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'CodeWorks', FILENAME = N'C:\DATA\CodeWorks.mdf' , SIZE = 10240KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CodeWorks_log', FILENAME = N'C:\DATA\CodeWorks_log.ldf' , SIZE = 35712KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [CodeWorks] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CodeWorks].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CodeWorks] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [CodeWorks] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [CodeWorks] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [CodeWorks] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [CodeWorks] SET ARITHABORT OFF 
GO
ALTER DATABASE [CodeWorks] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [CodeWorks] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [CodeWorks] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [CodeWorks] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [CodeWorks] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [CodeWorks] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [CodeWorks] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [CodeWorks] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [CodeWorks] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [CodeWorks] SET  DISABLE_BROKER 
GO
ALTER DATABASE [CodeWorks] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [CodeWorks] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [CodeWorks] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [CodeWorks] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [CodeWorks] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [CodeWorks] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [CodeWorks] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [CodeWorks] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [CodeWorks] SET  MULTI_USER 
GO
ALTER DATABASE [CodeWorks] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [CodeWorks] SET DB_CHAINING OFF 
GO
ALTER DATABASE [CodeWorks] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [CodeWorks] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [CodeWorks] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [CodeWorks] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [CodeWorks] SET QUERY_STORE = OFF
GO
USE [CodeWorks]
GO
/****** Object:  Table [dbo].[Employee]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Surname] [varchar](150) NOT NULL,
	[JobTitleId] [int] NOT NULL,
	[DateOfBirth] [date] NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EmployeeSkill]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmployeeSkill](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[EmployeeID] [int] NOT NULL,
	[SkillID] [int] NOT NULL,
 CONSTRAINT [PK_EmployeeSkill] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[JobTitle]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[JobTitle](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[JobTitle] [varchar](150) NOT NULL,
 CONSTRAINT [PK_JobTitle] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Project]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Project](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Startdate] [datetime] NOT NULL,
	[Enddate] [datetime] NULL,
	[Cost] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectEmployee]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectEmployee](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[EmployeeID] [int] NOT NULL,
 CONSTRAINT [PK_ProjectEmployee] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectLocations]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectLocations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Location] [varchar](150) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Skill]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Skill](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 2024/09/26 01:27:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
	[Surname] [varchar](150) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [varchar](50) NOT NULL,
	[Role] [int] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_E_JTID] FOREIGN KEY([JobTitleId])
REFERENCES [dbo].[JobTitle] ([id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_E_JTID]
GO
ALTER TABLE [dbo].[EmployeeSkill]  WITH CHECK ADD  CONSTRAINT [FK_ES_E_EmployeeID] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[EmployeeSkill] CHECK CONSTRAINT [FK_ES_E_EmployeeID]
GO
ALTER TABLE [dbo].[EmployeeSkill]  WITH CHECK ADD  CONSTRAINT [FK_ES_S_SkillID] FOREIGN KEY([SkillID])
REFERENCES [dbo].[Skill] ([id])
GO
ALTER TABLE [dbo].[EmployeeSkill] CHECK CONSTRAINT [FK_ES_S_SkillID]
GO
ALTER TABLE [dbo].[ProjectEmployee]  WITH CHECK ADD  CONSTRAINT [FK_PE_E_EmployeeID] FOREIGN KEY([EmployeeID])
REFERENCES [dbo].[Employee] ([id])
GO
ALTER TABLE [dbo].[ProjectEmployee] CHECK CONSTRAINT [FK_PE_E_EmployeeID]
GO
ALTER TABLE [dbo].[ProjectEmployee]  WITH CHECK ADD  CONSTRAINT [FK_PE_P_ProjectID] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Project] ([id])
GO
ALTER TABLE [dbo].[ProjectEmployee] CHECK CONSTRAINT [FK_PE_P_ProjectID]
GO
USE [master]
GO
ALTER DATABASE [CodeWorks] SET  READ_WRITE 
GO

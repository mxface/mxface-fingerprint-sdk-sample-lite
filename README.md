# MxFace Fingerprint SDK 2.0

## Overall Flow

This documentation provides a comprehensive guide for setting up and using the **MxFace Fingerprint SDK 2.0**. It covers installation, activation, database setup, interfaces, and usage examples.



## Prerequisites

### 1. Generating a Hardware ID
1. Navigate to the **Artifacts** folder in your root directory.
2. Open the **HardwareId Generator** folder.
3. Double-click on `MxFace.SDK.DeviceIdGenerator.exe`.
4. Note down the generated Hardware ID displayed in the format:
   
   **Example:** `Generated Device Id: MQAS5AGX3VWKYGH9ZNR07EHSA4BMDKE0JDXH7WQGAH8WH33BJ8W0`

### 2. Activating License Files
1. Navigate to the **Artifacts** folder in your root directory.
2. Open Command Prompt as **Run As Administrator**.
3. Copy the path of the `IdGen` folder (e.g., `D:\Activation\IDGen`) and change the directory in Command Prompt using:
   
   ```sh
   cd /d D:\Activation\IDGen
   ```
4. Ensure the `IdGen` folder contains `id_gen.exe` and `sn.txt` files.
5. Run the following command in Command Prompt:
   
   ```sh
   id_gen.exe sn.txt hardware.id
   ```
6. Provide the retrieved Hardware ID to the vendor for generating your License File.
7. Navigate back to the **Activation** folder in Command Prompt:
   
   ```sh
   cd ..
   ```
8. Once in the **Activation** folder, run the command:
   
   ```sh
   pg.exe -install
   ```

## Cloning the Sample Project from GitHub
1. Clone the sample project from GitHub.
2. Open the cloned project in **Visual Studio/Visual Studio Code**.
3. Open the terminal and run:
   
   ```sh
   dotnet build
   ```
4. Open the project folder in **File Explorer** and navigate to:
   
   ```sh
   \Artifacts\Activation\bin
   ```
5. Copy all **DLL files** from this folder.
6. Navigate to:
   
   ```sh
   \bin
   ```
7. Paste all copied DLL files here.
8. You will receive a license file named **Fingerprint.lic**. Place this file in the `bin` folder where you just pasted the DLL files.

## Activating the Windows Service
1. Navigate to **Artifacts/Activation/Windows Service**.
2. Double-click on `MFScanClientService`.
3. When prompted with **Yes/No**, click **Yes** to install the service.
4. If you are using the **MIS100V2 Fingerprint Device**, install its drivers from:
   
   ```sh
   Artifacts/Activation/Drivers
   ```

## Database Setup
To enroll, search, and match fingerprints, you need to connect to a database.

### 1. Install ODBC Driver
- If using **PostgreSQL**, download `psqlodbc_x64.msi` from:
  [PostgreSQL ODBC Driver](https://www.postgresql.org/ftp/odbc/releases/REL-17_00_0004-mimalloc/)
- If using **MySQL**, download `Windows (x86, 64-bit), MSI Installer` from:
  [MySQL ODBC Driver](https://dev.mysql.com/downloads/connector/odbc/)
- If using **MsSQL**, download `Microsoft ODBC Driver 18 for SQL Server (x64)` from:
  [MsSQL ODBC Driver](https://learn.microsoft.com/en-us/sql/connect/odbc/download-odbc-driver-for-sql-server?view=sql-server-ver16)

### 2. Configure Database
1. After installing the driver, set it up by opening the newly installed application.
2. Open **ODBC Data Sources (64-bit)** from the Windows Start Menu.
3. Click **Add** and select:
   
   ```sh
   PostgreSQL ANSI(x64)
   ```
4. Configure your database settings and **Test the Connection**.
5. If successful, run the following SQL query to create the required table:
   
   ```sql
   CREATE TABLE IF NOT EXISTS public."__FingerprintSubjects" (
       "Id" integer NOT NULL,
       "SubjectId" varchar(45),
       "Template" bytea,
       "Group" varchar(45),
       "ClientId" integer,
       CONSTRAINT "__FingerprintSubjects_pkey" PRIMARY KEY ("Id")
   ) TABLESPACE pg_default;
   
   ALTER TABLE public."__FingerprintSubjects" OWNER TO sa;
   ```

## Interfaces Overview
The **MxFace Fingerprint SDK** provides comprehensive fingerprint management through three main interfaces:

- **ISearch**
- **IDevice**
- **ICaptureService**

### **ISearch Interface**
Handles core fingerprint operations including **enrollment, verification, and searching**.

#### Enroll Method
```csharp
task Enroll(byte[] source, string externalId, string group);
```
- Stores a fingerprint in the database.
- Checks for duplicates before enrollment.

#### Verify Method
```csharp
task Verify(byte[] source, byte[] target);
```
- Compares two fingerprint samples.
- Provides a **matching score**.

#### Search Method
```csharp
task<List> Search(byte[] source, string group);
```
- Searches the database for matching fingerprints.
- Returns up to **5 best matches**.

### **IDevice Interface**
Handles fingerprint device management.

#### GetConnectedDevices Method
```csharp
task GetConnectedDevices(List devices);
```
- Detects connected devices.

#### Init Method
```csharp
task Init(string productName);
```
- Initializes the fingerprint device.

### **ICaptureService Interface**
Handles fingerprint capture operations.

#### StartCaptureAsync Method
```csharp
task StartCaptureAsync(int Timeout = 10, int MinimumQuality = 60);
```
- Captures a fingerprint image.
- Ensures quality control.

#### StopCaptureAsync Method
```csharp
task StopCaptureAsync();
```
- Stops the ongoing fingerprint capture process.

## Implementation Requirements
### System Requirements
- Valid **SDK license**.
- Compatible **fingerprint device**.
- Database configured for **search/enrollment** operations.

### Best Practices
- Always initialize the device before capture.
- Implement proper error handling.
- Check license status before operations.
- Use appropriate **quality thresholds**.

## Usage Examples
### Basic Device Initialization
```csharp
IDevice deviceService = new DeviceService(httpClient, logger);
await deviceService.Init("DeviceName");
```

### Fingerprint Capture
```csharp
ICaptureService captureService = new FingerprintCapturingService(httpClient);
var captureResult = await captureService.StartCaptureAsync(timeout: 15, minimumQuality: 75);
```

### Search Implementation
```csharp
ISearch searchService = new SearchService(configuration);
var searchResults = await searchService.Search(captureResult.FingerprintData, "GroupA");
```

## End of Documentation


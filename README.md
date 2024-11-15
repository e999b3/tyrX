# tyrX

**tyrX** is designed specifically for Autodesk Revit, starting from version 2025. With the shift to the .NET 8.0 framework in Revit 2025, it is essential for associated plugins to align with this update. Building on the legacy of its predecessor, **tyRevit**, **tyrX** continues to offer a lightweight external command solution for Revit, while upholding its core mission: empowering users to create custom Revit workflows using F# programming. It leverages the powerful IntelliSense capabilities of Visual Studio Community and Visual Studio Code to enhance and streamline coding efficiency for Revit development.

![intro](https://github.com/user-attachments/assets/c5360829-4fcf-46c9-b60c-6c290351cedc)

## Installation

To install **tyrX**, follow these straightforward steps:

### 1. Fork the Repository
Fork this repository to your own GitHub account.

### 2. Download the Project
You can either:
- Clone the repository using **GitHub Desktop**.
- Download the ZIP file directly from the repository and extract its contents.

### 3. Open the Project
- Open the unzipped folder with **Visual Studio Code** or load the solution file in **Visual Studio**.

### 4. Check RevitVersion and Compile
Depending on your Revit version, change the constant **RevitVersion** in the .fsproj file. 

![image](https://github.com/user-attachments/assets/6e380898-afac-414b-aa17-f3a3583a8278)

When compiling, Visual Studio will choose the targeted Revit API version to reference. If the chosen Revit major version is prior to 2025, the constant **TargetFramework** will be set to **net48**, otherwise it will target **net8.0-windows**.

### 5. Edit the Add-in Path
Modify the `tyrX.addin` file to point to the correct path of the built assembly `rX.dll`.

![image](https://github.com/user-attachments/assets/98498283-4524-4f56-8a62-2f240666bf67)

Change the highlighted line `<Assembly>...</Assembly>` to your own assembly path.

### 6. Copy the Add-in File
Copy the `tyrX.addin` file to the appropriate Revit add-in directory:

- **For current user**: `%appdata%\Autodesk\Revit\Addins\2025`
- **For all users**: `%programdata%\Autodesk\Revit\Addins\2025`

### 7. Launch Revit
Start **Revit 2025**, and you are ready to go!


## Usage

Once loaded into the Revit environment, the core application of **tyrX** will search for classes that implement the `IExternalCommand` interface within the compiled `rX.dll` assembly. These classes will then be loaded and recognized as add-ins within Revit.

### Pre-compiled F# Scripts
In the root folder, you will find two F# scripts:
- `DevCommandCompiler.fs`
- `SelectedCommandCompiler.fs`

These scripts are set to be compiled into the assembly and can be accessed in Revit under:

**Tab**: `Add-Ins`  
**Panel**: `External`  
**Drop-down button**: `External Tools`

![tyrX Add-in Interface](https://github.com/user-attachments/assets/696761cc-a8c7-48f0-85bc-8db3387b592d)

### Workflow

My preferred workflow when programming with F# for Revit is an iterative **try-and-error** approach. This involves making frequent changes to the code and testing them quickly. The most efficient way to do this is to **just save** my work-in-progress code and recompile it **only** by clicking the command button to run it. In fact, I use keyboard shortcuts to run my code, avoiding the time wasted searching for the right button to click. Once I'm satisfied with the code, or if I want to experiment with alternative solutions without losing my progress, I save a copy of the current code for comparison and further development.

This is where the two compiler scripts come into play.

- **Develop Command Compiler**: The `DevCommandCompiler.fs` script is designed to compile the `cmd/develop.fs` file, enabling you to focus on problem-solving without distractions.
- **Selected Command Compiler**: This command offers a drop-down list of all F# scripts located in the `cmd` folder, allowing you to select and execute any script easily. Once you’re done with `develop.fs`, simply duplicate it, give it a meaningful name, and keep it available for future use.

### Key Benefits
- **No Revit Restarts Needed**: There is no need to restart your Revit session! You can modify and test your scripts in the `cmd` folder as often as needed.
- **Accelerate Your Workflow**: This approach encourages experimenting with different solutions, ensuring a fast and effective Revit coding experience.

Enjoy exploring and refining your Revit coding workflow!

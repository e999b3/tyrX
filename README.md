# tyrX
 
**tyrX** is designed specifically for Autodesk Revit, starting from version 2025. With the shift to the .NET 8.0 framework in Revit 2025, it is essential for associated plugins to align with this update. Building on the legacy of its predecessor, **tyRevit**, **tyrX** continues to offer a lightweight external command solution for Revit, while upholding its core mission: empowering users to create custom Revit workflows using F# programming. It leverages the powerful IntelliSense capabilities of Visual Studio Community and Visual Studio Code to enhance and streamline coding efficiency for Revit development.

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

### 4. Edit the Add-in Path
Modify the `tyrX.addin` file to point to the correct path of the built assembly `r25.dll`.

### 5. Copy the Add-in File
Copy the `tyrX.addin` file to the appropriate Revit add-in directory:

- **For current user**: `%appdata%\Autodesk\Revit\Addins\2025`
- **For all users**: `%programdata%\Autodesk\Revit\Addins\2025`

### 6. Launch Revit
Start **Revit 2025**, and you are ready to go!


## Usage

Once loaded into the Revit environment, the core application of **tyrX** will search for classes that implement the `IExternalCommand` interface within the compiled `r25.dll` assembly. These classes will then be loaded and recognized as add-ins within Revit.

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

My preferred workflow when programming with F# for Revit is an iterative **try-and-error** approach. This involves making frequent changes to the code and testing them quickly. The most efficient way to do this is to save my work-in-progress code and recompile it only when I trigger the command to run it. In fact, I use keyboard shortcuts to run my code, avoiding the time wasted searching for the right button to click. Once I'm satisfied with the code, or if I want to experiment with alternative solutions without losing my progress, I save a copy of the current code for comparison and further development.

This is where the two compiler scripts come into play.

- **Develop Command Compiler**: The `DevCommandCompiler.fs` script is designed to compile the `cmd/develop.fs` file, enabling you to focus on problem-solving without distractions.
- **Selected Command Compiler**: This command offers a drop-down list of all F# scripts located in the `cmd` folder, allowing you to select and execute any script easily. Once you’re done with `develop.fs`, simply duplicate it, give it a meaningful name, and keep it available for future use.

### Key Benefits
- **No Revit Restarts Needed**: There is no need to restart your Revit session! You can modify and test your scripts in the `cmd` folder as often as needed.
- **Accelerate Your Workflow**: This approach encourages experimenting with different solutions, ensuring a fast and effective Revit coding experience.

Enjoy exploring and refining your Revit coding workflow!
